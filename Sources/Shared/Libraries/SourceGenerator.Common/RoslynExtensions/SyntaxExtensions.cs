// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Yoakke.SourceGenerator.Common.RoslynExtensions;

/// <summary>
/// Extension functionalify for syntax elements.
/// </summary>
public static class SyntaxExtensions
{
    /// <summary>
    /// Checks, if a <see cref="TypeDeclarationSyntax"/> node is partial.
    /// </summary>
    /// <param name="syntax">The <see cref="TypeDeclarationSyntax"/> to check.</param>
    /// <returns>True, if <paramref name="syntax"/> is declared partial.</returns>
    public static bool IsPartial(this TypeDeclarationSyntax syntax) =>
        syntax.Modifiers.Any(SyntaxKind.PartialKeyword);
}
