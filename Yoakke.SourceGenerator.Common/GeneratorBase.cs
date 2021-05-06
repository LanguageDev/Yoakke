using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Yoakke.SourceGenerator.Common
{
    public abstract class GeneratorBase : ISourceGenerator
    {
        protected GeneratorExecutionContext Context { get; private set; }

        private string libraryName;
        private Dictionary<string, INamedTypeSymbol> symbolCache;

        public GeneratorBase(string libraryName)
        {
            this.libraryName = libraryName;
            this.symbolCache = new();
        }

        public void Initialize(GeneratorInitializationContext context) =>
            context.RegisterForSyntaxNotifications(() => CreateSyntaxReceiver(context));

        public void Execute(GeneratorExecutionContext context)
        {
            if (!IsOwnSyntaxReceiver(context.SyntaxReceiver)) return;

            this.Context = context;

            GenerateCode(context.SyntaxReceiver);
        }

        protected abstract ISyntaxReceiver CreateSyntaxReceiver(GeneratorInitializationContext context);
        protected abstract bool IsOwnSyntaxReceiver(ISyntaxReceiver syntaxReceiver);
        protected abstract void GenerateCode(ISyntaxReceiver syntaxReceiver);

        protected void Report(DiagnosticDescriptor descriptor, params object[] args) =>
            Context.ReportDiagnostic(Diagnostic.Create(descriptor, null, args));

        protected void Report(DiagnosticDescriptor descriptor, Location? location, params object[] args) =>
            Context.ReportDiagnostic(Diagnostic.Create(descriptor, location, args));

        protected INamedTypeSymbol LoadSymbol(string name)
        {
            if (!symbolCache.TryGetValue(name, out var value))
            {
                value = Context.Compilation.GetTypeByMetadataName(name);
                symbolCache.Add(name, value);
            }
            return value;
        }

        protected bool TryGetAttribute(ISymbol symbol, string attributeName, out AttributeData result)
        {
            var attrSymbol = LoadSymbol(attributeName);
            result = symbol
                .GetAttributes()
                .Where(attr => SymbolEquals(attr.AttributeClass, attrSymbol))
                .FirstOrDefault();
            return result != null;
        }

        protected bool HasAttribute(ISymbol symbol, string attributeName) =>
            TryGetAttribute(symbol, attributeName, out var _);

        protected AttributeData GetAttribute(ISymbol symbol, string attributeName)
        {
            if (!TryGetAttribute(symbol, attributeName, out var attr)) throw new KeyNotFoundException();
            return attr;
        }

        protected void AddSource(string fileName, string text) => Context.AddSource(fileName, text);

        protected bool RequireLibrary(string name)
        {
            if (!Context.Compilation.ReferencedAssemblyNames.Any(ai => ai.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                Report(Diagnostics.RequiredLibraryNotReferenced, name);
                return false;
            }
            return true;
        }

        protected bool RequirePartial(TypeDeclarationSyntax syntax)
        {
            if (!syntax.IsPartial())
            {
                Report(Diagnostics.TypeDefinitionIsNotPartial, syntax.GetLocation(), syntax.Identifier.ValueText);
                return false;
            }
            return true;
        }

        protected bool RequireNonNested(ISymbol symbol)
        {
            if (symbol.IsNested())
            {
                Report(Diagnostics.SymbolIsNested, symbol.Locations.First(), symbol.Name);
                return false;
            }
            return true;
        }

        protected static bool SymbolEquals(ISymbol sym1, ISymbol sym2) =>
            SymbolEqualityComparer.Default.Equals(sym1, sym2);
    }
}
