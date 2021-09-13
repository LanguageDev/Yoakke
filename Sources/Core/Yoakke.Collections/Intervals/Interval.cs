// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// Represents a finite or infinite interval.
    /// </summary>
    /// <typeparam name="T">The type of the endpoint value.</typeparam>
    public class Interval<T> : IEquatable<Interval<T>>
    {
        /// <summary>
        /// A full interval.
        /// </summary>
        public static readonly Interval<T> Full = new(LowerBound<T>.Unbounded.Instance, UpperBound<T>.Unbounded.Instance);

        /// <summary>
        /// An empty interval.
        /// </summary>
        public static readonly Interval<T> Empty = new(new LowerBound<T>.Exclusive(default!), new UpperBound<T>.Exclusive(default!));

        /// <summary>
        /// The lower-bound of the interval.
        /// </summary>
        public LowerBound<T> Lower { get; init; }

        /// <summary>
        /// The upper-bound of the interval.
        /// </summary>
        public UpperBound<T> Upper { get; init; }

        /// <summary>
        /// True, if this interval is empty.
        /// </summary>
        public bool IsEmpty => IntervalComparer<T>.Default.IsEmpty(this);

        /// <summary>
        /// Initializes a new instance of the <see cref="Interval{T}"/> class.
        /// </summary>
        /// <param name="lower">The lower-bound of the interval.</param>
        /// <param name="upper">The upper-bound of the interval.</param>
        public Interval(LowerBound<T> lower, UpperBound<T> upper)
        {
            this.Lower = lower;
            this.Upper = upper;
        }

        /// <summary>
        /// Constructs an interval that contains a single element.
        /// </summary>
        /// <param name="value">The element that is contained by the interval.</param>
        /// <returns>A new interval, that only contains <paramref name="value"/>.</returns>
        public static Interval<T> Singleton(T value) =>
            new(new LowerBound<T>.Inclusive(value), new UpperBound<T>.Inclusive(value));

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Interval<T> other && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(Interval<T> other) => IntervalComparer<T>.Default.Equals(this, other);

        /// <inheritdoc/>
        public override int GetHashCode() => IntervalComparer<T>.Default.GetHashCode(this);

        /// <inheritdoc/>
        public override string ToString()
        {
            var lower = this.Lower switch
            {
                LowerBound<T>.Unbounded => "(-∞",
                LowerBound<T>.Exclusive e => $"({e.Value}",
                LowerBound<T>.Inclusive i => $"[{i.Value}",
                _ => throw new ArgumentOutOfRangeException(),
            };
            var upper = this.Upper switch
            {
                LowerBound<T>.Unbounded => "+∞)",
                LowerBound<T>.Exclusive e => $"{e.Value})",
                LowerBound<T>.Inclusive i => $"{i.Value}]",
                _ => throw new ArgumentOutOfRangeException(),
            };
            return $"{lower}; {upper}"
        }

        /// <summary>
        /// Checks if a value is inside an interval.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True, if this interval contains <paramref name="value"/>.</returns>
        public bool Contains(T value) => IntervalComparer<T>.Default.Contains(this, value);

        /// <summary>
        /// Checks if this interval is completely before another one, without overlapping.
        /// </summary>
        /// <param name="other">The interval to check.</param>
        /// <returns>True, if this is completely before <paramref name="other"/>.</returns>
        public bool IsBefore(Interval<T> other) => IntervalComparer<T>.Default.IsBefore(this, other);

        /// <summary>
        /// Checks if an interval is disjunct with this one.
        /// </summary>
        /// <param name="other">The other interval to check.</param>
        /// <returns>True, if this and <paramref name="other"/> are completely disjunct.</returns>
        public bool IsDisjunct(Interval<T> other) => IntervalComparer<T>.Default.IsDisjunct(this, other);

        /// <summary>
        /// Compares two <see cref="Interval{T}"/>s for equality.
        /// </summary>
        /// <param name="a">The first <see cref="Interval{T}"/> to compare.</param>
        /// <param name="b">The second <see cref="Interval{T}"/> to compare.</param>
        /// <returns>True, if <paramref name="a"/> and <paramref name="b"/> are equal.</returns>
        public static bool operator ==(Interval<T> a, Interval<T> b) => a.Equals(b);

        /// <summary>
        /// Compares two <see cref="Interval{T}"/>s for inequality.
        /// </summary>
        /// <param name="a">The first <see cref="Interval{T}"/> to compare.</param>
        /// <param name="b">The second <see cref="Interval{T}"/> to compare.</param>
        /// <returns>True, if <paramref name="a"/> and <paramref name="b"/> are not equal.</returns>
        public static bool operator !=(Interval<T> a, Interval<T> b) => !a.Equals(b);
    }
}
