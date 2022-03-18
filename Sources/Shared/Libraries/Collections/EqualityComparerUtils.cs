// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections;

/// <summary>
/// Utilities for <see cref="IEqualityComparer{T}"/>s.
/// </summary>
public static class EqualityComparerUtils
{
    private sealed record class TupleComparer<TFirst, TSecond>(
        IEqualityComparer<TFirst> FirstComparer,
        IEqualityComparer<TSecond> SecondComparer) : IEqualityComparer<(TFirst, TSecond)>
    {
        public bool Equals((TFirst, TSecond) x, (TFirst, TSecond) y) =>
            this.FirstComparer.Equals(x.Item1, y.Item1)
         && this.SecondComparer.Equals(x.Item2, y.Item2);

        public int GetHashCode((TFirst, TSecond) obj)
        {
            var h = default(HashCode);
            h.Add(obj.Item1, this.FirstComparer);
            h.Add(obj.Item2, this.SecondComparer);
            return h.ToHashCode();
        }
    }

    /// <summary>
    /// Constructs a tuple equality comparer from two element comparers.
    /// </summary>
    /// <typeparam name="TFirst">The first element type.</typeparam>
    /// <typeparam name="TSecond">The second element type.</typeparam>
    /// <param name="firstComparer">The comparer for the first element.</param>
    /// <param name="secondComparer">The comparer for the second element.</param>
    /// <returns>An equality comparer that compares the first elements using <paramref name="firstComparer"/>
    /// and the second elements using <paramref name="secondComparer"/>.</returns>
    public static IEqualityComparer<(TFirst, TSecond)> Tuple<TFirst, TSecond>(
        IEqualityComparer<TFirst> firstComparer,
        IEqualityComparer<TSecond> secondComparer) =>
        new TupleComparer<TFirst, TSecond>(firstComparer, secondComparer);
}
