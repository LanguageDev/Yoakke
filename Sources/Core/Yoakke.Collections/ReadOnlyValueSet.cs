// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Value.Collections
{
    /// <summary>
    /// A wrapper around <see cref="IReadOnlySet{T}"/>s to make them have value-based equality.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    public class ReadOnlyValueSet<T> : IReadOnlyValueSet<T>
        where T : IEquatable<T>
    {
        /// <inheritdoc/>
        public int Count => throw new NotImplementedException();

        /// <summary>
        /// The underlying wrapped <see cref="IReadOnlySet{T}"/>.
        /// </summary>
        public IReadOnlySet<T> Underlying { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReadOnlyValueSet{T}"/> class.
        /// </summary>
        /// <param name="underlying">The underlying <see cref="IReadOnlySet{T}"/> to wrap.</param>
        public ReadOnlyValueSet(IReadOnlySet<T> underlying)
        {
            this.Underlying = underlying;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj) => this.Equals(obj as IReadOnlySet<T>);

        /// <inheritdoc/>
        public bool Equals(IReadOnlySet<T>? other)
        {
            if (other is null || this.Count != other.Count) return false;
            return this.Underlying.SetEquals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            // NOTE: We simply use XOR as that is order-independent
            var hashCode = 0;
            foreach (var element in this.Underlying)
            {
                if (element is not null) hashCode ^= element.GetHashCode();
            }
            return hashCode;
        }

        /// <inheritdoc/>
        public bool Contains(T item) => this.Underlying.Contains(item);

        /// <inheritdoc/>
        public bool IsProperSubsetOf(IEnumerable<T> other) => this.Underlying.IsProperSubsetOf(other);

        /// <inheritdoc/>
        public bool IsProperSupersetOf(IEnumerable<T> other) => this.Underlying.IsProperSupersetOf(other);

        /// <inheritdoc/>
        public bool IsSubsetOf(IEnumerable<T> other) => this.Underlying.IsSubsetOf(other);

        /// <inheritdoc/>
        public bool IsSupersetOf(IEnumerable<T> other) => this.Underlying.IsSupersetOf(other);

        /// <inheritdoc/>
        public bool Overlaps(IEnumerable<T> other) => this.Underlying.Overlaps(other);

        /// <inheritdoc/>
        public bool SetEquals(IEnumerable<T> other) => this.Underlying.SetEquals(other);

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => this.Underlying.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => (this.Underlying as IEnumerable).GetEnumerator();
    }
}
