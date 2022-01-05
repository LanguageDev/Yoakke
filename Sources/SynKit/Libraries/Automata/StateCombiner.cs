// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Utilities for <see cref="IStateCombiner{TState, TResultState}"/>.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
public static class StateCombiner<TState>
{
    private class StateSetStateCombiner : IStateCombiner<TState, StateSet<TState>>
    {
        public IEqualityComparer<StateSet<TState>> ResultComparer => EqualityComparer<StateSet<TState>>.Default;

        private readonly IEqualityComparer<TState> comparer;

        public StateSetStateCombiner(IEqualityComparer<TState> comparer)
        {
            this.comparer = comparer;
        }

        public StateSet<TState> Combine(IEnumerable<TState> states) => new(states, this.comparer);
    }

    private class StateSetStateSetCombiner : IStateCombiner<StateSet<TState>, StateSet<TState>>
    {
        public IEqualityComparer<StateSet<TState>> ResultComparer => EqualityComparer<StateSet<TState>>.Default;

        private readonly IEqualityComparer<TState> comparer;

        public StateSetStateSetCombiner(IEqualityComparer<TState> comparer)
        {
            this.comparer = comparer;
        }

        public StateSet<TState> Combine(IEnumerable<StateSet<TState>> states) =>
            new(states.SelectMany(s => s).ToHashSet(this.comparer));
    }

    /// <summary>
    /// A default combiner that combines states into sets.
    /// </summary>
    public static IStateCombiner<TState, StateSet<TState>> DefaultToSetCombiner { get; } =
        ToSetCombiner(EqualityComparer<TState>.Default);

    /// <summary>
    /// A default combiner that merges state sets.
    /// </summary>
    public static IStateCombiner<StateSet<TState>, StateSet<TState>> DefaultSetCombiner { get; } =
        SetCombiner(EqualityComparer<TState>.Default);

    /// <summary>
    /// Retrieves a combiner that combines states into a state set.
    /// </summary>
    /// <param name="comparer">The state comparer to use.</param>
    /// <returns>The combiner that takes the states and creates a set from them.</returns>
    public static IStateCombiner<TState, StateSet<TState>> ToSetCombiner(IEqualityComparer<TState> comparer) =>
        new StateSetStateCombiner(comparer);

    /// <summary>
    /// Retrieves a combiner that combines state sets.
    /// </summary>
    /// <param name="comparer">The state comparer to use.</param>
    /// <returns>The combiner that takes state sets and merges them into one.</returns>
    public static IStateCombiner<StateSet<TState>, StateSet<TState>> SetCombiner(IEqualityComparer<TState> comparer) =>
        new StateSetStateSetCombiner(comparer);
}
