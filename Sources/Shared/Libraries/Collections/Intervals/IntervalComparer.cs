// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// An equality comparer that can compare <see cref="Interval{T}"/>s.
/// </summary>
/// <typeparam name="T">The type of the endpoint value.</typeparam>
public sealed class IntervalComparer<T> : IEqualityComparer<Interval<T>>
{
    /// <summary>
    /// The default instance of the comparer.
    /// </summary>
    public static IntervalComparer<T> Default { get; } = new(BoundComparer<T>.Default);

    /// <summary>
    /// The bounds comparer used.
    /// </summary>
    public BoundComparer<T> BoundComparer { get; }

    /// <summary>
    /// The element comparer used.
    /// </summary>
    public IComparer<T> ValueComparer => this.BoundComparer.ValueComparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntervalComparer{T}"/> class.
    /// </summary>
    /// <param name="boundComparer">The <see cref="BoundComparer{T}"/> to use.</param>
    public IntervalComparer(BoundComparer<T> boundComparer)
    {
        this.BoundComparer = boundComparer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntervalComparer{T}"/> class.
    /// </summary>
    /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
    /// <param name="comparer">The <see cref="IComparer{T}"/> to use.</param>
    public IntervalComparer(IEqualityComparer<T> equalityComparer, IComparer<T> comparer)
        : this(new(equalityComparer, comparer))
    {
    }

    /// <inheritdoc/>
    public bool Equals(Interval<T> x, Interval<T> y) =>
           (this.BoundComparer.Equals(x.Lower, y.Lower) && this.BoundComparer.Equals(x.Upper, y.Upper))
        || (this.IsEmpty(x) && this.IsEmpty(y));

    /// <inheritdoc/>
    public int GetHashCode(Interval<T> obj)
    {
        // NOTE: All empty intervals are equal
        if (this.IsEmpty(obj)) return 0;

        var h = default(HashCode);
        h.Add(obj.Lower, this.BoundComparer);
        h.Add(obj.Upper, this.BoundComparer);
        return h.ToHashCode();
    }

    /// <summary>
    /// Checks if a value is inside an interval.
    /// </summary>
    /// <param name="interval">The interval to check.</param>
    /// <param name="value">The value to check.</param>
    /// <returns>True, if <paramref name="interval"/> contains <paramref name="value"/>.</returns>
    public bool Contains(Interval<T> interval, T value) => (interval.Lower, interval.Upper) switch
    {
        (LowerBound<T>.Unbounded, UpperBound<T>.Unbounded) => true,

        (LowerBound<T>.Unbounded, UpperBound<T>.Exclusive r) => this.ValueComparer.Compare(value, r.Value) < 0,
        (LowerBound<T>.Unbounded, UpperBound<T>.Inclusive r) => this.ValueComparer.Compare(value, r.Value) <= 0,

        (LowerBound<T>.Exclusive l, UpperBound<T>.Unbounded) => this.ValueComparer.Compare(l.Value, value) < 0,
        (LowerBound<T>.Inclusive l, UpperBound<T>.Unbounded) => this.ValueComparer.Compare(l.Value, value) <= 0,

        (LowerBound<T>.Exclusive l, UpperBound<T>.Exclusive r) => this.ValueComparer.Compare(l.Value, value) < 0 && this.ValueComparer.Compare(value, r.Value) < 0,
        (LowerBound<T>.Inclusive l, UpperBound<T>.Inclusive r) => this.ValueComparer.Compare(l.Value, value) <= 0 && this.ValueComparer.Compare(value, r.Value) <= 0,

        (LowerBound<T>.Inclusive l, UpperBound<T>.Exclusive r) => this.ValueComparer.Compare(l.Value, value) <= 0 && this.ValueComparer.Compare(value, r.Value) < 0,
        (LowerBound<T>.Exclusive l, UpperBound<T>.Inclusive r) => this.ValueComparer.Compare(l.Value, value) < 0 && this.ValueComparer.Compare(value, r.Value) <= 0,

        _ => throw new ArgumentOutOfRangeException(nameof(interval), "The interval bounds are not of the allowed Inclusive/Exclusive/Unbounded"),
    };

    /// <summary>
    /// Checks if an interval completely contains another one.
    /// </summary>
    /// <param name="x">The container interval.</param>
    /// <param name="y">The contained interval.</param>
    /// <returns>True, if <paramref name="x"/> contains all elements of <paramref name="y"/>.</returns>
    public bool Contains(Interval<T> x, Interval<T> y) =>
           this.IsEmpty(y)
        || (this.BoundComparer.Compare(x.Lower, y.Lower) <= 0 && this.BoundComparer.Compare(x.Upper, y.Upper) >= 0);

    /// <summary>
    /// Checks if an interval is empty.
    /// </summary>
    /// <param name="interval">The interval to check.</param>
    /// <returns>True, if <paramref name="interval"/> is empty.</returns>
    public bool IsEmpty(Interval<T> interval) => (interval.Lower, interval.Upper) switch
    {
        (LowerBound<T>.Inclusive l, UpperBound<T>.Exclusive r) => this.ValueComparer.Compare(l.Value, r.Value) >= 0,
        (LowerBound<T>.Exclusive l, UpperBound<T>.Inclusive r) => this.ValueComparer.Compare(l.Value, r.Value) >= 0,
        (LowerBound<T>.Exclusive l, UpperBound<T>.Exclusive r) => this.ValueComparer.Compare(l.Value, r.Value) >= 0,
        (LowerBound<T>.Inclusive l, UpperBound<T>.Inclusive r) => this.ValueComparer.Compare(l.Value, r.Value) > 0,
        _ => false,
    };

    /// <summary>
    /// Checks if an interval is completely before another one, without overlapping.
    /// </summary>
    /// <param name="x">The first interval to check.</param>
    /// <param name="y">The second interval to check.</param>
    /// <returns>True, if <paramref name="x"/> is completely before <paramref name="y"/>.</returns>
    public bool IsBefore(Interval<T> x, Interval<T> y) =>
           this.BoundComparer.Compare(x.Upper, y.Lower) < 0
        && !(this.IsEmpty(x) && this.IsEmpty(y));

    /// <summary>
    /// Checks if an interval is disjunct with another one.
    /// </summary>
    /// <param name="x">The first interval to check.</param>
    /// <param name="y">The second interval to check.</param>
    /// <returns>True, if <paramref name="x"/> and <paramref name="y"/> are completely disjunct.</returns>
    public bool IsDisjunct(Interval<T> x, Interval<T> y) =>
        this.IsBefore(x, y) || this.IsBefore(y, x) || (this.IsEmpty(x) && this.IsEmpty(y));

    /// <summary>
    /// Checks if an interval is touching another.
    /// </summary>
    /// <param name="x">The first interval to check.</param>
    /// <param name="y">The second interval to check.</param>
    /// <returns>True, if <paramref name="x"/> and <paramref name="y"/> are touching on one of their endpoints.</returns>
    public bool IsTouching(Interval<T> x, Interval<T> y) =>
           !this.IsEmpty(x) && !this.IsEmpty(y)
        && (this.BoundComparer.IsTouching(x.Upper, y.Lower) || this.BoundComparer.IsTouching(x.Lower, y.Upper));

    /// <summary>
    /// Checks, if a sequence of intervals are all touching in the specified order.
    /// </summary>
    /// <param name="intervals">The sequence of intervals to check.</param>
    /// <returns>True, if all of <paramref name="intervals"/> are touching in the specified.</returns>
    public bool AreTouching(IEnumerable<Interval<T>> intervals)
    {
        Interval<T>? prev = null;
        foreach (var iv in intervals)
        {
            if (prev is not null && !this.IsTouching(prev.Value, iv)) return false;
            prev = iv;
        }
        return true;
    }

    /// <summary>
    /// Returns the intersection of two intervals.
    /// </summary>
    /// <param name="x">The first interval.</param>
    /// <param name="y">The second interval.</param>
    /// <returns>The intersection of <paramref name="x"/> and <paramref name="y"/>.</returns>
    public Interval<T> Intersection(Interval<T> x, Interval<T> y)
    {
        if (this.IsDisjunct(x, y)) return Interval<T>.Empty;
        var loCmp = this.BoundComparer.Compare(x.Lower, y.Lower);
        var hiCmp = this.BoundComparer.Compare(x.Upper, y.Upper);
        return new(loCmp < 0 ? y.Lower : x.Lower, hiCmp < 0 ? x.Upper : y.Upper);
    }

    /// <summary>
    /// Calculates the relation of two intervals.
    /// </summary>
    /// <param name="x">The first interval.</param>
    /// <param name="y">The second interval.</param>
    /// <returns>An <see cref="IntervalRelation{T}"/> of <paramref name="x"/> and <paramref name="y"/>.</returns>
    public IntervalRelation<T> Relation(Interval<T> x, Interval<T> y)
    {
        if (this.Equals(x, y)) return new IntervalRelation<T>.Equal(x);

        var (first, second) = this.BoundComparer.Compare(x.Lower, y.Lower) < 0 ? (x, y) : (y, x);
        if (this.BoundComparer.IsTouching(first.Upper, second.Lower)) return new IntervalRelation<T>.Touching(first, second);
        if (this.BoundComparer.Compare(first.Upper, second.Lower) < 0) return new IntervalRelation<T>.Disjunct(first, second);
        var upperCmp = this.BoundComparer.Compare(first.Upper, second.Upper);
        if (this.BoundComparer.Compare(first.Lower, second.Lower) == 0)
        {
            // Starting relation, depends on which ends first
            var (a, b) = upperCmp < 0 ? (first, second) : (second, first);
            return new IntervalRelation<T>.Starting(a, new Interval<T>(a.Upper.Touching!, b.Upper));
        }
        if (upperCmp == 0)
        {
            return new IntervalRelation<T>.Finishing(new Interval<T>(first.Lower, second.Lower.Touching!), second);
        }
        if (upperCmp > 0)
        {
            return new IntervalRelation<T>.Containing(
                new Interval<T>(first.Lower, second.Lower.Touching!),
                second,
                new Interval<T>(second.Upper.Touching!, first.Upper));
        }
        return new IntervalRelation<T>.Overlapping(
            new Interval<T>(first.Lower, second.Lower.Touching!),
            new Interval<T>(second.Lower, first.Upper),
            new Interval<T>(first.Upper.Touching!, second.Upper));
    }
}
