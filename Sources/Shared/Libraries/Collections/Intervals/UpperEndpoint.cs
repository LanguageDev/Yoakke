// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// Represents the upper-endpoint of an interval.
/// </summary>
/// <typeparam name="T">The endpoint type.</typeparam>
public readonly record struct UpperEndpoint<T>(EndpointType Type, T? Value)
    : IEquatable<UpperEndpoint<T>>,
      IComparable, IComparable<UpperEndpoint<T>>, IComparable<LowerEndpoint<T>>
{
    /// <summary>
    /// Constructs an unbounded upper endpoint.
    /// </summary>
    /// <returns>The constructed upper endpoint.</returns>
    public static UpperEndpoint<T> Unbounded() => new(EndpointType.Unbounded, default);

    /// <summary>
    /// Constructs an inclusive upper endpoint.
    /// </summary>
    /// <param name="value">The endpoint value.</param>
    /// <returns>The constructed upper endpoint.</returns>
    public static UpperEndpoint<T> Inclusive(T value) => new(EndpointType.Inclusive, value);

    /// <summary>
    /// Constructs an exclusive upper endpoint.
    /// </summary>
    /// <param name="value">The endpoint value.</param>
    /// <returns>The constructed upper endpoint.</returns>
    public static UpperEndpoint<T> Exclusive(T value) => new(EndpointType.Exclusive, value);

    /// <summary>
    /// The touching lower endpoint of this, if any.
    /// </summary>
    public LowerEndpoint<T>? Touching => this.Type switch
    {
        EndpointType.Unbounded => null,
        EndpointType.Inclusive => LowerEndpoint.Exclusive(this.Value!),
        EndpointType.Exclusive => LowerEndpoint.Inclusive(this.Value!),
        _ => throw new InvalidOperationException(),
    };

    /// <summary>
    /// Checks, if this endpoint is in touching relation with another one.
    /// </summary>
    /// <param name="other">The <see cref="LowerEndpoint{T}"/> to compare with.</param>
    /// <returns>True, if this is touching <paramref name="other"/>.</returns>
    public bool IsTouching(LowerEndpoint<T> other) => EndpointComparer<T>.Default.IsTouching(this, other);

    /// <inheritdoc/>
    public override string ToString() => this.Type switch
    {
        EndpointType.Unbounded => "+∞)",
        EndpointType.Inclusive => $"{this.Value}]",
        EndpointType.Exclusive => $"{this.Value})",
        _ => throw new InvalidOperationException(),
    };

    /// <inheritdoc/>
    public override int GetHashCode() => EndpointComparer<T>.Default.GetHashCode(this);

    /// <inheritdoc/>
    public bool Equals(UpperEndpoint<T> other) => EndpointComparer<T>.Default.Equals(this, other);

    /// <inheritdoc/>
    public int CompareTo(object obj) => obj switch
    {
        LowerEndpoint<T> l => this.CompareTo(l),
        UpperEndpoint<T> u => this.CompareTo(u),
        _ => throw new InvalidOperationException(),
    };

    /// <inheritdoc/>
    public int CompareTo(UpperEndpoint<T> other) => EndpointComparer<T>.Default.Compare(this, other);

    /// <inheritdoc/>
    public int CompareTo(LowerEndpoint<T> other) => EndpointComparer<T>.Default.Compare(this, other);

    /// <summary>
    /// Less-than compares endpoints.
    /// </summary>
    /// <param name="left">The first endpoint to compare.</param>
    /// <param name="right">The second endpoint to compare.</param>
    /// <returns>True, if <paramref name="left"/> comes strictly before <paramref name="right"/>.</returns>
    public static bool operator <(UpperEndpoint<T> left, UpperEndpoint<T> right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Less-or-equal compares endpoints.
    /// </summary>
    /// <param name="left">The first endpoint to compare.</param>
    /// <param name="right">The second endpoint to compare.</param>
    /// <returns>True, if <paramref name="left"/> comes before <paramref name="right"/>.</returns>
    public static bool operator <=(UpperEndpoint<T> left, UpperEndpoint<T> right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Greater-than compares endpoints.
    /// </summary>
    /// <param name="left">The first endpoint to compare.</param>
    /// <param name="right">The second endpoint to compare.</param>
    /// <returns>True, if <paramref name="left"/> comes strictly after <paramref name="right"/>.</returns>
    public static bool operator >(UpperEndpoint<T> left, UpperEndpoint<T> right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Greater-or-equal compares endpoints.
    /// </summary>
    /// <param name="left">The first endpoint to compare.</param>
    /// <param name="right">The second endpoint to compare.</param>
    /// <returns>True, if <paramref name="left"/> comes after <paramref name="right"/>.</returns>
    public static bool operator >=(UpperEndpoint<T> left, UpperEndpoint<T> right) => left.CompareTo(right) >= 0;

    /// <summary>
    /// Less-than compares endpoints.
    /// </summary>
    /// <param name="left">The first endpoint to compare.</param>
    /// <param name="right">The second endpoint to compare.</param>
    /// <returns>True, if <paramref name="left"/> comes strictly before <paramref name="right"/>.</returns>
    public static bool operator <(UpperEndpoint<T> left, LowerEndpoint<T> right) => left.CompareTo(right) < 0;

    /// <summary>
    /// Less-or-equal compares endpoints.
    /// </summary>
    /// <param name="left">The first endpoint to compare.</param>
    /// <param name="right">The second endpoint to compare.</param>
    /// <returns>True, if <paramref name="left"/> comes before <paramref name="right"/>.</returns>
    public static bool operator <=(UpperEndpoint<T> left, LowerEndpoint<T> right) => left.CompareTo(right) <= 0;

    /// <summary>
    /// Greater-than compares endpoints.
    /// </summary>
    /// <param name="left">The first endpoint to compare.</param>
    /// <param name="right">The second endpoint to compare.</param>
    /// <returns>True, if <paramref name="left"/> comes strictly after <paramref name="right"/>.</returns>
    public static bool operator >(UpperEndpoint<T> left, LowerEndpoint<T> right) => left.CompareTo(right) > 0;

    /// <summary>
    /// Greater-or-equal compares endpoints.
    /// </summary>
    /// <param name="left">The first endpoint to compare.</param>
    /// <param name="right">The second endpoint to compare.</param>
    /// <returns>True, if <paramref name="left"/> comes after <paramref name="right"/>.</returns>
    public static bool operator >=(UpperEndpoint<T> left, LowerEndpoint<T> right) => left.CompareTo(right) >= 0;
}

/// <summary>
/// Factory methods for <see cref="UpperEndpoint{T}"/>.
/// </summary>
public static class UpperEndpoint
{
    /// <summary>
    /// Constructs an unbounded upper endpoint.
    /// </summary>
    /// <returns>The constructed upper endpoint.</returns>
    public static UpperEndpoint<T> Unbounded<T>() => UpperEndpoint<T>.Unbounded();

    /// <summary>
    /// Constructs an inclusive upper endpoint.
    /// </summary>
    /// <param name="value">The endpoint value.</param>
    /// <returns>The constructed upper endpoint.</returns>
    public static UpperEndpoint<T> Inclusive<T>(T value) => UpperEndpoint<T>.Inclusive(value);

    /// <summary>
    /// Constructs an exclusive upper endpoint.
    /// </summary>
    /// <param name="value">The endpoint value.</param>
    /// <returns>The constructed upper endpoint.</returns>
    public static UpperEndpoint<T> Exclusive<T>(T value) => UpperEndpoint<T>.Exclusive(value);
}
