// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// An equality comparer that can compare <see cref="Interval{T}"/>s.
    /// </summary>
    /// <typeparam name="T">The type of the endpoint value.</typeparam>
    public class IntervalComparer<T> : IEqualityComparer<Interval<T>>
    {
        /// <summary>
        /// The default instance of the comparer.
        /// </summary>
        public static readonly IntervalComparer<T> Default = new(BoundComparer<T>.Default);

        private readonly BoundComparer<T> boundComparer;

        private IComparer<T> Comparer => this.boundComparer.Comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalComparer{T}"/> class.
        /// </summary>
        /// <param name="boundComparer">The <see cref="BoundComparer{T}"/> to use.</param>
        public IntervalComparer(BoundComparer<T> boundComparer)
        {
            this.boundComparer = boundComparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntervalComparer{T}"/> class.
        /// </summary>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> to use.</param>
        public IntervalComparer(IEqualityComparer<T> equalityComparer, IComparer<T> comparer)
            : this(new BoundComparer<T>(equalityComparer, comparer))
        {
        }

        /// <inheritdoc/>
        public bool Equals(Interval<T> x, Interval<T> y) => this.boundComparer.Equals(x.Lower, y.Lower)
                                                         && this.boundComparer.Equals(x.Upper, y.Upper);

        /// <inheritdoc/>
        public int GetHashCode(Interval<T> obj)
        {
            var h = default(HashCode);
            h.Add(obj.Lower, this.boundComparer);
            h.Add(obj.Upper, this.boundComparer);
            return h.ToHashCode();
        }

        /// <summary>
        /// Checks if a value is inside an interval.
        /// </summary>
        /// <param name="interval">The interval to check.</param>
        /// <param name="value">The value to check.</param>
        /// <returns>True, if <paramref name="interval"/> contains <paramref name="value"/>.</returns>
        public bool Contains(Interval<T> interval, T value) => (interval.Lower, interval.Upper) switch
        {
            (LowerBound<T>.Unbounded, UpperBound<T>.Unbounded) => true,

            (LowerBound<T>.Unbounded, UpperBound<T>.Exclusive r) => this.Comparer.Compare(value, r.Value) < 0,
            (LowerBound<T>.Unbounded, UpperBound<T>.Inclusive r) => this.Comparer.Compare(value, r.Value) <= 0,

            (LowerBound<T>.Exclusive l, UpperBound<T>.Unbounded) => this.Comparer.Compare(l.Value, value) < 0,
            (LowerBound<T>.Inclusive l, UpperBound<T>.Unbounded) => this.Comparer.Compare(l.Value, value) <= 0,

            (LowerBound<T>.Exclusive l, UpperBound<T>.Exclusive r) => this.Comparer.Compare(l.Value, value) < 0 && this.Comparer.Compare(value, r.Value) < 0,
            (LowerBound<T>.Inclusive l, UpperBound<T>.Inclusive r) => this.Comparer.Compare(l.Value, value) <= 0 && this.Comparer.Compare(value, r.Value) <= 0,

            (LowerBound<T>.Inclusive l, UpperBound<T>.Exclusive r) => this.Comparer.Compare(l.Value, value) <= 0 && this.Comparer.Compare(value, r.Value) < 0,
            (LowerBound<T>.Exclusive l, UpperBound<T>.Inclusive r) => this.Comparer.Compare(l.Value, value) < 0 && this.Comparer.Compare(value, r.Value) <= 0,

            _ => throw new ArgumentOutOfRangeException(),
        };

        /// <summary>
        /// Checks if an interval is empty.
        /// </summary>
        /// <param name="interval">The interval to check.</param>
        /// <returns>True, if <paramref name="interval"/> is empty.</returns>
        public bool IsEmpty(Interval<T> interval) => (interval.Lower, interval.Upper) switch
        {
            (LowerBound<T>.Inclusive l, UpperBound<T>.Exclusive r) => this.Comparer.Compare(l.Value, r.Value) >= 0,
            (LowerBound<T>.Exclusive l, UpperBound<T>.Inclusive r) => this.Comparer.Compare(l.Value, r.Value) >= 0,
            (LowerBound<T>.Exclusive l, UpperBound<T>.Exclusive r) => this.Comparer.Compare(l.Value, r.Value) >= 0,
            _ => false,
        };

        /// <summary>
        /// Checks if an interval is completely before another one, without overlapping.
        /// </summary>
        /// <param name="x">The first interval to check.</param>
        /// <param name="y">The second interval to check.</param>
        /// <returns>True, if <paramref name="x"/> is completely before <paramref name="y"/>.</returns>
        public bool IsBefore(Interval<T> x, Interval<T> y) => this.boundComparer.Compare(x.Upper, y.Lower) < 0;

        /// <summary>
        /// Checks if an interval is disjunct with another one.
        /// </summary>
        /// <param name="x">The first interval to check.</param>
        /// <param name="y">The second interval to check.</param>
        /// <returns>True, if <paramref name="x"/> and <paramref name="y"/> are completely disjunct.</returns>
        public bool IsDisjunct(Interval<T> x, Interval<T> y) => this.IsBefore(x, y) || this.IsBefore(y, x);
    }
}
