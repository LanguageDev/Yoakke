// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// A set represented as a union of disjunct intervals.
    /// </summary>
    /// <typeparam name="T">The set element type.</typeparam>
    public interface IDenseSet<T> : IReadOnlyDenseSet<T>, ICollection<Interval<T>>
    {
        /// <summary>
        /// Adds an element to the set.
        /// </summary>
        /// <param name="item">The <paramref name="item"/> to add.</param>
        public void Add(T item);

        /// <summary>
        /// Removes an element from the set.
        /// </summary>
        /// <param name="item">The <paramref name="item"/> to remove.</param>
        /// <returns>True, if there was an element to remove.</returns>
        public bool Remove(T item);

        /// <summary>
        /// Inverts this set, meaning it will cover all values, that it does not currently.
        /// </summary>
        public void Invert();
    }
}
