// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Yoakke.SourceGenerator.Common.RoslynExtensions;

namespace Yoakke.SourceGenerator.Common
{
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
        /// Tries to retrieve <see cref="AttributeData"/> attached to a given <see cref="ISymbol"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to searched the attribute on.</param>
        /// <param name="attributeName">The fully-qualified name of the attribute type.</param>
        /// <param name="result">The found <see cref="AttributeData"/> gets written here, if the attribute
        /// is attached onto <paramref name="symbol"/>.</param>
        /// <returns>True, if the attribute with type name <paramref name="attributeName"/> was found on
        /// <paramref name="symbol"/>.</returns>
        protected bool TryGetAttribute(ISymbol symbol, string attributeName, out AttributeData result)
        {
            var attrSymbol = this.LoadSymbol(attributeName);
            result = symbol
                .GetAttributes()
                .Where(attr => SymbolEquals(attr.AttributeClass, attrSymbol))
                .FirstOrDefault();
            return result != null;
        }

        /// <summary>
        /// Checks if a given <see cref="ISymbol"/> has a given attribute on it.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check for the attribute.</param>
        /// <param name="attributeName">The fully-qualified name of the attribute type.</param>
        /// <returns>True, if the attribute with type name <paramref name="attributeName"/> was found on
        /// <paramref name="symbol"/>.</returns>
        protected bool HasAttribute(ISymbol symbol, string attributeName) =>
            this.TryGetAttribute(symbol, attributeName, out var _);

        /// <summary>
        /// Retrieves an <see cref="AttributeData"/> attached onto a <see cref="ISymbol"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to search for the attribute.</param>
        /// <param name="attributeName">The fully-qualified name of the attribute type.</param>
        /// <returns>The <see cref="AttributeData"/> attached to <paramref name="symbol"/> with
        /// type name <paramref name="attributeName"/>.</returns>
        protected AttributeData GetAttribute(ISymbol symbol, string attributeName)
        {
            if (!this.TryGetAttribute(symbol, attributeName, out var attr)) throw new KeyNotFoundException();
            return attr;
        }

        /// <summary>
        /// Adds a source-file to the current <see cref="GeneratorExecutionContext"/>.
        /// </summary>
        /// <param name="fileName">The name of the file to add.</param>
        /// <param name="text">The contents of the file to add.</param>
        protected void AddSource(string fileName, string text) => this.Context.AddSource(fileName, text);

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
        /// Shorthand for <see cref="ISymbol"/> equality.
        /// </summary>
        /// <param name="sym1">The first <see cref="ISymbol"/> to compare.</param>
        /// <param name="sym2">The second <see cref="ISymbol"/> to compare.</param>
        /// <returns>True, if the two symbols refer to the same thing.</returns>
        protected static bool SymbolEquals(ISymbol? sym1, ISymbol? sym2) =>
            SymbolEqualityComparer.Default.Equals(sym1, sym2);
    }
}
