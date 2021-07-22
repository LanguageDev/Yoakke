// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Utilities.Compatibility
{
    /// <summary>
    /// Polyfill extensions, mostly for .netstandard 2 libraries in the project.
    /// Sadly source-generators are still .netstandard 2, if hosted in VS.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Pops the top element off a <see cref="Stack{T}"/>, if there is an element to pop.
        /// </summary>
        /// <typeparam name="T">The element-type of the <see cref="Stack{T}"/>.</typeparam>
        /// <param name="stack">The <see cref="Stack{T}"/> to try to pop the element from.</param>
        /// <param name="value">The popped off element gets written here, if there was one.</param>
        /// <returns>True, if there was an element to pop.</returns>
        public static bool TryPop<T>(this Stack<T> stack, [MaybeNullWhen(false)] out T? value)
        {
            if (stack.Count == 0)
            {
                value = default;
                return false;
            }
            else
            {
                value = stack.Pop();
                return true;
            }
        }

        /// <summary>
        /// Deconstructor pattern for <see cref="KeyValuePair{TKey, TValue}"/>s.
        /// </summary>
        /// <typeparam name="T1">The key type.</typeparam>
        /// <typeparam name="T2">The value type.</typeparam>
        /// <param name="tuple">The <see cref="KeyValuePair{T1, T2}"/> to deconstruct.</param>
        /// <param name="key">The deconstructed key gets written here.</param>
        /// <param name="value">The deconstructed value gets written here.</param>
        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
        {
            key = tuple.Key;
            value = tuple.Value;
        }

        /// <summary>
        /// Collects the elements into a <see cref="HashSet{T}"/>.
        /// </summary>
        /// <typeparam name="T">The element type of the <see cref="HashSet{T}"/>.</typeparam>
        /// <param name="values">The <see cref="IEnumerable{T}"/> values to collect.</param>
        /// <param name="comparer">The value comparer to use.</param>
        /// <returns>The collected values in a <see cref="HashSet{T}"/>.</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> values, IEqualityComparer<T> comparer)
        {
            var result = new HashSet<T>(comparer);
            foreach (var item in values) result.Add(item);
            return result;
        }

        /// <summary>
        /// Collects the elements into a <see cref="HashSet{T}"/>.
        /// </summary>
        /// <typeparam name="T">The element type of the <see cref="HashSet{T}"/>.</typeparam>
        /// <param name="values">The <see cref="IEnumerable{T}"/> values to collect.</param>
        /// <returns>The collected values in a <see cref="HashSet{T}"/>.</returns>
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> values)
        {
            var result = new HashSet<T>();
            foreach (var item in values) result.Add(item);
            return result;
        }
    }
}
