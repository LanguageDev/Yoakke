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
    /// A read-only set interface that stores the contained elements as intervals.
    /// </summary>
    /// <typeparam name="T">The set element type.</typeparam>
    public interface IReadOnlyDenseSet<T> : IReadOnlyMathSet<T>, IEnumerable<Interval<T>>
    {
        /// <summary>
        /// Determines if the set contains a full interval of items.
        /// </summary>
        /// <param name="interval">The interval to check if the set contains.</param>
        /// <returns>True if all elements of <paramref name="interval"/> are contained in the set,
        /// otherwise false.</returns>
        public bool Contains(Interval<T> interval);
    }
}
