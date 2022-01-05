// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Dense;

/// <summary>
/// A mapping interface that stores the contained elements as intervals.
/// </summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public interface IDenseMap<TKey, TValue> : IReadOnlyDenseMap<TKey, TValue>, ICollection<KeyValuePair<Interval<TKey>, TValue>>
{
    /// <summary>
    /// Adds an element with the provided interval of keys and value to the mapping.
    /// </summary>
    /// <param name="keys">The object to use as the keys of the element to add.</param>
    /// <param name="value">The object to use as the value of the element to add.</param>
    public void Add(Interval<TKey> keys, TValue value);

    /// <summary>
    /// Removes the element with the specified interval of keys from the mapping.
    /// </summary>
    /// <param name="keys">The keys of the element to remove.</param>
    /// <returns>True if the element is successfully removed, otherwise false.
    /// This method also returns false if no keys were found in the mapping.</returns>
    public bool Remove(Interval<TKey> keys);
}
