// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals.Internal;

internal sealed class SortedIntervalList<TKey, TValue> : IEnumerable<KeyValuePair<Interval<TKey>, TValue>>
{
    public int Count => this.values.Count;

    public KeyValuePair<Interval<TKey>, TValue> this[int index]
    {
        get => this.values[index];
        set => this.values[index] = value;
    }

    public IntervalComparer<TKey> Comparer { get; }

    public EndpointComparer<TKey> EndpointComparer => this.Comparer.EndpointComparer;

    private readonly List<KeyValuePair<Interval<TKey>, TValue>> values = new();

    public SortedIntervalList(IntervalComparer<TKey> comparer)
    {
        this.Comparer = comparer;
    }

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
            var existingLower = existing.Key.Lower;
            var existingUpper = existing.Key.Upper;
            var lowerCompare = this.EndpointComparer.Compare(existingLower, interval.Lower);
            var upperCompare = this.EndpointComparer.Compare(existingUpper, interval.Upper);
            if (lowerCompare >= 0 && upperCompare <= 0)
            {
                // Simplest case, we just remove the entry, as the interval completely covers this one
                this.values.RemoveAt(from);
            }
            else if (lowerCompare >= 0)
            {
                // The upper bound does not match, we need to modify
                var newInterval = Interval.Of(interval.Upper.Touching!.Value, existingUpper);
                this.values[from] = new(newInterval, existing.Value);
            }
            else if (upperCompare <= 0)
            {
                // The lower bound does not match, we need to modify
                var newInterval = Interval.Of(existingLower, interval.Lower.Touching!.Value);
                this.values[from] = new(newInterval, existing.Value);
            }
            else
            {
                // The interval is being split into 2
                var newInterval1 = Interval.Of(existingLower, interval.Lower.Touching!.Value);
                var newInterval2 = Interval.Of(interval.Upper.Touching!.Value, existingUpper);
                this.values[from] = new(newInterval1, existing.Value);
                this.values.Insert(from + 1, new(newInterval2, existing.Value));
            }
        }
        else
        {
            // Intersects multiple intervals
            // Let's look at the edge relations
            var lowerExisting = this.values[from];
            var upperExisting = this.values[to - 1];
            var lowerExistingLower = lowerExisting.Key.Lower;
            var upperExistingUpper = upperExisting.Key.Upper;
            var lowerCompare = this.EndpointComparer.Compare(lowerExistingLower, interval.Lower);
            var upperCompare = this.EndpointComparer.Compare(upperExistingUpper, interval.Upper);
            // Split edges if needed, track indices for deletion
            var deleteFrom = from;
            var deleteTo = to;
            if (lowerCompare < 0)
            {
                // Need to split lower
                var newLower = Interval.Of(lowerExistingLower, interval.Lower.Touching!.Value);
                this.values[from] = new(newLower, lowerExisting.Value);
                ++deleteFrom;
            }
            if (upperCompare > 0)
            {
                // Need to split upper
                var newUpper = Interval.Of(interval.Upper.Touching!.Value, upperExistingUpper);
                this.values[to - 1] = new(newUpper, upperExisting.Value);
                --deleteTo;
            }
            // Remove all fully removed intervals
            this.values.RemoveRange(deleteFrom, deleteTo - deleteFrom);
        }
        return true;
    }

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
            var existing = this.values[from].Key;
            var loCmp = this.EndpointComparer.Compare(existing.Lower, interval.Lower);
            var hiCmp = this.EndpointComparer.Compare(existing.Upper, interval.Upper);
            return (loCmp >= 0 && hiCmp <= 0) ? (from, to) : (from, from);
        }
        else
        {
            // Intersects multiple
            var fromLower = this.values[from].Key.Lower;
            var toUpper = this.values[to - 1].Key.Upper;
            var loCmp = this.EndpointComparer.Compare(fromLower, interval.Lower);
            var hiCmp = this.EndpointComparer.Compare(toUpper, interval.Upper);
            return (loCmp >= 0 ? from : from + 1, hiCmp <= 0 ? to : to - 1);
        }
    }

    public (int From, int To) TouchingRange(Interval<TKey> interval)
    {
        var (from, to) = this.IntersectingRange(interval);
        if (from != 0
         && this.EndpointComparer.IsTouching(interval.Lower, this.values[from - 1].Key.Upper)) from -= 1;
        if (to != this.values.Count
         && this.EndpointComparer.IsTouching(interval.Upper, this.values[to].Key.Lower)) to += 1;
        return (from, to);
    }

    public (int From, int To) IntersectingRange(Interval<TKey> interval)
    {
        var from = this.BinarySearch(0, interval.Lower);
        var to = this.BinarySearch(from, interval.Upper);
        return (from, to);
    }

    public void Insert(int index, KeyValuePair<Interval<TKey>, TValue> item) => this.values.Insert(index, item);

    public void RemoveAt(int index) => this.values.RemoveAt(index);

    public void Add(KeyValuePair<Interval<TKey>, TValue> item) => this.values.Add(item);

    public void AddRange(IEnumerable<KeyValuePair<Interval<TKey>, TValue>> collection) => this.values.AddRange(collection);

    public void Clear() => this.values.Clear();

    public void RemoveRange(int index, int count) => this.values.RemoveRange(index, count);

    public IEnumerator<KeyValuePair<Interval<TKey>, TValue>> GetEnumerator() => this.values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    private int BinarySearch(int start, LowerEndpoint<TKey> searchedKey)
    {
        var size = this.values.Count - start;
        if (size == 0) return start;

        while (size > 1)
        {
            var half = size / 2;
            var mid = start + half;
            var key = this.values[mid].Key.Upper;
            var cmp = this.EndpointComparer.Compare(searchedKey, key);
            start = cmp > 0 ? mid : start;
            size -= half;
        }

        var resultKey = this.values[start].Key.Upper;
        var resultCmp = this.EndpointComparer.Compare(searchedKey, resultKey);
        return start + (resultCmp > 0 ? 1 : 0);
    }

    private int BinarySearch(int start, UpperEndpoint<TKey> searchedKey)
    {
        var size = this.values.Count - start;
        if (size == 0) return start;

        while (size > 1)
        {
            var half = size / 2;
            var mid = start + half;
            var key = this.values[mid].Key.Lower;
            var cmp = this.EndpointComparer.Compare(searchedKey, key);
            start = cmp > 0 ? mid : start;
            size -= half;
        }

        var resultKey = this.values[start].Key.Lower;
        var resultCmp = this.EndpointComparer.Compare(searchedKey, resultKey);
        return start + (resultCmp > 0 ? 1 : 0);
    }
}
