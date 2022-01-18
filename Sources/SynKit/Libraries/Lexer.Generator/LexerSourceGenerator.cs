// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Yoakke.SynKit.Automata;
using Yoakke.SynKit.Automata.Dense;
using Yoakke.Collections.Intervals;
using Yoakke.SourceGenerator.Common;
using Yoakke.SourceGenerator.Common.RoslynExtensions;
using Yoakke.SynKit.Lexer.Generator.Model;
using System.IO;
using System.Reflection;
using Scriban;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System.Threading;
using System.Collections.Immutable;
using Yoakke.SynKit.Lexer.Attributes;

namespace Yoakke.SynKit.Lexer.Generator;

/// <summary>
/// Source generator for lexers.
/// Generates a DFA-driven lexer from annotated token types.
/// </summary>
[Generator]
public class LexerSourceGenerator : IIncrementalGenerator
{
    private class LexerAttributeModel
    {
        public INamedTypeSymbol? TokenType { get; set; }
    }

    private class RegexAttributeModel
    {
        public string Regex { get; set; } = string.Empty;
    }

    private class TokenAttributeModel
    {
        public string Text { get; set; } = string.Empty;
    }

    private record class Symbols(
        INamedTypeSymbol LexerAttribute,
        INamedTypeSymbol CharSourceAttribute,
        INamedTypeSymbol RegexAttribute,
        INamedTypeSymbol TokenAttribute,
        INamedTypeSymbol EndAttribute,
        INamedTypeSymbol ErrorAttribute,
        INamedTypeSymbol IgnoreAttribute);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Inject source code
        var assembly = Assembly.GetExecutingAssembly();
        var sourcesToInject = assembly
            .GetManifestResourceNames()
            .Where(m => m.StartsWith("InjectedSources."));
        context.RegisterPostInitializationOutput(ctx =>
        {
            foreach (var source in sourcesToInject)
            {
                ctx.AddSource(
                    source,
                    SourceText.From(
                        new StreamReader(assembly.GetManifestResourceStream(source)).ReadToEnd(),
                        Encoding.UTF8));
            }
        });

