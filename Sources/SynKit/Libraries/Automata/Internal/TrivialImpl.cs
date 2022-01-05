// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;

namespace Yoakke.SynKit.Automata.Internal;

/// <summary>
/// Trivially implementable algorithms, to avoid duplication.
/// </summary>
internal static class TrivialImpl
{
    /// <summary>
    /// Implements <see cref="IFiniteAutomaton{TState, TSymbol}.RemoveUnreachable"/>.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="automaton">The automaton to remove unreachable states from.</param>
    /// <returns>True, if there were unreachable states removed.</returns>
    public static bool RemoveUnreachable<TState, TSymbol>(IFiniteAutomaton<TState, TSymbol> automaton)
    {
        var unreachable = automaton.States.Except(automaton.ReachableStates, automaton.StateComparer).ToList();
        var result = false;
        foreach (var state in unreachable)
        {
            if (automaton.States.Remove(state)) result = true;
        }
        return result;
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyFiniteAutomaton{TState, TSymbol}.Accepts(IEnumerable{TSymbol})"/> for DFAs.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="dfa">The DFA to check acceptance on.</param>
    /// <param name="input">The input to check acceptance for.</param>
    /// <returns>True, if the <paramref name="input"/> is accepted by <paramref name="dfa"/>.</returns>
    public static bool Accepts<TState, TSymbol>(IReadOnlyDfa<TState, TSymbol> dfa, IEnumerable<TSymbol> input)
    {
        var currentState = dfa.InitialState;
        foreach (var symbol in input)
        {
            if (!dfa.TryGetTransition(currentState, symbol, out var destinationState)) return false;
            currentState = destinationState;
        }
        return dfa.AcceptingStates.Contains(currentState);
    }

    /// <summary>
    /// Implements <see cref="IReadOnlyFiniteAutomaton{TState, TSymbol}.Accepts(IEnumerable{TSymbol})"/> for NFAs.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="nfa">The NFA to check acceptance on.</param>
    /// <param name="input">The input to check acceptance for.</param>
    /// <returns>True, if the <paramref name="input"/> is accepted by <paramref name="nfa"/>.</returns>
    public static bool Accepts<TState, TSymbol>(IReadOnlyNfa<TState, TSymbol> nfa, IEnumerable<TSymbol> input)
    {
        var currentState = new StateSet<TState>(nfa.InitialStates, nfa.StateComparer);
        foreach (var symbol in input)
        {
            currentState = nfa.GetTransitions(currentState, symbol);
            if (currentState.Count == 0) return false;
        }
        return currentState.Overlaps(nfa.AcceptingStates);
    }

    /// <summary>
    /// Implements <see cref="INfa{TState, TSymbol}.EliminateEpsilonTransitions"/>.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="nfa">The NFA to eliminate epsilon-transitions from.</param>
    /// <param name="copyTransitions">A method to copy transitions from one state to another.</param>
    /// <returns>True, if there were epsilon-transitions to eliminate.</returns>
    public static bool EliminateEpsilonTransitions<TState, TSymbol>(
        INfa<TState, TSymbol> nfa,
        Action<TState, TState> copyTransitions)
    {
        if (nfa.EpsilonTransitions.Count == 0) return false;

        foreach (var state in nfa.States)
        {
            // For each state we look at its epsilon closure
            // For each element in the closure we copy the non-epsilon transitions from state to the others
            // We can omit the state itself from the copy
            var epsilonClosure = nfa
                .EpsilonClosure(state)
                .Where(s => !nfa.StateComparer.Equals(s, state))
                .ToHashSet(nfa.StateComparer);
            foreach (var toState in epsilonClosure)
            {
                copyTransitions(state, toState);
                // If v1 is a starting state, we need to make v2 one aswell
                if (nfa.InitialStates.Contains(state)) nfa.InitialStates.Add(toState);
                // If v2 is a final state, v1 needs to be aswell
                if (nfa.AcceptingStates.Contains(toState)) nfa.AcceptingStates.Add(state);
            }
        }

        nfa.EpsilonTransitions.Clear();

        return true;
    }
}
