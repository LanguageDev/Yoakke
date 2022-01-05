// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Runtime.CompilerServices;

namespace System.Collections.Generic.Polyfill;

/// <summary>
/// An <see cref="IEqualityComparer{T}"/> that uses reference equality (<see cref="object.ReferenceEquals(object, object)"/>)
/// instead of value equality (<see cref="object.Equals(object)"/>) when comparing two object instances.
/// </summary>
public sealed class ReferenceEqualityComparer : IEqualityComparer<object>, IEqualityComparer
{
    /// <summary>
    /// Gets the singleton <see cref="ReferenceEqualityComparer"/> instance.
    /// </summary>
    public static ReferenceEqualityComparer Instance { get; } = new();

    /// <summary>
    /// Determines whether two object references refer to the same object instance.
    /// </summary>
    /// <param name="x">The first object to compare.</param>
    /// <param name="y">The second object to compare.</param>
    /// <returns>True if both <paramref name="x"/> and <paramref name="y"/> refer to the same object instance or
    /// if both are null; otherwise, false.</returns>
    public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

    /// <summary>
    /// Returns a hash code for the specified object. The returned hash code is based on the object identity,
    /// not on the contents of the object.
    /// </summary>
    /// <param name="obj">The object for which to retrieve the hash code.</param>
    /// <returns>A hash code for the identity of <paramref name="obj"/>.</returns>
    public int GetHashCode(object? obj) => RuntimeHelpers.GetHashCode(obj);
}
