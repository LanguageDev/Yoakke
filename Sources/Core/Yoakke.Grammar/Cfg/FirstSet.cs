// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.Cfg
{
    /// <summary>
    /// Represents the first-set of some symbol.
    /// </summary>
    public readonly struct FirstSet
    {
        /// <summary>
        /// The symbol or symbols that the first-set was calculated for.
        /// </summary>
        public IReadOnlyList<Symbol> Symbols { get; }

        /// <summary>
        /// True, if the empty word (usually called epsilon) is part of this set.
        /// </summary>
        public bool HasEmpty { get; }

        /// <summary>
        /// The terminals present in the first-set.
        /// </summary>
        public IReadOnlyCollection<Terminal> Terminals { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstSet"/> struct.
        /// </summary>
        /// <param name="symbols">The symbols that the first-set was calculated for.</param>
        /// <param name="hasEmpty">True, if the empty word (usually called epsilon) is part of this set.</param>
        /// <param name="terminals">The terminals present in the first-set.</param>
        public FirstSet(IReadOnlyList<Symbol> symbols, bool hasEmpty, IReadOnlyCollection<Terminal> terminals)
        {
            this.Symbols = symbols;
            this.HasEmpty = hasEmpty;
            this.Terminals = terminals;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstSet"/> struct.
        /// </summary>
        /// <param name="symbol">The symbol that the first-set was calculated for.</param>
        /// <param name="hasEmpty">True, if the empty word (usually called epsilon) is part of this set.</param>
        /// <param name="terminals">The terminals present in the first-set.</param>
        public FirstSet(Symbol symbol, bool hasEmpty, IReadOnlyCollection<Terminal> terminals)
            : this(new[] { symbol }, hasEmpty, terminals)
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"FIRST({(this.Symbols.Count == 0 ? "ε" : string.Join(" ", this.Symbols))}) = {{ ");
            if (this.HasEmpty)
            {
                sb.Append('ε');
                if (this.Terminals.Count > 0) sb.Append(", ");
            }
            sb.Append(string.Join(", ", this.Terminals));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}
