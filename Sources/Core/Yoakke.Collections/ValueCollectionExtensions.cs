// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections;

namespace Yoakke.Collections
{
    /// <summary>
    /// Extension methods for value-based equality collection creations.
    /// </summary>
    public static class ValueCollectionExtensions
    {
        /// <summary>
        /// Wraps an <see cref="IReadOnlyList{T}"/> to have value-based equality.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list">The <see cref="IReadOnlyList{T}"/> to wrap.</param>
        /// <returns>The <paramref name="list"/> wrapped as <see cref="IReadOnlyValueList{T}"/>.</returns>
        public static IReadOnlyValueList<T> AsValue<T>(this IReadOnlyList<T> list)
            where T : IEquatable<T> =>
            list is IReadOnlyValueList<T> readOnly
                ? readOnly
                : new ReadOnlyValueList<T>(list);

        /// <summary>
        /// Wraps an <see cref="IReadOnlySet{T}"/> to have value-based equality.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="set">The <see cref="IReadOnlySet{T}"/> to wrap.</param>
        /// <returns>The <paramref name="set"/> wrapped as <see cref="IReadOnlyValueSet{T}"/>.</returns>
        public static IReadOnlyValueSet<T> AsValue<T>(this IReadOnlySet<T> set)
            where T : IEquatable<T> =>
            set is IReadOnlyValueSet<T> readOnly
                ? readOnly
                : new ReadOnlyValueSet<T>(set);

        /// <summary>
        /// Wraps an <see cref="IReadOnlyValueDictionary{TKey, TValue}"/> to have value-based equality.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The <see cref="IReadOnlyDictionary{TKey, TValue}"/> to wrap.</param>
        /// <returns>The <paramref name="dictionary"/> wrapped as <see cref="IReadOnlyValueDictionary{TKey, TValue}"/>.</returns>
        public static IReadOnlyValueDictionary<TKey, TValue> AsValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary)
            where TKey : IEquatable<TKey>
            where TValue : IEquatable<TValue> =>
            dictionary is IReadOnlyValueDictionary<TKey, TValue> readOnly
                ? readOnly
                : new ReadOnlyValueDictionary<TKey, TValue>(dictionary);
    }
}
