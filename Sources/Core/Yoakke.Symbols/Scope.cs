// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;

namespace Yoakke.Symbols
{
    /// <summary>
    /// A simple scope implementation with a tag type.
    /// </summary>
    /// <typeparam name="TTag">The tag type associated with the scope.</typeparam>
    public class Scope<TTag> : IScope
        where TTag : struct
    {
        /// <summary>
        /// The tag associated with the scope.
        /// </summary>
        public TTag Tag { get; }

        /// <inheritdoc/>
        public IReadOnlyScope? ContainingScope { get; }

        /// <inheritdoc/>
        public bool IsGlobal => this.ContainingScope is null;

        /// <inheritdoc/>
        public IReadOnlyDictionary<string, ISymbol> Symbols => this.symbols;

        private readonly Dictionary<string, ISymbol> symbols = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="Scope{TTag}"/> class.
        /// </summary>
        /// <param name="parent">The parent (containing) scope of this one.</param>
        /// <param name="tag">The tag of this scope.</param>
        public Scope(IReadOnlyScope? parent = null, TTag tag = default)
        {
            this.ContainingScope = parent;
            this.Tag = tag;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scope{TTag}"/> class.
        /// </summary>
        /// <param name="tag">The tag of this scope.</param>
        public Scope(TTag tag)
            : this(null, tag)
        {
        }

        /// <inheritdoc/>
        public IEnumerable<ISymbol> GetReachableSymbols() => this.ContainingScope is null
            ? this.Symbols.Values
            : this.Symbols.Values.Concat(this.ContainingScope.GetReachableSymbols());

        /// <inheritdoc/>
        public ISymbol? ReferenceSymbol(string name) => this.ReferenceSymbol(name, _ => true);

        /// <inheritdoc/>
        public ISymbol? ReferenceSymbol(string name, Predicate<IReadOnlyScope> canCross)
        {
            if (this.Symbols.TryGetValue(name, out var symbol)) return symbol;
            if (canCross(this) && this.ContainingScope is not null) return this.ContainingScope.ReferenceSymbol(name, canCross);
            return null;
        }

        /// <inheritdoc/>
        public void DefineSymbol(ISymbol symbol)
        {
            if (symbol.Scope != this)
            {
                throw new ArgumentException("the scope of the symbol is not set to this", nameof(symbol));
            }
            this.symbols[symbol.Name] = symbol;
        }
    }
}
