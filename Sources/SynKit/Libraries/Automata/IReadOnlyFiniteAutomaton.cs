// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Represents a generic, read-only finite automaton.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface IReadOnlyFiniteAutomaton<TState, TSymbol>
{
    /// <summary>
    /// All states of the automaton.
    /// </summary>
    public IReadOnlyCollection<TState> States { get; }

    /// <summary>
    /// The accepting states of the automaton.
    /// </summary>
    public IReadOnlyCollection<TState> AcceptingStates { get; }

    /// <summary>
    /// Finds all the reachable states from the initial state.
    /// </summary>
    public IEnumerable<TState> ReachableStates { get; }

    /// <summary>
    /// The state comparer used by the automaton.
    /// </summary>
    public IEqualityComparer<TState> StateComparer { get; }

    /// <summary>
    /// Checks if this automaton results in an accepting state for the given input.
    /// </summary>
    /// <param name="input">The sequence of input to feed in for the automaton.</param>
    /// <returns>True, if the automaton accepts the input, false otherwise.</returns>
    public bool Accepts(IEnumerable<TSymbol> input);

    /// <summary>
    /// Creates a Graphviz DOT code representation of this automaton for visualization.
    /// </summary>
    /// <returns>The full graphviz code.</returns>
    public string ToDot();
}
