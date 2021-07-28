// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections
{
    /// <summary>
    /// A read-only wrapper for <see cref="IDictionary{TKey, TValue}"/>s to have covariance.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TReadValue">The covariant read value for <typeparamref name="TValue"/>.</typeparam>
    public class ReadOnlyDictionary<TKey, TValue, TReadValue> : IReadOnlyDictionary<TKey, TReadValue>
        where TValue : TReadValue
    {
        private readonly IDictionary<TKey, TValue> underlying;

        /// <inheritdoc/>
        public int Count => this.underlying.Count;

        /// <inheritdoc/>
        public IEnumerable<TKey> Keys => this.underlying.Keys;

        /// <inheritdoc/>
        public IEnumerable<TReadValue> Values => this.underlying.Values.Cast<TReadValue>();

        /// <inheritdoc/>
        public TReadValue this[TKey key] => this.underlying[key];

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyDictionary{TKey, TValue, TReadValue}"/> class.
        /// </summary>
        /// <param name="underlying">The <see cref="IDictionary{TKey, TValue}"/> to wrap.</param>
        public ReadOnlyDictionary(IDictionary<TKey, TValue> underlying)
        {
            this.underlying = underlying;
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key) => this.underlying.ContainsKey(key);

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TReadValue value)
        {
            if (this.underlying.TryGetValue(key, out var wvalue))
            {
                value = wvalue;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TReadValue>> GetEnumerator() => this.underlying
            .Select(kv => new KeyValuePair<TKey, TReadValue>(kv.Key, kv.Value))
            .GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => (this.underlying as IEnumerable).GetEnumerator();
    }
}
