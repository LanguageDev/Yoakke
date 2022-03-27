// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;

namespace Yoakke.Collections;

/// <summary>
/// Hashing utilities.
/// </summary>
public static class HashUtils
{
    /// <summary>
    /// Combines the hash code of the given sequence of elements into a single hash-code in an order-dependent way.
    /// </summary>
    /// <typeparam name="T">The sequence element type.</typeparam>
    /// <param name="items">The items to combine the hash code of.</param>
    /// <param name="comparer">The comparer to use.</param>
    /// <returns>The combined hash-code of <paramref name="items"/>.</returns>
    public static int CombineSequence<T>(IEnumerable<T> items, IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;
        var h = default(HashCode);
        foreach (var item in items) h.Add(item, comparer);
        return h.ToHashCode();
    }

    /// <summary>
    /// Combines the hash code of the given sequence of elements into a single hash-code by XOR-ing them tohether,
    /// meaning that the hash-code stays order-independent.
    /// </summary>
    /// <typeparam name="T">The sequence element type.</typeparam>
    /// <param name="items">The items to combine the hash code of.</param>
    /// <param name="comparer">The comparer to use.</param>
    /// <returns>The combined hash-code of <paramref name="items"/>.</returns>
    public static int CombineXor<T>(IEnumerable<T> items, IEqualityComparer<T>? comparer = null)
    {
        comparer ??= EqualityComparer<T>.Default;
        var h = 0;
        foreach (var item in items) h ^= comparer.GetHashCode(item);
        return h;
    }
}
