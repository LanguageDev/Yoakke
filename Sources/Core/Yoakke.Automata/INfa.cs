// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Automata
{
    /// <summary>
    /// Represents a generic nondeterministic finite automaton.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public interface INfa<TState, TSymbol> : IReadOnlyNfa<TState, TSymbol>, IFiniteAutomaton<TState, TSymbol>
    {
        /// <summary>
        /// Adds an epsilon transition to this automaton.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="to">The state to transition to.</param>
        /// <returns>True, if the transition was new and successfully added.</returns>
        public bool AddEpsilonTransition(TState from, TState to);

        /// <summary>
        /// Removes an epsilon transition from this automaton.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="to">The state to transition to.</param>
        /// <returns>True, if the transition was found and successfully removed.</returns>
        public bool RemoveEpsilonTransition(TState from, TState to);

        /// <summary>
        /// Constructs an equivalent NFA that has no epsilon-transitions.
        /// </summary>
        /// <typeparam name="TResultState">The result state type.</typeparam>
        /// <param name="combiner">The state combiner to use.</param>
        /// <returns>The equivalent epsilon transition-less NFA.</returns>
        public new INfa<TResultState, TSymbol> EliminateEpsilonTransitions<TResultState>(IStateCombiner<TState, TResultState> combiner);

        /// <summary>
        /// Constructs an equivalent DFA from this NFA.
        /// </summary>
        /// <typeparam name="TResultState">The result state type.</typeparam>
        /// <param name="combiner">The state combiner to use.</param>
        /// <returns>The constructed DFA.</returns>
        public new IDfa<TResultState, TSymbol> Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner);
    }
}
