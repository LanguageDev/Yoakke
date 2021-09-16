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
        /// Gets the value that is associated with the specified interval of keys.
        /// </summary>
        /// <param name="keys">The keys to locate.</param>
        /// <param name="value">When this method returns, the value associated with the specified keys, if the keys are found,
        /// otherwise, the default value for the type of the value parameter. This parameter is passed uninitialized.</param>
        /// <returns>True if the mapping contains the elements that has the specified keys, otherwise false.</returns>
        public bool TryGetValue(Interval<TKey> keys, out TValue value);
    }
}
