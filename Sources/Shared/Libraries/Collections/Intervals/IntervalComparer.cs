// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

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
    public static IntervalComparer<T> Default { get; } = new(EndpointComparer<T>.Default);

    /// <summary>
    /// The endpoint comparer used.
    /// </summary>
    public EndpointComparer<T> EndpointComparer { get; }

    /// <summary>
    /// The element comparer used.
    /// </summary>
    public IComparer<T> ValueComparer => this.EndpointComparer.ValueComparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntervalComparer{T}"/> class.
    /// </summary>
    /// <param name="endpointComparer">The <see cref="EndpointComparer{T}"/> to use.</param>
    public IntervalComparer(EndpointComparer<T> endpointComparer)
    {
        this.EndpointComparer = endpointComparer;
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
        (this.EndpointComparer.Equals(x.Lower, y.Lower) && this.EndpointComparer.Equals(x.Upper, y.Upper))
     || (this.IsEmpty(x) && this.IsEmpty(y));

    /// <inheritdoc/>
    public int GetHashCode(Interval<T> obj)
    {
        // NOTE: All empty intervals are equal
        if (this.IsEmpty(obj)) return 0;

        var h = default(HashCode);
        h.Add(obj.Lower, this.EndpointComparer);
        h.Add(obj.Upper, this.EndpointComparer);
        return h.ToHashCode();
    }

    /// <summary>
    /// Checks if a value is inside an interval.
    /// </summary>
    /// <param name="interval">The interval to check.</param>
    /// <param name="value">The value to check.</param>
    /// <returns>True, if <paramref name="interval"/> contains <paramref name="value"/>.</returns>
    public bool Contains(Interval<T> interval, T value) =>
        this.Contains(interval, Interval.Singleton(value));

    /// <summary>
    /// Checks if an interval completely contains another one.
    /// </summary>
    /// <param name="x">The container interval.</param>
    /// <param name="y">The contained interval.</param>
    /// <returns>True, if <paramref name="x"/> contains all elements of <paramref name="y"/>.</returns>
    public bool Contains(Interval<T> x, Interval<T> y) =>
        this.IsEmpty(y)
     || (this.EndpointComparer.Compare(x.Lower, y.Lower) <= 0 && this.EndpointComparer.Compare(x.Upper, y.Upper) >= 0);

    /// <summary>
    /// Checks if an interval is empty.
    /// </summary>
    /// <param name="interval">The interval to check.</param>
    /// <returns>True, if <paramref name="interval"/> is empty.</returns>
    public bool IsEmpty(Interval<T> interval) => (interval.Lower.Type, interval.Upper.Type) switch
    {
        (EndpointType.Inclusive, EndpointType.Exclusive)
     or (EndpointType.Exclusive, EndpointType.Inclusive)
     or (EndpointType.Exclusive, EndpointType.Exclusive) =>
            this.ValueComparer.Compare(interval.Lower.Value!, interval.Upper.Value!) >= 0,
        (EndpointType.Inclusive, EndpointType.Inclusive) =>
            this.ValueComparer.Compare(interval.Lower.Value!, interval.Upper.Value!) > 0,
        _ => false,
    };

    /// <summary>
    /// Checks if an interval is completely before another one, without overlapping.
    /// </summary>
    /// <param name="x">The first interval to check.</param>
    /// <param name="y">The second interval to check.</param>
    /// <returns>True, if <paramref name="x"/> is completely before <paramref name="y"/>.</returns>
    public bool IsBefore(Interval<T> x, Interval<T> y) =>
           this.EndpointComparer.Compare(x.Upper, y.Lower) < 0
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
        && (this.EndpointComparer.IsTouching(x.Upper, y.Lower) || this.EndpointComparer.IsTouching(x.Lower, y.Upper));

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
        if (this.IsDisjunct(x, y)) return Interval.Empty<T>();
        var loCmp = this.EndpointComparer.Compare(x.Lower, y.Lower);
        var hiCmp = this.EndpointComparer.Compare(x.Upper, y.Upper);
        return new(loCmp < 0 ? y.Lower : x.Lower, hiCmp < 0 ? x.Upper : y.Upper);
    }
}