        // Incremental pipeline
        var typeDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                // Filter for type declarations with at least one attribute
                predicate: IsSyntaxTargetForGeneration,
                // Transform to either the semantic value or null, if not the right attribute
                transform: GetSemanticTargetForGeneration)
            // Discard null
            .Where(m => m is not null);

        // Combine the collected type declarations with the compilation
        var compilationAndDecls = context.CompilationProvider.Combine(typeDeclarations.Collect());

        // Generate the sources
        context.RegisterSourceOutput(
            compilationAndDecls,
            (spc, source) => Execute(source.Left, source.Right!, spc));
    }

    private static bool IsSyntaxTargetForGeneration(
        SyntaxNode node,
        CancellationToken cancellationToken) =>
        node is TypeDeclarationSyntax typeDeclSyntax && typeDeclSyntax.AttributeLists.Count > 0;

    private static TypeDeclarationSyntax? GetSemanticTargetForGeneration(
        GeneratorSyntaxContext context,
        CancellationToken cancellationToken)
    {
        var typeDeclSyntax = (TypeDeclarationSyntax)context.Node;

        // Loop through all the attributes on the method
        foreach (var attributeListSyntax in typeDeclSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).Symbol is not IMethodSymbol attributeSymbol) continue;

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                // Is the attribute the searched attribute
                if (fullName == typeof(LexerAttribute).FullName) return typeDeclSyntax;
            }
        }

        // We didn't find the attribute we were looking for
        return null;
    }

    private static void Execute(
        Compilation compilation,
        ImmutableArray<TypeDeclarationSyntax> types,
        SourceProductionContext context)
    {
        // Do nothing on empty
        if (types.IsDefaultOrEmpty) return;

        // [LoggerMessage] does this, so we imitate
        var distinctTypes = types.Distinct();

        // Convert each type to the proper model
        var models = GetGenerationModels(compilation, context, distinctTypes, context.CancellationToken);

        // If no models remain, we return
        if (models.Count == 0) return;

        // Otherwise we translate each model to a Scriban model
        var scribanModels = models.Select(ToScribanModel);

        // Otherwise we generate each lexer with the Scriban template
        var sources = GenerateSources(scribanModels, context.CancellationToken);

        // Finally add all sources
        foreach (var (name, text) in sources) context.AddSource(name, SourceText.From(text, Encoding.UTF8));
    }

    private static IReadOnlyList<LexerModel> GetGenerationModels(
        Compilation compilation,
        SourceProductionContext context,
        IEnumerable<TypeDeclarationSyntax> types,
        CancellationToken cancellationToken)
    {
        // Load the attributes
        var symbols = LoadSymbols(compilation);

        // List to hold the results
        var models = new List<LexerModel>();

        foreach (var typeDeclSyntax in types)
        {
            // If the operation is canceled, abort here
            cancellationToken.ThrowIfCancellationRequested();

            // Get the declared symbol
            var semanticModel = compilation.GetSemanticModel(typeDeclSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(typeDeclSyntax, cancellationToken) is not INamedTypeSymbol declaredSymbol) continue;

            // Generate the model
            var model = GetGenerationModel(context, symbols, declaredSymbol, cancellationToken);
            if (model is null) continue;

            models.Add(model);
        }

        return models;
    }

    private static IReadOnlyList<(string Name, string Text)> GenerateSources(
        IEnumerable<(string Name, object Model)> models,
        CancellationToken cancellationToken)
    {
        // List of generated sources
        var sources = new List<(string Name, string Text)>();

        // Load the Scriban template
        var template = LoadScribanTemplate();

        foreach (var (name, model) in models)
        {
            // If the operation is canceled, abort here
            cancellationToken.ThrowIfCancellationRequested();

            // Generate source code
            var source = GenerateSource(template, model, cancellationToken);
            sources.Add((name, source));
        }

        return sources;
    }

    private static Symbols LoadSymbols(Compilation compilation) => new(
        LexerAttribute: LoadRequiredSymbol(compilation, typeof(LexerAttribute)),
        CharSourceAttribute: LoadRequiredSymbol(compilation, typeof(CharSourceAttribute)),
        RegexAttribute: LoadRequiredSymbol(compilation, typeof(RegexAttribute)),
        TokenAttribute: LoadRequiredSymbol(compilation, typeof(TokenAttribute)),
        EndAttribute: LoadRequiredSymbol(compilation, typeof(EndAttribute)),
        ErrorAttribute: LoadRequiredSymbol(compilation, typeof(ErrorAttribute)),
        IgnoreAttribute: LoadRequiredSymbol(compilation, typeof(IgnoreAttribute))
    );

    private static INamedTypeSymbol LoadRequiredSymbol(Compilation compilation, Type type) =>
        compilation.GetTypeByMetadataName(type.FullName) ?? throw new InvalidOperationException($"could not load type {type.FullName}");

    private static Template LoadScribanTemplate()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var text = new StreamReader(assembly.GetManifestResourceStream("InjectedSources.lexer.sbncs")).ReadToEnd();
        var template = Template.Parse(text);

        if (template.HasErrors)
        {
            var errors = string.Join(" | ", template.Messages.Select(x => x.Message));
            throw new InvalidOperationException($"Template parse error: {template.Messages}");
        }

        return template;
    }

    private static LexerModel? GetGenerationModel(
        SourceProductionContext context,
        Symbols symbols,
        INamedTypeSymbol lexerSymbol,
        CancellationToken cancellationToken)
    {
        // Check, if we should cancel
        cancellationToken.ThrowIfCancellationRequested();

        // We extract all information from the lexer attribute
        if (lexerSymbol.TryGetAttribute(symbols.LexerAttribute, out LexerAttributeModel? lexerAttribute)) return null;

        // From this we get to know the token type, which we inspect for the fields
        // Check, if it's even an enumeration type
        var tokenType = lexerAttribute!.TokenType;
        if (tokenType?.TypeKind != TypeKind.Enum)
        {
            // TODO
            // context.ReportDiagnostic();
            return null;
        }

        // The source field is easy to extract from the lexer class
        // It is optional, so a null is acceptable here
        var sourceField = lexerSymbol
            .GetMembers()
            .FirstOrDefault(f => f.HasAttribute(symbols.CharSourceAttribute));

        // TODO
        throw new NotImplementedException();
    }

    private static (string Name, object Model) ToScribanModel(LexerModel lexerModel)
    {
        // TODO
        throw new NotImplementedException("Got to scriban model translation!");
    }

    private static string GenerateSource(
        Template template,
        object model,
        CancellationToken cancellationToken)
    {
        // TODO
        throw new NotImplementedException("Got to generate source!");
    }
}
