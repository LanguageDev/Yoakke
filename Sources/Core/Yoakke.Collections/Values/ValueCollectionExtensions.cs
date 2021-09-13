// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;

namespace Yoakke.Collections.Values
{
    /// <summary>
    /// Extension methods for value-based equality collection creations.
    /// </summary>
    public static class ValueCollectionExtensions
    {
        #region ValueList

        /// <summary>
        /// Wraps an <see cref="IReadOnlyList{T}"/> to have value-based equality.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list">The <see cref="IReadOnlyList{T}"/> to wrap.</param>
        /// <returns>The <paramref name="list"/> wrapped as <see cref="IReadOnlyValueList{T}"/>.</returns>
        public static IReadOnlyValueList<T> ToValue<T>(this IReadOnlyList<T> list)
            where T : IEquatable<T> => list.ToValue(EqualityComparer<T>.Default);

        /// <summary>
        /// Wraps an <see cref="IReadOnlyList{T}"/> to have value-based equality.
        /// </summary>
        /// <typeparam name="T">The element type.</typeparam>
        /// <param name="list">The <see cref="IReadOnlyList{T}"/> to wrap.</param>
        /// <param name="comparer">The comparer to use.</param>
        /// <returns>The <paramref name="list"/> wrapped as <see cref="IReadOnlyValueList{T}"/>.</returns>
        public static IReadOnlyValueList<T> ToValue<T>(this IReadOnlyList<T> list, IEqualityComparer<T> comparer)
            where T : IEquatable<T> => new ReadOnlyValueList<T>(list, comparer);

        #endregion

        #region ValueDictionary

        /// <summary>
        /// Wraps an <see cref="IReadOnlyValueDictionary{TKey, TValue}"/> to have value-based equality.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The <see cref="IReadOnlyDictionary{TKey, TValue}"/> to wrap.</param>
        /// <returns>The <paramref name="dictionary"/> wrapped as <see cref="IReadOnlyValueDictionary{TKey, TValue}"/>.</returns>
        public static IReadOnlyValueDictionary<TKey, TValue> ToValue<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary)
            where TKey : IEquatable<TKey>
            where TValue : IEquatable<TValue> => dictionary.ToValue(EqualityComparer<TKey>.Default, EqualityComparer<TValue>.Default);

        /// <summary>
        /// Wraps an <see cref="IReadOnlyValueDictionary{TKey, TValue}"/> to have value-based equality.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The <see cref="IReadOnlyDictionary{TKey, TValue}"/> to wrap.</param>
        /// <param name="keyComparer">The key comparer to use.</param>
        /// <returns>The <paramref name="dictionary"/> wrapped as <see cref="IReadOnlyValueDictionary{TKey, TValue}"/>.</returns>
        public static IReadOnlyValueDictionary<TKey, TValue> ToValue<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> dictionary,
            IEqualityComparer<TKey> keyComparer)
            where TKey : IEquatable<TKey>
            where TValue : IEquatable<TValue> => dictionary.ToValue(keyComparer, EqualityComparer<TValue>.Default);

        /// <summary>
        /// Wraps an <see cref="IReadOnlyValueDictionary{TKey, TValue}"/> to have value-based equality.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The <see cref="IReadOnlyDictionary{TKey, TValue}"/> to wrap.</param>
        /// <param name="valueComparer">The value comparer to use.</param>
        /// <returns>The <paramref name="dictionary"/> wrapped as <see cref="IReadOnlyValueDictionary{TKey, TValue}"/>.</returns>
        public static IReadOnlyValueDictionary<TKey, TValue> ToValue<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> dictionary,
            IEqualityComparer<TValue> valueComparer)
            where TKey : IEquatable<TKey>
            where TValue : IEquatable<TValue> => dictionary.ToValue(EqualityComparer<TKey>.Default, valueComparer);

        /// <summary>
        /// Wraps an <see cref="IReadOnlyValueDictionary{TKey, TValue}"/> to have value-based equality.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The <see cref="IReadOnlyDictionary{TKey, TValue}"/> to wrap.</param>
        /// <param name="keyComparer">The key comparer to use.</param>
        /// <param name="valueComparer">The value comparer to use.</param>
        /// <returns>The <paramref name="dictionary"/> wrapped as <see cref="IReadOnlyValueDictionary{TKey, TValue}"/>.</returns>
        public static IReadOnlyValueDictionary<TKey, TValue> ToValue<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> dictionary,
            IEqualityComparer<TKey> keyComparer,
            IEqualityComparer<TValue> valueComparer)
            where TKey : IEquatable<TKey>
            where TValue : IEquatable<TValue> => new ReadOnlyValueDictionary<TKey, TValue>(dictionary, keyComparer, valueComparer);

        #endregion
    }
}
