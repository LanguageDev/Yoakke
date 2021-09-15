// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections
{
    /// <summary>
    /// A read-only set interface that allows for an infinite amount of elements and does not require element count or iterability.
    /// </summary>
    /// <typeparam name="T">The set element type.</typeparam>
    public interface IReadOnlyMathSet<T>
    {
        /// <summary>
        /// True, if the set contains no elements.
        /// </summary>
        public bool IsEmpty { get; }

        /// <summary>
        /// Gets the number of elements in the set, if it is finite.
        /// It is null, if the set contains infinite elements.
        /// </summary>
        public int? Count { get; }

        /// <summary>
        /// Gets the elements in the set, if the elements are iterable.
        /// It is null, if the elements are not iterable.
        /// </summary>
        public IEnumerable<T>? Values { get; }

        /// <summary>
        /// Determines if the set contains a specific item.
        /// </summary>
        /// <param name="item">The item to check if the set contains.</param>
        /// <returns>True if found, otherwise false.</returns>
        public bool Contains(T item);

        /// <summary>
        /// Determines whether the current set is a proper (strict) subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>True if the current set is a proper subset of other, otherwise false.</returns>
        public bool IsProperSubsetOf(IEnumerable<T> other);

        /// <summary>
        /// Determines whether the current set is a proper (strict) superset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>True if the collection is a proper superset of other, otherwise false.</returns>
        public bool IsProperSupersetOf(IEnumerable<T> other);

        /// <summary>
        /// Determine whether the current set is a subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>True if the current set is a subset of other, otherwise false.</returns>
        public bool IsSubsetOf(IEnumerable<T> other);

        /// <summary>
        /// Determine whether the current set is a super set of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>True if the current set is a subset of other, otherwise false.</returns>
        public bool IsSupersetOf(IEnumerable<T> other);

        /// <summary>
        /// Determine whether the current set is a subset of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <param name="proper">True gets written here, if the subset relation is a proper subset relation,
        /// otherwise false.</param>
        /// <returns>True if the current set is a subset of other, otherwise false.</returns>
        public bool IsSubsetOf(IEnumerable<T> other, out bool proper);

        /// <summary>
        /// Determine whether the current set is a super set of a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <param name="proper">True gets written here, if the superset relation is a proper superset relation,
        /// otherwise false.</param>
        /// <returns>True if the current set is a subset of other, otherwise false.</returns>
        public bool IsSupersetOf(IEnumerable<T> other, out bool proper);

        /// <summary>
        /// Determines whether the current set overlaps with the specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>True, if the current set and other share at least one common element, otherwise false.</returns>
        public bool Overlaps(IEnumerable<T> other);

        /// <summary>
        /// Determines whether the current set and the specified collection contain the same elements.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        /// <returns>True if the current set is equal to other, otherwise false.</returns>
        public bool SetEquals(IEnumerable<T> other);
    }
}
