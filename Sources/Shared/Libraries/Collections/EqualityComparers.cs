// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;

namespace Yoakke.Collections;

/// <summary>
/// <see cref="IEqualityComparer{T}"/> construction utilities.
/// </summary>
public static class EqualityComparers
{
    private sealed record class LambdaEqualityComparer<T>(
        Func<T?, T?, bool> Equality,
        Func<T, int> Hash) : IEqualityComparer<T>
    {
        public bool Equals(T? x, T? y) => this.Equality(x, y);

        public int GetHashCode(T obj) => this.Hash(obj);
    }

    /// <summary>
    /// Creates a new equality comparer from an equality and a hash function.
    /// </summary>
    /// <typeparam name="T">The type to create the equality comparer for.</typeparam>
    /// <param name="equals">The equality function.</param>
    /// <param name="hash">The hash function.</param>
    /// <returns>A new equality comparer that uses <paramref name="equals"/> and <paramref name="hash"/>
    /// for comparisons.</returns>
    public static IEqualityComparer<T> Create<T>(
        Func<T?, T?, bool> equals,
        Func<T, int> hash) => new LambdaEqualityComparer<T>(equals, hash);

    /// <summary>
    /// Constructs a <see cref="KeyValuePair{TKey, TValue}"/> equality comparer.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="keyComparer">The first element comparer.</param>
    /// <param name="valueComparer">The second element comparer.</param>
    /// <returns>An equality comparer that compares the <see cref="KeyValuePair{TKey, TValue}"/>
    /// elements with the given comparers.</returns>
    public static IEqualityComparer<KeyValuePair<TKey, TValue>> CreateKeyValuePair<TKey, TValue>(
        IEqualityComparer<TKey>? keyComparer = null,
        IEqualityComparer<TValue>? valueComparer = null)
    {
        var cmpKey = keyComparer ?? EqualityComparer<TKey>.Default;
        var cmpValue = valueComparer ?? EqualityComparer<TValue>.Default;
        return Create<KeyValuePair<TKey, TValue>>(
            (x, y) => cmpKey.Equals(x.Key, y.Key) && cmpValue.Equals(x.Value, y.Value),
            x =>
            {
                var h = default(HashCode);
                h.Add(x.Key, keyComparer);
                h.Add(x.Value, valueComparer);
                return h.ToHashCode();
            });
    }

    /// <summary>
    /// Constructs a tuple equality comparer.
    /// </summary>
    /// <typeparam name="T1">The first element type.</typeparam>
    /// <typeparam name="T2">The second element type.</typeparam>
    /// <param name="first">The first element comparer.</param>
    /// <param name="second">The second element comparer.</param>
    /// <returns>An equality comparer that compares the tuple elements with the given comparers.</returns>
    public static IEqualityComparer<(T1, T2)> CreateTuple<T1, T2>(
        IEqualityComparer<T1>? first = null,
        IEqualityComparer<T2>? second = null)
    {
        var cmp1 = first ?? EqualityComparer<T1>.Default;
        var cmp2 = second ?? EqualityComparer<T2>.Default;
        return Create<(T1, T2)>(
            (x, y) => cmp1.Equals(x.Item1, y.Item1) && cmp2.Equals(x.Item2, y.Item2),
            x =>
            {
                var h = default(HashCode);
                h.Add(x.Item1, cmp1);
                h.Add(x.Item2, cmp2);
                return h.ToHashCode();
            });
    }

    /// <summary>
    /// Constructs a set equality comparer that compares two sequences order-independently.
    /// </summary>
    /// <typeparam name="T">The element type of the sequence.</typeparam>
    /// <param name="elementComparer">The comparer to compare elements with.</param>
    /// <returns>An <see cref="IEqualityComparer{T}"/> that compares elements order independently.</returns>
    public static IEqualityComparer<IReadOnlyCollection<T>> CreateSet<T>(IEqualityComparer<T>? elementComparer = null)
    {
        var cmp = elementComparer ?? EqualityComparer<T>.Default;
        return Create<IReadOnlyCollection<T>>(
            (x, y) =>
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                if (x.Count != y.Count) return false;
                return x.ToHashSet(cmp).SetEquals(y);
            },
            x =>
            {
                // NOTE: Order-independent hash
                var hashCode = 0;
                foreach (var item in x)
                {
                    if (item is not null) hashCode ^= cmp.GetHashCode(item);
                }
                return hashCode;
            });
    }

    /// <summary>
    /// Constructs a sequence equality comparer that compares two sequences order-dependently.
    /// </summary>
    /// <typeparam name="T">The element type of the sequence.</typeparam>
    /// <param name="elementComparer">The comparer to compare elements with.</param>
    /// <returns>An <see cref="IEqualityComparer{T}"/> that compares elements order dependently.</returns>
    public static IEqualityComparer<IReadOnlyCollection<T>> CreateSequence<T>(IEqualityComparer<T>? elementComparer = null)
    {
        var cmp = elementComparer ?? EqualityComparer<T>.Default;
        return Create<IReadOnlyCollection<T>>(
            (x, y) =>
            {
                if (ReferenceEquals(x, y)) return true;
                if (x is null || y is null) return false;
                if (x.Count != y.Count) return false;
                return x.SequenceEqual(y, cmp);
            },
            x =>
            {
                var h = default(HashCode);
                foreach (var item in x) h.Add(item, cmp);
                return h.ToHashCode();
            });
    }
}
