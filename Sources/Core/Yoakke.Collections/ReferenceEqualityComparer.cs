// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

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
        public static readonly ReferenceEqualityComparer<T> Instance = new();

        /// <inheritdoc/>
        public bool Equals(T? x, T? y) => ReferenceEqualityComparer.Instance.Equals(x, y);

        /// <inheritdoc/>
        public int GetHashCode(T obj) => ReferenceEqualityComparer.Instance.GetHashCode(obj);
    }
}
