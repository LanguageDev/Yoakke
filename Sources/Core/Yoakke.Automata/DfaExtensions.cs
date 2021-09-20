// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Automata
{
    /// <summary>
    /// Extension functionality for DFAs.
    /// </summary>
    public static class DfaExtensions
    {
        /// <summary>
        /// Minimizes the DFA.
        /// </summary>
        /// <typeparam name="TState">The state type.</typeparam>
        /// <typeparam name="TSymbol">The symbol type.</typeparam>
        /// <typeparam name="TResultState">The resulting state type.</typeparam>
        /// <param name="dfa">The DFA to minimize.</param>
        /// <param name="combiner">The state combiner to use.</param>
        /// <param name="differentiate">The list of states to differentiate from every other state.</param>
        /// <returns>The minimized DFA.</returns>
        public static IDfa<TResultState, TSymbol> Minimize<TState, TSymbol, TResultState>(
            this IReadOnlyDfa<TState, TSymbol> dfa,
            IStateCombiner<TState, TResultState> combiner,
            IEnumerable<TState> differentiate) =>
            dfa.Minimize(combiner, differentiate.SelectMany(s1 => dfa.States.Select(s2 => (s1, s2))));

        /// <summary>
        /// Minimizes the DFA.
        /// </summary>
        /// <typeparam name="TState">The state type.</typeparam>
        /// <typeparam name="TSymbol">The symbol type.</typeparam>
        /// <typeparam name="TResultState">The resulting state type.</typeparam>
        /// <param name="dfa">The DFA to minimize.</param>
        /// <param name="combiner">The state combiner to use.</param>
        /// <returns>The minimized DFA.</returns>
        public static IDfa<TResultState, TSymbol> Minimize<TState, TSymbol, TResultState>(
            this IReadOnlyDfa<TState, TSymbol> dfa,
            IStateCombiner<TState, TResultState> combiner) =>
            dfa.Minimize(combiner, Enumerable.Empty<(TState, TState)>());

        /// <summary>
        /// Minimizes the DFA.
        /// </summary>
        /// <typeparam name="TState">The state type.</typeparam>
        /// <typeparam name="TSymbol">The symbol type.</typeparam>
        /// <param name="dfa">The DFA to minimize.</param>
        /// <returns>The minimized DFA.</returns>
        public static IDfa<StateSet<TState>, TSymbol> Minimize<TState, TSymbol>(this IReadOnlyDfa<TState, TSymbol> dfa) =>
            dfa.Minimize(StateCombiner<TState>.ToSetCombiner(dfa.StateComparer), Enumerable.Empty<(TState, TState)>());

        /// <summary>
        /// Minimizes the DFA.
        /// </summary>
        /// <typeparam name="TState">The state type.</typeparam>
        /// <typeparam name="TSymbol">The symbol type.</typeparam>
        /// <param name="dfa">The DFA to minimize.</param>
        /// <param name="differentiatePairs">The pairs of states to differentiate.</param>
        /// <returns>The minimized DFA.</returns>
        public static IDfa<StateSet<TState>, TSymbol> Minimize<TState, TSymbol>(
            this IReadOnlyDfa<TState, TSymbol> dfa,
            IEnumerable<(TState, TState)> differentiatePairs) =>
            dfa.Minimize(StateCombiner<TState>.ToSetCombiner(dfa.StateComparer), differentiatePairs);

        /// <summary>
        /// Minimizes the DFA.
        /// </summary>
        /// <typeparam name="TState">The state type.</typeparam>
        /// <typeparam name="TSymbol">The symbol type.</typeparam>
        /// <param name="dfa">The DFA to minimize.</param>
        /// <param name="differentiate">The states to differentiate from every other state.</param>
        /// <returns>The minimized DFA.</returns>
        public static IDfa<StateSet<TState>, TSymbol> Minimize<TState, TSymbol>(
            this IReadOnlyDfa<TState, TSymbol> dfa,
            IEnumerable<TState> differentiate) =>
            dfa.Minimize(StateCombiner<TState>.ToSetCombiner(dfa.StateComparer), differentiate);
    }
}
