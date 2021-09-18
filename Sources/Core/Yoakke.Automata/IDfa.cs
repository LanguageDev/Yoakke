// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Automata
{
    /// <summary>
    /// Represents a generic deterministic finite automaton.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public interface IDfa<TState, TSymbol> : IReadOnlyDfa<TState, TSymbol>, IFiniteAutomaton<TState, TSymbol>
    {
        /// <summary>
        /// Completes this DFA over the given alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet to complete over.</param>
        /// <param name="trap">A default trap state to transition to.</param>
        /// <returns>True, if this DFA was not completed and needed completion, false otherwise.</returns>
        public bool Complete(IEnumerable<TSymbol> alphabet, TState trap);

        /// <summary>
        /// Minimizes this DFA into an equivalent one with the least amount of states possible.
        /// </summary>
        /// <param name="differentiatePairs">The pairs of states that must not be merged in the minimization process.
        /// This can be useful if some states have associated values to them that we want to keep.</param>
        /// <returns>The minimized DFA.</returns>
        public new IDfa<StateSet<TState>, TSymbol> Minimize(IEnumerable<(TState, TState)> differentiatePairs);

        /// <summary>
        /// Minimizes this DFA into an equivalent one with the least amount of states possible.
        /// </summary>
        /// <param name="differentiate">The states that must not be merged with anything in the minimization process.
        /// This can be useful if some states have associated values to them that we want to keep.</param>
        /// <returns>The minimized DFA.</returns>
        public new IDfa<StateSet<TState>, TSymbol> Minimize(IEnumerable<TState> differentiate);

        /// <summary>
        /// Minimizes this DFA into an equivalent one with the least amount of states possible.
        /// </summary>
        /// <returns>The minimized DFA.</returns>
        public new IDfa<StateSet<TState>, TSymbol> Minimize();
    }
}
