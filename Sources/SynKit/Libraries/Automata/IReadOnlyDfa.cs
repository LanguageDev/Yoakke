// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Represents a generic, readonly deterministic finite automaton.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface IReadOnlyDfa<TState, TSymbol> : IReadOnlyFiniteAutomaton<TState, TSymbol>
{
    /// <summary>
    /// The initial state of the automaton.
    /// </summary>
    public TState InitialState { get; }

    /// <summary>
    /// Retrieves a transition from a given state on a given input.
    /// </summary>
    /// <param name="from">The transition to get the transition from.</param>
    /// <param name="on">The symbol to get the transition on.</param>
    /// <param name="to">The resulting state gets written here, if found.</param>
    /// <returns>True, if the transition is found, otherwise false.</returns>
    public bool TryGetTransition(TState from, TSymbol on, [MaybeNullWhen(false)] out TState to);

    /// <summary>
    /// Minimizes this DFA into an equivalent one with the least amount of states possible.
    /// </summary>
    /// <typeparam name="TResultState">The state type of the resulting DFA.</typeparam>
    /// <param name="combiner">The state combiner to use.</param>
    /// <param name="differentiatePairs">The pairs of states that must not be merged in the minimization process.
    /// This can be useful if some states have associated values to them that we want to keep.</param>
    /// <returns>The minimized DFA.</returns>
    public IDfa<TResultState, TSymbol> Minimize<TResultState>(
        IStateCombiner<TState, TResultState> combiner,
        IEnumerable<(TState, TState)> differentiatePairs);
}
