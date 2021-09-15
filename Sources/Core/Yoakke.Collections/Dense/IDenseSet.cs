// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Dense
{
    /// <summary>
    /// A set interface that stores the contained elements as intervals.
    /// </summary>
    /// <typeparam name="T">The set element type.</typeparam>
    public interface IDenseSet<T> : IReadOnlyDenseSet<T>, IMathSet<T>
    {
        /// <summary>
        /// Adds an interval of elements to the current set and returns a value to indicate if there was any value newly added.
        /// </summary>
        /// <param name="interval">The interval of elements to add to the set.</param>
        /// <returns>True if the there were any elements added to the set.</returns>
        public bool Add(Interval<T> interval);

        /// <summary>
        /// Removes an interval of elements from the set.
        /// </summary>
        /// <param name="interval">The interval to remove.</param>
        /// <returns>True, if there were any elements removed, false otherwise.</returns>
        public bool Remove(Interval<T> interval);

        /// <summary>
        /// Removes all intervals of elements in the specified collection from the current set.
        /// </summary>
        /// <param name="other">The collection of intervals to remove from the set.</param>
        public void ExceptWith(IEnumerable<Interval<T>> other);

        /// <summary>
        /// Modifies the current set so that it contains only elements that are also in a specified collection.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public void IntersectWith(IEnumerable<Interval<T>> other);

        /// <summary>
        /// Modifies the current set so that it contains only elements that are present either in the current set
        /// or in the specified collection, but not both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public void SymmetricExceptWith(IEnumerable<Interval<T>> other);

        /// <summary>
        /// Modifies the current set so that it contains all elements that are present in the current set, in the
        /// specified collection, or in both.
        /// </summary>
        /// <param name="other">The collection to compare to the current set.</param>
        public void UnionWith(IEnumerable<Interval<T>> other);
    }
}
