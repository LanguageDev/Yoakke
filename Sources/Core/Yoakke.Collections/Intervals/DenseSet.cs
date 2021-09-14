// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// A default implementation of <see cref="IDenseSet{T}"/>.
    /// </summary>
    /// <typeparam name="T">The set element type.</typeparam>
    public class DenseSet<T> : IDenseSet<T>
    {
        /// <inheritdoc/>
        public int Count => intervals.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <summary>
        /// The comparer used.
        /// </summary>
        public IntervalComparer<T> Comparer { get; }

        private readonly List<Interval<T>> intervals = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseSet{T}"/> class.
        /// </summary>
        public DenseSet()
            : this(IntervalComparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseSet{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use.</param>
        public DenseSet(IntervalComparer<T> comparer)
        {
            this.Comparer = comparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseSet{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use.</param>
        public DenseSet(BoundComparer<T> comparer)
        {
            this.Comparer = new(comparer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseSet{T}"/> class.
        /// </summary>
        /// <param name="equalityComparer">The equality comparer to use.</param>
        /// <param name="comparer">The comparer to use.</param>
        public DenseSet(IEqualityComparer<T> equalityComparer, IComparer<T> comparer)
            : this(new BoundComparer<T>(equalityComparer, comparer))
        {
        }

        /// <inheritdoc/>
        public bool Contains(T item) => this.Contains(Interval<T>.Singleton(item));

        /// <inheritdoc/>
        public bool Contains(Interval<T> item) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Clear() => this.intervals.Clear();

        /// <inheritdoc/>
        public void Add(T item) => this.Add(Interval<T>.Singleton(item));

        /// <inheritdoc/>
        public void Add(Interval<T> item) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Remove(T item) => this.Remove(Interval<T>.Singleton(item));

        /// <inheritdoc/>
        public bool Remove(Interval<T> item) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void CopyTo(Interval<T>[] array, int arrayIndex) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Invert() => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEnumerator<Interval<T>> GetEnumerator() => this.intervals.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => (this.intervals as IEnumerable).GetEnumerator();
    }
}
