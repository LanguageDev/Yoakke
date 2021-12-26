// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Yoakke.SourceGenerator.Common.RoslynExtensions;

namespace Yoakke.SourceGenerator.Common;

/// <summary>
/// Abstraction for <see cref="ISourceGenerator"/>s to provide some common operations.
/// </summary>
public abstract class GeneratorBase : ISourceGenerator
{
    /// <summary>
    /// The current <see cref="GeneratorExecutionContext"/>.
    /// </summary>
    protected GeneratorExecutionContext Context { get; private set; }

    private readonly string libraryName;
    private readonly Dictionary<string, INamedTypeSymbol> symbolCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratorBase"/> class.
    /// </summary>
    /// <param name="libraryName">The name of the library this generator is for.</param>
    protected GeneratorBase(string libraryName)
    {
        this.libraryName = libraryName;
        this.symbolCache = new();
    }

    /// <inheritdoc/>
    public void Initialize(GeneratorInitializationContext context) =>
        context.RegisterForSyntaxNotifications(() => this.CreateSyntaxReceiver(context));

    /// <inheritdoc/>
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxReceiver is null) return;
        if (!this.IsOwnSyntaxReceiver(context.SyntaxReceiver)) return;

        this.Context = context;

        this.GenerateCode(context.SyntaxReceiver);
    }

    /// <summary>
    /// Creates a proper <see cref="ISyntaxReceiver"/> for this source generator.
    /// </summary>
    /// <param name="context">The received <see cref="GeneratorInitializationContext"/>.</param>
    /// <returns>The <see cref="ISyntaxReceiver"/> to be used by the source generator.</returns>
    protected abstract ISyntaxReceiver CreateSyntaxReceiver(GeneratorInitializationContext context);

    /// <summary>
    /// Checks if the <see cref="ISyntaxReceiver"/> is one that belongs to this source generator.
    /// Usually an 'is' type-check.
    /// </summary>
    /// <param name="syntaxReceiver">The <see cref="ISyntaxReceiver"/> to check.</param>
    /// <returns>True, if <paramref name="syntaxReceiver"/> belongs to this source generator.</returns>
    protected abstract bool IsOwnSyntaxReceiver(ISyntaxReceiver syntaxReceiver);

    /// <summary>
    /// Commands the source generator to do the code generation.
    /// </summary>
    /// <param name="syntaxReceiver">The <see cref="ISyntaxReceiver"/> that this source generator created.</param>
    protected abstract void GenerateCode(ISyntaxReceiver syntaxReceiver);

    /// <summary>
    /// Utility to report a <see cref="Diagnostic"/>.
    /// </summary>
    /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/> for the <see cref="Diagnostic"/>.</param>
    /// <param name="args">The arguments for the formatted diagnostic message.</param>
    protected void Report(DiagnosticDescriptor descriptor, params object[] args) =>
        this.Context.ReportDiagnostic(Diagnostic.Create(descriptor, null, args));

    /// <summary>
    /// Utility to report a <see cref="Diagnostic"/>.
    /// </summary>
    /// <param name="descriptor">The <see cref="DiagnosticDescriptor"/> for the <see cref="Diagnostic"/>.</param>
    /// <param name="location">The <see cref="Location"/> of the <see cref="Diagnostic"/>.</param>
    /// <param name="args">The arguments for the formatted diagnostic message.</param>
    protected void Report(DiagnosticDescriptor descriptor, Location location, params object[] args) =>
        this.Context.ReportDiagnostic(Diagnostic.Create(descriptor, location, args));

    /// <summary>
    /// Loads an <see cref="INamedTypeSymbol"/>, caching the result for later use.
    /// </summary>
    /// <param name="name">The fully-qualified name of the symbol.</param>
    /// <returns>The loaded <see cref="INamedTypeSymbol"/>.</returns>
    protected INamedTypeSymbol LoadSymbol(string name)
    {
        if (!this.symbolCache.TryGetValue(name, out var value))
        {
            value = this.Context.Compilation.GetTypeByMetadataName(name);
            if (value is null) throw new ArgumentException("can't load symbol with name", nameof(name));
            this.symbolCache.Add(name, value);
        }
        return value;
    }

    /// <summary>
    /// Adds a source-file to the current <see cref="GeneratorExecutionContext"/>.
    /// </summary>
    /// <param name="fileName">The name of the file to add.</param>
    /// <param name="text">The contents of the file to add.</param>
    protected void AddSource(string fileName, string text) => this.Context.AddSource(SanitizeFileName(fileName), text);

    /// <summary>
    /// Requires a library to be referenced by the user.
    /// If it is not found, a <see cref="Diagnostic"/> error is raised.
    /// </summary>
    /// <param name="name">The fully-qualified name of the library to require.</param>
    /// <returns>True, if the library was found.</returns>
    protected bool RequireLibrary(string name)
    {
        if (!this.Context.Compilation.ReferencedAssemblyNames.Any(ai => ai.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            this.Report(Diagnostics.RequiredLibraryNotReferenced, name);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Requires a <see cref="TypeDeclarationSyntax"/> node to be declared partial.
    /// If it is not, a <see cref="Diagnostic"/> error is raised.
    /// </summary>
    /// <param name="syntax">The <see cref="TypeDeclarationSyntax"/> to check.</param>
    /// <returns>True, if <paramref name="syntax"/> is partial.</returns>
    protected bool RequirePartial(TypeDeclarationSyntax syntax)
    {
        if (!syntax.IsPartial())
        {
            this.Report(Diagnostics.TypeDefinitionIsNotPartial, syntax.GetLocation(), syntax.Identifier.ValueText);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Requires an <see cref="ITypeSymbol"/> to be declared partial.
    /// If it is not, a <see cref="Diagnostic"/> error is raised.
    /// </summary>
    /// <param name="symbol">The <see cref="ITypeSymbol"/> to check.</param>
    /// <returns>True, if <paramref name="symbol"/> is partial.</returns>
    protected bool RequirePartial(ITypeSymbol symbol)
    {
        if (!symbol.IsPartial())
        {
            this.Report(Diagnostics.TypeDefinitionIsNotPartial, symbol.Locations.First(), symbol.Name);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Requires a <see cref="ISymbol"/> not to be a nested type.
    /// If it is a nested type, a <see cref="Diagnostic"/> error is raised.
    /// </summary>
    /// <param name="symbol">The <see cref="ISymbol"/> to check.</param>
    /// <returns>True, if <paramref name="symbol"/> is not nested.</returns>
    protected bool RequireNonNested(ISymbol symbol)
    {
        if (symbol.IsNested())
        {
            this.Report(Diagnostics.SymbolIsNested, symbol.Locations.First(), symbol.Name);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Requires a <see cref="ISymbol"/> to be able to accept declarations inside from external sources.
    /// This can mean that the symbol is a namespace or partial.
    /// If it does not, a <see cref="Diagnostic"/> error is raised.
    /// </summary>
    /// <param name="symbol">The <see cref="ISymbol"/> to check.</param>
    /// <returns>True, if <paramref name="symbol"/> accepts declarations inside from external sources.</returns>
    protected bool RequireDeclarableInside(ISymbol symbol)
    {
        if (!symbol.CanDeclareInsideExternally())
        {
            this.Report(Diagnostics.SymbolDoesNotAcceptExternalDeclarations, symbol.Locations.First(), symbol.Name);
            return false;
        }
        return true;
    }

    private static string SanitizeFileName(string str) => str
        .Replace("<", "_lt_")
        .Replace(">", "_gt_");
}
