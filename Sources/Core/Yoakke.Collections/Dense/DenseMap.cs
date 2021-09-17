// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Dense
{
    /// <summary>
    /// A default <see cref="IDenseMap{TKey, TValue}"/> implementation backed by a list of intervals.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class DenseMap<TKey, TValue> : IDenseMap<TKey, TValue>
    {
        /// <inheritdoc/>
        public bool IsEmpty => this.intervals.Count == 0;

        // NOTE: We can implement these later
        // For that we need some domain information with some IDomain<T> interface

        /// <inheritdoc/>
        public int? Count => null;

        // NOTE: See above note

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<TKey, TValue>>? Values => null;

        /// <inheritdoc/>
        TValue IReadOnlyMathMap<TKey, TValue>.this[TKey key] => this[key];

        /// <inheritdoc/>
        public TValue this[TKey key]
        {
            get
            {
                if (!this.TryGetValue(key, out var value)) throw new KeyNotFoundException();
                return value;
            }
            set => this.Add(key, value);
        }

        /// <summary>
        /// The comparer used.
        /// </summary>
        public IntervalComparer<TKey> Comparer { get; }

        /// <summary>
        /// The combiner used.
        /// </summary>
        public ICombiner<TValue> Combiner { get; set; }

        private BoundComparer<TKey> BoundComparer => this.Comparer.BoundComparer;

        private readonly List<KeyValuePair<Interval<TKey>, TValue>> intervals = new();

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
            this.Comparer = comparer;
            this.Combiner = combiner;
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
        public void Add(TKey key, TValue value) => this.Add(ToInterval(key), value);

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
            var (from, to) = this.IntersectingRange(keys);
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
        public bool Remove(TKey key) => this.Remove(ToInterval(key));

        /// <inheritdoc/>
        public bool Remove(Interval<TKey> keys)
        {
            if (this.Comparer.IsEmpty(keys)) return false;

            // Find the range that this interval intersects
            var (from, to) = this.IntersectingRange(keys);

            // If intersects nothing, return
            if (from == to) return false;

            // It intersects one or more intervals
            var lowerExisting = this.intervals[from];
            var upperExisting = this.intervals[to - 1];
            var lowerCmp = this.BoundComparer.Compare(lowerExisting.Key.Lower, keys.Lower);
            var upperCmp = this.BoundComparer.Compare(upperExisting.Key.Upper, keys.Upper);
            // Split edges if needed, track indices for deletion
            var deleteFrom = from;
            var deleteTo = to;
            if (lowerCmp < 0)
            {
                // Split lower
                var newInterval = new Interval<TKey>(lowerExisting.Key.Lower, keys.Lower.Touching!);
                this.intervals[from] = new(newInterval, lowerExisting.Value);
                ++deleteFrom;
            }
            if (upperCmp > 0)
            {
                // Split upper
                var newInterval = new Interval<TKey>(keys.Upper.Touching!, upperExisting.Key.Upper);
                this.intervals[to - 1] = new(newInterval, upperExisting.Value);
                --deleteTo;
            }
            // Delete everything in between that is completely covered
            this.intervals.RemoveRange(deleteFrom, deleteTo - deleteFrom);
            return true;
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key) => this.ContainsKeys(ToInterval(key));

        /// <inheritdoc/>
        public bool ContainsKeys(Interval<TKey> keys)
        {
            var (from, to) = this.IntersectingRange(keys);
            if (to - from != 1) return false;
            var existing = this.intervals[from].Key;
            return this.Comparer.Contains(existing, keys);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
        {
            var seq = this.GetValues(ToInterval(key));
            var enumerator = seq.GetEnumerator();
            if (enumerator.MoveNext())
            {
                value = enumerator.Current;
                Debug.Assert(!enumerator.MoveNext(), "The singleton element should have only intersected at most one interval.");
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<TValue> GetValues(Interval<TKey> keys)
        {
            var (from, to) = this.IntersectingRange(keys);
            for (; from != to; ++from) yield return this.intervals[from].Value;
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<Interval<TKey>, TValue>> GetEnumerator() => this.intervals.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => (this.intervals as IEnumerable).GetEnumerator();

        private (int From, int To) IntersectingRange(Interval<TKey> interval)
        {
            var from = this.BinarySearch(0, interval.Lower, iv => iv.Upper);
            var to = this.BinarySearch(from, interval.Upper, iv => iv.Lower);
            return (from, to);
        }

        private int BinarySearch(int start, Bound<TKey> searchedKey, Func<Interval<TKey>, Bound<TKey>> keySelector)
        {
            var size = this.intervals.Count - start;
            if (size == 0) return start;

            while (size > 1)
            {
                var half = size / 2;
                var mid = start + half;
                var key = keySelector(this.intervals[mid].Key);
                var cmp = this.BoundComparer.Compare(searchedKey, key);
                start = cmp > 0 ? mid : start;
                size -= half;
            }

            var resultKey = keySelector(this.intervals[start].Key);
            var resultCmp = this.BoundComparer.Compare(searchedKey, resultKey);
            return start + (resultCmp > 0 ? 1 : 0);
        }

        private static Interval<TKey> ToInterval(TKey value) => Interval<TKey>.Singleton(value);
    }
}
