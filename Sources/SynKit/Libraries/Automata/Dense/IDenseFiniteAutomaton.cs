// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata.Dense;

/// <summary>
/// Represents a dense finite automaton that stores all transitions as intervals.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface IDenseFiniteAutomaton<TState, TSymbol>
    : IReadOnlyDenseFiniteAutomaton<TState, TSymbol>,
      IFiniteAutomaton<TState, TSymbol>
{
    /// <summary>
    /// The alphabet of this automaton.
    /// </summary>
    public new ICollection<Interval<TSymbol>> Alphabet { get; }

    /// <summary>
    /// The transitions of this automaton.
    /// </summary>
    public new ICollection<Transition<TState, Interval<TSymbol>>> Transitions { get; }

    /// <summary>
    /// Adds a transition to this automaton.
    /// </summary>
    /// <param name="from">The state to transition from.</param>
    /// <param name="on">The interval of symbols that triggers this transition.</param>
    /// <param name="to">The state to transition to.</param>
    public void AddTransition(TState from, Interval<TSymbol> on, TState to);

    /// <summary>
    /// Removes a transition from this automaton.
    /// </summary>
    /// <param name="from">The state to transition from.</param>
    /// <param name="on">The interval of symbols that triggers this transition.</param>
    /// <param name="to">The state to transition to.</param>
    /// <returns>True, if the transition was found and removed, false otherwise.</returns>
    public bool RemoveTransition(TState from, Interval<TSymbol> on, TState to);
}
