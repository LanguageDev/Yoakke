// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Yoakke.Collections
{
    /// <summary>
    /// Extensions for enumerables.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Queries the first element of an enumerable.
        /// </summary>
        /// <typeparam name="T">The sequence element type.</typeparam>
        /// <param name="enumerable">The enumerable to query.</param>
        /// <param name="element">The first element gets written here, if there was any.</param>
        /// <returns>True, if there was an element in the query.</returns>
        public static bool TryFirst<T>(this IEnumerable<T> enumerable, [MaybeNullWhen(false)] out T element)
        {
            var enumerator = enumerable.GetEnumerator();
            if (enumerator.MoveNext())
            {
                element = enumerator.Current;
                return true;
            }
            else
            {
                element = default;
                return false;
            }
        }

        /// <summary>
        /// Queries for the indices of some element.
        /// </summary>
        /// <typeparam name="T">The sequence element type.</typeparam>
        /// <param name="enumerable">The enumerable to query.</param>
        /// <param name="item">The item to search for.</param>
        /// <returns>The sequence of indices where <paramref name="enumerable"/> contains <paramref name="item"/>.</returns>
        public static IEnumerable<int> IndicesOf<T>(this IEnumerable<T> enumerable, T item) => enumerable
            .IndicesOf(item, EqualityComparer<T>.Default);

        /// <summary>
        /// Queries for the indices of some element.
        /// </summary>
        /// <typeparam name="T">The sequence element type.</typeparam>
        /// <param name="enumerable">The enumerable to query.</param>
        /// <param name="item">The item to search for.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <returns>The sequence of indices where <paramref name="enumerable"/> contains <paramref name="item"/>.</returns>
        public static IEnumerable<int> IndicesOf<T>(this IEnumerable<T> enumerable, T item, IEqualityComparer<T> comparer) => enumerable
            .IndicesOf(foundItem => comparer.Equals(item, foundItem));

        /// <summary>
        /// Queries for the indices of some property being true.
        /// </summary>
        /// <typeparam name="T">The sequence element type.</typeparam>
        /// <param name="enumerable">The enumerable to query.</param>
        /// <param name="predicate">The predicate that has to apply.</param>
        /// <returns>The sequence of indices where <paramref name="predicate"/> is true in <paramref name="enumerable"/>.</returns>
        public static IEnumerable<int> IndicesOf<T>(this IEnumerable<T> enumerable, Predicate<T> predicate) => enumerable
            .Select((e, i) => (Element: e, Index: i))
            .Where(pair => predicate(pair.Element))
            .Select(pair => pair.Index);
    }
}
