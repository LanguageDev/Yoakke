using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.SourceGenerator.Common
{
    public static class SymbolExtensions
    {
        public static bool IsNested(this ISymbol symbol) =>
            !SymbolEqualityComparer.Default.Equals(symbol.ContainingSymbol, symbol.ContainingNamespace);

        public static bool ImplementsInterface(this ITypeSymbol symbol, INamedTypeSymbol interf) =>
            symbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, interf));

        public static bool ImplementsGenericInterface(this ITypeSymbol symbol, INamedTypeSymbol interf) =>
               SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, interf)
            || symbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, interf));
    }
}
