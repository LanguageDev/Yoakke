// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Represents a generic, finite state automaton.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface IFiniteStateAutomaton<TState, TSymbol>
{
    /// <summary>
    /// All states of the automaton.
    /// </summary>
    public ICollection<TState> States { get; }

    /// <summary>
    /// The initial states of the automaton.
    /// </summary>
    public IReadOnlyCollection<TState> InitialStates { get; }

    /// <summary>
    /// The accepting states of the automaton.
    /// </summary>
    public ICollection<TState> AcceptingStates { get; }

    /// <summary>
    /// The transitions of the automaton.
    /// </summary>
    public ICollection<Transition<TState, Interval<TSymbol>>> Transitions { get; }

    /// <summary>
    /// The epsilon-transitions of the automaton.
    /// </summary>
    public IReadOnlyCollection<EpsilonTransition<TState>> EpsilonTransitions { get; }

    /// <summary>
    /// The alphabet of the automaton.
    /// </summary>
    public ICollection<Interval<TSymbol>> Alphabet { get; }

    /// <summary>
    /// All the reachable states from the initial states.
    /// </summary>
    public IEnumerable<TState> ReachableStates { get; }

    /// <summary>
    /// Checks if this automaton results in an accepting state for the given input.
    /// </summary>
    /// <param name="input">The sequence of input to feed in for the automaton.</param>
    /// <returns>True, if the automaton accepts <paramref name="input"/>, false otherwise.</returns>
    public bool Accepts(IEnumerable<TSymbol> input);
}
