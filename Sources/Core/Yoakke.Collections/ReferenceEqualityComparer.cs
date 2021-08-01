// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections
{
    /// <summary>
    /// A generic implementation of <see cref="ReferenceEqualityComparer"/>.
    /// </summary>
    /// <typeparam name="T">The types to compare by reference.</typeparam>
    public class ReferenceEqualityComparer<T> : IEqualityComparer<T>
    {
        /// <summary>
        /// A default instance to use.
        /// </summary>
        public static readonly ReferenceEqualityComparer<T> Default = new();

        /// <inheritdoc/>
        public bool Equals(T? x, T? y) => ReferenceEqualityComparer.Instance.Equals(x, y);

        /// <inheritdoc/>
        public int GetHashCode([DisallowNull] T obj) => ReferenceEqualityComparer.Instance.GetHashCode(obj);
    }
}
