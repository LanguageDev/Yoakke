// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// A comparer that compares <see cref="LowerEndpoint{T}"/>s and <see cref="UpperEndpoint{T}"/>s.
/// </summary>
/// <typeparam name="T">The type of the endpoint value.</typeparam>
public sealed class EndpointComparer<T> : IEqualityComparer<LowerEndpoint<T>>, IEqualityComparer<UpperEndpoint<T>>,
                                          IComparer<LowerEndpoint<T>>, IComparer<UpperEndpoint<T>>
{
    /// <summary>
    /// A default instance of the comparer.
    /// </summary>
    public static EndpointComparer<T> Default { get; } = new(EqualityComparer<T>.Default, Comparer<T>.Default);

    /// <summary>
    /// The used <see cref="IEqualityComparer{T}"/>.
    /// </summary>
    public IEqualityComparer<T> ValueEqualityComparer { get; }

    /// <summary>
    /// The used <see cref="IComparer{T}"/>.
    /// </summary>
    public IComparer<T> ValueComparer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EndpointComparer{T}"/> class.
    /// </summary>
    /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
    /// <param name="comparer">The <see cref="IComparer{T}"/> to use.</param>
    public EndpointComparer(IEqualityComparer<T> equalityComparer, IComparer<T> comparer)
    {
        this.ValueEqualityComparer = equalityComparer;
        this.ValueComparer = comparer;
    }

    /// <inheritdoc/>
    public int GetHashCode(LowerEndpoint<T> obj) => this.GetHashCode(obj.Type, obj.Value);

    /// <inheritdoc/>
    public int GetHashCode(UpperEndpoint<T> obj) => this.GetHashCode(obj.Type, obj.Value);

    private int GetHashCode(EndpointType type, T? value) => type switch
    {
        EndpointType.Unbounded => EndpointType.Unbounded.GetHashCode(),
        EndpointType.Inclusive or EndpointType.Exclusive => this.MakeHash(type, value!),
        _ => throw new InvalidOperationException(),
    };

    /// <inheritdoc/>
    public bool Equals(LowerEndpoint<T> x, LowerEndpoint<T> y) =>
        this.EqualsSameSide(x.Type, x.Value, y.Type, y.Value);

    /// <inheritdoc/>
    public bool Equals(UpperEndpoint<T> x, UpperEndpoint<T> y) =>
        this.EqualsSameSide(x.Type, x.Value, y.Type, y.Value);

    private bool EqualsSameSide(EndpointType t1, T? v1, EndpointType t2, T? v2)
    {
        if (t1 != t2) return false;
        if (t1 == EndpointType.Unbounded) return true;
        return this.ValueEqualityComparer.Equals(v1!, v2!);
    }

    /// <inheritdoc/>
    public int Compare(LowerEndpoint<T> x, LowerEndpoint<T> y) => (x.Type, y.Type) switch
    {
        (EndpointType.Unbounded, EndpointType.Unbounded) => 0,
        (EndpointType.Unbounded, _) => -1,
        (_, EndpointType.Unbounded) => 1,
        (EndpointType.Exclusive, EndpointType.Exclusive)
     or (EndpointType.Inclusive, EndpointType.Inclusive) => this.ValueComparer.Compare(x.Value!, y.Value!),
        (EndpointType.Exclusive, EndpointType.Inclusive) => this.ValueComparer.Compare(x.Value!, y.Value!) switch
        {
            0 => 1,
            var n => n,
        },
        (EndpointType.Inclusive, EndpointType.Exclusive) => this.ValueComparer.Compare(x.Value!, y.Value!) switch
        {
            0 => -1,
            var n => n,
        },
        _ => throw new InvalidOperationException(),
    };

    /// <inheritdoc/>
    public int Compare(UpperEndpoint<T> x, UpperEndpoint<T> y) =>  (x.Type, y.Type) switch
    {
        (EndpointType.Unbounded, EndpointType.Unbounded) => 0,
        (EndpointType.Unbounded, _) => 1,
        (_, EndpointType.Unbounded) => -1,
        (EndpointType.Exclusive, EndpointType.Exclusive)
     or (EndpointType.Inclusive, EndpointType.Inclusive) => this.ValueComparer.Compare(x.Value!, y.Value!),
        (EndpointType.Exclusive, EndpointType.Inclusive) => this.ValueComparer.Compare(x.Value!, y.Value!) switch
        {
            0 => -1,
            var n => n,
        },
        (EndpointType.Inclusive, EndpointType.Exclusive) => this.ValueComparer.Compare(x.Value!, y.Value!) switch
        {
            0 => 1,
            var n => n,
        },
        _ => throw new InvalidOperationException(),
    };

    /// <summary>
    /// Compares a lower endpoing to an upper endpoint.
    /// </summary>
    /// <param name="x">The lower endpoint to compare.</param>
    /// <param name="y">The upper endpoint to compare.</param>
    /// <returns>The integer describing the relation of <paramref name="x"/> and <paramref name="y"/>.</returns>
    public int Compare(LowerEndpoint<T> x, UpperEndpoint<T> y) => (x.Type, y.Type) switch
    {
        (EndpointType.Unbounded, _) or (_, EndpointType.Unbounded) => -1,
        (EndpointType.Exclusive, EndpointType.Exclusive)
     or (EndpointType.Exclusive, EndpointType.Inclusive)
     or (EndpointType.Inclusive, EndpointType.Exclusive) => this.ValueComparer.Compare(x.Value!, y.Value!) switch
        {
            0 => 1,
            var n => n,
        },
        (EndpointType.Inclusive, EndpointType.Inclusive) => this.ValueComparer.Compare(x.Value!, y.Value!) switch
        {
            0 => -1,
            var n => n,
        },
        _ => throw new InvalidOperationException(),
    };

    /// <summary>
    /// Compares an upper endpoing to a lower endpoint.
    /// </summary>
    /// <param name="x">The upper endpoint to compare.</param>
    /// <param name="y">The lower endpoint to compare.</param>
    /// <returns>The integer describing the relation of <paramref name="x"/> and <paramref name="y"/>.</returns>
    public int Compare(UpperEndpoint<T> x, LowerEndpoint<T> y) => -this.Compare(y, x);

    /// <summary>
    /// Checks, if a lower endpoint is in touching relation with an upper endpoint.
    /// </summary>
    /// <param name="x">The <see cref="LowerEndpoint{T}"/> to check.</param>
    /// <param name="y">The <see cref="UpperEndpoint{T}"/> to check.</param>
    /// <returns>True, if <paramref name="x"/> is touching <paramref name="y"/>.</returns>
    public bool IsTouching(LowerEndpoint<T> x, UpperEndpoint<T> y) => (x.Type, y.Type) switch
    {
        (EndpointType.Inclusive, EndpointType.Exclusive)
     or (EndpointType.Exclusive, EndpointType.Inclusive) => this.ValueEqualityComparer.Equals(x.Value!, y.Value!),
        _ => false,
    };

    /// <summary>
    /// Checks, if a lower endpoint is in touching relation with an upper endpoint.
    /// </summary>
    /// <param name="x">The <see cref="UpperEndpoint{T}"/> to check.</param>
    /// <param name="y">The <see cref="LowerEndpoint{T}"/> to check.</param>
    /// <returns>True, if <paramref name="x"/> is touching <paramref name="y"/>.</returns>
    public bool IsTouching(UpperEndpoint<T> x, LowerEndpoint<T> y) => this.IsTouching(y, x);

    private int MakeHash(EndpointType type, T value)
    {
        var h = default(HashCode);
        h.Add(type);
        h.Add(value, this.ValueEqualityComparer);
        return h.ToHashCode();
    }
}
