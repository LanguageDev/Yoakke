// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Represents a set of states that result from transformations merging multiple states.
/// Useful for keeping readability.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
public sealed class StateSet<TState> : IReadOnlySet<TState>, IEquatable<StateSet<TState>>
{
    /// <inheritdoc/>
    public int Count => this.states.Count;

    private readonly HashSet<TState> states;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateSet{TState}"/> class.
    /// </summary>
    /// <param name="set">The set of states.</param>
    internal StateSet(HashSet<TState> set)
    {
        this.states = set;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StateSet{TState}"/> class.
    /// </summary>
    /// <param name="states">The states this set consists of.</param>
    /// <param name="equalityComparer">The state comparer to use.</param>
    public StateSet(IEnumerable<TState> states, IEqualityComparer<TState> equalityComparer)
    {
        this.states = states.ToHashSet(equalityComparer);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StateSet{TState}"/> class.
    /// </summary>
    /// <param name="state">The singleton state to make a set out of.</param>
    /// <param name="equalityComparer">The state comparer to use.</param>
    public StateSet(TState state, IEqualityComparer<TState> equalityComparer)
    {
        this.states = new(equalityComparer) { state };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StateSet{TState}"/> class.
    /// </summary>
    /// <param name="equalityComparer">The state comparer to use.</param>
    public StateSet(IEqualityComparer<TState> equalityComparer)
    {
        this.states = new(equalityComparer);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is StateSet<TState> other && this.Equals(other);

    /// <inheritdoc/>
    public bool Equals(StateSet<TState> other) => this.SetEquals(other);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var h = 0;
        // NOTE: We use XOR to have order-independent hashing
        foreach (var s in this.states) h ^= this.states.Comparer.GetHashCode(s);
        return h;
    }

    /// <inheritdoc/>
    public override string ToString() => string.Join(", ", this.states);

    /// <inheritdoc/>
    public bool Contains(TState item) => this.states.Contains(item);

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<TState> other) => this.states.IsProperSubsetOf(other);

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<TState> other) => this.states.IsProperSupersetOf(other);

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<TState> other) => this.states.IsSubsetOf(other);

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<TState> other) => this.states.IsSupersetOf(other);

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<TState> other) => this.states.Overlaps(other);

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<TState> other) => this.states.SetEquals(other);

    /// <inheritdoc/>
    public IEnumerator<TState> GetEnumerator() => this.states.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => (this.states as IEnumerable).GetEnumerator();
}
