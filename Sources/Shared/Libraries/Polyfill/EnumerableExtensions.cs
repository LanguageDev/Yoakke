// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace System.Collections.Generic.Polyfill;

/// <summary>
/// Extensions for <see cref="IEnumerable{T}"/>.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/> using the
    /// <paramref name="comparer"/> to compare keys.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to create a <see cref="HashSet{T}"/> from.</param>
    /// <param name="comparer">An <see cref="IEqualityComparer{T}"/> to compare keys.</param>
    /// <returns>A <see cref="HashSet{T}"/> that contains values of type <typeparamref name="TSource"/>
    /// selected from the input sequence.</returns>
    public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource>? comparer)
    {
        var result = new HashSet<TSource>(comparer);
        foreach (var item in source) result.Add(item);
        return result;
    }

    /// <summary>
    /// Creates a <see cref="HashSet{T}"/> from an <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of <paramref name="source"/>.</typeparam>
    /// <param name="source">An <see cref="IEnumerable{T}"/> to create a <see cref="HashSet{T}"/> from.</param>
    /// <returns>A <see cref="HashSet{T}"/> that contains values of type <typeparamref name="TSource"/>
    /// selected from the input sequence.</returns>
    public static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source) => source.ToHashSet(null);

    /// <summary>
    /// Returns a new enumerable collection that contains the elements from <paramref name="source"/> with the last
    /// <paramref name="count"/> elements of the source collection omitted.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements in the enumerable collection.</typeparam>
    /// <param name="source">An enumerable collection instance.</param>
    /// <param name="count">The number of elements to omit from the end of the collection.</param>
    /// <returns>A new enumerable collection that contains the elements from <paramref name="source"/> minus
    /// <paramref name="count"/> elements from the end of the collection.</returns>
    public static IEnumerable<TSource> SkipLast<TSource>(this IEnumerable<TSource> source, int count) => count <= 0
        ? source
        : SkipLastImpl(source, count);

    private static IEnumerable<TSource> SkipLastImpl<TSource>(IEnumerable<TSource> source, int count)
    {
        var backBuffer = new TSource[count];
        var index = 0;
        foreach (var item in source)
        {
            var swapIndex = (index + count) % count;
            var oldItem = backBuffer[swapIndex];
            backBuffer[swapIndex] = item;
            if (index >= count) yield return oldItem;
            ++index;
        }
    }
}
