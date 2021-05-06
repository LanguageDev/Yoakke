using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SourceGenerator.Common
{
    public static class SymbolExtensions
    {
        public static bool IsNested(this ISymbol symbol) =>
            !SymbolEqualityComparer.Default.Equals(symbol.ContainingSymbol, symbol.ContainingNamespace);
    }
}
