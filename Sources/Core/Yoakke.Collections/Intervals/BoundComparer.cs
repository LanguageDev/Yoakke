// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// A comparer that can compare <see cref="LowerBound{T}"/> and <see cref="UpperBound{T}"/> instances.
    /// </summary>
    /// <typeparam name="T">The type of the endpoint value.</typeparam>
    public class BoundComparer<T> :
        IEqualityComparer<LowerBound<T>>, IEqualityComparer<UpperBound<T>>,
        IComparer<LowerBound<T>>, IComparer<UpperBound<T>>
    {
        /// <summary>
        /// The default instance of the comparer.
        /// </summary>
        public static readonly BoundComparer<T> Default = new(EqualityComparer<T>.Default, Comparer<T>.Default);

        private readonly IEqualityComparer<T> equalityComparer;
        private readonly IComparer<T> comparer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoundComparer{T}"/> class.
        /// </summary>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
        /// <param name="comparer">The <see cref="IComparer{T}"/> to use.</param>
        public BoundComparer(IEqualityComparer<T> equalityComparer, IComparer<T> comparer)
        {
            this.equalityComparer = equalityComparer;
            this.comparer = comparer;
        }

        /// <inheritdoc/>
        public bool Equals(LowerBound<T> x, LowerBound<T> y) => (x, y) switch
        {
            (LowerBound<T>.Unbounded, LowerBound<T>.Unbounded) => true,
            (LowerBound<T>.Exclusive l, LowerBound<T>.Exclusive r) => this.equalityComparer.Equals(l.Value, r.Value),
            (LowerBound<T>.Inclusive l, LowerBound<T>.Inclusive r) => this.equalityComparer.Equals(l.Value, r.Value),
            _ => false,
        };

        /// <inheritdoc/>
        public bool Equals(UpperBound<T> x, UpperBound<T> y) => (x, y) switch
        {
            (UpperBound<T>.Unbounded, UpperBound<T>.Unbounded) => true,
            (UpperBound<T>.Exclusive l, UpperBound<T>.Exclusive r) => this.equalityComparer.Equals(l.Value, r.Value),
            (UpperBound<T>.Inclusive l, UpperBound<T>.Inclusive r) => this.equalityComparer.Equals(l.Value, r.Value),
            _ => false,
        };

        /// <inheritdoc/>
        public int GetHashCode(LowerBound<T> obj) => obj switch
        {
            LowerBound<T>.Unbounded => typeof(LowerBound<T>.Unbounded).GetHashCode(),
            LowerBound<T>.Exclusive e => this.MakeHash(typeof(LowerBound<T>.Exclusive), e.Value),
            UpperBound<T>.Inclusive i => this.MakeHash(typeof(LowerBound<T>.Inclusive), i.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(obj)),
        };

        /// <inheritdoc/>
        public int GetHashCode(UpperBound<T> obj) => obj switch
        {
            UpperBound<T>.Unbounded => typeof(UpperBound<T>.Unbounded).GetHashCode(),
            UpperBound<T>.Exclusive e => this.MakeHash(typeof(UpperBound<T>.Exclusive), e.Value),
            UpperBound<T>.Inclusive i => this.MakeHash(typeof(UpperBound<T>.Inclusive), i.Value),
            _ => throw new ArgumentOutOfRangeException(nameof(obj)),
        };

        /// <inheritdoc/>
        public int Compare(LowerBound<T> x, LowerBound<T> y) => (x, y) switch
        {
            (LowerBound<T>.Unbounded, LowerBound<T>.Unbounded) => 0,
            (LowerBound<T>.Unbounded, _) => -1,
            (_, LowerBound<T>.Unbounded) => 1,
            (LowerBound<T>.Exclusive l, LowerBound<T>.Exclusive r) => this.comparer.Compare(l.Value, r.Value),
            (LowerBound<T>.Inclusive l, LowerBound<T>.Inclusive r) => this.comparer.Compare(l.Value, r.Value),
            (LowerBound<T>.Exclusive l, LowerBound<T>.Inclusive r) => this.comparer.Compare(l.Value, r.Value) switch
            {
                0 => 1,
                var n => n,
            },
            (LowerBound<T>.Inclusive l, LowerBound<T>.Exclusive r) => this.comparer.Compare(l.Value, r.Value) switch
            {
                0 => -1,
                var n => n,
            },
            _ => throw new ArgumentOutOfRangeException(),
        };

        /// <inheritdoc/>
        public int Compare(UpperBound<T> x, UpperBound<T> y) => (x, y) switch
        {
            (UpperBound<T>.Unbounded, UpperBound<T>.Unbounded) => 0,
            (UpperBound<T>.Unbounded, _) => 1,
            (_, UpperBound<T>.Unbounded) => -1,
            (UpperBound<T>.Exclusive l, UpperBound<T>.Exclusive r) => this.comparer.Compare(l.Value, r.Value),
            (UpperBound<T>.Inclusive l, UpperBound<T>.Inclusive r) => this.comparer.Compare(l.Value, r.Value),
            (UpperBound<T>.Exclusive l, UpperBound<T>.Inclusive r) => this.comparer.Compare(l.Value, r.Value) switch
            {
                0 => -1,
                var n => n,
            },
            (UpperBound<T>.Inclusive l, UpperBound<T>.Exclusive r) => this.comparer.Compare(l.Value, r.Value) switch
            {
                0 => 1,
                var n => n,
            },
            _ => throw new ArgumentOutOfRangeException(),
        };

        /// <summary>
        /// Compares a lower and upper bound with a comparer.
        /// </summary>
        /// <param name="x">The <see cref="LowerBound{T}"/> to compare.</param>
        /// <param name="y">The <see cref="UpperBound{T}"/> to compare.</param>
        /// <returns>The result of comparing <paramref name="x"/> and <paramref name="y"/>.
        /// See <see cref="IComparer{T}.Compare(T, T)"/> for further information.</returns>
        public int Compare(LowerBound<T> x, UpperBound<T> y) => (x, y) switch
        {
            (LowerBound<T>.Unbounded, _) or (_, UpperBound<T>.Unbounded) => -1,
            (LowerBound<T>.Exclusive l, LowerBound<T>.Exclusive r) => this.comparer.Compare(l.Value, r.Value) switch
            {
                0 => 1,
                var n => n,
            },
            (LowerBound<T>.Exclusive l, LowerBound<T>.Inclusive r) => this.comparer.Compare(l.Value, r.Value) switch
            {
                0 => 1,
                var n => n,
            },
            (LowerBound<T>.Inclusive l, LowerBound<T>.Exclusive r) => this.comparer.Compare(l.Value, r.Value) switch
            {
                0 => 1,
                var n => n,
            },
            (LowerBound<T>.Inclusive l, LowerBound<T>.Inclusive r) => this.comparer.Compare(l.Value, r.Value) switch
            {
                0 => -1,
                var n => n,
            },
            _ => throw new ArgumentOutOfRangeException(),
        };

        /// <summary>
        /// Compares an upper and lower bound with a comparer.
        /// </summary>
        /// <param name="x">The <see cref="UpperBound{T}"/> to compare.</param>
        /// <param name="y">The <see cref="LowerBound{T}"/> to compare.</param>
        /// <returns>The result of comparing <paramref name="x"/> and <paramref name="y"/>.
        /// See <see cref="IComparer{T}.Compare(T, T)"/> for further information.</returns>
        public int Compare(UpperBound<T> x, LowerBound<T> y) => -this.Compare(y, x);

        private int MakeHash(Type type, T value)
        {
            var h = default(HashCode);
            h.Add(type);
            h.Add(value, this.equalityComparer);
            return h.ToHashCode();
        }
    }
}
