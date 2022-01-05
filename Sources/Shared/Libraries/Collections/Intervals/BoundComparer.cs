// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// A comparer that can compare <see cref="LowerBound{T}"/> and <see cref="UpperBound{T}"/> instances.
/// </summary>
/// <typeparam name="T">The type of the endpoint value.</typeparam>
public sealed class BoundComparer<T>
    : IEqualityComparer<Bound<T>>, IEqualityComparer<LowerBound<T>>, IEqualityComparer<UpperBound<T>>,
      IComparer<Bound<T>>, IComparer<LowerBound<T>>, IComparer<UpperBound<T>>
{
    /// <summary>
    /// The default instance of the comparer.
    /// </summary>
    public static BoundComparer<T> Default { get; } = new(EqualityComparer<T>.Default, Comparer<T>.Default);

    /// <summary>
    /// The used <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    public IEqualityComparer<T> ValueEqualityComparer { get; }

    /// <summary>
    /// The used <see cref="IComparer{T}"/>.
    /// </summary>
    public IComparer<T> ValueComparer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BoundComparer{T}"/> class.
    /// </summary>
    /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
    /// <param name="comparer">The <see cref="IComparer{T}"/> to use.</param>
    public BoundComparer(IEqualityComparer<T> equalityComparer, IComparer<T> comparer)
    {
        this.ValueEqualityComparer = equalityComparer;
        this.ValueComparer = comparer;
    }

    /// <inheritdoc/>
    public bool Equals(Bound<T> x, Bound<T> y) => (x, y) switch
    {
        (LowerBound<T> l, LowerBound<T> r) => this.Equals(l, r),
        (UpperBound<T> l, UpperBound<T> r) => this.Equals(l, r),
        _ => false,
    };

    /// <inheritdoc/>
    public bool Equals(LowerBound<T> x, LowerBound<T> y) => (x, y) switch
    {
        (LowerBound<T>.Unbounded, LowerBound<T>.Unbounded) => true,
        (LowerBound<T>.Exclusive l, LowerBound<T>.Exclusive r) => this.ValueEqualityComparer.Equals(l.Value, r.Value),
        (LowerBound<T>.Inclusive l, LowerBound<T>.Inclusive r) => this.ValueEqualityComparer.Equals(l.Value, r.Value),
        _ => false,
    };

    /// <inheritdoc/>
    public bool Equals(UpperBound<T> x, UpperBound<T> y) => (x, y) switch
    {
        (UpperBound<T>.Unbounded, UpperBound<T>.Unbounded) => true,
        (UpperBound<T>.Exclusive l, UpperBound<T>.Exclusive r) => this.ValueEqualityComparer.Equals(l.Value, r.Value),
        (UpperBound<T>.Inclusive l, UpperBound<T>.Inclusive r) => this.ValueEqualityComparer.Equals(l.Value, r.Value),
        _ => false,
    };

    /// <inheritdoc/>
    public int GetHashCode(Bound<T> obj) => obj switch
    {
        LowerBound<T> l => this.GetHashCode(l),
        UpperBound<T> u => this.GetHashCode(u),
        _ => throw new ArgumentOutOfRangeException(nameof(obj)),
    };

    /// <inheritdoc/>
    public int GetHashCode(LowerBound<T> obj) => obj switch
    {
        LowerBound<T>.Unbounded => typeof(LowerBound<T>.Unbounded).GetHashCode(),
        LowerBound<T>.Exclusive e => this.MakeHash(typeof(LowerBound<T>.Exclusive), e.Value),
        LowerBound<T>.Inclusive i => this.MakeHash(typeof(LowerBound<T>.Inclusive), i.Value),
        _ => throw new ArgumentOutOfRangeException(nameof(obj)),
    };

    /// <inheritdoc/>
    public int GetHashCode(UpperBound<T> obj) => obj switch
    {
        UpperBound<T>.Unbounded => typeof(UpperBound<T>.Unbounded).GetHashCode(),
        UpperBound<T>.Exclusive e => this.MakeHash(typeof(UpperBound<T>.Exclusive), e.Value),
        UpperBound<T>.Inclusive i => this.MakeHash(typeof(UpperBound<T>.Inclusive), i.Value),
        _ => throw new ArgumentOutOfRangeException(nameof(obj)),
    };

    /// <inheritdoc/>
    public int Compare(Bound<T> x, Bound<T> y) => (x, y) switch
    {
        (LowerBound<T> l, LowerBound<T> r) => this.Compare(l, r),
        (LowerBound<T> l, UpperBound<T> r) => this.Compare(l, r),
        (UpperBound<T> l, LowerBound<T> r) => this.Compare(l, r),
        (UpperBound<T> l, UpperBound<T> r) => this.Compare(l, r),
        (LowerBound<T> or UpperBound<T>, _) => throw new ArgumentOutOfRangeException(nameof(y)),
        _ => throw new ArgumentOutOfRangeException(nameof(x)),
    };

    /// <inheritdoc/>
    public int Compare(LowerBound<T> x, LowerBound<T> y) => (x, y) switch
    {
        (LowerBound<T>.Unbounded, LowerBound<T>.Unbounded) => 0,
        (LowerBound<T>.Unbounded, _) => -1,
        (_, LowerBound<T>.Unbounded) => 1,
        (LowerBound<T>.Exclusive l, LowerBound<T>.Exclusive r) => this.ValueComparer.Compare(l.Value, r.Value),
        (LowerBound<T>.Inclusive l, LowerBound<T>.Inclusive r) => this.ValueComparer.Compare(l.Value, r.Value),
        (LowerBound<T>.Exclusive l, LowerBound<T>.Inclusive r) => this.ValueComparer.Compare(l.Value, r.Value) switch
        {
            0 => 1,
            var n => n,
        },
        (LowerBound<T>.Inclusive l, LowerBound<T>.Exclusive r) => this.ValueComparer.Compare(l.Value, r.Value) switch
        {
            0 => -1,
            var n => n,
        },
        (LowerBound<T>.Inclusive or LowerBound<T>.Exclusive or LowerBound<T>.Unbounded, _) => throw new ArgumentOutOfRangeException(nameof(y)),
        _ => throw new ArgumentOutOfRangeException(nameof(x)),
    };

    /// <inheritdoc/>
    public int Compare(UpperBound<T> x, UpperBound<T> y) => (x, y) switch
    {
        (UpperBound<T>.Unbounded, UpperBound<T>.Unbounded) => 0,
        (UpperBound<T>.Unbounded, _) => 1,
        (_, UpperBound<T>.Unbounded) => -1,
        (UpperBound<T>.Exclusive l, UpperBound<T>.Exclusive r) => this.ValueComparer.Compare(l.Value, r.Value),
        (UpperBound<T>.Inclusive l, UpperBound<T>.Inclusive r) => this.ValueComparer.Compare(l.Value, r.Value),
        (UpperBound<T>.Exclusive l, UpperBound<T>.Inclusive r) => this.ValueComparer.Compare(l.Value, r.Value) switch
        {
            0 => -1,
            var n => n,
        },
        (UpperBound<T>.Inclusive l, UpperBound<T>.Exclusive r) => this.ValueComparer.Compare(l.Value, r.Value) switch
        {
            0 => 1,
            var n => n,
        },
        (UpperBound<T>.Inclusive or UpperBound<T>.Exclusive or UpperBound<T>.Unbounded, _) => throw new ArgumentOutOfRangeException(nameof(y)),
        _ => throw new ArgumentOutOfRangeException(nameof(x)),
    };

    /// <summary>
    /// Compares a lower and upper bound with a comparer.
    /// </summary>
    /// <param name="x">The <see cref="LowerBound{T}"/> to compare.</param>
    /// <param name="y">The <see cref="UpperBound{T}"/> to compare.</param>
    /// <returns>The result of comparing <paramref name="x"/> and <paramref name="y"/>.
    /// See <see cref="IComparer{T}.Compare(T, T)"/> for further information.</returns>
    public int Compare(LowerBound<T> x, UpperBound<T> y) => (x, y) switch
    {
        (LowerBound<T>.Unbounded, _) or (_, UpperBound<T>.Unbounded) => -1,
        (LowerBound<T>.Exclusive l, UpperBound<T>.Exclusive r) => this.ValueComparer.Compare(l.Value, r.Value) switch
        {
            0 => 1,
            var n => n,
        },
        (LowerBound<T>.Exclusive l, UpperBound<T>.Inclusive r) => this.ValueComparer.Compare(l.Value, r.Value) switch
        {
            0 => 1,
            var n => n,
        },
        (LowerBound<T>.Inclusive l, UpperBound<T>.Exclusive r) => this.ValueComparer.Compare(l.Value, r.Value) switch
        {
            0 => 1,
            var n => n,
        },
        (LowerBound<T>.Inclusive l, UpperBound<T>.Inclusive r) => this.ValueComparer.Compare(l.Value, r.Value) switch
        {
            0 => -1,
            var n => n,
        },
        (LowerBound<T>.Inclusive or LowerBound<T>.Exclusive or LowerBound<T>.Unbounded, _) => throw new ArgumentOutOfRangeException(nameof(y)),
        _ => throw new ArgumentOutOfRangeException(nameof(x)),
    };

    /// <summary>
    /// Compares an upper and lower bound with a comparer.
    /// </summary>
    /// <param name="x">The <see cref="UpperBound{T}"/> to compare.</param>
    /// <param name="y">The <see cref="LowerBound{T}"/> to compare.</param>
    /// <returns>The result of comparing <paramref name="x"/> and <paramref name="y"/>.
    /// See <see cref="IComparer{T}.Compare(T, T)"/> for further information.</returns>
    public int Compare(UpperBound<T> x, LowerBound<T> y) => -this.Compare(y, x);

    /// <summary>
    /// Checks, if a lower bound is in touching relation with an upper bound.
    /// </summary>
    /// <param name="x">The <see cref="LowerBound{T}"/> to check.</param>
    /// <param name="y">The <see cref="UpperBound{T}"/> to check.</param>
    /// <returns>True, if <paramref name="x"/> is touching <paramref name="y"/>.</returns>
    public bool IsTouching(LowerBound<T> x, UpperBound<T> y) => (x, y) switch
    {
        (LowerBound<T>.Inclusive l, UpperBound<T>.Exclusive r) => this.ValueEqualityComparer.Equals(l.Value, r.Value),
        (LowerBound<T>.Exclusive l, UpperBound<T>.Inclusive r) => this.ValueEqualityComparer.Equals(l.Value, r.Value),
        _ => false,
    };

    /// <summary>
    /// Checks, if a lower bound is in touching relation with an upper bound.
    /// </summary>
    /// <param name="x">The <see cref="UpperBound{T}"/> to check.</param>
    /// <param name="y">The <see cref="LowerBound{T}"/> to check.</param>
    /// <returns>True, if <paramref name="x"/> is touching <paramref name="y"/>.</returns>
    public bool IsTouching(UpperBound<T> x, LowerBound<T> y) => this.IsTouching(y, x);

    /// <summary>
    /// Retrieves the smaller lower bound of the two passed.
    /// </summary>
    /// <param name="x">The first bound.</param>
    /// <param name="y">The second bound.</param>
    /// <returns>The smaller between <paramref name="x"/> and <paramref name="y"/>.</returns>
    public LowerBound<T> Min(LowerBound<T> x, LowerBound<T> y) => this.Compare(x, y) < 0 ? x : y;

    /// <summary>
    /// Retrieves the smaller upper bound of the two passed.
    /// </summary>
    /// <param name="x">The first bound.</param>
    /// <param name="y">The second bound.</param>
    /// <returns>The smaller between <paramref name="x"/> and <paramref name="y"/>.</returns>
    public UpperBound<T> Min(UpperBound<T> x, UpperBound<T> y) => this.Compare(x, y) < 0 ? x : y;

    /// <summary>
    /// Retrieves the larger lower bound of the two passed.
    /// </summary>
    /// <param name="x">The first bound.</param>
    /// <param name="y">The second bound.</param>
    /// <returns>The larger between <paramref name="x"/> and <paramref name="y"/>.</returns>
    public LowerBound<T> Max(LowerBound<T> x, LowerBound<T> y) => this.Compare(x, y) > 0 ? x : y;

    /// <summary>
    /// Retrieves the larger upper bound of the two passed.
    /// </summary>
    /// <param name="x">The first bound.</param>
    /// <param name="y">The second bound.</param>
    /// <returns>The larger between <paramref name="x"/> and <paramref name="y"/>.</returns>
    public UpperBound<T> Max(UpperBound<T> x, UpperBound<T> y) => this.Compare(x, y) > 0 ? x : y;

    private int MakeHash(Type type, T value)
    {
        var h = default(HashCode);
        h.Add(type);
        h.Add(value, this.ValueEqualityComparer);
        return h.ToHashCode();
    }
}
