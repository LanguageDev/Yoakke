// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Value.Collections
{
    /// <summary>
    /// Represents a generic associative container of keys and values that implements value-based equality.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public interface IReadOnlyValueDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, IEquatable<IReadOnlyDictionary<TKey, TValue>>
        where TKey : IEquatable<TKey>
        where TValue : IEquatable<TValue>
    {
    }
}
