// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;

namespace Yoakke.Collections.Values;

/// <summary>
/// Represents a generic associative container of keys and values that implements value-based equality.
/// </summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
public interface IReadOnlyValueDictionary<TKey, TValue>
    : IReadOnlyDictionary<TKey, TValue>,
      IEquatable<IReadOnlyDictionary<TKey, TValue>>,
      IEquatable<IReadOnlyValueDictionary<TKey, TValue>>
{
}
