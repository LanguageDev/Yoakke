// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Dense
{
    /// <summary>
    /// A default <see cref="IDenseSet{T}"/> implementation backed by a list of intervals.
    /// </summary>
    /// <typeparam name="T">The set element and interval endpoint type.</typeparam>
    public class DenseSet<T> : IDenseSet<T>
    {
        // NOTE: We can implement these later
        // For that we need some domain information with some IDomain<T> interface

        /// <inheritdoc/>
        public int? Count => null;

        // NOTE: See above note

        /// <inheritdoc/>
        public IEnumerable<T>? Values => null;

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
            : this(new IntervalComparer<T>(comparer))
        {
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
        public void Clear() => this.intervals.Clear();

        /// <inheritdoc/>
        public bool Add(T item) => this.Add(Interval<T>.Singleton(item));

        /// <inheritdoc/>
        public bool Add(Interval<T> iterval) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Remove(T item) => this.Remove(Interval<T>.Singleton(item));

        /// <inheritdoc/>
        public bool Remove(Interval<T> iterval) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Contains(T item) => this.Contains(Interval<T>.Singleton(item));

        /// <inheritdoc/>
        public bool Contains(Interval<T> interval) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Overlaps(Interval<T> iterval) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsProperSubsetOf(IEnumerable<T> other) => this.IsProperSubsetOf(other.Select(i => Interval<T>.Singleton(i)));

        /// <inheritdoc/>
        public bool IsProperSubsetOf(IEnumerable<Interval<T>> other) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsProperSupersetOf(IEnumerable<T> other) => this.IsProperSupersetOf(other.Select(i => Interval<T>.Singleton(i)));

        /// <inheritdoc/>
        public bool IsProperSupersetOf(IEnumerable<Interval<T>> other) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsSubsetOf(IEnumerable<T> other) => this.IsSubsetOf(other.Select(i => Interval<T>.Singleton(i)));

        /// <inheritdoc/>
        public bool IsSubsetOf(IEnumerable<Interval<T>> other) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsSupersetOf(IEnumerable<T> other) => this.IsSupersetOf(other.Select(i => Interval<T>.Singleton(i)));

        /// <inheritdoc/>
        public bool IsSupersetOf(IEnumerable<Interval<T>> other)
        {
            foreach (var iv in other)
            {
                if (!this.Contains(iv)) return false;
            }
            return true;
        }

        /// <inheritdoc/>
        public bool Overlaps(IEnumerable<T> other) => this.Overlaps(other.Select(i => Interval<T>.Singleton(i)));

        /// <inheritdoc/>
        public bool Overlaps(IEnumerable<Interval<T>> other)
        {
            foreach (var iv in other)
            {
                if (this.Overlaps(iv)) return true;
            }
            return false;
        }

        /// <inheritdoc/>
        public bool SetEquals(IEnumerable<T> other) => this.SetEquals(other.Select(i => Interval<T>.Singleton(i)));

        /// <inheritdoc/>
        public bool SetEquals(IEnumerable<Interval<T>> other) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void ExceptWith(IEnumerable<T> other) => this.ExceptWith(other.Select(i => Interval<T>.Singleton(i)));

        /// <inheritdoc/>
        public void ExceptWith(IEnumerable<Interval<T>> other)
        {
            foreach (var iv in other) this.Remove(iv);
        }

        /// <inheritdoc/>
        public void IntersectWith(IEnumerable<T> other) => this.IntersectWith(other.Select(i => Interval<T>.Singleton(i)));

        /// <inheritdoc/>
        public void IntersectWith(IEnumerable<Interval<T>> other) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void SymmetricExceptWith(IEnumerable<T> other) => this.SymmetricExceptWith(other.Select(i => Interval<T>.Singleton(i)));

        /// <inheritdoc/>
        public void SymmetricExceptWith(IEnumerable<Interval<T>> other) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void UnionWith(IEnumerable<T> other) => this.UnionWith(other.Select(i => Interval<T>.Singleton(i)));

        /// <inheritdoc/>
        public void UnionWith(IEnumerable<Interval<T>> other)
        {
            foreach (var iv in other) this.Add(iv);
        }

        /// <inheritdoc/>
        public IEnumerator<Interval<T>> GetEnumerator() => this.intervals.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => (this.intervals as IEnumerable).GetEnumerator();
    }
}
