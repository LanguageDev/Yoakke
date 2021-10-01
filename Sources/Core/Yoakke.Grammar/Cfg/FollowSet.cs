// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.Cfg
{
    /// <summary>
    /// Represents the follow-set of some symbol.
    /// </summary>
    public readonly struct FollowSet
    {
        /// <summary>
        /// The symbol or symbols that the follow-set was calculated for.
        /// </summary>
        public IReadOnlyList<Symbol> Symbols { get; }

        /// <summary>
        /// The terminals present in the follow-set.
        /// </summary>
        public IReadOnlyCollection<Symbol.Terminal> Terminals { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FollowSet"/> struct.
        /// </summary>
        /// <param name="symbols">The symbols that the follow-set was calculated for.</param>
        /// <param name="terminals">The terminals present in the follow-set.</param>
        public FollowSet(IReadOnlyList<Symbol> symbols, IReadOnlyCollection<Symbol.Terminal> terminals)
        {
            this.Symbols = symbols;
            this.Terminals = terminals;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FollowSet"/> struct.
        /// </summary>
        /// <param name="symbol">The symbol that the follow-set was calculated for.</param>
        /// <param name="terminals">The terminals present in the follow-set.</param>
        public FollowSet(Symbol symbol, IReadOnlyCollection<Symbol.Terminal> terminals)
            : this(new[] { symbol }, terminals)
        {
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"FOLLOW({(this.Symbols.Count == 0 ? "ε" : string.Join(" ", this.Symbols))}) = {{ ");
            sb.Append(string.Join(", ", this.Terminals));
            sb.Append(" }");
            return sb.ToString();
        }
    }
}
