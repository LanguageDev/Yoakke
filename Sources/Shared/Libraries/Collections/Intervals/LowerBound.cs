// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// Represents the lower-bound of an interval.
/// </summary>
/// <typeparam name="T">The type of the endpoint value.</typeparam>
public abstract record LowerBound<T> : Bound<T>, IComparable<LowerBound<T>>, IComparable<UpperBound<T>>
{
    /// <inheritdoc/>
    public int CompareTo(LowerBound<T> other) => BoundComparer<T>.Default.Compare(this, other);

    /// <inheritdoc/>
    public int CompareTo(UpperBound<T> other) => BoundComparer<T>.Default.Compare(this, other);

    /// <inheritdoc/>
    public override int GetHashCode() => BoundComparer<T>.Default.GetHashCode(this);

    /// <summary>
    /// Checks, if this bound is in touching relation with an upper bound.
    /// </summary>
    /// <param name="other">The <see cref="UpperBound{T}"/> to compare with.</param>
    /// <returns>True, if this is touching <paramref name="other"/>.</returns>
    public bool IsTouching(UpperBound<T> other) => BoundComparer<T>.Default.IsTouching(this, other);

    /// <summary>
    /// The touching bound of this one.
    /// </summary>
    public abstract UpperBound<T>? Touching { get; }

    /// <summary>
    /// Unbounded endpoint.
    /// </summary>
    public sealed record Unbounded : LowerBound<T>
    {
        private Unbounded()
        {
        }

        /// <summary>
        /// A singleton instance to use.
        /// </summary>
        public static Unbounded Instance { get; } = new();

        /// <inheritdoc/>
        public override UpperBound<T>? Touching => null;

        /// <inheritdoc/>
        public override string ToString() => "(-∞";
    }

    /// <summary>
    /// Exclusive endpoint.
    /// </summary>
    public sealed record Exclusive(T Value) : LowerBound<T>
    {
        /// <inheritdoc/>
        public override UpperBound<T>? Touching => new UpperBound<T>.Inclusive(this.Value);

        /// <inheritdoc/>
        public override string ToString() => $"({this.Value}";
    }

    /// <summary>
    /// Inclusive endpoint.
    /// </summary>
    public sealed record Inclusive(T Value) : LowerBound<T>
    {
        /// <inheritdoc/>
        public override UpperBound<T>? Touching => new UpperBound<T>.Exclusive(this.Value);

        /// <inheritdoc/>
        public override string ToString() => $"[{this.Value}";
    }

    #region Operators

    /// <summary>
    /// Less-than compares two <see cref="LowerBound{T}"/>s.
    /// </summary>
    /// <param name="a">The first <see cref="LowerBound{T}"/> to compare.</param>
    /// <param name="b">The second <see cref="LowerBound{T}"/> to compare.</param>
    /// <returns>True, if <paramref name="a"/> comes before <paramref name="b"/>.</returns>
    public static bool operator <(LowerBound<T> a, LowerBound<T> b) => a.CompareTo(b) < 0;

    /// <summary>
    /// Greater-than compares two <see cref="LowerBound{T}"/>s.
    /// </summary>
    /// <param name="a">The first <see cref="LowerBound{T}"/> to compare.</param>
    /// <param name="b">The second <see cref="LowerBound{T}"/> to compare.</param>
    /// <returns>True, if <paramref name="a"/> comes after <paramref name="b"/>.</returns>
    public static bool operator >(LowerBound<T> a, LowerBound<T> b) => a.CompareTo(b) > 0;

    /// <summary>
    /// Less-than or equals compares two <see cref="LowerBound{T}"/>s.
    /// </summary>
    /// <param name="a">The first <see cref="LowerBound{T}"/> to compare.</param>
    /// <param name="b">The second <see cref="LowerBound{T}"/> to compare.</param>
    /// <returns>True, if <paramref name="a"/> comes before <paramref name="b"/> or they are equal.</returns>
    public static bool operator <=(LowerBound<T> a, LowerBound<T> b) => a.CompareTo(b) <= 0;

    /// <summary>
    /// Greater-than or equals compares two <see cref="LowerBound{T}"/>s.
    /// </summary>
    /// <param name="a">The first <see cref="LowerBound{T}"/> to compare.</param>
    /// <param name="b">The second <see cref="LowerBound{T}"/> to compare.</param>
    /// <returns>True, if <paramref name="a"/> comes after <paramref name="b"/> or they are equal.</returns>
    public static bool operator >=(LowerBound<T> a, LowerBound<T> b) => a.CompareTo(b) >= 0;

    /// <summary>
    /// Less-than compares a <see cref="LowerBound{T}"/> with an <see cref="UpperBound{T}"/>.
    /// </summary>
    /// <param name="a">The <see cref="LowerBound{T}"/> to compare.</param>
    /// <param name="b">The <see cref="UpperBound{T}"/> to compare.</param>
    /// <returns>True, if <paramref name="a"/> comes before <paramref name="b"/>.</returns>
    public static bool operator <(LowerBound<T> a, UpperBound<T> b) => a.CompareTo(b) < 0;

    /// <summary>
    /// Greater-than compares a <see cref="LowerBound{T}"/> with an <see cref="UpperBound{T}"/>.
    /// </summary>
    /// <param name="a">The <see cref="LowerBound{T}"/> to compare.</param>
    /// <param name="b">The <see cref="UpperBound{T}"/> to compare.</param>
    /// <returns>True, if <paramref name="a"/> comes after <paramref name="b"/>.</returns>
    public static bool operator >(LowerBound<T> a, UpperBound<T> b) => a.CompareTo(b) > 0;

    #endregion
}
