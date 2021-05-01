using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// A generic interval map implementation.
    /// </summary>
    /// <typeparam name="TKey">The interval value type.</typeparam>
    /// <typeparam name="TValue">The associated value type.</typeparam>
    public class IntervalMap<TKey, TValue> : IIntervalMap<TKey, TValue>
    {
        public int Count => values.Count;
        public IEnumerable<Interval<TKey>> Intervals => values.Select(p => p.Key);
        public IEnumerable<TValue> Values => values.Select(p => p.Value);

        public TValue this[TKey key]
        {
            get
            {
                if (!TryGetValue(key, out var value)) throw new KeyNotFoundException();
                return value;
            }
        }

        private List<KeyValuePair<Interval<TKey>, TValue>> values = new List<KeyValuePair<Interval<TKey>, TValue>>();
        private IComparer<TKey> comparer;

        /// <summary>
        /// Initializes an empty <see cref="IntervalMap{TKey, TValue}"/> with the default comparer.
        /// </summary>
        public IntervalMap()
            : this(Comparer<TKey>.Default)
        {
        }

        /// <summary>
        /// Initializes an empty <see cref="IntervalMap{TKey, TValue}"/> with a given comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use.</param>
        public IntervalMap(IComparer<TKey> comparer)
        {
            this.comparer = comparer;
        }

        public void Clear() => values.Clear();
        public bool ContainsKey(TKey key) => TryGetValue(key, out var _);

        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue? value)
        {
            var interval = Interval<TKey>.Singleton(key);
            var (from, to) = IntersectingIndexRange(interval);
            if (from == to)
            {
                value = default;
                return false;
            }
            else
            {
                value = values[from].Value;
                return true;
            }
        }

        public void AddAndUpdate(Interval<TKey> interval, TValue value, Func<TValue, TValue, TValue> updateFunc)
        {
            if (values.Count == 0)
            {
                values.Add(new KeyValuePair<Interval<TKey>, TValue>(interval, value));
                return;
            }
            // Not empty
            var (start, end) = IntersectingIndexRange(interval);
            if (start == end)
            {
                // Intersects nothing, just insert
                values.Insert(start, new KeyValuePair<Interval<TKey>, TValue>(interval, value));
            }
            else if (end - start == 1)
            {
                // Intersects one entry
                AddAndUpdateSingle(start, interval, value, updateFunc);
            }
            else
            {
                // Intersects multiple entries
                var (lower, off) = AddAndUpdateSingleLower(start, interval, value, updateFunc);
                var lastUpper = AddAndUpdateSingleUpper(end + off - 1, interval, value, updateFunc);

                var idx = start + off + 1;
                for (var i = start; i < end - 2; ++i)
                {
                    // Unify values
                    values[idx] = new KeyValuePair<Interval<TKey>, TValue>(values[idx].Key, updateFunc(values[idx].Value, value));
                    // Add interval in between
                    var upper = values[idx].Key.Lower.GetTouching().Value;
                    var between = new Interval<TKey>(lower, upper);
                    lower = values[idx].Key.Upper.GetTouching().Value;
                    if (!between.IsEmpty(comparer))
                    {
                        values.Insert(idx, new KeyValuePair<Interval<TKey>, TValue>(between, value));
                        idx += 2;
                    }
                    else
                    {
                        idx += 1;
                    }
                }
                var lastBetween = new Interval<TKey>(lower, lastUpper);
                if (!lastBetween.IsEmpty(comparer)) values.Insert(idx, new KeyValuePair<Interval<TKey>, TValue>(lastBetween, value));
            }
        }

        public IEnumerator<KeyValuePair<Interval<TKey>, TValue>> GetEnumerator() => values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void AddAndUpdateSingle(int idx, Interval<TKey> interval, TValue value, Func<TValue, TValue, TValue> updateFunc)
        {
            void Update(
                (Interval<TKey>, TValue)? before,
                (Interval<TKey>, TValue) middle,
                (Interval<TKey>, TValue)? after) => AddAndUpdateSingleImpl(idx, before, middle, after);

            var existing = values[idx];
            var rel = existing.Key.RelationTo(interval, comparer);
            switch (rel)
            {
            case IntervalRelation<TKey>.Equal:
                Update(null, (existing.Key, updateFunc(existing.Value, value)), null);
                break;

            case IntervalRelation<TKey>.Containing containment:
                if (existing.Key.Lower.CompareTo(containment.Contained.Lower, comparer) == 0)
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
                if (existing.Key.Lower.CompareTo(overlap.FirstDisjunct.Lower, comparer) == 0)
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
                if (existing.Key.Upper.CompareTo(starting.Overlap.Upper, comparer) == 0)
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
                if (existing.Key.Lower.CompareTo(finishing.Disjunct.Lower, comparer) == 0)
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
                (Interval<TKey>, TValue) middle) => AddAndUpdateSingleImpl(idx, before, middle, null);

            var existing = values[idx];
            var rel = existing.Key.RelationTo(interval, comparer);
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
                (Interval<TKey>, TValue)? after) => AddAndUpdateSingleImpl(idx, before, middle, after);

            var existing = values[idx];
            var rel = existing.Key.RelationTo(interval, comparer);
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
            values[idx] = new KeyValuePair<Interval<TKey>, TValue>(middle.Item1, middle.Item2);
            // Add the sides (first the latter one so the indicies don't shift)
            if (after != null)
            {
                var afterValue = after.Value;
                values.Insert(idx + 1, new KeyValuePair<Interval<TKey>, TValue>(afterValue.Item1, afterValue.Item2));
            }
            if (before != null)
            {
                var beforeValue = before.Value;
                values.Insert(idx, new KeyValuePair<Interval<TKey>, TValue>(beforeValue.Item1, beforeValue.Item2));
            }
        }

        private (int, int) IntersectingIndexRange(Interval<TKey> interval)
        {
            ReadOnlySpan<KeyValuePair<Interval<TKey>, TValue>> span = CollectionsMarshal.AsSpan(values);
            var (from, _) = span.BinarySearch(interval.Lower, iv => iv.Key.Upper, (k1, k2) => k1.CompareTo(k2, comparer));
            var (to, _) = span.Slice(from).BinarySearch(interval.Upper, iv => iv.Key.Lower, (k1, k2) => k1.CompareTo(k2, comparer));
            to += from;
            return (from, to);
        }
    }
}
