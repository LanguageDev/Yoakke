// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;

namespace Yoakke.Collections
{
    public static class BinarySearchExtension
    {
        /// <summary>
        /// Performs a binary search on a sequence of values.
        ///
        /// This is a specialized version if binary search that can compare different key-types.
        /// </summary>
        /// <typeparam name="TItem">The element type to search in.</typeparam>
        /// <typeparam name="TKeyBy">The key type to search by.</typeparam>
        /// <typeparam name="TKeyFind">The selected key type.</typeparam>
        /// <param name="list">The list to search in.</param>
        /// <param name="searchedKey">The key to search for.</param>
        /// <param name="keySelector">The key selector function that selects the key for each element in the list.</param>
        /// <returns>A pair of the resulting index to insert the searched key to keep the ordering and a bool indicating if
        /// it's an exact match.</returns>
        public static (int Index, bool Exact) BinarySearch<TItem, TKeyBy, TKeyFind>(
            this IReadOnlyList<TItem> list,
            TKeyBy searchedKey,
            Func<TItem, TKeyFind> keySelector)
            where TKeyBy : IComparable<TKeyFind> =>
            list.BinarySearch(0, searchedKey, keySelector, (k1, k2) => k1.CompareTo(k2));

        /// <summary>
        /// Performs a binary search on a sequence of values.
        ///
        /// This is a specialized version if binary search that can compare different key-types.
        /// </summary>
        /// <typeparam name="TItem">The element type to search in.</typeparam>
        /// <typeparam name="TKeyBy">The key type to search by.</typeparam>
        /// <typeparam name="TKeyFind">The selected key type.</typeparam>
        /// <param name="list">The list to search in.</param>
        /// <param name="searchedKey">The key to search for.</param>
        /// <param name="keySelector">The key selector function that selects the key for each element in the list.</param>
        /// <param name="keyComparer">The comparer to use.</param>
        /// <returns>A pair of the resulting index to insert the searched key to keep the ordering and a bool indicating if
        /// it's an exact match.</returns>
        public static (int Index, bool Exact) BinarySearch<TItem, TKeyBy, TKeyFind>(
            this IReadOnlyList<TItem> list,
            TKeyBy searchedKey,
            Func<TItem, TKeyFind> keySelector,
            Func<TKeyBy, TKeyFind, int> keyComparer) =>
            list.BinarySearch(0, list.Count, searchedKey, keySelector, keyComparer);

        /// <summary>
        /// Performs a binary search on a sequence of values.
        ///
        /// This is a specialized version if binary search that can compare different key-types.
        /// </summary>
        /// <typeparam name="TItem">The element type to search in.</typeparam>
        /// <typeparam name="TKeyBy">The key type to search by.</typeparam>
        /// <typeparam name="TKeyFind">The selected key type.</typeparam>
        /// <param name="list">The list to search in.</param>
        /// <param name="start">The start index of the search.</param>
        /// <param name="searchedKey">The key to search for.</param>
        /// <param name="keySelector">The key selector function that selects the key for each element in the list.</param>
        /// <param name="keyComparer">The comparer to use.</param>
        /// <returns>A pair of the resulting index to insert the searched key to keep the ordering and a bool indicating if
        /// it's an exact match.</returns>
        public static (int Index, bool Exact) BinarySearch<TItem, TKeyBy, TKeyFind>(
            this IReadOnlyList<TItem> list,
            int start,
            TKeyBy searchedKey,
            Func<TItem, TKeyFind> keySelector,
            Func<TKeyBy, TKeyFind, int> keyComparer) =>
            list.BinarySearch(start, list.Count - start, searchedKey, keySelector, keyComparer);

        /// <summary>
        /// Performs a binary search on a sequence of values.
        ///
        /// This is a specialized version if binary search that can compare different key-types.
        /// </summary>
        /// <typeparam name="TItem">The element type to search in.</typeparam>
        /// <typeparam name="TKeyBy">The key type to search by.</typeparam>
        /// <typeparam name="TKeyFind">The selected key type.</typeparam>
        /// <param name="list">The list to search in.</param>
        /// <param name="start">The start index of the search.</param>
        /// <param name="length">The length of the range to search.</param>
        /// <param name="searchedKey">The key to search for.</param>
        /// <param name="keySelector">The key selector function that selects the key for each element in the list.</param>
        /// <param name="keyComparer">The comparer to use.</param>
        /// <returns>A pair of the resulting index to insert the searched key to keep the ordering and a bool indicating if
        /// it's an exact match.</returns>
        public static (int Index, bool Exact) BinarySearch<TItem, TKeyBy, TKeyFind>(
            this IReadOnlyList<TItem> list,
            int start,
            int length,
            TKeyBy searchedKey,
            Func<TItem, TKeyFind> keySelector,
            Func<TKeyBy, TKeyFind, int> keyComparer)
        {
            var size = length;
            if (size == 0) return (start, false);

            var from = start;
            while (size > 1)
            {
                var half = size / 2;
                var mid = from + half;
                var key = keySelector(list[mid]);
                var cmp = keyComparer(searchedKey, key);
                from = cmp > 0 ? mid : from;
                size -= half;
            }

            var resultKey = keySelector(list[from]);
            var resultCmp = keyComparer(searchedKey, resultKey);
            if (resultCmp == 0) return (from, true);
            else return (from + (resultCmp > 0 ? 1 : 0), false);
        }
    }
}
