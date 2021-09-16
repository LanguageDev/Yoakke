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
                // Intersects one interval
                // This interval potentially has to be split once or twice
                throw new NotImplementedException();
            }
            else
            {
                // Intersects more intervals
                // The first and last intervals potentially have to be split
                // The rest are unified and spaces inbetween are filled in
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public bool Remove(TKey key) => this.Remove(ToInterval(key));

        /// <inheritdoc/>
        public bool Remove(Interval<TKey> keys) => throw new NotImplementedException();

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
