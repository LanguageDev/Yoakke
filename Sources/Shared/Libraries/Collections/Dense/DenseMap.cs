// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Yoakke.Collections.Internal;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Dense;

/// <summary>
/// A default <see cref="IDenseMap{TKey, TValue}"/> implementation backed by a list of intervals.
/// </summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public sealed class DenseMap<TKey, TValue> : IDenseMap<TKey, TValue>
{
    /// <inheritdoc/>
    public int Count => this.intervals.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public IEnumerable<Interval<TKey>> Keys => this.intervals.Select(kv => kv.Key);

    /// <inheritdoc/>
    public IEnumerable<TValue> Values => this.intervals.Select(kv => kv.Value);

    /// <summary>
    /// The comparer used.
    /// </summary>
    public IntervalComparer<TKey> Comparer => this.intervals.Comparer;

    /// <summary>
    /// The combiner used.
    /// </summary>
    public ICombiner<TValue> Combiner { get; set; }

    private BoundComparer<TKey> BoundComparer => this.Comparer.BoundComparer;

    private readonly SortedIntervalList<TKey, KeyValuePair<Interval<TKey>, TValue>> intervals;

    /// <summary>
    /// Initializes a new instance of the <see cref="DenseMap{TKey, TValue}"/> class.
    /// </summary>
    public DenseMap()
        : this(IntervalComparer<TKey>.Default, Combiner<TValue>.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DenseMap{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="comparer">The comparer to use.</param>
    /// <param name="combiner">The combiner to use.</param>
    public DenseMap(IntervalComparer<TKey> comparer, ICombiner<TValue> combiner)
    {
        this.Combiner = combiner;
        this.intervals = new(comparer, kv => kv.Key, (kv, newIv) => new(newIv, kv.Value));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DenseMap{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="comparer">The comparer to use.</param>
    public DenseMap(IntervalComparer<TKey> comparer)
        : this(comparer, Combiner<TValue>.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DenseMap{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="combiner">The combiner to use.</param>
    public DenseMap(ICombiner<TValue> combiner)
        : this(IntervalComparer<TKey>.Default, combiner)
    {
    }

    /// <inheritdoc/>
    public void Clear() => this.intervals.Clear();

    /// <inheritdoc/>
    public void Add(KeyValuePair<Interval<TKey>, TValue> item) => this.Add(item.Key, item.Value);

    /// <inheritdoc/>
    public void Add(Interval<TKey> keys, TValue value)
    {
        if (this.Comparer.IsEmpty(keys)) return;

        // For an empty set, it's trivial, we just add it
        if (this.intervals.Count == 0)
        {
            this.intervals.Add(new(keys, value));
            return;
        }

        // Mapping is not empty, find the intersecting intervals
        var (from, to) = this.intervals.IntersectingRange(keys);
        if (from == to)
        {
            // Intersects nothing, we can just insert
            this.intervals.Insert(from, new(keys, value));
        }
        else if (to - from == 1)
        {
            // Intersects a single interval
            // This can either not split the interval, or split it once or twice
            // First, compare the bounds
            var existing = this.intervals[from];
            var existingValue = existing.Value;
            var lowerCmp = this.BoundComparer.Compare(existing.Key.Lower, keys.Lower);
            var upperCmp = this.BoundComparer.Compare(existing.Key.Upper, keys.Upper);
            var combinedValue = this.Combiner.Combine(existingValue, value);
            // First we observe the upper bound
            if (upperCmp < 0)
            {
                // We overextend past the end, completely cover and need to add extra
                this.intervals[from] = new(existing.Key, combinedValue);
                this.intervals.Insert(from + 1, new(new(existing.Key.Upper.Touching!, keys.Upper), value));
            }
            else if (upperCmp > 0)
            {
                // We don't reach the end, need to split that off
                this.intervals.Insert(from + 1, new(new(keys.Upper.Touching!, existing.Key.Upper), existingValue));
                this.intervals[from] = new(new(existing.Key.Lower, keys.Upper), combinedValue);
            }
            // Update the existing value
            existing = this.intervals[from];
            // Lastly we observe the lower bound
            if (lowerCmp > 0)
            {
                // We overextend before the start, completely cover and need to add extra
                this.intervals[from] = new(existing.Key, combinedValue);
                this.intervals.Insert(from, new(new(keys.Lower, existing.Key.Lower.Touching!), value));
            }
            else if (lowerCmp < 0)
            {
                // We don't reach the start, need to split that off
                this.intervals.Insert(from, new(new(existing.Key.Lower, keys.Lower.Touching!), existingValue));
                this.intervals[from + 1] = new(new(keys.Lower, existing.Key.Upper), combinedValue);
            }
            // If both bounds match, just update with the combined value
            if (lowerCmp == 0 && upperCmp == 0) this.intervals[from] = new(existing.Key, combinedValue);
        }
        else
        {
            // Intersects more intervals
            // The first and last intervals potentially have to be split
            // The rest between are unified and spaces inbetween are filled in
            // First comper the bottom and top intersecting intervals
            var lowerExisting = this.intervals[from];
            var upperExisting = this.intervals[to - 1];
            var lowerCmp = this.BoundComparer.Compare(lowerExisting.Key.Lower, keys.Lower);
            var upperCmp = this.BoundComparer.Compare(upperExisting.Key.Upper, keys.Upper);
            var lowerCombinedValue = this.Combiner.Combine(lowerExisting.Value, value);
            var upperCombinedValue = this.Combiner.Combine(upperExisting.Value, value);
            // In general we move from top to bottom with insertions, as that will cause less indexing mess-up
            // First we observe the upper intersecting interval
            if (upperCmp < 0)
            {
                // We overextend past the end, completely cover and need to add extra
                this.intervals[to - 1] = new(upperExisting.Key, upperCombinedValue);
                this.intervals.Insert(to, new(new(upperExisting.Key.Upper.Touching!, keys.Upper), value));
            }
            else if (upperCmp > 0)
            {
                // We don't reach the end, need to split that off
                this.intervals.Insert(to, new(new(keys.Upper.Touching!, upperExisting.Key.Upper), upperExisting.Value));
                this.intervals[to - 1] = new(new(upperExisting.Key.Lower, keys.Upper), upperCombinedValue);
            }
            else
            {
                // We just update with the combined value
                this.intervals[to - 1] = new(upperExisting.Key, upperCombinedValue);
            }
            // Second, we fill up the gap in between lower and upper intersected intervals
            for (var offset = to - 2; offset > from; --offset)
            {
                var current = this.intervals[offset];
                var next = this.intervals[offset + 1];
                // Fill the gap after
                var gapInterval = new Interval<TKey>(current.Key.Upper.Touching!, next.Key.Lower.Touching!);
                // If not empty gap, fill it
                if (!this.Comparer.IsEmpty(gapInterval)) this.intervals.Insert(offset + 1, new(gapInterval, value));
                // Unify this intervals value with the added one and update
                var combinedValue = this.Combiner.Combine(current.Value, value);
                this.intervals[offset] = new(current.Key, combinedValue);
            }
            // There is one more gap right after the lower intersecting
            {
                var next = this.intervals[from + 1];
                var gapInterval = new Interval<TKey>(lowerExisting.Key.Upper.Touching!, next.Key.Lower.Touching!);
                // If not empty gap, fill it
                if (!this.Comparer.IsEmpty(gapInterval)) this.intervals.Insert(from + 1, new(gapInterval, value));
            }
            // Lastly we observe the lower intersecting interval
            if (lowerCmp > 0)
            {
                // We overextend before the start, completely cover and need to add extra
                this.intervals[from] = new(lowerExisting.Key, lowerCombinedValue);
                this.intervals.Insert(from, new(new(keys.Lower, lowerExisting.Key.Lower.Touching!), value));
            }
            else if (lowerCmp < 0)
            {
                // We don't reach the start, need to split that off
                this.intervals.Insert(from, new(new(lowerExisting.Key.Lower, keys.Lower.Touching!), lowerExisting.Value));
                this.intervals[from + 1] = new(new(keys.Lower, lowerExisting.Key.Upper), lowerCombinedValue);
            }
            else
            {
                // We just update with the combined value
                this.intervals[from] = new(lowerExisting.Key, lowerCombinedValue);
            }
        }
    }

    /// <inheritdoc/>
    public bool Remove(Interval<TKey> keys) => this.intervals.Remove(keys);

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<Interval<TKey>, TValue> item)
    {
        // NOTE: Kinda ineffective but will work for now
        var intersecting = this.GetIntervalsAndValues(item.Key).ToList();
        var anyRemoved = false;
        foreach (var (iv, value) in intersecting)
        {
            if (!EqualityComparer<TValue>.Default.Equals(item.Value, value)) continue;
            var commonIv = this.Comparer.Intersection(iv, item.Key);
            this.Remove(commonIv);
            anyRemoved = true;
        }
        return anyRemoved;
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<Interval<TKey>, TValue>>.Contains(KeyValuePair<Interval<TKey>, TValue> item)
    {
        if (!this.TryGetContained(item.Key, out var values)) return false;
        return values.All(v => EqualityComparer<TValue>.Default.Equals(v.Value, item.Value));
    }

    /// <inheritdoc/>
    public bool ContainsKeys(Interval<TKey> keys) => this.TryGetContained(keys, out _);

    private bool TryGetContained(Interval<TKey> keys, out IEnumerable<KeyValuePair<Interval<TKey>, TValue>> values)
    {
        values = Enumerable.Empty<KeyValuePair<Interval<TKey>, TValue>>();
        if (this.Comparer.IsEmpty(keys)) return true;

        // Get all intersecting ranges
        var (from, to) = this.intervals.IntersectingRange(keys);
        // No intersecting intervals, certainly does not contain
        if (to - from == 0) return false;

        // Intersects one or more intervals
        var lowerExisting = this.intervals[from];
        var upperExisting = this.intervals[to - 1];
        var lowerCmp = this.BoundComparer.Compare(lowerExisting.Key.Lower, keys.Lower);
        var upperCmp = this.BoundComparer.Compare(upperExisting.Key.Upper, keys.Upper);

        // If we overextend, we don't contain it
        if (lowerCmp > 0 || upperCmp < 0) return false;

        // All the in-between intervals must touch
        for (var i = from; i < to - 1; ++i)
        {
            var a = this.intervals[i].Key;
            var b = this.intervals[i + 1].Key;
            if (!this.BoundComparer.IsTouching(a.Upper, b.Lower)) return false;
        }

        IEnumerable<KeyValuePair<Interval<TKey>, TValue>> YieldValues()
        {
            for (var i = from; i < to; ++i) yield return this.intervals[i];
        }

        values = YieldValues();
        return true;
    }

    /// <inheritdoc/>
    public IEnumerable<TValue> GetValues(Interval<TKey> keys) => this.GetIntervalsAndValues(keys).Select(kv => kv.Value);

    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<Interval<TKey>, TValue>[] array, int arrayIndex) => this.intervals.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerable<KeyValuePair<Interval<TKey>, TValue>> GetIntervalsAndValues(Interval<TKey> keys)
    {
        var (from, to) = this.intervals.IntersectingRange(keys);
        for (; from != to; ++from) yield return this.intervals[from];
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<Interval<TKey>, TValue>> GetEnumerator() => this.intervals.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => (this.intervals as IEnumerable).GetEnumerator();
}
