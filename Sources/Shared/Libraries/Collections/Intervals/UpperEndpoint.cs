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

    /// <inheritdoc/>
    public override string ToString() => this.Type switch
    {
        EndpointType.Unbounded => "+∞)",
        EndpointType.Inclusive => $"{this.Value}]",
        EndpointType.Exclusive => $"{this.Value})",
        _ => throw new InvalidOperationException(),
    };
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
