// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// A read-only set represented as a union of disjunct intervals.
    /// </summary>
    /// <typeparam name="T">The set element type.</typeparam>
    public interface IReadOnlyDenseSet<T> : IReadOnlyCollection<Interval<T>>
    {
        /// <summary>
        /// Checks if the given value is covered by the intervals.
        /// </summary>
        /// <param name="item">The value to locate.</param>
        /// <returns>True, if this set contains <paramref name="item"/>.</returns>
        public bool Contains(T item);
    }
}
