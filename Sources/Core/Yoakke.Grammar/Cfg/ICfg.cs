// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Values;

namespace Yoakke.Grammar.Cfg
{
    /// <summary>
    /// Represents a context-free grammar.
    /// </summary>
    public interface ICfg : IReadOnlyCfg
    {
        /// <summary>
        /// The start symbol of the grammar.
        /// </summary>
        public new Nonterminal StartSymbol { get; set; }

        /// <summary>
        /// The terminals in this grammar.
        /// </summary>
        public new ICollection<Terminal> Terminals { get; }

        /// <summary>
        /// The nonterminals in this grammar.
        /// </summary>
        public new ICollection<Nonterminal> Nonterminals { get; }

        /// <summary>
        /// The productions in this grammar.
        /// </summary>
        public new ICollection<Production> Productions { get; }

        /// <summary>
        /// Retrieves the collection of productions associated with the nonterminal <paramref name="nonterminal"/>.
        /// </summary>
        /// <param name="nonterminal">The nonterminal to get the productions for.</param>
        /// <returns>The productions associated with <paramref name="nonterminal"/>.</returns>
        public new ICollection<IReadOnlyValueList<Symbol>> this[Nonterminal nonterminal] { get; }

        /// <summary>
        /// Creates a new start symbol, that is guaranteed to be not recursive.
        /// </summary>
        public void AugmentStartSymbol();
    }
}
