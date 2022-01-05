// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Internal;

/// <summary>
/// Represents a sorted list of intervals associated to some other value to help the interval set and map
/// implementations.
/// </summary>
/// <typeparam name="TKey">The interval endpoint type.</typeparam>
/// <typeparam name="TValue">The stored type.</typeparam>
internal class SortedIntervalList<TKey, TValue> : IReadOnlyList<TValue>, IList<TValue>
{
    /// <inheritdoc/>
    public int Count => this.values.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public TValue this[int index] { get => this.values[index]; set => this.values[index] = value; }

    /// <summary>
    /// The comparer used.
    /// </summary>
    public IntervalComparer<TKey> Comparer { get; }

    private BoundComparer<TKey> BoundComparer => this.Comparer.BoundComparer;

    private readonly List<TValue> values = new();
    private readonly Func<TValue, Interval<TKey>> intervalSelector;
    private readonly Func<TValue, Interval<TKey>, TValue> intervalMover;

    /// <summary>
    /// Initializes a new instance of the <see cref="SortedIntervalList{TStored, TValue}"/> class.
    /// </summary>
    /// <param name="comparer">The comparer to use.</param>
    /// <param name="intervalSelector">The function to select intervals from the elmenets.</param>
    /// <param name="intervalMover">The function that moves a value to a new interval.</param>
    public SortedIntervalList(
        IntervalComparer<TKey> comparer,
        Func<TValue, Interval<TKey>> intervalSelector,
        Func<TValue, Interval<TKey>, TValue> intervalMover)
    {
        this.Comparer = comparer;
        this.intervalSelector = intervalSelector;
        this.intervalMover = intervalMover;
    }

    /// <summary>
    /// Removes the values that <paramref name="interval"/> covers.
    /// </summary>
    /// <param name="interval">The interval of values to remove.</param>
    /// <returns>True, if any values were removed.</returns>
    public bool Remove(Interval<TKey> interval)
    {
        // An empty set or an empty removal is trivial
        if (this.values.Count == 0 || this.Comparer.IsEmpty(interval)) return false;

        // Not empty, find all the intervals that are intersecting
        var (from, to) = this.IntersectingRange(interval);

        // If the removed interval intersects nothing, we are done
        if (from == to) return false;

        if (to - from == 1)
        {
            // Intersects a single interval
            var existing = this.values[from];
            var existingLower = this.intervalSelector(existing).Lower;
            var existingUpper = this.intervalSelector(existing).Upper;
            var lowerCompare = this.BoundComparer.Compare(existingLower, interval.Lower);
            var upperCompare = this.BoundComparer.Compare(existingUpper, interval.Upper);
            if (lowerCompare >= 0 && upperCompare <= 0)
            {
                // Simplest case, we just remove the entry, as the interval completely covers this one
                this.values.RemoveAt(from);
            }
            else if (lowerCompare >= 0)
            {
                // The upper bound does not match, we need to modify
                var newInterval = new Interval<TKey>(interval.Upper.Touching!, existingUpper);
                this.values[from] = this.intervalMover(existing, newInterval);
            }
            else if (upperCompare <= 0)
            {
                // The lower bound does not match, we need to modify
                var newInterval = new Interval<TKey>(existingLower, interval.Lower.Touching!);
                this.values[from] = this.intervalMover(existing, newInterval);
            }
            else
            {
                // The interval is being split into 2
                var newInterval1 = new Interval<TKey>(existingLower, interval.Lower.Touching!);
                var newInterval2 = new Interval<TKey>(interval.Upper.Touching!, existingUpper);
                this.values[from] = this.intervalMover(existing, newInterval1);
                this.values.Insert(from + 1, this.intervalMover(existing, newInterval2));
            }
        }
        else
        {
            // Intersects multiple intervals
            // Let's look at the edge relations
            var lowerExisting = this.values[from];
            var upperExisting = this.values[to - 1];
            var lowerExistingLower = this.intervalSelector(lowerExisting).Lower;
            var upperExistingUpper = this.intervalSelector(upperExisting).Upper;
            var lowerCompare = this.BoundComparer.Compare(lowerExistingLower, interval.Lower);
            var upperCompare = this.BoundComparer.Compare(upperExistingUpper, interval.Upper);
            // Split edges if needed, track indices for deletion
            var deleteFrom = from;
            var deleteTo = to;
            if (lowerCompare < 0)
            {
                // Need to split lower
                var newLower = new Interval<TKey>(lowerExistingLower, interval.Lower.Touching!);
                this.values[from] = this.intervalMover(lowerExisting, newLower);
                ++deleteFrom;
            }
            if (upperCompare > 0)
            {
                // Need to split upper
                var newUpper = new Interval<TKey>(interval.Upper.Touching!, upperExistingUpper);
                this.values[to - 1] = this.intervalMover(upperExisting, newUpper);
                --deleteTo;
            }
            // Remove all fully removed intervals
            this.values.RemoveRange(deleteFrom, deleteTo - deleteFrom);
        }
        return true;
    }

