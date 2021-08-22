// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.Utilities.FiniteAutomata
{
    /// <summary>
    /// An interface for all finite automata.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type the automata steps on.</typeparam>
    public interface IFiniteAutomata<TSymbol>
    {
        /// <summary>
        /// The initial state of the automata.
        /// </summary>
        public State InitalState { get; }

        /// <summary>
        /// The accepting states.
        /// </summary>
        public IEnumerable<State> AcceptingStates { get; }

        /// <summary>
        /// All states in the automata.
        /// </summary>
        public IEnumerable<State> States { get; }

        /// <summary>
        /// Checks if the given state is an accepting state in this automata.
        /// </summary>
        /// <param name="state">The state to check.</param>
        /// <returns>True, if the state is accepting, false otherwise.</returns>
        public bool IsAccepting(State state);

        /// <summary>
        /// Gets all the possible transitions from a given state and on a given symbol.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="on">The symbol to transition on.</param>
        /// <returns>The possible resulting states.</returns>
        public IEnumerable<State> GetTransitions(State from, TSymbol on);

        /// <summary>
        /// Converts this finite automata into a DOT graph that can be visualized for debugging.
        /// </summary>
        /// <returns>The DOT description of the automata.</returns>
        public string ToDebugDOT();
    }
}
