// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections
{
    /// <summary>
    /// A comparer for <see cref="ISet{T}"/>s.
    /// </summary>
    /// <typeparam name="T">The set item type.</typeparam>
    public sealed class SetEqualityComparer<T> : IEqualityComparer<ISet<T>>
    {
        /// <summary>
        /// The default instance to use.
        /// </summary>
        public static SetEqualityComparer<T> Default { get; } = new(EqualityComparer<T>.Default);

        /// <summary>
        /// The item comparer.
        /// </summary>
        public IEqualityComparer<T> Comparer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SetEqualityComparer{T}"/> class.
        /// </summary>
        /// <param name="comparer">The item comparer.</param>
        public SetEqualityComparer(IEqualityComparer<T> comparer)
        {
            this.Comparer = comparer;
        }

        /// <inheritdoc/>
        public bool Equals(ISet<T> x, ISet<T> y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is HashSet<T> xh && xh.Comparer.Equals(this.Comparer)) return xh.SetEquals(y);
            if (y is HashSet<T> yh && yh.Comparer.Equals(this.Comparer)) return yh.SetEquals(x);
            return new HashSet<T>(x, this.Comparer).SetEquals(y);
        }

        /// <inheritdoc/>
        public int GetHashCode(ISet<T> obj)
        {
            // NOTE: We use xor to be symmetric
            var h = 0;
            foreach (var item in obj) h ^= this.Comparer.GetHashCode(item);
            return h;
        }
    }
}
