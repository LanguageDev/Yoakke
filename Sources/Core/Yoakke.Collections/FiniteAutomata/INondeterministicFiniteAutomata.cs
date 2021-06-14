// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.Collections.FiniteAutomata
{
    /// <summary>
    /// Represents a nondeterministic finite automata that has multiple ways to step on a symbol as
    /// well as epsilon transitions.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type the automata steps on.</typeparam>
    public interface INondeterministicFiniteAutomata<TSymbol> : IFiniteAutomata<TSymbol>
    {
        /// <summary>
        /// Retrieves the epsilon-closure of a given state, which is all the states reachable with only
        /// epsilon transitions.
        /// </summary>
        /// <param name="state">The state to calculate the epsilon-closure from.</param>
        /// <returns>The states of the epsilon-closure.</returns>
        public IEnumerable<State> EpsilonClosure(State state);

        /// <summary>
        /// Determinizes this nondeterministic finite automaton into a deterministic one.
        /// </summary>
        /// <returns>The constructed DFA.</returns>
        public IDeterministicFiniteAutomata<TSymbol> Determinize();
    }
}
