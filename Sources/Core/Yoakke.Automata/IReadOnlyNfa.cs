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
        /// The initial states of the automaton.
        /// </summary>
        public IReadOnlyCollection<TState> InitialStates { get; }

        /// <summary>
        /// Gets all transitions that are valid from a set of states on an input.
        /// </summary>
        /// <param name="from">The set of states to transition from.</param>
        /// <param name="on">The symbol to transition on.</param>
        /// <returns>The set of states that are valid from the starting set on the input.</returns>
        public StateSet<TState> GetTransitions(StateSet<TState> from, TSymbol on);

        /// <summary>
        /// Checks if this automaton results in an accepting state for the given input.
        /// </summary>
        /// <param name="initial">The initial set of states to start from.</param>
        /// <param name="input">The sequence of input to feed in for the automaton.</param>
        /// <returns>True, if the automaton accepts the input, false otherwise.</returns>
        public bool Accepts(StateSet<TState> initial, IEnumerable<TSymbol> input);

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
        /// <typeparam name="TResultState">The result state type.</typeparam>
        /// <param name="combiner">The state combiner to use.</param>
        /// <returns>The constructed DFA.</returns>
        public IDfa<TResultState, TSymbol> Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner);
    }
}
