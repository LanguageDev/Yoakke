// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// A generic NFA implementation.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public sealed class Nfa<TState, TSymbol> : IFiniteStateAutomaton<TState, TSymbol>
{
    /// <inheritdoc/>
    public ICollection<TState> States => throw new NotImplementedException();

    /// <summary>
    /// The initial states of the automaton.
    /// </summary>
    public ICollection<TState> InitialStates => throw new NotImplementedException();

    /// <inheritdoc/>
    IReadOnlyCollection<TState> IFiniteStateAutomaton<TState, TSymbol>.InitialStates => throw new NotImplementedException();

    /// <inheritdoc/>
    public ICollection<TState> AcceptingStates => throw new NotImplementedException();

    /// <inheritdoc/>
    public ICollection<Transition<TState, Interval<TSymbol>>> Transitions => throw new NotImplementedException();

    /// <summary>
    /// The epsilon-tramsitions of the automaton.
    /// </summary>
    public ICollection<EpsilonTransition<TState>> EpsilonTransitions => throw new NotImplementedException();

    /// <inheritdoc/>
    IReadOnlyCollection<EpsilonTransition<TState>> IFiniteStateAutomaton<TState, TSymbol>.EpsilonTransitions =>
        throw new NotImplementedException();

    /// <inheritdoc/>
    public bool Accepts(IEnumerable<TSymbol> input) => throw new NotImplementedException();
}
