// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Yoakke.Collections
{
    /// <summary>
    /// A read-only mapping/dictionary interface that allows for an infinite number of elements and does not require element
    /// count or iterability.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public interface IReadOnlyMathMap<TKey, TValue>
    {
        /// <summary>
        /// True, if the mapping contains no elements.
        /// </summary>
        public bool IsEmpty { get; }

        /// <summary>
        /// Gets the number of elements in the mapping, if it is finite.
        /// It is null, if the mapping contains infinite elements.
        /// </summary>
        public int? Count { get; }

        /// <summary>
        /// Gets the key-value pairs in the mapping, if it is iterable.
        /// It is null, if the elements are not iterable.
        /// </summary>
        public IEnumerable<KeyValuePair<TKey, TValue>>? Values { get; }

        /// <summary>
        /// Gets the element that has the specified key in the mapping.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>The element that has the specified key in the mapping.</returns>
        public TValue this[TKey key] { get; }

        /// <summary>
        /// Determines whether the mapping contains an element that has the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>True if the mapping contains an element that has the specified key, otherwise, false.</returns>
        public bool ContainsKey(TKey key);

        /// <summary>
        /// Gets the value that is associated with the specified key.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified key, if the key is found,
        /// otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>True if the mapping contains an element that has the specified key, otherwise false.</returns>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
    }
}
