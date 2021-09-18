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
        /// Removes all epsilon transitions from this NFA (while keeping it equivalent).
        /// </summary>
        /// <returns>True, if there were epsilon transitions to remove, false otherwise.</returns>
        public bool RemoveEpsilonTransitions();

        /// <summary>
        /// Determinizes this nondeterministic finite automaton into a deterministic one.
        /// </summary>
        /// <returns>The constructed DFA.</returns>
        public new IDfa<StateSet<TState>, TSymbol> Determinize();
    }
}
