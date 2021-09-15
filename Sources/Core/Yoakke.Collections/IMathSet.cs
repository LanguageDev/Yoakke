// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections
{
    /// <summary>
    /// A set interface that allows for an infinite number of elements and does not require element count or iterability.
    /// </summary>
    /// <typeparam name="T">The set element type.</typeparam>
    public interface IMathSet<T> : IReadOnlyMathSet<T>
    {
        /// <summary>
        /// Removes all items from the set.
        /// </summary>
        public void Clear();

        /// <summary>
        /// Adds an element to the current set and returns a value to indicate if the element was successfully added.
        /// </summary>
        /// <param name="item">The element to add to the set.</param>
        /// <returns>True if the element is added to the set, false if the element is already in the set.</returns>
        public bool Add(T item);

        /// <summary>
        /// Removes the specified item from the set.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        /// <returns>True, if the item was dound and successfully removed from the set, false, if it was not found.</returns>
        public bool Remove(T item);

        /// <summary>
        /// Removes all elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">The collection of items to remove from the set.</param>
        public void ExceptWith(IEnumerable<T> other);

        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public void IntersectWith(IEnumerable<T> other);

        /// <summary>
        /// Modifies the current set so that it contains only elements that are present either in the current set
        /// or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public void SymmetricExceptWith(IEnumerable<T> other);

        /// <summary>
        /// Modifies the current set so that it contains all elements that are present in the current set, in the
        /// specified collection, or in both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public void UnionWith(IEnumerable<T> other);
    }
}
