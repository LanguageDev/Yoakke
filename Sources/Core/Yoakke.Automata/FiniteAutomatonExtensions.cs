// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Automata
{
    /// <summary>
    /// Extensions for finite automatons.
    /// </summary>
    public static class FiniteAutomatonExtensions
    {
        /// <summary>
        /// Finds all the reachable states from the initial state.
        /// </summary>
        /// <typeparam name="TState">The state type.</typeparam>
        /// <typeparam name="TSymbol">The symbol type.</typeparam>
        /// <param name="automaton">The automaton to check reachability in.</param>
        /// <returns>The sequence of reachable states.</returns>
        public static IEnumerable<TState> ReachableStates<TState, TSymbol>(
            this IReadOnlyFiniteAutomaton<TState, TSymbol> automaton) =>
            automaton.ReachableStates(automaton.InitialState);

        /// <summary>
        /// Checks if this automaton results in an accepting state for the given input.
        /// </summary>
        /// <typeparam name="TState">The state type.</typeparam>
        /// <typeparam name="TSymbol">The symbol type.</typeparam>
        /// <param name="automaton">The automaton to check acceptance on.</param>
        /// <param name="input">The sequence of input to feed in for the automaton.</param>
        /// <returns>True, if the automaton accepts the input, false otherwise.</returns>
        public static bool Accepts<TState, TSymbol>(
            this IReadOnlyFiniteAutomaton<TState, TSymbol> automaton,
            IEnumerable<TSymbol> input) => automaton.Accepts(automaton.InitialState, input);

        /// <summary>
        /// Removes all unreachable states and transitions from the automaton (looking from the initial state).
        /// </summary>
        /// <typeparam name="TState">The state type.</typeparam>
        /// <typeparam name="TSymbol">The symbol type.</typeparam>
        /// <param name="automaton">The automaton to remove unreachable states from.</param>
        /// <returns>True, if there were unreachable states, false otherwise.</returns>
        public static bool RemoveUnreachable<TState, TSymbol>(this IFiniteAutomaton<TState, TSymbol> automaton) =>
            automaton.RemoveUnreachable(automaton.InitialState);
    }
}
