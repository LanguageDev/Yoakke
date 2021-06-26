// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Yoakke.Utilities.Compatibility;

namespace Yoakke.Utilities.Intervals
{
    /// <summary>
    /// Base class for describing the relation of two intervals.
    /// </summary>
    /// <typeparam name="T">The type of the interval values.</typeparam>
    public abstract class IntervalRelation<T> : IEquatable<IntervalRelation<T>>
    {
        public abstract bool Equals(IntervalRelation<T> other, IComparer<T> comparer);

        public override abstract int GetHashCode();

        public override bool Equals(object obj) => obj is IntervalRelation<T> ir && this.Equals(ir);

        public bool Equals(IntervalRelation<T> other) => this.Equals(other, Comparer<T>.Default);

        public static bool operator ==(IntervalRelation<T> a, IntervalRelation<T> b) => a.Equals(b);

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

            public Disjunct(Interval<T> first, Interval<T> second)
            {
                this.First = first;
                this.Second = second;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Disjunct d && this.First.Equals(d.First, comparer) && this.Second.Equals(d.Second, comparer);

            public override int GetHashCode() => HashCode.Combine(this.First, this.Second);
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

            public Touching(Interval<T> first, Interval<T> second)
            {
                this.First = first;
                this.Second = second;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Touching t && this.First.Equals(t.First, comparer) && this.Second.Equals(t.Second, comparer);

            public override int GetHashCode() => HashCode.Combine(this.First, this.Second);
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

            public Overlapping(Interval<T> firstDisjunct, Interval<T> overlap, Interval<T> secondDisjunct)
            {
                this.FirstDisjunct = firstDisjunct;
                this.Overlap = overlap;
                this.SecondDisjunct = secondDisjunct;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                   other is Overlapping o
                && this.FirstDisjunct.Equals(o.FirstDisjunct, comparer)
                && this.Overlap.Equals(o.Overlap, comparer)
                && this.SecondDisjunct.Equals(o.SecondDisjunct, comparer);

            public override int GetHashCode() => HashCode.Combine(this.FirstDisjunct, this.Overlap, this.SecondDisjunct);
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

            public Containing(Interval<T> firstDisjunct, Interval<T> contained, Interval<T> secondDisjunct)
            {
                this.FirstDisjunct = firstDisjunct;
                this.Contained = contained;
                this.SecondDisjunct = secondDisjunct;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                   other is Containing c
                && this.FirstDisjunct.Equals(c.FirstDisjunct, comparer)
                && this.Contained.Equals(c.Contained, comparer)
                && this.SecondDisjunct.Equals(c.SecondDisjunct, comparer);

            public override int GetHashCode() => HashCode.Combine(this.FirstDisjunct, this.Contained, this.SecondDisjunct);
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

            public Starting(Interval<T> overlap, Interval<T> disjunct)
            {
                this.Overlap = overlap;
                this.Disjunct = disjunct;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Starting s && this.Overlap.Equals(s.Overlap, comparer) && this.Disjunct.Equals(s.Disjunct, comparer);

            public override int GetHashCode() => HashCode.Combine(this.Overlap, this.Disjunct);
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

            public Finishing(Interval<T> disjunct, Interval<T> overlap)
            {
                this.Disjunct = disjunct;
                this.Overlap = overlap;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Finishing f && this.Overlap.Equals(f.Overlap, comparer) && this.Disjunct.Equals(f.Disjunct, comparer);

            public override int GetHashCode() => HashCode.Combine(this.Disjunct, this.Overlap);
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

            public Equal(Interval<T> interval)
            {
                this.Interval = interval;
            }

            public override bool Equals(IntervalRelation<T> other, IComparer<T> comparer) =>
                other is Equal e && this.Interval.Equals(e.Interval, comparer);

            public override int GetHashCode() => this.Interval.GetHashCode();
        }
    }
}
