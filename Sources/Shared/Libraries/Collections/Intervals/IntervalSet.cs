// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Collections.Intervals.Internal;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// Stores a disjunct set of intervals.
/// </summary>
/// <typeparam name="T">The interval endpoint type.</typeparam>
public sealed class IntervalSet<T> : IReadOnlyCollection<Interval<T>>, ICollection<Interval<T>>,
                                     ISet<Interval<T>>
{
    /// <inheritdoc/>
    int IReadOnlyCollection<Interval<T>>.Count => this.intervals.Count;

    /// <inheritdoc/>
    int ICollection<Interval<T>>.Count => this.intervals.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// The interval comparer used.
    /// </summary>
    public IntervalComparer<T> IntervalComparer => this.intervals.Comparer;

    /// <summary>
    /// The endpoint comparer used.
    /// </summary>
    public EndpointComparer<T> EndpointComparer => this.IntervalComparer.EndpointComparer;

    /// <summary>
    /// The value comparer used.
    /// </summary>
    public IComparer<T> ValueComparer => this.EndpointComparer.ValueComparer;

    /// <summary>
    /// The value equality comparer used.
    /// </summary>
    public IEqualityComparer<T> ValueEqualityComparer => this.EndpointComparer.ValueEqualityComparer;

    private readonly SortedIntervalList<T, byte> intervals;

    /// <summary>
    /// Constructs a new, empty <see cref="IntervalSet{T}"/>.
    /// </summary>
    /// <param name="comparer">The interval comparer to use.</param>
    public IntervalSet(IntervalComparer<T>? comparer)
    {
        this.intervals = new(comparer ?? IntervalComparer<T>.Default);
    }

    /// <summary>
    /// Constructs a new, empty <see cref="IntervalSet{T}"/>.
    /// </summary>
    /// <param name="comparer">The endpoint comparer to use.</param>
    public IntervalSet(EndpointComparer<T>? comparer)
        : this(comparer is null ? null : new IntervalComparer<T>(comparer))
    {
    }

    /// <summary>
    /// Constructs a new <see cref="IntervalSet{T}"/> from the given intervals.
    /// </summary>
    /// <param name="intervals">The intervals to add to the set.</param>
    /// <param name="comparer">The interval comparer to use.</param>
    public IntervalSet(IEnumerable<Interval<T>> intervals, IntervalComparer<T>? comparer = null)
        : this(comparer)
    {
        this.UnionWith(intervals);
    }

    /// <summary>
    /// Constructs a new <see cref="IntervalSet{T}"/> from the given intervals.
    /// </summary>
    /// <param name="intervals">The intervals to add to the set.</param>
    /// <param name="comparer">The endpoint comparer to use.</param>
    public IntervalSet(IEnumerable<Interval<T>> intervals, EndpointComparer<T>? comparer)
        : this(comparer)
    {
        this.UnionWith(intervals);
    }

    /// <summary>
    /// Checks if this set contains a given value.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True, if <paramref name="value"/> is contained; false otherwise.</returns>
    public bool Contains(T value) => this.Contains(Interval.Singleton(value));

    /// <inheritdoc/>
    public bool Contains(Interval<T> item)
    {
        if (this.IntervalComparer.IsEmpty(item)) return true;
        var (from, to) = this.intervals.IntersectingRange(item);
        if (to - from != 1) return false;
        var existing = this.intervals[from].Key;
        return this.IntervalComparer.Contains(existing, item);
    }

    /// <inheritdoc/>
    void ICollection<Interval<T>>.Add(Interval<T> item) => this.Add(item);

    /// <summary>
    /// Adds a single item to this set.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <returns>True, if there were any values added.</returns>
    public bool Add(T item) => this.Add(Interval.Singleton(item));

    /// <summary>
    /// Adds an interval to this set.
    /// </summary>
    /// <param name="item">The interval to add.</param>
    /// <returns>True, if there were any values added.</returns>
    public bool Add(Interval<T> item)
    {
        if (this.IntervalComparer.IsEmpty(item)) return false;

        // For an empty set, it's trivial, we just add it
        if (this.intervals.Count == 0)
        {
            this.intervals.Add(new(item, default));
            return true;
        }

        // Not empty, find all the intervals that are touched
        var (from, to) = this.intervals.TouchingRange(item);

        if (from == to)
        {
            // Just insert, touches nothing
            this.intervals.Insert(from, new(item, default));
            return true;
        }
        else
        {
            // We need to remove all touched intervals
            // First we need to modify the inserted interval, because the touched intervals might extend beyond
            // the inserted one
            var firstLower = this.intervals[from].Key.Lower;
            var lastUpper = this.intervals[to - 1].Key.Upper;
            var lowerCmp = this.EndpointComparer.Compare(firstLower, item.Lower);
            var upperCmp = this.EndpointComparer.Compare(lastUpper, item.Upper);
            // There are 3 ways we can cover a new value
            //  - There are more than 1 touched intervals, because that means we fill out values in between
            //  - The first touched intervals lower value is above the inserted one
            //  - The last touched intervals upper value is below the inserted one
            var coversNewValue = to - from > 1 || lowerCmp > 0 || upperCmp < 0;
            if (!coversNewValue) return false;
            // It covers something new, we need to do the insertion
            item = new(lowerCmp > 0 ? item.Lower : firstLower, upperCmp < 0 ? item.Upper : lastUpper);
            // Remove all touched ranges, except the first one
            this.intervals.RemoveRange(from + 1, to - from - 1);
            // We modify the first, not removed touched range to save on memory juggling a bit
            this.intervals[from] = new(item, default);
            return true;
        }
    }

    /// <summary>
    /// Removes a single value from this set.
    /// </summary>
    /// <param name="item">The value to remove.</param>
    /// <returns>True, if <paramref name="item"/> was present and removed; false otherwise.</returns>
    public bool Remove(T item) => this.Remove(Interval.Singleton(item));

    /// <inheritdoc/>
    public bool Remove(Interval<T> item) => this.intervals.Remove(item);

    /// <inheritdoc/>
    public void Clear() => this.intervals.Clear();

    /// <summary>
    /// Complements this set, meaning that all values will be present that were not before and vice versa.
    /// </summary>
    public void Complement()
    {
        if (this.intervals.Count == 0)
        {
            // Inverse of the empty set is the full interval
            this.intervals.Add(new(Interval.Full<T>(), default));
            return;
        }

        var lowerUnbounded = this.intervals[0].Key.Lower.Type == EndpointType.Unbounded;
        var upperUnbounded = this.intervals[^1].Key.Upper.Type == EndpointType.Unbounded;

        if (this.intervals.Count == 1 && lowerUnbounded && upperUnbounded)
        {
            // Inverse of full interval is the empty one
            this.intervals.Clear();
            return;
        }

        // The interval set is neither empty, nor full, there are 3 cases:
        //  - Both ends are unbounded: for N intervals this creates N - 1 intervals when inverted
        //  - One end is unbounded: for N intervals this creates N intervals when inverted
        //  - Both ends are bounded: for N intervals this creates N + 1 intervals when inverted
        if (lowerUnbounded && upperUnbounded)
        {
            var nIntervals = this.intervals.Count - 1;
            for (var i = 0; i < nIntervals; ++i)
            {
                var lower = this.intervals[i].Key.Upper.Touching!.Value;
                var upper = this.intervals[i + 1].Key.Lower.Touching!.Value;
                this.intervals[i] = new(new(lower, upper), default);
            }
            this.intervals.RemoveAt(this.intervals.Count - 1);
        }
        else if (lowerUnbounded)
        {
            var nIntervals = this.intervals.Count;
            if (nIntervals > 1)
            {
                var prevTouch = this.intervals[nIntervals - 1].Key.Lower.Touching!.Value;

                // Modify the last one
                var lastLower = this.intervals[nIntervals - 1].Key.Upper.Touching!.Value;
                this.intervals[nIntervals - 1] = new(new(lastLower, UpperEndpoint.Unbounded<T>()), default);

                for (var i = nIntervals - 2; i > 0; --i)
                {
                    var loTouch = prevTouch;
                    var hiTouch = this.intervals[i].Key.Upper.Touching!.Value;
                    prevTouch = this.intervals[i].Key.Lower.Touching!.Value;
                    this.intervals[i] = new(new(hiTouch, loTouch), default);
                }

                // First one
                var hTouch = this.intervals[0].Key.Upper.Touching!.Value;
                this.intervals[0] = new(new(hTouch, prevTouch), default);
            }
            else
            {
                // Modify the only one
                var lower = this.intervals[0].Key.Upper.Touching!.Value;
                this.intervals[0] = new(new(lower, UpperEndpoint.Unbounded<T>()), default);
            }
        }
        else if (upperUnbounded)
        {
            var nIntervals = this.intervals.Count;
            if (nIntervals > 1)
            {
                var prevTouch = this.intervals[0].Key.Upper.Touching!.Value;

                // Modify the first one
                var firstUpper = this.intervals[0].Key.Lower.Touching!.Value;
                this.intervals[0] = new(new(LowerEndpoint.Unbounded<T>(), firstUpper), default);

                for (var i = 1; i < nIntervals - 1; ++i)
                {
                    var loTouch = this.intervals[i].Key.Lower.Touching!.Value;
                    var hiTouch = prevTouch;
                    prevTouch = this.intervals[i].Key.Upper.Touching!.Value;
                    this.intervals[i] = new(new(hiTouch, loTouch), default);
                }

                // Last one
                var lTouch = this.intervals[nIntervals - 1].Key.Lower.Touching!.Value;
                this.intervals[nIntervals - 1] = new(new(prevTouch, lTouch), default);
            }
            else
            {
                // Modify the only one
                var upper = this.intervals[0].Key.Lower.Touching!.Value;
                this.intervals[0] = new(new(LowerEndpoint.Unbounded<T>(), upper), default);
            }
        }
        else
        {
            // Bounded, meaning N + 1 entries
            var nIntervals = this.intervals.Count;

            // Add a last entry
            this.intervals.Add(new(new(this.intervals[^1].Key.Upper.Touching!.Value, UpperEndpoint.Unbounded<T>()), default));

            var prevTouch = this.intervals[0].Key.Upper.Touching!.Value;

            // Modify first one
            var firstUpper = this.intervals[0].Key.Lower.Touching!.Value;
            this.intervals[0] = new(new(LowerEndpoint.Unbounded<T>(), firstUpper), default);

            for (var i = 1; i < nIntervals; ++i)
            {
                var loTouch = this.intervals[i].Key.Lower.Touching!.Value;
                var upper = prevTouch;
                prevTouch = this.intervals[i].Key.Upper.Touching!.Value;
                this.intervals[i] = new(new(upper, loTouch), default);
            }
        }
    }

    /// <inheritdoc/>
    public void CopyTo(Interval<T>[] array, int arrayIndex)
    {
        if (arrayIndex + this.intervals.Count > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));

        for (var i = 0; i < this.intervals.Count; ++i) array[arrayIndex + i] = this.intervals[i].Key;
    }

    /// <inheritdoc/>
    public IEnumerator<Interval<T>> GetEnumerator() => this.intervals.Select(kv => kv.Key).GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<Interval<T>> other) => this.IsSubsetOf(other, out var p) && p;

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<Interval<T>> other) => this.IsSupersetOf(other, out var p) && p;

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<Interval<T>> other) => this.IsSubsetOf(other, out _);

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<Interval<T>> other) => this.IsSupersetOf(other, out _);

    /// <summary>
    /// Checks, if this set overlaps with an interval.
    /// </summary>
    /// <param name="interval">The interval to check overlap with.</param>
    /// <returns>True, if <paramref name="interval"/> overlaps with this set.</returns>
    public bool Overlaps(Interval<T> interval)
    {
        if (this.IntervalComparer.IsEmpty(interval)) return false;
        var (from, to) = this.intervals.IntersectingRange(interval);
        return from != to;
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<Interval<T>> other) => other.Any(this.Overlaps);

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<Interval<T>> other)
    {
        var otherSet = this.AsIntervalSet(other);
        if (this.intervals.Count != otherSet.intervals.Count) return false;
        for (var i = 0; i < this.intervals.Count; ++i)
        {
            if (!this.IntervalComparer.Equals(this.intervals[i].Key, otherSet.intervals[i].Key)) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<Interval<T>> other)
    {
        foreach (var iv in other) this.Add(iv);
    }

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<Interval<T>> other)
    {
        foreach (var iv in other) this.Remove(iv);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<Interval<T>> other)
    {
        // Use the identity A xor B = (A \ B) U (B \ A)
        var thisSet = this.ToList();
        var otherSet = new IntervalSet<T>(other, this.IntervalComparer);
        this.ExceptWith(otherSet);
        otherSet.ExceptWith(thisSet);
        this.UnionWith(otherSet);
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<Interval<T>> other)
    {
        // Make the result using the identity A /\ B = A \ (A \ B)
        var otherSet = new IntervalSet<T>(this, this.IntervalComparer);
        otherSet.ExceptWith(other);
        this.ExceptWith(otherSet);
    }

    private bool IsSupersetOf(IEnumerable<Interval<T>> other, out bool proper)
    {
        var otherSet = this.AsIntervalSet(other);
        proper = this.intervals.Count > otherSet.intervals.Count;
        foreach (var iv in otherSet)
        {
            var (from, to) = this.intervals.IntersectingRange(iv);
            if (to - from != 1) return false;
            var existing = this.intervals[from];
            // Some efficiency on the comparisons
            var lowerCmp = this.EndpointComparer.Compare(existing.Key.Lower, iv.Lower);
            var upperCmp = this.EndpointComparer.Compare(existing.Key.Upper, iv.Upper);
            if (lowerCmp > 0 || upperCmp < 0) return false;
            proper = proper || lowerCmp < 0 || upperCmp > 0;
        }
        return true;
    }

    private bool IsSubsetOf(IEnumerable<Interval<T>> other, out bool proper)
    {
        // Just make a dense set of the other
        var otherSet = this.AsIntervalSet(other);
        return otherSet.IsSupersetOf(this, out proper);
    }

    private IntervalSet<T> AsIntervalSet(IEnumerable<Interval<T>> intervals)
    {
        if (intervals is IntervalSet<T> set) return set;
        return new(intervals, this.IntervalComparer);
    }
}
