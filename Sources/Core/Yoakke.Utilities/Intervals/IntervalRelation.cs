// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;

namespace Yoakke.Utilities.Intervals
{
    /// <summary>
    /// Base class for describing the relation of two intervals.
    /// </summary>
    /// <typeparam name="T">The type of the interval values.</typeparam>
    public abstract class IntervalRelation<T> : IEquatable<IntervalRelation<T>>
    {
        /// <summary>
        /// Compares this <see cref="IntervalRelation{T}"/> for equality with another one,
        /// using the given primitive comparer.
        /// </summary>
        /// <param name="other">The other <see cref="IntervalRelation{T}"/> to compare.</param>
        /// <param name="comparer">The primitive comparer to use.</param>
        /// <returns>True, if this is equivalent to <paramref name="other"/>.</returns>
        public abstract bool Equals(IntervalRelation<T> other, IComparer<T> comparer);

        /// <inheritdoc/>
        public override abstract int GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is IntervalRelation<T> ir && this.Equals(ir);

        /// <inheritdoc/>
        public bool Equals(IntervalRelation<T> other) => this.Equals(other, Comparer<T>.Default);

        /// <summary>
        /// Compares two <see cref="IntervalRelation{T}"/>s for equality.
        /// </summary>
        /// <param name="a">The first <see cref="IntervalRelation{T}"/> to compare.</param>
        /// <param name="b">The second <see cref="IntervalRelation{T}"/> to compare.</param>
        /// <returns>True, if <paramref name="a"/> and <paramref name="b"/> are equal.</returns>
        public static bool operator ==(IntervalRelation<T> a, IntervalRelation<T> b) => a.Equals(b);

        /// <summary>
        /// Compares two <see cref="IntervalRelation{T}"/>s for inequality.
        /// </summary>
        /// <param name="a">The first <see cref="IntervalRelation{T}"/> to compare.</param>
        /// <param name="b">The second <see cref="IntervalRelation{T}"/> to compare.</param>
        /// <returns>True, if <paramref name="a"/> and <paramref name="b"/> are not equal.</returns>
        public static bool operator !=(IntervalRelation<T> a, IntervalRelation<T> b) => !a.Equals(b);

        /// <summary>
        /// Represents that the two intervals are completely disjunct.
        /// </summary>
        public class Disjunct : IntervalRelation<T>
        {
            /// <summary>
            /// The first interval of the disjunct pair.
            /// </summary>
            public Interval<T> First { get; }

            /// <summary>
            /// The second interval of the disjunct pair.
            /// </summary>
            public Interval<T> Second { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Disjunct"/> class.
            /// </summary>
            /// <param name="first">The first disjunct <see cref="Interval{T}"/>.</param>
            /// <param name="second">The second disjunct <see cref="Interval{T}"/>.</param>
            public Disjunct(Interval<T> first, Interval<T> second)
            {
                this.First = first;
                this.Second = second;
            }

            /// <inheritdoc/>
            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Disjunct d && this.First.Equals(d.First, comparer) && this.Second.Equals(d.Second, comparer);

            /// <inheritdoc/>
            public override int GetHashCode() => (this.First, this.Second).GetHashCode();
        }

        /// <summary>
        /// Represents that the two intervals are touching.
        /// </summary>
        public class Touching : IntervalRelation<T>
        {
            /// <summary>
            /// The first interval of the touching pair.
            /// </summary>
            public Interval<T> First { get; }

            /// <summary>
            /// The second interval of the touching pair.
            /// </summary>
            public Interval<T> Second { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Touching"/> class.
            /// </summary>
            /// <param name="first">The first touching <see cref="Interval{T}"/>.</param>
            /// <param name="second">The second touching <see cref="Interval{T}"/>.</param>
            public Touching(Interval<T> first, Interval<T> second)
            {
                this.First = first;
                this.Second = second;
            }

            /// <inheritdoc/>
            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Touching t && this.First.Equals(t.First, comparer) && this.Second.Equals(t.Second, comparer);

            /// <inheritdoc/>
            public override int GetHashCode() => (this.First, this.Second).GetHashCode();
        }

        /// <summary>
        /// Represents that the two intervals are overlapping.
        /// </summary>
        public class Overlapping : IntervalRelation<T>
        {
            /// <summary>
            /// The first disjunct part of the intervals (before the overlap).
            /// </summary>
            public Interval<T> FirstDisjunct { get; }

            /// <summary>
            /// The overlapping portion of the intervals.
            /// </summary>
            public Interval<T> Overlap { get; }

