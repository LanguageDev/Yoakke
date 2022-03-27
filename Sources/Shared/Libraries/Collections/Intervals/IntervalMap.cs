// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Collections.Intervals.Internal;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// Represents interval-value associations.
/// </summary>
/// <typeparam name="TKey">The interval endpoint type.</typeparam>
/// <typeparam name="TValue">The associated value type.</typeparam>
public sealed class IntervalMap<TKey, TValue> : IReadOnlyCollection<KeyValuePair<Interval<TKey>, TValue>>,
                                                ICollection<KeyValuePair<Interval<TKey>, TValue>>
{
    /// <inheritdoc/>
    int IReadOnlyCollection<KeyValuePair<Interval<TKey>, TValue>>.Count => this.intervals.Count;

    /// <inheritdoc/>
    int ICollection<KeyValuePair<Interval<TKey>, TValue>>.Count => this.intervals.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// The keys in the map.
    /// </summary>
    public IEnumerable<Interval<TKey>> Keys => this.intervals.Select(kv => kv.Key);

    /// <summary>
    /// The values in the map.
    /// </summary>
    public IEnumerable<TValue> Values => this.intervals.Select(kv => kv.Value);

    /// <summary>
    /// The interval comparer used.
    /// </summary>
    public IntervalComparer<TKey> IntervalComparer => this.intervals.Comparer;

    /// <summary>
    /// The endpoint comparer used.
    /// </summary>
    public EndpointComparer<TKey> EndpointComparer => this.IntervalComparer.EndpointComparer;

    /// <summary>
    /// The value comparer used.
    /// </summary>
    public IComparer<TKey> ValueComparer => this.EndpointComparer.ValueComparer;

    /// <summary>
    /// The value equality comparer used.
    /// </summary>
    public IEqualityComparer<TKey> ValueEqualityComparer => this.EndpointComparer.ValueEqualityComparer;

    private readonly SortedIntervalList<TKey, TValue> intervals;

    /// <summary>
    /// Constructs a new, empty <see cref="IntervalMap{TKey, TValue}"/>.
    /// </summary>
    /// <param name="comparer">The interval comparer to use.</param>
    public IntervalMap(IntervalComparer<TKey>? comparer)
    {
        this.intervals = new(comparer ?? IntervalComparer<TKey>.Default);
    }

    /// <summary>
    /// Constructs a new, empty <see cref="IntervalMap{TKey, TValue}"/>.
    /// </summary>
    /// <param name="comparer">The endpoint comparer to use.</param>
    public IntervalMap(EndpointComparer<TKey>? comparer)
        : this(comparer is null ? null : new IntervalComparer<TKey>(comparer))
    {
    }

    /// <summary>
    /// Retrieves the intersecting entries in this map.
    /// </summary>
    /// <param name="keys">The interval of keys to search intersection with.</param>
    /// <returns>The sequence of key-value pairs that are intersecting with <paramref name="keys"/>.</returns>
    public IEnumerable<KeyValuePair<Interval<TKey>, TValue>> Intersecting(Interval<TKey> keys)
    {
        var (from, to) = this.intervals.IntersectingRange(keys);
        for (; from != to; ++from) yield return this.intervals[from];
    }

    /// <inheritdoc/>
    public void Clear() => this.intervals.Clear();

    /// <inheritdoc/>
    void ICollection<KeyValuePair<Interval<TKey>, TValue>>.Add(KeyValuePair<Interval<TKey>, TValue> item) =>
        throw new NotSupportedException();

    /// <summary>
    /// Adds an interval with an associated value.
    /// </summary>
    /// <typeparam name="TCombiner">The combiner type to use.</typeparam>
    /// <param name="keys">The interval of keys to add.</param>
    /// <param name="value">The associated values.</param>
    /// <param name="combiner">The combiner to use.</param>
    public void Add<TCombiner>(Interval<TKey> keys, TValue value, TCombiner combiner)
        where TCombiner : IValueCombiner<TValue>
    {
        if (this.IntervalComparer.IsEmpty(keys)) return;

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
            // First, compare the undpoints
            var existing = this.intervals[from];
            var existingValue = existing.Value;
            var lowerCmp = this.EndpointComparer.Compare(existing.Key.Lower, keys.Lower);
            var upperCmp = this.EndpointComparer.Compare(existing.Key.Upper, keys.Upper);
            var combinedValue = combiner.Combine(existingValue, value);
            // First we observe the upper endpoint
            if (upperCmp < 0)
            {
                // We overextend past the end, completely cover and need to add extra
                this.intervals[from] = new(existing.Key, combinedValue);
                this.intervals.Insert(from + 1, new(new(existing.Key.Upper.Touching!.Value, keys.Upper), value));
            }
            else if (upperCmp > 0)
            {
                // We don't reach the end, need to split that off
                this.intervals.Insert(from + 1, new(new(keys.Upper.Touching!.Value, existing.Key.Upper), existingValue));
                this.intervals[from] = new(new(existing.Key.Lower, keys.Upper), combinedValue);
            }
            // Update the existing value
            existing = this.intervals[from];
            // Lastly we observe the lower endpoint
            if (lowerCmp > 0)
            {
                // We overextend before the start, completely cover and need to add extra
                this.intervals[from] = new(existing.Key, combinedValue);
                this.intervals.Insert(from, new(new(keys.Lower, existing.Key.Lower.Touching!.Value), value));
            }
            else if (lowerCmp < 0)
            {
                // We don't reach the start, need to split that off
                this.intervals.Insert(from, new(new(existing.Key.Lower, keys.Lower.Touching!.Value), existingValue));
                this.intervals[from + 1] = new(new(keys.Lower, existing.Key.Upper), combinedValue);
            }
            // If both endpoints match, just update with the combined value
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
            var lowerCmp = this.EndpointComparer.Compare(lowerExisting.Key.Lower, keys.Lower);
            var upperCmp = this.EndpointComparer.Compare(upperExisting.Key.Upper, keys.Upper);
            var lowerCombinedValue = combiner.Combine(lowerExisting.Value, value);
            var upperCombinedValue = combiner.Combine(upperExisting.Value, value);
            // In general we move from top to bottom with insertions, as that will cause less indexing mess-up
            // First we observe the upper intersecting interval
            if (upperCmp < 0)
            {
                // We overextend past the end, completely cover and need to add extra
                this.intervals[to - 1] = new(upperExisting.Key, upperCombinedValue);
                this.intervals.Insert(to, new(new(upperExisting.Key.Upper.Touching!.Value, keys.Upper), value));
            }
            else if (upperCmp > 0)
            {
                // We don't reach the end, need to split that off
                this.intervals.Insert(to, new(new(keys.Upper.Touching!.Value, upperExisting.Key.Upper), upperExisting.Value));
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
                var gapInterval = new Interval<TKey>(current.Key.Upper.Touching!.Value, next.Key.Lower.Touching!.Value);
                // If not empty gap, fill it
                if (!this.IntervalComparer.IsEmpty(gapInterval)) this.intervals.Insert(offset + 1, new(gapInterval, value));
                // Unify this intervals value with the added one and update
                var combinedValue = combiner.Combine(current.Value, value);
                this.intervals[offset] = new(current.Key, combinedValue);
            }
            // There is one more gap right after the lower intersecting
            {
                var next = this.intervals[from + 1];
                var gapInterval = new Interval<TKey>(lowerExisting.Key.Upper.Touching!.Value, next.Key.Lower.Touching!.Value);
                // If not empty gap, fill it
                if (!this.IntervalComparer.IsEmpty(gapInterval)) this.intervals.Insert(from + 1, new(gapInterval, value));
            }
            // Lastly we observe the lower intersecting interval
            if (lowerCmp > 0)
            {
                // We overextend before the start, completely cover and need to add extra
                this.intervals[from] = new(lowerExisting.Key, lowerCombinedValue);
                this.intervals.Insert(from, new(new(keys.Lower, lowerExisting.Key.Lower.Touching!.Value), value));
            }
            else if (lowerCmp < 0)
            {
                // We don't reach the start, need to split that off
                this.intervals.Insert(from, new(new(lowerExisting.Key.Lower, keys.Lower.Touching!.Value), lowerExisting.Value));
                this.intervals[from + 1] = new(new(keys.Lower, lowerExisting.Key.Upper), lowerCombinedValue);
            }
            else
            {
                // We just update with the combined value
                this.intervals[from] = new(lowerExisting.Key, lowerCombinedValue);
            }
        }
    }

    /// <summary>
    /// Removes entries from the given interval.
    /// </summary>
    /// <param name="keys">The interval to remove.</param>
    /// <returns>True, if there were elements removed.</returns>
    public bool Remove(Interval<TKey> keys) => this.intervals.Remove(keys);

    /// <summary>
    /// Checks, if a given set of intervals is contained.
    /// </summary>
    /// <param name="keys">The interval of keys to check.</param>
    /// <returns>True, if <paramref name="keys"/> are contained.</returns>
    public bool ContainsKeys(Interval<TKey> keys) => this.TryGetContained(keys, out _);

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<Interval<TKey>, TValue>>.Contains(KeyValuePair<Interval<TKey>, TValue> item)
    {
        if (!this.TryGetContained(item.Key, out var values)) return false;
        return values.All(v => EqualityComparer<TValue>.Default.Equals(v.Value, item.Value));
    }

    /// <inheritdoc/>
    bool ICollection<KeyValuePair<Interval<TKey>, TValue>>.Remove(KeyValuePair<Interval<TKey>, TValue> item)
    {
        // NOTE: Kinda ineffective but will work for now
        var intersecting = this.Intersecting(item.Key).ToList();
        var anyRemoved = false;
        foreach (var kv in intersecting)
        {
            var iv = kv.Key;
            var value = kv.Value;
            if (!EqualityComparer<TValue>.Default.Equals(item.Value, value)) continue;
            var commonIv = this.IntervalComparer.Intersection(iv, item.Key);
            this.Remove(commonIv);
            anyRemoved = true;
        }
        return anyRemoved;
    }


    /// <inheritdoc/>
    public void CopyTo(KeyValuePair<Interval<TKey>, TValue>[] array, int arrayIndex)
    {
        if (arrayIndex + this.intervals.Count > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        for (var i = 0; i < this.intervals.Count; ++i) array[arrayIndex + i] = this.intervals[i];
    }

    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<Interval<TKey>, TValue>> GetEnumerator() => this.intervals.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    private bool TryGetContained(Interval<TKey> keys, out IEnumerable<KeyValuePair<Interval<TKey>, TValue>> values)
    {
        values = Enumerable.Empty<KeyValuePair<Interval<TKey>, TValue>>();
        if (this.IntervalComparer.IsEmpty(keys)) return true;

        // Get all intersecting ranges
        var (from, to) = this.intervals.IntersectingRange(keys);
        // No intersecting intervals, certainly does not contain
        if (to - from == 0) return false;

        // Intersects one or more intervals
        var lowerExisting = this.intervals[from];
        var upperExisting = this.intervals[to - 1];
        var lowerCmp = this.EndpointComparer.Compare(lowerExisting.Key.Lower, keys.Lower);
        var upperCmp = this.EndpointComparer.Compare(upperExisting.Key.Upper, keys.Upper);

        // If we overextend, we don't contain it
        if (lowerCmp > 0 || upperCmp < 0) return false;

        // All the in-between intervals must touch
        for (var i = from; i < to - 1; ++i)
        {
            var a = this.intervals[i].Key;
            var b = this.intervals[i + 1].Key;
            if (!this.EndpointComparer.IsTouching(a.Upper, b.Lower)) return false;
        }

        IEnumerable<KeyValuePair<Interval<TKey>, TValue>> YieldValues()
        {
            for (var i = from; i < to; ++i) yield return this.intervals[i];
        }

        values = YieldValues();
        return true;
    }
}
