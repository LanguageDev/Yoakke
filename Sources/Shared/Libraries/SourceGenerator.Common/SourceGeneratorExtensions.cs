// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Yoakke.SourceGenerator.Common;

/// <summary>
/// Extensions for the Source Generator API.
/// </summary>
public static class SourceGeneratorExtensions
{
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
}