    /// <summary>
    /// Retrieves the range that is contained by a given interval.
    /// </summary>
    /// <param name="interval">The interval to check containment with.</param>
    /// <returns>The index range to index <see cref="values"/> with.</returns>
    public (int From, int To) ContainedRange(Interval<TKey> interval)
    {
        var (from, to) = this.IntersectingRange(interval);
        if (from == to)
        {
            // No intersection
            return (from, to);
        }
        else if (to - from == 1)
        {
            // Intersects one
            var existing = this.intervalSelector(this.values[from]);
            var loCmp = this.BoundComparer.Compare(existing.Lower, interval.Lower);
            var hiCmp = this.BoundComparer.Compare(existing.Upper, interval.Upper);
            return (loCmp >= 0 && hiCmp <= 0) ? (from, to) : (from, from);
        }
        else
        {
            // Intersects multiple
            var fromLower = this.intervalSelector(this.values[from]).Lower;
            var toUpper = this.intervalSelector(this.values[to - 1]).Upper;
            var loCmp = this.BoundComparer.Compare(fromLower, interval.Lower);
            var hiCmp = this.BoundComparer.Compare(toUpper, interval.Upper);
            return (loCmp >= 0 ? from : from + 1, hiCmp <= 0 ? to : to - 1);
        }
    }

    /// <summary>
    /// Retrieves the range that is intersecting or at least touching a given interval.
    /// </summary>
    /// <param name="interval">The interval to check touch with.</param>
    /// <returns>The index range to index <see cref="values"/> with.</returns>
    public (int From, int To) TouchingRange(Interval<TKey> interval)
    {
        var (from, to) = this.IntersectingRange(interval);
        if (from != 0
         && this.BoundComparer.IsTouching(interval.Lower, this.intervalSelector(this.values[from - 1]).Upper)) from -= 1;
        if (to != this.values.Count
         && this.BoundComparer.IsTouching(interval.Upper, this.intervalSelector(this.values[to]).Lower)) to += 1;
        return (from, to);
    }

    /// <summary>
    /// Retrieves the range that is intersecting a given interval.
    /// </summary>
    /// <param name="interval">The interval to check intersection with.</param>
    /// <returns>The index range to index <see cref="values"/> with.</returns>
    public (int From, int To) IntersectingRange(Interval<TKey> interval)
    {
        var from = this.BinarySearch(0, interval.Lower, iv => iv.Upper);
        var to = this.BinarySearch(from, interval.Upper, iv => iv.Lower);
        return (from, to);
    }

    /* General list methods */

    /// <inheritdoc/>
    public int IndexOf(TValue item) => this.values.IndexOf(item);

    /// <inheritdoc/>
    public void Insert(int index, TValue item) => this.values.Insert(index, item);

    /// <inheritdoc/>
    public void RemoveAt(int index) => this.values.RemoveAt(index);

    /// <inheritdoc/>
    public void Add(TValue item) => this.values.Add(item);

    /// <summary>
    /// Adds the elements of the specified collection to the end of the underlying collection.
    /// </summary>
    /// <param name="collection">The collection whose elements should be added to the end of the underlying collection.</param>
    public void AddRange(IEnumerable<TValue> collection) => this.values.AddRange(collection);

    /// <inheritdoc/>
    public void Clear() => this.values.Clear();

    /// <inheritdoc/>
    public bool Contains(TValue item) => this.values.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(TValue[] array, int arrayIndex) => this.values.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public bool Remove(TValue item) => this.values.Remove(item);

    /// <summary>
    /// Removes a range of elements from the underlying container.
    /// </summary>
    /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
    /// <param name="count">The number of elements to remove.</param>
    public void RemoveRange(int index, int count) => this.values.RemoveRange(index, count);

    /// <inheritdoc/>
    public IEnumerator<TValue> GetEnumerator() => this.values.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => (this.values as IEnumerable).GetEnumerator();

    private int BinarySearch(int start, Bound<TKey> searchedKey, Func<Interval<TKey>, Bound<TKey>> keySelector)
    {
        var size = this.values.Count - start;
        if (size == 0) return start;

        while (size > 1)
        {
            var half = size / 2;
            var mid = start + half;
            var key = keySelector(this.intervalSelector(this.values[mid]));
            var cmp = this.BoundComparer.Compare(searchedKey, key);
            start = cmp > 0 ? mid : start;
            size -= half;
        }

        var resultKey = keySelector(this.intervalSelector(this.values[start]));
        var resultCmp = this.BoundComparer.Compare(searchedKey, resultKey);
        return start + (resultCmp > 0 ? 1 : 0);
    }
}
