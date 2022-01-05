// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Yoakke.Collections.Internal;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Dense;

/// <summary>
/// A default <see cref="IDenseSet{T}"/> implementation backed by a list of intervals.
/// </summary>
/// <typeparam name="T">The set element and interval endpoint type.</typeparam>
public sealed class DenseSet<T> : IDenseSet<T>
{
    /// <inheritdoc/>
    public int Count => this.intervals.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// The comparer used.
    /// </summary>
    public IntervalComparer<T> Comparer => this.intervals.Comparer;

    private BoundComparer<T> BoundComparer => this.Comparer.BoundComparer;

    private readonly SortedIntervalList<T, Interval<T>> intervals;

    /// <summary>
    /// Initializes a new instance of the <see cref="DenseSet{T}"/> class.
    /// </summary>
    public DenseSet()
        : this(IntervalComparer<T>.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DenseSet{T}"/> class.
    /// </summary>
    /// <param name="comparer">The comparer to use.</param>
    public DenseSet(IntervalComparer<T> comparer)
    {
        this.intervals = new(comparer, iv => iv, (o, n) => n);
    }

    #region Elemental Operations

    /// <inheritdoc/>
    public void Clear() => this.intervals.Clear();

    /// <inheritdoc/>
    public bool Add(Interval<T> interval)
    {
        if (this.Comparer.IsEmpty(interval)) return false;

        // For an empty set, it's trivial, we just add it
        if (this.intervals.Count == 0)
        {
            this.intervals.Add(interval);
            return true;
        }

        // Not empty, find all the intervals that are touched
        var (from, to) = this.intervals.TouchingRange(interval);

        if (from == to)
        {
            // Just insert, touches nothing
            this.intervals.Insert(from, interval);
            return true;
        }
        else
        {
            // We need to remove all touched intervals
            // First we need to modify the inserted interval, because the touched intervals might extend beyond
            // the inserted one
            var firstLower = this.intervals[from].Lower;
            var lastUpper = this.intervals[to - 1].Upper;
            var lowerCmp = this.BoundComparer.Compare(firstLower, interval.Lower);
            var upperCmp = this.BoundComparer.Compare(lastUpper, interval.Upper);
            // There are 3 ways we can cover a new value
            //  - There are more than 1 touched intervals, because that means we fill out values in between
            //  - The first touched intervals lower value is above the inserted one
            //  - The last touched intervals upper value is below the inserted one
            var coversNewValue = to - from > 1 || lowerCmp > 0 || upperCmp < 0;
            if (!coversNewValue) return false;
            // It covers something new, we need to do the insertion
            interval = new(lowerCmp > 0 ? interval.Lower : firstLower, upperCmp < 0 ? interval.Upper : lastUpper);
            // Remove all touched ranges, except the first one
            this.intervals.RemoveRange(from + 1, to - from - 1);
            // We modify the first, not removed touched range to save on memory juggling a bit
            this.intervals[from] = interval;
            return true;
        }
    }

    /// <inheritdoc/>
    void ICollection<Interval<T>>.Add(Interval<T> item) => this.Add(item);

    /// <inheritdoc/>
    public bool Remove(Interval<T> interval) => this.intervals.Remove(interval);

    /// <inheritdoc/>
    public void Complement()
    {
        if (this.intervals.Count == 0)
        {
            // Inverse of the empty set is the full interval
            this.intervals.Add(Interval<T>.Full);
            return;
        }

        if (this.intervals.Count == 1
         && this.intervals[0].Lower is LowerBound<T>.Unbounded
         && this.intervals[0].Upper is UpperBound<T>.Unbounded)
        {
            // Inverse of full interval is the empty one
            this.intervals.Clear();
            return;
        }

        // The interval set is neither empty, nor full, there are 3 cases:
        //  - Both ends are unbounded: for N intervals this creates N - 1 intervals when inverted
        //  - One end is unbounded: for N intervals this creates N intervals when inverted
        //  - Both ends are bounded: for N intervals this creates N + 1 intervals when inverted
        var lowerUnbounded = this.intervals[0].Lower is LowerBound<T>.Unbounded;
        var upperUnbounded = this.intervals[^1].Upper is UpperBound<T>.Unbounded;

        if (lowerUnbounded && upperUnbounded)
        {
            var nIntervals = this.intervals.Count - 1;
            for (var i = 0; i < nIntervals; ++i)
            {
                var lower = this.intervals[i].Upper.Touching!;
                var upper = this.intervals[i + 1].Lower.Touching!;
                this.intervals[i] = new(lower, upper);
            }
            this.intervals.RemoveAt(this.intervals.Count - 1);
        }
        else if (lowerUnbounded)
        {
            var nIntervals = this.intervals.Count;
            if (nIntervals > 1)
            {
                var prevTouch = this.intervals[nIntervals - 1].Lower.Touching!;

                // Modify the last one
                var lastLower = this.intervals[nIntervals - 1].Upper.Touching!;
                this.intervals[nIntervals - 1] = new(lastLower, UpperBound<T>.Unbounded.Instance);

                for (var i = nIntervals - 2; i > 0; --i)
                {
                    var loTouch = prevTouch;
                    var hiTouch = this.intervals[i].Upper.Touching!;
                    prevTouch = this.intervals[i].Lower.Touching!;
                    this.intervals[i] = new(hiTouch, loTouch);
                }

                // First one
                var hTouch = this.intervals[0].Upper.Touching!;
                this.intervals[0] = new(hTouch, prevTouch);
            }
            else
            {
                // Modify the only one
                var lower = this.intervals[0].Upper.Touching!;
                this.intervals[0] = new(lower, UpperBound<T>.Unbounded.Instance);
            }
        }
        else if (upperUnbounded)
        {
            var nIntervals = this.intervals.Count;
            if (nIntervals > 1)
            {
                var prevTouch = this.intervals[0].Upper.Touching!;

                // Modify the first one
                var firstUpper = this.intervals[0].Lower.Touching!;
                this.intervals[0] = new(LowerBound<T>.Unbounded.Instance, firstUpper);

                for (var i = 1; i < nIntervals - 1; ++i)
                {
                    var loTouch = this.intervals[i].Lower.Touching!;
                    var hiTouch = prevTouch;
                    prevTouch = this.intervals[i].Upper.Touching!;
                    this.intervals[i] = new(hiTouch, loTouch);
                }

                // Last one
                var lTouch = this.intervals[nIntervals - 1].Lower.Touching!;
                this.intervals[nIntervals - 1] = new(prevTouch, lTouch);
            }
            else
            {
                // Modify the only one
                var upper = this.intervals[0].Lower.Touching!;
                this.intervals[0] = new(LowerBound<T>.Unbounded.Instance, upper);
            }
        }
        else
        {
            // Bounded, meaning N + 1 entries
            var nIntervals = this.intervals.Count;

            // Add a last entry
            this.intervals.Add(new(this.intervals[^1].Upper.Touching!, UpperBound<T>.Unbounded.Instance));

            var prevTouch = this.intervals[0].Upper.Touching!;

            // Modify first one
            var firstUpper = this.intervals[0].Lower.Touching!;
            this.intervals[0] = new(LowerBound<T>.Unbounded.Instance, firstUpper);

            for (var i = 1; i < nIntervals; ++i)
            {
                var loTouch = this.intervals[i].Lower.Touching!;
                var upper = prevTouch;
                prevTouch = this.intervals[i].Upper.Touching!;
                this.intervals[i] = new(upper, loTouch);
            }
        }
    }

    /// <inheritdoc/>
    public bool Contains(Interval<T> interval)
    {
        if (this.Comparer.IsEmpty(interval)) return true;
        var (from, to) = this.intervals.IntersectingRange(interval);
        if (to - from != 1) return false;
        var existing = this.intervals[from];
        return this.Comparer.Contains(existing, interval);
    }

    #endregion

    #region Set Relation

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<Interval<T>> other) => this.IsSubsetOf(other, out var proper) && proper;

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<Interval<T>> other) => this.IsSupersetOf(other, out var proper) && proper;

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<Interval<T>> other) => this.IsSubsetOf(other, out _);

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<Interval<T>> other, out bool proper)
    {
        // Just make a dense set of the other
        var otherSet = this.AsReadOnlyDenseSet(other);
        return otherSet.IsSupersetOf(this, out proper);
    }

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<Interval<T>> other) => this.IsSupersetOf(other, out _);

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<Interval<T>> other, out bool proper)
    {
        var otherSet = this.AsReadOnlyDenseSet(other);
        proper = this.intervals.Count > otherSet.Count;
        foreach (var iv in otherSet)
        {
            var (from, to) = this.intervals.IntersectingRange(iv);
            if (to - from != 1) return false;
            var existing = this.intervals[from];
            // Some efficiency on the comparisons
            var lowerCmp = this.BoundComparer.Compare(existing.Lower, iv.Lower);
            var upperCmp = this.BoundComparer.Compare(existing.Upper, iv.Upper);
            if (lowerCmp > 0 || upperCmp < 0) return false;
            proper = proper || lowerCmp < 0 || upperCmp > 0;
        }
        return true;
    }

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<Interval<T>> other) => other.Any(this.Overlaps);

    /// <inheritdoc/>
    public bool Overlaps(Interval<T> interval)
    {
        if (this.Comparer.IsEmpty(interval)) return false;
        var (from, to) = this.intervals.IntersectingRange(interval);
        return from != to;
    }

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<Interval<T>> other) => this.IsSubsetOf(other) && this.IsSupersetOf(other);

    #endregion

    #region Set Operations

    /// <inheritdoc/>
    public void ExceptWith(IEnumerable<Interval<T>> other)
    {
        foreach (var iv in other) this.Remove(iv);
    }

    /// <inheritdoc/>
    public void IntersectWith(IEnumerable<Interval<T>> other)
    {
        // Make the result using the identity A /\ B = ~B \ A
        var otherSet = this.ToDenseSet(other);
        otherSet.Complement();
        otherSet.ExceptWith(this);

        // Copy back
        this.intervals.Clear();
        this.intervals.AddRange(otherSet);
    }

    /// <inheritdoc/>
    public void SymmetricExceptWith(IEnumerable<Interval<T>> other)
    {
        // Use the identity A xor B = (A \ B) U (B \ A)
        var thisSet = this.intervals.ToList();
        var otherSet = this.ToDenseSet(other);
        this.ExceptWith(otherSet);
        otherSet.ExceptWith(thisSet);
        this.UnionWith(otherSet);
    }

    /// <inheritdoc/>
    public void UnionWith(IEnumerable<Interval<T>> other)
    {
        foreach (var iv in other) this.Add(iv);
    }

    #endregion

    /// <inheritdoc/>
    public void CopyTo(Interval<T>[] array, int arrayIndex) => this.intervals.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<Interval<T>> GetEnumerator() => this.intervals.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => (this.intervals as IEnumerable).GetEnumerator();

    private IDenseSet<T> ToDenseSet(IEnumerable<Interval<T>> intervals)
    {
        var result = new DenseSet<T>(this.Comparer);
        foreach (var iv in intervals) result.Add(iv);
        return result;
    }

    private IReadOnlyDenseSet<T> AsReadOnlyDenseSet(IEnumerable<Interval<T>> intervals)
    {
        if (intervals is IReadOnlyDenseSet<T> set) return set;
        var result = new DenseSet<T>(this.Comparer);
        foreach (var iv in intervals) result.Add(iv);
        return result;
    }
}
