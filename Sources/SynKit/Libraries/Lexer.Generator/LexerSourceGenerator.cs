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

namespace Yoakke.SynKit.Lexer.Generator;

/// <summary>
/// Source generator for lexers.
/// Generates a DFA-driven lexer from annotated token types.
/// </summary>
[Generator]
public class LexerSourceGenerator : IIncrementalGenerator
{
    private class SyntaxReceiver : ISyntaxReceiver
    {
        public IList<TypeDeclarationSyntax> CandidateTypes { get; } = new List<TypeDeclarationSyntax>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is TypeDeclarationSyntax typeDeclSyntax && typeDeclSyntax.AttributeLists.Count > 0)
            {
                this.CandidateTypes.Add(typeDeclSyntax);
            }
        }
    }

    private class LexerAttribute
    {
        public INamedTypeSymbol? TokenType { get; set; }
    }

    private class RegexAttribute
    {
        public string Regex { get; set; } = string.Empty;
    }

    private class TokenAttribute
    {
        public string Text { get; set; } = string.Empty;
    }

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
                    SourceText.From(new StreamReader(assembly.GetManifestResourceStream(source)).ReadToEnd()));
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
        // TODO
        throw new NotImplementedException();
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
        var models = GetModelsToGenerate(compilation, distinctTypes, context.CancellationToken);

        // If no models remain, we return
        if (models.Count == 0) return;

        // Otherwise we generate each lexer with the Scriban template
        var sources = GenerateSourceFromModels(models, context.CancellationToken);

        // Finally add all sources
        foreach (var (name, text) in sources) context.AddSource(name, SourceText.From(text));
    }

    private static IReadOnlyList<(string Name, object Model)> GetModelsToGenerate(
        Compilation compilation,
        IEnumerable<TypeDeclarationSyntax> types,
        CancellationToken cancellationToken)
    {
        // List to hold the results
        var models = new List<(string Name, object Model)>();

        foreach (var typeDeclSyntax in types)
        {
            // If the operation is canceled, abort here
            cancellationToken.ThrowIfCancellationRequested();

            // Get the declared symbol
            var semanticModel = compilation.GetSemanticModel(typeDeclSyntax.SyntaxTree);
            if (semanticModel.GetDeclaredSymbol(typeDeclSyntax, cancellationToken) is not INamedTypeSymbol declaredSymbol) continue;

            var model = GetModelToGenerate(declaredSymbol, cancellationToken);
            models.Add((declaredSymbol.ToDisplayString(), model));
        }

        return models;
    }

    private static IReadOnlyList<(string Name, string Text)> GenerateSourceFromModels(
        IEnumerable<(string Name, object Model)> models,
        CancellationToken cancellationToken)
    {
        // List of generated sources
        var sources = new List<(string Name, string Text)>();

        foreach (var (name, model) in models)
        {
            // If the operation is canceled, abort here
            cancellationToken.ThrowIfCancellationRequested();

            // Generate source code
            var source = GenerateSource(model, cancellationToken);
            sources.Add((name, source));
        }

        return sources;
    }

    private static object GetModelToGenerate(
        INamedTypeSymbol symbol,
        CancellationToken cancellationToken)
    {
        // TODO
        throw new NotImplementedException();
    }

    private static string GenerateSource(
        object model,
        CancellationToken cancellationToken)
    {
        // TODO
        throw new NotImplementedException();
    }
}
