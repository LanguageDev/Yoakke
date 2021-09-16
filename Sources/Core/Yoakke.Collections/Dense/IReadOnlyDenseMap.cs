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
    /// A read-only mapping interface that stores the contained elements as intervals.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public interface IReadOnlyDenseMap<TKey, TValue>
        : IReadOnlyMathMap<TKey, TValue>,
          IEnumerable<KeyValuePair<Interval<TKey>, TValue>>
    {
        /// <summary>
        /// Determines whether the mapping contains the keys in the specified interval.
        /// </summary>
        /// <param name="keys">The keys to locate.</param>
        /// <returns>True if the mapping contains the keys in the specified interval, otherwise false.</returns>
        public bool ContainsKeys(Interval<TKey> keys);

        /// <summary>
        /// Gets the values that are associated with the specified interval of keys.
        /// </summary>
        /// <param name="keys">The keys to locate.</param>
        /// <returns>The sequence of values that are overlapped with <paramref name="keys"/>.</returns>
        public IEnumerable<TValue> GetValues(Interval<TKey> keys);
    }
}
