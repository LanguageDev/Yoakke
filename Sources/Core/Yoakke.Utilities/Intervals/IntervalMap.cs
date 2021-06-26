// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Yoakke.Utilities.Intervals
{
    /// <summary>
    /// A generic interval map implementation.
    /// </summary>
    /// <typeparam name="TKey">The interval value type.</typeparam>
    /// <typeparam name="TValue">The associated value type.</typeparam>
    public class IntervalMap<TKey, TValue> : IIntervalMap<TKey, TValue>
    {
        public int Count => this.values.Count;

        public IEnumerable<Interval<TKey>> Intervals => this.values.Select(p => p.Key);

        public IEnumerable<TValue> Values => this.values.Select(p => p.Value);

        /// <summary>
        /// The comparer used for sorting interval keys.
        /// </summary>
        public IComparer<TKey> Comparer { get; }

        /// <summary>
        /// The equality comparer used for merging values.
        /// </summary>
        public IEqualityComparer<TValue> ValueComparer { get; }

        public TValue this[TKey key]
        {
            get
            {
                if (!this.TryGetValue(key, out var value)) throw new KeyNotFoundException();
                return value!;
            }
        }

        private readonly List<KeyValuePair<Interval<TKey>, TValue>> values = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalMap{TKey, TValue}"/> class.
        /// </summary>
        public IntervalMap()
            : this(Comparer<TKey>.Default, EqualityComparer<TValue>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalMap{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="comparer">The key comparer to use.</param>
        public IntervalMap(IComparer<TKey> comparer)
            : this(comparer, EqualityComparer<TValue>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalMap{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="comparer">The key comparer to use.</param>
        /// <param name="valueComparer">The value equality comparer to use.</param>
        public IntervalMap(IComparer<TKey> comparer, IEqualityComparer<TValue> valueComparer)
        {
            this.Comparer = comparer;
            this.ValueComparer = valueComparer;
        }

        public void Clear() => this.values.Clear();

        public bool ContainsKey(TKey key) => this.TryGetValue(key, out var _);

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue? value)
        {
            var interval = Interval<TKey>.Singleton(key);
            var (from, to) = this.IntersectingIndexRange(interval);
            if (from == to)
            {
                value = default;
                return false;
            }
            else
            {
                value = this.values[from].Value;
                return true;
            }
        }

        public void AddAndUpdate(Interval<TKey> interval, TValue value, Func<TValue, TValue, TValue> updateFunc)
        {
            if (this.values.Count == 0)
            {
                this.values.Add(new KeyValuePair<Interval<TKey>, TValue>(interval, value));
                return;
            }
            // Not empty
            var (start, end) = this.IntersectingIndexRange(interval);
            if (start == end)
            {
                // Intersects nothing, just insert
                this.values.Insert(start, new KeyValuePair<Interval<TKey>, TValue>(interval, value));
            }
            else if (end - start == 1)
            {
                // Intersects one entry
                this.AddAndUpdateSingle(start, interval, value, updateFunc);
            }
            else
            {
                // Intersects multiple entries
                var (lower, off) = this.AddAndUpdateSingleLower(start, interval, value, updateFunc);
                var lastUpper = this.AddAndUpdateSingleUpper(end + off - 1, interval, value, updateFunc);

                var idx = start + off + 1;
                for (var i = start; i < end - 2; ++i)
                {
                    // Unify values
                    this.values[idx] = new KeyValuePair<Interval<TKey>, TValue>(this.values[idx].Key, updateFunc(this.values[idx].Value, value));
                    // Add interval in between
                    var upper = this.values[idx].Key.Lower.GetTouching()!.Value;
                    var between = new Interval<TKey>(lower, upper);
                    lower = this.values[idx].Key.Upper.GetTouching()!.Value;
                    if (!between.IsEmpty(this.Comparer))
                    {
                        this.values.Insert(idx, new KeyValuePair<Interval<TKey>, TValue>(between, value));
                        idx += 2;
                    }
                    else
                    {
                        idx += 1;
                    }
                }
                var lastBetween = new Interval<TKey>(lower, lastUpper);
                if (!lastBetween.IsEmpty(this.Comparer)) this.values.Insert(idx, new KeyValuePair<Interval<TKey>, TValue>(lastBetween, value));
            }
        }

        public void MergeTouching()
        {
            for (var i = 0; i < this.values.Count - 1; )
            {
                var v1 = this.values[i];
                var v2 = this.values[i + 1];
                if (v1.Key.Upper.IsTouching(v2.Key.Lower, this.Comparer) && this.ValueComparer.Equals(v1.Value, v2.Value))
                {
                    // They are touching and have the same values, merge them
                    this.values[i] = new KeyValuePair<Interval<TKey>, TValue>(
                        new Interval<TKey>(v1.Key.Lower, v2.Key.Upper),
                        v1.Value);
                    this.values.RemoveAt(i + 1);
                }
                else
                {
                    ++i;
                }
            }
        }

        public IEnumerator<KeyValuePair<Interval<TKey>, TValue>> GetEnumerator() => this.values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private void AddAndUpdateSingle(int idx, Interval<TKey> interval, TValue value, Func<TValue, TValue, TValue> updateFunc)
        {
            void Update(
                (Interval<TKey>, TValue)? before,
                (Interval<TKey>, TValue) middle,
                (Interval<TKey>, TValue)? after) => this.AddAndUpdateSingleImpl(idx, before, middle, after);

            var existing = this.values[idx];
            var rel = existing.Key.RelationTo(interval, this.Comparer);
            switch (rel)
            {
            case IntervalRelation<TKey>.Equal:
                Update(null, (existing.Key, updateFunc(existing.Value, value)), null);
                break;

            case IntervalRelation<TKey>.Containing containment:
                if (existing.Key.Lower.CompareTo(containment.Contained.Lower, this.Comparer) == 0)
                {
                    // The new interval completely covers the existing one
                    Update(
                        (containment.FirstDisjunct, value),
                        (containment.Contained, updateFunc(existing.Value, value)),
                        (containment.SecondDisjunct, value));
                }
                else
                {
                    // The new interval is completely inside the existing one
                    Update(
                        (containment.FirstDisjunct, existing.Value),
                        (containment.Contained, updateFunc(existing.Value, value)),
                        (containment.SecondDisjunct, existing.Value));
                }
                break;

            case IntervalRelation<TKey>.Overlapping overlap:
                if (existing.Key.Lower.CompareTo(overlap.FirstDisjunct.Lower, this.Comparer) == 0)
                {
                    // Existing is before the new one
                    Update(
                        (overlap.FirstDisjunct, existing.Value),
                        (overlap.Overlap, updateFunc(existing.Value, value)),
                        (overlap.SecondDisjunct, value));
                }
                else
                {
                    // Existing is after the new one
                    Update(
                        (overlap.FirstDisjunct, value),
                        (overlap.Overlap, updateFunc(existing.Value, value)),
                        (overlap.SecondDisjunct, existing.Value));
                }
                break;

            case IntervalRelation<TKey>.Starting starting:
                if (existing.Key.Upper.CompareTo(starting.Overlap.Upper, this.Comparer) == 0)
                {
                    // New interval overextends existing
                    Update(
                        null,
                        (starting.Overlap, updateFunc(existing.Value, value)),
                        (starting.Disjunct, value));
                }
                else
                {
                    // Existing overextends new
                    Update(
                        null,
                        (starting.Overlap, updateFunc(existing.Value, value)),
                        (starting.Disjunct, existing.Value));
                }
                break;

            case IntervalRelation<TKey>.Finishing finishing:
                if (existing.Key.Lower.CompareTo(finishing.Disjunct.Lower, this.Comparer) == 0)
                {
                    // Existing overextends new
                    Update(
                        (finishing.Disjunct, existing.Value),
                        (finishing.Overlap, updateFunc(existing.Value, value)),
                        null);
                }
                else
                {
                    // New interval overextends existing
                    Update(
                        (finishing.Disjunct, value),
                        (finishing.Overlap, updateFunc(existing.Value, value)),
                        null);
                }
                break;

            default: throw new InvalidOperationException();
            }
        }

        private (LowerBound<TKey> Bound, int Offset) AddAndUpdateSingleLower(int idx, Interval<TKey> interval, TValue value, Func<TValue, TValue, TValue> updateFunc)
        {
            void Update(
                (Interval<TKey>, TValue)? before,
                (Interval<TKey>, TValue) middle) => this.AddAndUpdateSingleImpl(idx, before, middle, null);

            var existing = this.values[idx];
            var rel = existing.Key.RelationTo(interval, this.Comparer);
            switch (rel)
            {
            case IntervalRelation<TKey>.Containing containment:
                // The newly added interval must completely cover the existing one
                Update((containment.FirstDisjunct, value), (containment.Contained, updateFunc(existing.Value, value)));
                return (containment.SecondDisjunct.Lower, 1);

            case IntervalRelation<TKey>.Overlapping overlap:
                Update((overlap.FirstDisjunct, existing.Value), (overlap.Overlap, updateFunc(existing.Value, value)));
                return (overlap.SecondDisjunct.Lower, 1);

            case IntervalRelation<TKey>.Starting starting:
                Update(null, (starting.Overlap, updateFunc(existing.Value, value)));
                return (starting.Disjunct.Lower, 0);

            default: throw new InvalidOperationException();
            }
        }

        private UpperBound<TKey> AddAndUpdateSingleUpper(int idx, Interval<TKey> interval, TValue value, Func<TValue, TValue, TValue> updateFunc)
        {
            void Update(
                (Interval<TKey>, TValue)? before,
                (Interval<TKey>, TValue) middle,
                (Interval<TKey>, TValue)? after) => this.AddAndUpdateSingleImpl(idx, before, middle, after);

            var existing = this.values[idx];
            var rel = existing.Key.RelationTo(interval, this.Comparer);
            switch (rel)
            {
            case IntervalRelation<TKey>.Containing containment:
                // The newly added interval must completely cover the existing one
                Update(null, (containment.Contained, updateFunc(existing.Value, value)), (containment.SecondDisjunct, value));
                return containment.FirstDisjunct.Upper;

            case IntervalRelation<TKey>.Overlapping overlap:
                Update((overlap.Overlap, updateFunc(existing.Value, value)), (overlap.SecondDisjunct, existing.Value), null);
                return overlap.FirstDisjunct.Upper;

            case IntervalRelation<TKey>.Finishing finishing:
                Update(null, (finishing.Overlap, updateFunc(existing.Value, value)), null);
                return finishing.Disjunct.Upper;

            default: throw new InvalidOperationException();
            }
        }

        private void AddAndUpdateSingleImpl(
            int idx,
            (Interval<TKey>, TValue)? before,
            (Interval<TKey>, TValue) middle,
            (Interval<TKey>, TValue)? after)
        {
            this.values[idx] = new KeyValuePair<Interval<TKey>, TValue>(middle.Item1, middle.Item2);
            // Add the sides (first the latter one so the indicies don't shift)
            if (after != null)
            {
                var afterValue = after.Value;
                this.values.Insert(idx + 1, new KeyValuePair<Interval<TKey>, TValue>(afterValue.Item1, afterValue.Item2));
            }
            if (before != null)
            {
                var beforeValue = before.Value;
                this.values.Insert(idx, new KeyValuePair<Interval<TKey>, TValue>(beforeValue.Item1, beforeValue.Item2));
            }
        }

        private (int, int) IntersectingIndexRange(Interval<TKey> interval)
        {
            var (from, _) = this.values.BinarySearch(interval.Lower, iv => iv.Key.Upper, (k1, k2) => k1.CompareTo(k2, this.Comparer));
            var (to, _) = this.values.BinarySearch(from, interval.Upper, iv => iv.Key.Lower, (k1, k2) => k1.CompareTo(k2, this.Comparer));
            return (from, to);
        }
    }
}
