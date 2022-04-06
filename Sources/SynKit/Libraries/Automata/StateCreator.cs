// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Utilities for <see cref="IStateCreator{TState}"/>s.
/// </summary>
public static class StateCreator
{
    private readonly record struct DelegateStateCreator<TState>(Func<TState> MakeState) : IStateCreator<TState>
    {
        public TState Create() => this.MakeState();
    }

    private readonly record struct EnumeratorStateCreator<TState>(IEnumerator<TState> Enumerator) : IStateCreator<TState>
    {
        public TState Create()
        {
            if (!this.Enumerator.MoveNext()) throw new InvalidOperationException("The enumerator reached the end, no more states can be extracted.");
            return this.Enumerator.Current;
        }
    }

    private readonly record struct RandomStateCreator<TState>(ISet<TState> Used, Func<TState> Sample) : IStateCreator<TState>
    {
        public TState Create()
        {
            while (true)
            {
                var element = this.Sample();
                if (this.Used.Add(element)) return element;
            }
        }
    }

    /// <summary>
    /// Constructs a state creator from the given delegate. The uniqueness of the created function must
    /// be ensured by the delegate.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <param name="makeState">The function that constructs a unique state.</param>
    /// <returns>A state creator that uses <paramref name="makeState"/> for state creation.</returns>
    public static IStateCreator<TState> Delegate<TState>(Func<TState> makeState) =>
        new DelegateStateCreator<TState>(makeState);

    /// <summary>
    /// Constructs a state creator from an enumerable sequence of values.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <param name="states">An enumerable that must hold enough states for the construction.</param>
    /// <returns>A state creator using elements from <paramref name="states"/> in sequence.</returns>
    public static IStateCreator<TState> Enumerate<TState>(IEnumerable<TState> states) =>
        new EnumeratorStateCreator<TState>(states.GetEnumerator());

    /// <summary>
    /// Constructs a state creator that creates random states, automatically filtering duplicates.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <param name="sampler">A delegate returning a randomly sampled state.</param>
    /// <param name="stateComparer">The state comparer to use to filter equivalent states.</param>
    /// <returns>A state creator that calls the sampler function until it finds a previously not created state.</returns>
    public static IStateCreator<TState> Random<TState>(Func<TState> sampler, IEqualityComparer<TState>? stateComparer = null)
    {
        stateComparer ??= EqualityComparer<TState>.Default;
        var set = new HashSet<TState>(stateComparer);
        return new RandomStateCreator<TState>(set, sampler);
    }

    // Utilities

    /// <summary>
    /// Constructs an enumerating state creator that creates integer state IDs starting from 0.
    /// </summary>
    /// <returns>A state creator starting from 0.</returns>
    public static IStateCreator<int> Enumerate() => Enumerate(Enumerable.Range(0, int.MaxValue));

    /// <summary>
    /// Constructs a random state creator that creates random positive integer IDs.
    /// </summary>
    /// <returns>A state creator creating random integer IDs.</returns>
    public static IStateCreator<int> Random()
    {
        var rnd = new Random();
        return Random(() => rnd.Next());
    }
}
