// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// Represents an interval with two endpoints.
/// </summary>
/// <typeparam name="T">The endpoint value type.</typeparam>
/// <param name="Lower">The lower endpoint.</param>
/// <param name="Upper">The upper endpoint.</param>
public readonly record struct Interval<T>(LowerEndpoint<T> Lower, UpperEndpoint<T> Upper)
{
    /// <summary>
    /// Constructs an empty interval.
    /// </summary>
    /// <returns>The constructed interval.</returns>
    public static Interval<T> Empty() => new(LowerEndpoint.Exclusive<T>(default!), UpperEndpoint.Exclusive<T>(default!));

    /// <summary>
    /// Constructs an interval containing every possible value.
    /// </summary>
    /// <returns>The constructed interval.</returns>
    public static Interval<T> Full() => new(LowerEndpoint.Unbounded<T>(), UpperEndpoint.Unbounded<T>());

    /// <summary>
    /// Constructs an interval containing the single specified value.
    /// </summary>
    /// <param name="value">The single value to be contained.</param>
    /// <returns>The constructed interval.</returns>
    public static Interval<T> Singleton(T value) => new(LowerEndpoint.Inclusive<T>(value), UpperEndpoint.Inclusive<T>(value));

    /// <summary>
    /// True, if this interval is empty.
    /// </summary>
    public bool IsEmpty => IntervalComparer<T>.Default.IsEmpty(this);

    /// <inheritdoc/>
    public override string ToString() => $"{this.Lower}; {this.Upper}";

    /// <inheritdoc/>
    public bool Equals(Interval<T> other) => IntervalComparer<T>.Default.Equals(this, other);

    /// <inheritdoc/>
    public override int GetHashCode() => IntervalComparer<T>.Default.GetHashCode(this);

    /// <summary>
    /// Checks if a value is inside an interval.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <returns>True, if this interval contains <paramref name="value"/>.</returns>
    public bool Contains(T value) => IntervalComparer<T>.Default.Contains(this, value);

    /// <summary>
    /// Checks if this interval is completely before another one, without overlapping.
    /// </summary>
    /// <param name="other">The interval to check.</param>
    /// <returns>True, if this is completely before <paramref name="other"/>.</returns>
    public bool IsBefore(Interval<T> other) => IntervalComparer<T>.Default.IsBefore(this, other);

    /// <summary>
    /// Checks if an interval is disjunct with this one.
    /// </summary>
    /// <param name="other">The other interval to check.</param>
    /// <returns>True, if this and <paramref name="other"/> are completely disjunct.</returns>
    public bool IsDisjunct(Interval<T> other) => IntervalComparer<T>.Default.IsDisjunct(this, other);

    /// <summary>
    /// Checks if an interval is touching another one.
    /// </summary>
    /// <param name="other">The other interval to check.</param>
    /// <returns>True, if this and <paramref name="other"/> are touching on an endpoint.</returns>
    public bool IsTouching(Interval<T> other) => IntervalComparer<T>.Default.IsTouching(this, other);

    /// <summary>
    /// Returns the intersection of this interval with another.
    /// </summary>
    /// <param name="other">The other interval to get the intersection with.</param>
    /// <returns>The intersection of this and <paramref name="other"/>.</returns>
    public Interval<T> Intersection(Interval<T> other) => IntervalComparer<T>.Default.Intersection(this, other);
}

/// <summary>
/// Factory methods for <see cref="Interval{T}"/>.
/// </summary>
public static class Interval
{
    /// <summary>
    /// Constructs an interval from the given endpoints.
    /// </summary>
    /// <param name="lower">The lower endpoint.</param>
    /// <param name="upper">The upper endpoint.</param>
    /// <returns>The constructed interval.</returns>
    public static Interval<T> Of<T>(LowerEndpoint<T> lower, UpperEndpoint<T> upper) => new(lower, upper);

    /// <summary>
    /// Constructs an empty interval.
    /// </summary>
    /// <returns>The constructed interval.</returns>
    public static Interval<T> Empty<T>() => Interval<T>.Empty();

    /// <summary>
    /// Constructs an interval containing every possible value.
    /// </summary>
    /// <returns>The constructed interval.</returns>
    public static Interval<T> Full<T>() => Interval<T>.Full();

    /// <summary>
    /// Constructs an interval containing the single specified value.
    /// </summary>
    /// <param name="value">The single value to be contained.</param>
    /// <returns>The constructed interval.</returns>
    public static Interval<T> Singleton<T>(T value) => Interval<T>.Singleton(value);
}
