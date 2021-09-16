// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
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
        public void Add(Interval<TKey> keys, TValue value) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Remove(TKey key) => this.Remove(ToInterval(key));

        /// <inheritdoc/>
        public bool Remove(Interval<TKey> keys) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool ContainsKey(TKey key) => this.ContainsKeys(ToInterval(key));

        /// <inheritdoc/>
        public bool ContainsKeys(Interval<TKey> keys) => this.TryGetValue(keys, out _);

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value) => this.TryGetValue(ToInterval(key), out value);

        /// <inheritdoc/>
        public bool TryGetValue(Interval<TKey> keys, out TValue value) => throw new NotImplementedException();

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
