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
    /// A wrapper around <see cref="IReadOnlyDictionary{TKey, TValue}"/>s to make them have value-based equality.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    public class ReadOnlyValueDictionary<TKey, TValue> : IReadOnlyValueDictionary<TKey, TValue>
        where TKey : IEquatable<TKey>
        where TValue : IEquatable<TValue>
    {
        /// <inheritdoc/>
        public TValue this[TKey key] => this.Underlying[key];

        /// <inheritdoc/>
        public IEnumerable<TKey> Keys => this.Underlying.Keys;

        /// <inheritdoc/>
        public IEnumerable<TValue> Values => this.Underlying.Values;

        /// <inheritdoc/>
        public int Count => this.Underlying.Count;

        /// <summary>
        /// The underlying wrapped <see cref="IReadOnlyDictionary{TKey, TValue}"/>.
        /// </summary>
        public IReadOnlyDictionary<TKey, TValue> Underlying { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyValueDictionary{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="underlying">The underlying <see cref="IReadOnlyDictionary{TKey, TValue}"/> to wrap.</param>
        public ReadOnlyValueDictionary(IReadOnlyDictionary<TKey, TValue> underlying)
        {
            this.Underlying = underlying;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => this.Equals(obj as IReadOnlyDictionary<TKey, TValue>);

        /// <inheritdoc/>
        public bool Equals(IReadOnlyDictionary<TKey, TValue>? other)
        {
            if (other is null || this.Count != other.Count) return false;

            foreach (var (key, value) in this.Underlying)
            {
                if (!other.TryGetValue(key, out var otherValue)) return false;
                if (!value.Equals(otherValue)) return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public bool Equals(IReadOnlyValueDictionary<TKey, TValue>? other) => this.Equals(other as IReadOnlyDictionary<TKey, TValue>);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // NOTE: We simply use XOR as that is order-independent
            var hashCode = 0;
            foreach (var element in this.Underlying)
            {
                hashCode ^= element.GetHashCode();
            }
            return hashCode;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{{ {string.Join(", ", this.Underlying.Select(kv => $"{kv.Key} => {kv.Value}"))} }}";

        /// <inheritdoc/>
        public bool ContainsKey(TKey key) => this.Underlying.ContainsKey(key);

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TValue value) => this.Underlying.TryGetValue(key, out value);

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => this.Underlying.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => (this.Underlying as IEnumerable).GetEnumerator();
    }
}
