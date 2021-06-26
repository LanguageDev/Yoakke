// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.Utilities.Intervals
{
    /// <summary>
    /// Represents a generic collection of disjunct intervals.
    /// </summary>
    /// <typeparam name="T">The interval value type.</typeparam>
    public interface IIntervalSet<T> : IReadOnlyCollection<Interval<T>>
    {
        /// <summary>
        /// Clears this interval set.
        /// </summary>
        public void Clear();

        /// <summary>
        /// Checks if the given value is covered by the intervals.
        /// </summary>
        /// <param name="value">The value to locate.</param>
        /// <returns>True, if an interval covers this value, false otherwise.</returns>
        public bool Contains(T value);

        /// <summary>
        /// Adds an interval to the set.
        /// </summary>
        /// <param name="interval">The interval to add.</param>
        public void Add(Interval<T> interval);

        /// <summary>
        /// Inverts this interval set, meaning that it will cover every value it didn't before,
        /// and it won't cover any value covered before.
        /// </summary>
        public void Invert();
    }
}
