// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.SynKit.Automata.Sparse;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Represents a generic nondeterministic finite automaton.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface INfa<TState, TSymbol> : IReadOnlyNfa<TState, TSymbol>, IFiniteAutomaton<TState, TSymbol>
{
    /// <summary>
    /// The initial states of the automaton.
    /// </summary>
    public new ICollection<TState> InitialStates { get; }

    /// <summary>
    /// The epsilon transitions of this automaton.
    /// </summary>
    public new ICollection<EpsilonTransition<TState>> EpsilonTransitions { get; }

    /// <summary>
    /// Adds an epsilon transition to this automaton.
    /// </summary>
    /// <param name="from">The state to transition from.</param>
    /// <param name="to">The state to transition to.</param>
    public void AddEpsilonTransition(TState from, TState to);

    /// <summary>
    /// Removes an epsilon transition from this automaton.
    /// </summary>
    /// <param name="from">The state to transition from.</param>
    /// <param name="to">The state to transition to.</param>
    /// <returns>True, if the transition was found and successfully removed.</returns>
    public bool RemoveEpsilonTransition(TState from, TState to);

    /// <summary>
    /// Eliminates the epsilon-transitions from this NFA, keeping it equivalent.
    /// </summary>
    /// <returns>True, if there were epsilon transitions to eliminate.</returns>
    public bool EliminateEpsilonTransitions();
}
