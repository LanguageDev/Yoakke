// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Yoakke.SourceGenerator.Common;

/// <summary>
/// Extensions for the Source Generator API.
/// </summary>
public static class SourceGeneratorExtensions
{
    /// <summary>
    /// Registers a post-initialization step for the source generator context to add some embedded resources to the
    /// compilation.
    /// </summary>
    /// <param name="context">The source generator context.</param>
    /// <param name="prefix">The prefix of the embedded sources to add.</param>
    public static void RegisterEmbeddedSourceCodeInjection(
        this IncrementalGeneratorInitializationContext context,
        string prefix)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var sourcesToInject = assembly
            .GetManifestResourceNames()
            .Where(m => m.StartsWith(prefix));
        context.RegisterPostInitializationOutput(ctx =>
        {
            foreach (var source in sourcesToInject)
            {
                ctx.AddSource(
                    source,
                    SourceText.From(assembly.ReadManifestResourceText(source), Encoding.UTF8));
            }
        });
    }

    /// <summary>
    /// Loads a required type from a compilation. Throws, if the type could not be loaded.
    /// </summary>
    /// <param name="compilation">The compilation to load the type from.</param>
    /// <param name="type">The type to load.</param>
    /// <returns>The loaded type symbol.</returns>
    public static INamedTypeSymbol GetRequiredType(
        this Compilation compilation,
        Type type) =>
           compilation.GetTypeByMetadataName(type.FullName)
        ?? throw new InvalidOperationException($"could not load type {type.Name}");

    /// <summary>
    /// Creates a syntax provider that looks for syntax nodes with a given attribute.
    /// </summary>
    /// <typeparam name="T">The exact syntax node type to search for.</typeparam>
    /// <param name="syntaxValueProvider">The syntax value provider to register on.</param>
    /// <param name="attributeType">The attribute type to search for.</param>
    /// <returns>The values provider with the syntax nodes attributed with an attribute of type
    /// <paramref name="attributeType"/>.</returns>
    public static IncrementalValuesProvider<T> CreateAttributedSyntaxProvider<T>(
        this SyntaxValueProvider syntaxValueProvider,
        Type attributeType)
        where T : MemberDeclarationSyntax =>
            syntaxValueProvider.CreateSyntaxProvider(
                // Filter for type declarations with at least one attribute
                predicate: (n, _) => IsSyntaxTargetForGeneration<T>(n),
                // Transform to either the semantic value or null, if not the right attribute
                transform: (ctx, ct) => GetSemanticTargetForGeneration<T>(ctx, attributeType.FullName, ct))
            // Discard null
            .Where(m => m is not null)!;

    /// <summary>
    /// Registers a two-phase source output step:
    ///  1) Loads an extra type using <paramref name="loadExtra"/>, this is the extra context object for the next step.
    ///  2) Transforms each syntax value into a generational model type using <paramref name="toGenerationModel"/>.
    ///  3) Transforms each generational model to a Scriban model using <paramref name="toScribanModel"/>.
    ///  4) Loads the Scriban template using <paramref name="loadScribanTemplate"/>.
    ///  5) Generates and adds the source of each rendered Scriban template to the compilation.
    /// </summary>
    /// <typeparam name="TSyntax">The syntax node type.</typeparam>
    /// <typeparam name="TGenerationModel">The generation model type.</typeparam>
    /// <typeparam name="TExtra">The type of the extra member passed in for model conversion.</typeparam>
    /// <param name="context">The generator initialization context to register on.</param>
    /// <param name="values">The incremental values provider connected to this generational step.</param>
    /// <param name="loadExtra">The function that loads extra context for the generation model transformation.</param>
    /// <param name="toGenerationModel">The function doing the generational model transformation.</param>
    /// <param name="toScribanModel">The function doing the Scriban model transformation.</param>
    /// <param name="loadScribanTemplate">The function loading the scriban template.</param>
    public static void RegisterTwoPhaseSourceOutput<TSyntax, TGenerationModel, TExtra>(
        this IncrementalGeneratorInitializationContext context,
        IncrementalValuesProvider<TSyntax> values,
        Func<Compilation, TExtra> loadExtra,
        Func<SourceProductionContext, TExtra, ISymbol, CancellationToken, TGenerationModel?> toGenerationModel,
        Func<TGenerationModel, CancellationToken, object?> toScribanModel,
        Func<Compilation, Scriban.Template> loadScribanTemplate)
        where TSyntax : MemberDeclarationSyntax
    {
        // Combine the collected syntaxes with the compilation
        var compilationAndSyntax = context.CompilationProvider.Combine(values.Collect());

        // Generate the sources
        context.RegisterSourceOutput(
            compilationAndSyntax,
            (spc, source) =>
            {
                var compilation = source.Left;
                var syntaxes = source.Right;

                // Do nothing on empty
                if (syntaxes.IsDefaultOrEmpty) return;

                // [LoggerMessage] does this, so we imitate
                var distinctSyntaxes = syntaxes.Distinct();

                // Load extra context
                var extra = loadExtra(compilation);

                // Convert each type to the proper model
                var models = GetGenerationModels(
                    compilation,
                    spc,
                    distinctSyntaxes,
                    extra,
                    toGenerationModel,
                    spc.CancellationToken);

                // If no models remain, we return
                if (models.Count == 0) return;

                // Otherwise we translate each model to a Scriban model
                var scribanModels = models
                    .Select(m => (
                        Name: m.DeclaredSymbol.ToDisplayString(),
                        ScribanModel: toScribanModel(m.GenerationModel, spc.CancellationToken)))
                    .Where(m => m.ScribanModel is not null);

                // Load the Scriban template
                var template = loadScribanTemplate(compilation);

                // We generate each source text with the Scriban template
                var sources = GenerateSources(template, scribanModels!, spc.CancellationToken);

                // Finally add all sources
                foreach (var (name, text) in sources)
                {
                    var sourceName = SanitizeFileName($"{name}.Generated.cs");
                    spc.AddSource(sourceName, SourceText.From(text, Encoding.UTF8));
                }
            });
    }

    private static bool IsSyntaxTargetForGeneration<T>(SyntaxNode node)
        where T : MemberDeclarationSyntax =>
        node is T tSyntax && tSyntax.AttributeLists.Count > 0;

    private static T? GetSemanticTargetForGeneration<T>(
        GeneratorSyntaxContext context,
        string attributeFullName,
        CancellationToken cancellationToken)
        where T : MemberDeclarationSyntax
    {
        var tSyntax = (T)context.Node;

        // Loop through all the attributes on the method
        foreach (var attributeListSyntax in tSyntax.AttributeLists)
        {
            // Loop through each attribute annotation in the list
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax, cancellationToken).Symbol is not IMethodSymbol attributeSymbol) continue;

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                // Is the attribute the searched attribute
                if (fullName == attributeFullName) return tSyntax;
            }
        }

        // We didn't find the attribute we were looking for
        return null;
    }

    private static IReadOnlyList<(TGenerationModel GenerationModel, ISymbol DeclaredSymbol)>
        GetGenerationModels<TSyntax, TExtra, TGenerationModel>(
        Compilation compilation,
        SourceProductionContext context,
        IEnumerable<TSyntax> syntaxes,
        TExtra extra,
        Func<SourceProductionContext, TExtra, ISymbol, CancellationToken, TGenerationModel?> toGenerationModel,
        CancellationToken cancellationToken)
        where TSyntax : MemberDeclarationSyntax
    {
        // List to hold the results
        var models = new List<(TGenerationModel GenerationModel, ISymbol DeclaredSymbol)>();

        foreach (var syntax in syntaxes)
        {
            // If the operation is canceled, abort here
            cancellationToken.ThrowIfCancellationRequested();

            // Get the declared symbol
            var semanticModel = compilation.GetSemanticModel(syntax.SyntaxTree);
            var symbol = semanticModel.GetDeclaredSymbol(syntax, cancellationToken);
            if (symbol is null) continue;

            // Generate the model
            var model = toGenerationModel(context, extra, symbol, cancellationToken);
            if (model is null) continue;

            models.Add((model, symbol));
        }

        return models;
    }

    private static IReadOnlyList<(TGenerationModel GenerationModel, string Text)> GenerateSources<TGenerationModel>(
        Scriban.Template template,
        IEnumerable<(TGenerationModel GenerationModel, object ScribanModel)> models,
        CancellationToken cancellationToken)
    {
        // List of generated sources
        var sources = new List<(TGenerationModel GenerationModel, string Text)>();

        foreach (var (genModel, sbnModel) in models)
        {
            // If the operation is canceled, abort here
            cancellationToken.ThrowIfCancellationRequested();

            // Generate source code
            var source = template.Render(model: sbnModel, format: true, cancellationToken);
            sources.Add((genModel, source));
        }

        return sources;
    }

    private static string SanitizeFileName(string str) => str
        .Replace("<", "_lt_")
        .Replace(">", "_gt_");
}
