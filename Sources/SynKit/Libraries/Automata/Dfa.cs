// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// A generic DFA implementation.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public sealed class Dfa<TState, TSymbol> : IFiniteStateAutomaton<TState, TSymbol>
{
    /// <inheritdoc/>
    public ICollection<TState> States => throw new NotImplementedException();

    /// <summary>
    /// The initial states of the automaton.
    /// </summary>
    public TState InitialState
    { 
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    /// <inheritdoc/>
    IReadOnlyCollection<TState> IFiniteStateAutomaton<TState, TSymbol>.InitialStates =>
        new EnumerableCollection<TState>(EnumerableExtensions.Singleton(this.InitialState), 1);

    /// <inheritdoc/>
    public ICollection<TState> AcceptingStates => throw new NotImplementedException();

    /// <inheritdoc/>
    public ICollection<Transition<TState, Interval<TSymbol>>> Transitions => throw new NotImplementedException();

    /// <inheritdoc/>
    IReadOnlyCollection<EpsilonTransition<TState>> IFiniteStateAutomaton<TState, TSymbol>.EpsilonTransitions =>
        Array.Empty<EpsilonTransition<TState>>();

    /// <inheritdoc/>
    public ICollection<Interval<TSymbol>> Alphabet => throw new NotImplementedException();

    /// <inheritdoc/>
    public bool Accepts(IEnumerable<TSymbol> input) => throw new NotImplementedException();
}
