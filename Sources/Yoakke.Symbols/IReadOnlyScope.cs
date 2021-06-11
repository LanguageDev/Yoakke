// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;

namespace Yoakke.Symbols
{
    /// <summary>
    /// A scope containing <see cref="ISymbol"/>s in a hierarchy.
    /// </summary>
    public interface IReadOnlyScope
    {
        /// <summary>
        /// The containing scope of this one.
        /// </summary>
        public IReadOnlyScope? ContainingScope { get; }

        /// <summary>
        /// True, if this is the global scope.
        /// </summary>
        public bool IsGlobal { get; }

        /// <summary>
        /// The symbols defined in this scope.
        /// </summary>
        public IReadOnlyDictionary<string, ISymbol> Symbols { get; }

        /// <summary>
        /// Enumerates over all accessible symbols from this scope.
        /// </summary>
        /// <returns>An enumerator that enumerates all symbols from this scope.</returns>
        public IEnumerable<ISymbol> GetReachableSymbols();

        /// <summary>
        /// Tries to reference a symbol in this or any parent scope.
        ///
        /// The predicate is useful for enforcing function-local scoping for example.
        /// </summary>
        /// <param name="name">The name of the symbol to reference.</param>
        /// <param name="canCross">A predicate that decides if the search can cross a given scope boundlary.</param>
        /// <returns>The symbol, if found, null otherwise.</returns>
        public ISymbol? ReferenceSymbol(string name, Predicate<IReadOnlyScope> canCross);

        /// <summary>
        /// Tries to reference a symbol in this or any parent scope.
        /// </summary>
        /// <param name="name">The name of the symbol to reference.</param>
        /// <returns>The symbol, if found, null otherwise.</returns>
        public ISymbol? ReferenceSymbol(string name);
    }
}
