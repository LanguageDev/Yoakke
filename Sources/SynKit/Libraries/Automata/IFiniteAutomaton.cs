// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Represents a generic finite automaton.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface IFiniteAutomaton<TState, TSymbol> : IReadOnlyFiniteAutomaton<TState, TSymbol>
{
    /// <summary>
    /// All states of the automaton.
    /// </summary>
    public new ICollection<TState> States { get; }

    /// <summary>
    /// The accepting states of the automaton.
    /// </summary>
    public new ICollection<TState> AcceptingStates { get; }

    /// <summary>
    /// Adds a transition to this automaton.
    /// </summary>
    /// <param name="from">The state to transition from.</param>
    /// <param name="on">The symbol that triggers this transition.</param>
    /// <param name="to">The state to transition to.</param>
    public void AddTransition(TState from, TSymbol on, TState to);

    /// <summary>
    /// Removes a transition from this automaton.
    /// </summary>
    /// <param name="from">The state to transition from.</param>
    /// <param name="on">The symbol that triggers this transition.</param>
    /// <param name="to">The state to transition to.</param>
    /// <returns>True, if the transition was found and removed, false otherwise.</returns>
    public bool RemoveTransition(TState from, TSymbol on, TState to);

    /// <summary>
    /// Removes states and transitions from the automaton that are not reachable from the initial state.
    /// </summary>
    /// <returns>True, if there were unreachable states, false otherwise.</returns>
    public bool RemoveUnreachable();
}
