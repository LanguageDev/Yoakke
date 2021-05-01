using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections
{
    public static class BinarySearchExtension
    {
        /// <summary>
        /// Performs a binary search on a sequence of values.
        /// 
        /// This is a specialized version if binary search that can compare different key-types.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="KSearch"></typeparam>
        /// <typeparam name="KValue"></typeparam>
        /// <param name="list">The list to search in.</param>
        /// <param name="searchedKey">The key to search for.</param>
        /// <param name="keySelector">The key selector function that selects the key for each element in the list.</param>
        /// <returns>A pair of the resulting index to insert the searched key to keep the ordering and a bool indicating if
        /// it's an exact match.</returns>
        public static (int Index, bool Exact) BinarySearch<TValue, KSearch, KValue>(
            this ReadOnlySpan<TValue> list,
            KSearch searchedKey,
            Func<TValue, KValue> keySelector) where KSearch : IComparable<KValue> =>
            list.BinarySearch(searchedKey, keySelector, (k1, k2) => k1.CompareTo(k2));

        /// <summary>
        /// Performs a binary search on a sequence of values.
        /// 
        /// This is a specialized version if binary search that can compare different key-types.
        /// </summary>
        /// <typeparam name="TValue">The element type to search in.</typeparam>
        /// <typeparam name="KSearch">The key type to search by.</typeparam>
        /// <typeparam name="KValue">The selected key type.</typeparam>
        /// <param name="list">The list to search in.</param>
        /// <param name="searchedKey">The key to search for.</param>
        /// <param name="keySelector">The key selector function that selects the key for each element in the list.</param>
        /// <param name="keyComparer">The comparer to use.</param>
        /// <returns>A pair of the resulting index to insert the searched key to keep the ordering and a bool indicating if
        /// it's an exact match.</returns>
        public static (int Index, bool Exact) BinarySearch<TValue, KSearch, KValue>(
            this ReadOnlySpan<TValue> list,
            KSearch searchedKey,
            Func<TValue, KValue> keySelector,
            Func<KSearch, KValue, int> keyComparer)
        {
            var size = list.Length;
            if (size == 0) return (0, false);

            var from = 0;
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
