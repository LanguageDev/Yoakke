// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections
{
    /// <summary>
    /// A mapping/dictionary interface that allows for an infinite number of elements and does not require element count or
    /// iterability.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public interface IMathMap<TKey, TValue> : IReadOnlyMathMap<TKey, TValue>
    {
        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">The key of the element to get or set.</param>
        /// <returns>The element with the specified key.</returns>
        public new TValue this[TKey key] { get; set; }

        /// <summary>
        /// Removes all items from the mapping.
        /// </summary>
        public void Clear();

        /// <summary>
        /// Adds an element with the provided key and value to the mapping.
        /// </summary>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        public void Add(TKey key, TValue value);

        /// <summary>
        /// Removes the element with the specified key from the mapping.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>True if the element is successfully removed, otherwise false.
        /// This method also returns false if key was not found in the mapping.</returns>
        public bool Remove(TKey key);
    }
}
