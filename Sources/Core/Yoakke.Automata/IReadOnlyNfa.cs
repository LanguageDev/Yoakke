// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Automata
{
    /// <summary>
    /// Represents a generic, readonly nondeterministic finite automaton.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public interface IReadOnlyNfa<TState, TSymbol> : IReadOnlyFiniteAutomaton<TState, TSymbol>
    {
        /// <summary>
        /// Retrieves the epsilon-closure of a given state, which is all the states reachable with only
        /// epsilon transitions.
        /// </summary>
        /// <param name="state">The state to calculate the epsilon-closure for.</param>
        /// <returns>The states of the epsilon-closure.</returns>
        public StateSet<TState> EpsilonClosure(TState state);

        /// <summary>
        /// Constructs an equivalent DFA from this NFA.
        /// </summary>
        /// <returns>The constructed DFA.</returns>
        public IReadOnlyDfa<TState, StateSet<TSymbol>> Determinize();
    }
}
