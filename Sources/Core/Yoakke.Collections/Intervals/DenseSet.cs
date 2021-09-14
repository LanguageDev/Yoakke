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
        public int Count => this.intervals.Count;

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
        public bool Contains(Interval<T> item)
        {
            if (this.Comparer.IsEmpty(item)) return true;
            this.BinarySearch(0, this.intervals.Count, item.Lower, iv => iv.Upper, out var from);
            if (from == this.intervals.Count) return false;
            return this.Comparer.Contains(this.intervals[from], item);
        }

        /// <inheritdoc/>
        public void Clear() => this.intervals.Clear();

        /// <inheritdoc/>
        public void Add(T item) => this.Add(Interval<T>.Singleton(item));

        /// <inheritdoc/>
        public void Add(Interval<T> item)
        {
            // If there are no items, it's trivial
            if (this.intervals.Count == 0)
            {
                this.intervals.Add(item);
                return;
            }

            // Not empty, find all the intervals that are touched
            var (from, to) = this.TouchingRange(item);

            if (from == to)
            {
                // Just insert, touches nothing
                this.intervals.Insert(from, item);
            }
            else
            {
                // We need to remove all touched intervals
                // First we need to modify the inserted interval, because the touched intervals might extend beyond
                // the inserted one
                item = new(
                    this.Comparer.BoundComparer.Min(this.intervals[from].Lower, item.Lower),
                    this.Comparer.BoundComparer.Max(this.intervals[to - 1].Upper, item.Upper));
                // Remove all touched ranges, except the first one
                this.intervals.RemoveRange(from + 1, to - from - 1);
                // We modify the first, not removed touched range to save on memory juggling a bit
                this.intervals[from] = item;
            }
        }

        /// <inheritdoc/>
        public bool Remove(T item) => this.Remove(Interval<T>.Singleton(item));

        /// <inheritdoc/>
        public bool Remove(Interval<T> item) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Invert() => throw new NotImplementedException();

        /// <inheritdoc/>
        public void CopyTo(Interval<T>[] array, int arrayIndex) => this.intervals.CopyTo(array, arrayIndex);

        /// <inheritdoc/>
        public IEnumerator<Interval<T>> GetEnumerator() => this.intervals.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => (this.intervals as IEnumerable).GetEnumerator();

        private (int From, int To) TouchingRange(Interval<T> interval)
        {
            var (from, to) = this.IntersectingRange(interval);
            if (from != 0 && this.Comparer.BoundComparer.IsTouching(interval.Lower, this.intervals[from - 1].Upper)) from -= 1;
            if (to != this.intervals.Count && this.Comparer.BoundComparer.IsTouching(interval.Upper, this.intervals[to].Lower)) to += 1;
            return (from, to);
        }

        private (int From, int To) IntersectingRange(Interval<T> interval)
        {
            this.BinarySearch(0, this.intervals.Count, interval.Lower, iv => iv.Upper, out var from);
            this.BinarySearch(from, this.intervals.Count - from, interval.Upper, iv => iv.Lower, out var to);
            return (from, to);
        }

        private bool BinarySearch(int start, int size, Bound<T> searchedKey, Func<Interval<T>, Bound<T>> keySelector, out int index)
        {
            if (size == 0)
            {
                index = 0;
                return false;
            }

            while (size > 1)
            {
                var half = size / 2;
                var mid = start + half;
                var key = keySelector(this.intervals[mid]);
                var cmp = this.Comparer.BoundComparer.Compare(searchedKey, key);
                start = cmp > 0 ? mid : start;
                size -= half;
            }

            var resultKey = keySelector(this.intervals[start]);
            var resultCmp = this.Comparer.BoundComparer.Compare(searchedKey, resultKey);
            if (resultCmp == 0)
            {
                index = start;
                return true;
            }
            else
            {
                index = start + (resultCmp > 0 ? 1 : 0);
                return false;
            }
        }
    }
}
