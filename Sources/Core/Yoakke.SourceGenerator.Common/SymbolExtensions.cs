// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Yoakke.SourceGenerator.Common
{
    /// <summary>
    /// Extension functionalities for <see cref="ISymbol"/>s.
    /// </summary>
    public static class SymbolExtensions
    {
        /// <summary>
        /// Retrieves the type kind of a <see cref="ISymbol"/> that can be used in codegen.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to get the kind of.</param>
        /// <returns>The code-friendly name of the type kind.</returns>
        public static string GetTypeKindName(this ITypeSymbol symbol) => symbol.TypeKind switch
        {
            TypeKind.Class => symbol.IsRecord ? "record" : "class",
            TypeKind.Struct => "struct",
            TypeKind.Interface => "interface",
            _ => throw new NotSupportedException(),
        };

        /// <summary>
        /// Checks, if a <see cref="ISymbol"/> is a nested type.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check.</param>
        /// <returns>True, if <paramref name="symbol"/> is a nested type.</returns>
        public static bool IsNested(this ISymbol symbol) =>
            !SymbolEqualityComparer.Default.Equals(symbol.ContainingSymbol, symbol.ContainingNamespace);

        /// <summary>
        /// Checks, if a <see cref="ISymbol"/> implements a given interface.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check the interface for.</param>
        /// <param name="interf">The interface symbol to search for.</param>
        /// <returns>True, if <paramref name="symbol"/> implements <paramref name="interf"/>.</returns>
        public static bool ImplementsInterface(this ITypeSymbol symbol, INamedTypeSymbol interf) =>
            symbol.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, interf));

        /// <summary>
        /// Checks, if a <see cref="ISymbol"/> implements a generic interface.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check the interface for.</param>
        /// <param name="interf">The generic interface symbol to search for.</param>
        /// <returns>True, if <paramref name="symbol"/> implements <paramref name="interf"/>.</returns>
        public static bool ImplementsGenericInterface(this ITypeSymbol symbol, INamedTypeSymbol interf) =>
            ImplementsGenericInterface(symbol, interf, out var _);

        /// <summary>
        /// Checks, if a <see cref="ISymbol"/> implements a generic interface.
        /// </summary>
        /// <param name="symbol">The <see cref="ISymbol"/> to check the interface for.</param>
        /// <param name="interf">The generic interface symbol to search for.</param>
        /// <param name="genericArgs">The passed in type-arguments for <paramref name="interf"/> are written here,
        /// in case <paramref name="symbol"/> implements it.</param>
        /// <returns>True, if <paramref name="symbol"/> implements <paramref name="interf"/>.</returns>
        public static bool ImplementsGenericInterface(
            this ITypeSymbol symbol,
            INamedTypeSymbol interf,
            [MaybeNullWhen(false)] out IReadOnlyList<ITypeSymbol>? genericArgs)
        {
            if (SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, interf))
            {
                genericArgs = ((INamedTypeSymbol)symbol).TypeArguments;
                return true;
            }
            var sub = symbol.AllInterfaces.FirstOrDefault(i => SymbolEqualityComparer.Default.Equals(i.OriginalDefinition, interf));
            if (sub != null)
            {
                genericArgs = sub.TypeArguments;
                return true;
            }
            genericArgs = null;
            return false;
        }
    }
}