            /// <summary>
            /// The second disjunct part of the intervals (after the overlap).
            /// </summary>
            public Interval<T> SecondDisjunct { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Overlapping"/> class.
            /// </summary>
            /// <param name="firstDisjunct">The first disjunct <see cref="Interval{T}"/>.</param>
            /// <param name="overlap">The overlapping <see cref="Interval{T}"/>.</param>
            /// <param name="secondDisjunct">The second disjunct <see cref="Interval{T}"/>.</param>
            public Overlapping(Interval<T> firstDisjunct, Interval<T> overlap, Interval<T> secondDisjunct)
            {
                this.FirstDisjunct = firstDisjunct;
                this.Overlap = overlap;
                this.SecondDisjunct = secondDisjunct;
            }

            /// <inheritdoc/>
            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                   other is Overlapping o
                && this.FirstDisjunct.Equals(o.FirstDisjunct, comparer)
                && this.Overlap.Equals(o.Overlap, comparer)
                && this.SecondDisjunct.Equals(o.SecondDisjunct, comparer);

            /// <inheritdoc/>
            public override int GetHashCode() => (this.FirstDisjunct, this.Overlap, this.SecondDisjunct).GetHashCode();
        }

        /// <summary>
        /// Represents that one interval completely contains the other.
        /// </summary>
        public class Containing : IntervalRelation<T>
        {
            /// <summary>
            /// The first disjunct part of the intervals (before the containment).
            /// </summary>
            public Interval<T> FirstDisjunct { get; }

            /// <summary>
            /// The contained portion of the intervals.
            /// </summary>
            public Interval<T> Contained { get; }

            /// <summary>
            /// The second disjunct part of the intervals (after the containment).
            /// </summary>
            public Interval<T> SecondDisjunct { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Containing"/> class.
            /// </summary>
            /// <param name="firstDisjunct">The first disjunct <see cref="Interval{T}"/>.</param>
            /// <param name="contained">The contained <see cref="Interval{T}"/>.</param>
            /// <param name="secondDisjunct">The second disjunct <see cref="Interval{T}"/>.</param>
            public Containing(Interval<T> firstDisjunct, Interval<T> contained, Interval<T> secondDisjunct)
            {
                this.FirstDisjunct = firstDisjunct;
                this.Contained = contained;
                this.SecondDisjunct = secondDisjunct;
            }

            /// <inheritdoc/>
            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                   other is Containing c
                && this.FirstDisjunct.Equals(c.FirstDisjunct, comparer)
                && this.Contained.Equals(c.Contained, comparer)
                && this.SecondDisjunct.Equals(c.SecondDisjunct, comparer);

            /// <inheritdoc/>
            public override int GetHashCode() => (this.FirstDisjunct, this.Contained, this.SecondDisjunct).GetHashCode();
        }

        /// <summary>
        /// Represents that one interval starts with the other.
        /// </summary>
        public class Starting : IntervalRelation<T>
        {
            /// <summary>
            /// The overlapping portion of the intervals (start).
            /// </summary>
            public Interval<T> Overlap { get; }

            /// <summary>
            /// The disjunct part of the intervals (after the overlap).
            /// </summary>
            public new Interval<T> Disjunct { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Starting"/> class.
            /// </summary>
            /// <param name="overlap">The overlapping <see cref="Interval{T}"/>.</param>
            /// <param name="disjunct">The disjunct <see cref="Interval{T}"/>.</param>
            public Starting(Interval<T> overlap, Interval<T> disjunct)
            {
                this.Overlap = overlap;
                this.Disjunct = disjunct;
            }

            /// <inheritdoc/>
            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Starting s && this.Overlap.Equals(s.Overlap, comparer) && this.Disjunct.Equals(s.Disjunct, comparer);

            /// <inheritdoc/>
            public override int GetHashCode() => (this.Overlap, this.Disjunct).GetHashCode();
        }

        /// <summary>
        /// Represents that one interval finishes with the other.
        /// </summary>
        public class Finishing : IntervalRelation<T>
        {
            /// <summary>
            /// The disjunct part of the intervals (after the before).
            /// </summary>
            public new Interval<T> Disjunct { get; }

            /// <summary>
            /// The overlapping portion of the intervals (end).
            /// </summary>
            public Interval<T> Overlap { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Finishing"/> class.
            /// </summary>
            /// <param name="disjunct">The disjunct <see cref="Interval{T}"/>.</param>
            /// <param name="overlap">The overlapping <see cref="Interval{T}"/>.</param>
            public Finishing(Interval<T> disjunct, Interval<T> overlap)
            {
                this.Disjunct = disjunct;
                this.Overlap = overlap;
            }

            /// <inheritdoc/>
            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Finishing f && this.Overlap.Equals(f.Overlap, comparer) && this.Disjunct.Equals(f.Disjunct, comparer);

            /// <inheritdoc/>
            public override int GetHashCode() => (this.Disjunct, this.Overlap).GetHashCode();
        }

        /// <summary>
        /// Represents that the two intervals are completely equal.
        /// </summary>
        public class Equal : IntervalRelation<T>
        {
            /// <summary>
            /// The equal intervals.
            /// </summary>
            public Interval<T> Interval { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Equal"/> class.
            /// </summary>
            /// <param name="interval">The equaling <see cref="Interval{T}"/>.</param>
            public Equal(Interval<T> interval)
            {
                this.Interval = interval;
            }

            /// <inheritdoc/>
            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Equal e && this.Interval.Equals(e.Interval, comparer);

            /// <inheritdoc/>
            public override int GetHashCode() => this.Interval.GetHashCode();
        }
    }
}
