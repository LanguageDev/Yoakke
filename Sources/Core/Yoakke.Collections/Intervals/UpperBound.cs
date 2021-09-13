// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// Represents the upper-bound of an interval.
    /// </summary>
    /// <typeparam name="T">The type of the endpoint value.</typeparam>
    public abstract record UpperBound<T> : IComparable<LowerBound<T>>, IComparable<UpperBound<T>>
    {
        /// <inheritdoc/>
        public int CompareTo(LowerBound<T> other) => BoundComparer<T>.Default.Compare(this, other);

        /// <inheritdoc/>
        public int CompareTo(UpperBound<T> other) => BoundComparer<T>.Default.Compare(this, other);

        /// <summary>
        /// Unbounded endpoint.
        /// </summary>
        public record Unbounded : UpperBound<T>
        {
            /// <summary>
            /// A singleton instance to use.
            /// </summary>
            public static readonly Unbounded Instance = new();
        }

        /// <summary>
        /// Exclusive endpoint.
        /// </summary>
        public record Exclusive(T Value) : UpperBound<T>;

        /// <summary>
        /// Inclusive endpoint.
        /// </summary>
        public record Inclusive(T Value) : UpperBound<T>;
    }
}
