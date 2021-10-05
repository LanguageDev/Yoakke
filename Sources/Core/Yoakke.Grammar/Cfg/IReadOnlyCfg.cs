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
    /// Represents a read-only context-free grammar.
    /// </summary>
    public interface IReadOnlyCfg
    {
        /// <summary>
        /// The start symbol of the grammar.
        /// </summary>
        public Nonterminal StartSymbol { get; }

        /// <summary>
        /// The terminals in this grammar.
        /// </summary>
        public IReadOnlyCollection<Terminal> Terminals { get; }

        /// <summary>
        /// The nonterminals in this grammar.
        /// </summary>
        public IReadOnlyCollection<Nonterminal> Nonterminals { get; }

        /// <summary>
        /// The productions in this grammar.
        /// </summary>
        public IReadOnlyCollection<Production> Productions { get; }

        /// <summary>
        /// Retrieves the collection of productions associated with the nonterminal <paramref name="nonterminal"/>.
        /// </summary>
        /// <param name="nonterminal">The nonterminal to get the productions for.</param>
        /// <returns>The productions associated with <paramref name="nonterminal"/>.</returns>
        public IReadOnlyCollection<IReadOnlyValueList<Symbol>> this[Nonterminal nonterminal] { get; }

        /// <summary>
        /// Converts this CFG to a TeX representation.
        /// </summary>
        /// <returns>The TeX code describing this grammar.</returns>
        public string ToTex();

        /// <summary>
        /// Checks, if a given symbol derives the empty word.
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>True, if <paramref name="symbol"/> derives the empty word.</returns>
        public bool DerivesEmpty(Symbol symbol);

        /// <summary>
        /// Gets the first-set of <paramref name="symbol"/>, which is all the terminals that can start the given symbol.
        /// </summary>
        /// <param name="symbol">The symbol to calculate the first-set for.</param>
        /// <returns>The <see cref="FirstSet"/> of <paramref name="symbol"/>.</returns>
        public FirstSet First(Symbol symbol);

        /// <summary>
        /// Gets the first-set of <paramref name="symbolSequence"/>, which is all the terminals that can start the given
        /// symbol sequence.
        /// </summary>
        /// <param name="symbolSequence">The symbol sequence to calculate the first-set for.</param>
        /// <returns>The <see cref="FirstSet"/> of <paramref name="symbolSequence"/>.</returns>
        public FirstSet First(IEnumerable<Symbol> symbolSequence);

        /// <summary>
        /// Gets the follow-set of <paramref name="nonterminal"/>, which is all the terminals that can follow the given symbol.
        /// </summary>
        /// <param name="nonterminal">The nonterminal symbol to calculate the follow-set for.</param>
        /// <returns>The <see cref="FollowSet"/> of <paramref name="nonterminal"/>.</returns>
        public FollowSet Follow(Nonterminal nonterminal);

        /// <summary>
        /// Generates sentences that the grammar accepts.
        /// </summary>
        /// <returns>A potentially infinite sequence of sentences.</returns>
        public IEnumerable<IEnumerable<Terminal>> GenerateSentences();
    }
}
