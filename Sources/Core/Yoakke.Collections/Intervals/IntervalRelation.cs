// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// Represents the relation of two intervals.
    /// </summary>
    /// <typeparam name="T">The type of the endpoint value.</typeparam>
    public abstract record IntervalRelation<T>
    {
        /// <summary>
        /// The lower disjunct part of the relation.
        /// </summary>
        public abstract Interval<T> LowerDisjunct { get; }

        /// <summary>
        /// The intersecting part of the relation.
        /// </summary>
        public abstract Interval<T> Intersecting { get; }

        /// <summary>
        /// The upper disjunct part of the relation.
        /// </summary>
        public abstract Interval<T> UpperDisjunct { get; }

        /// <summary>
        /// Represents a disjunct relation of intervals, where the two intervals share no elements and are not touching.
        /// </summary>
        public record Disjunct(Interval<T> First, Interval<T> Second) : IntervalRelation<T>
        {
            /// <inheritdoc/>
            public override Interval<T> LowerDisjunct => this.First;

            /// <inheritdoc/>
            public override Interval<T> Intersecting => Interval<T>.Empty;

            /// <inheritdoc/>
            public override Interval<T> UpperDisjunct => this.Second;
        }

        /// <summary>
        /// Represents a touching relation of intervals, where the two intervals share no elements, but their endpoints touch.
        /// </summary>
        public record Touching(Interval<T> First, Interval<T> Second) : IntervalRelation<T>
        {
            /// <inheritdoc/>
            public override Interval<T> LowerDisjunct => this.First;

            /// <inheritdoc/>
            public override Interval<T> Intersecting => Interval<T>.Empty;

            /// <inheritdoc/>
            public override Interval<T> UpperDisjunct => this.Second;
        }

        /// <summary>
        /// Represents an overlapping relation of intervals, where the two intervals share elements,
        /// but neither contain the other.
        /// </summary>
        public record Overlapping(Interval<T> FirstDisjunct, Interval<T> Overlap, Interval<T> SecondDisjunct) : IntervalRelation<T>
        {
            /// <inheritdoc/>
            public override Interval<T> LowerDisjunct => this.FirstDisjunct;

            /// <inheritdoc/>
            public override Interval<T> Intersecting => this.Overlap;

            /// <inheritdoc/>
            public override Interval<T> UpperDisjunct => this.SecondDisjunct;
        }

        /// <summary>
        /// Represents a containing relation of intervals, where one interval completely contains another, but none
        /// of the endpoints are equal.
        /// </summary>
        public record Containing(Interval<T> FirstDisjunct, Interval<T> Overlap, Interval<T> SecondDisjunct) : IntervalRelation<T>
        {
            /// <inheritdoc/>
            public override Interval<T> LowerDisjunct => this.FirstDisjunct;

            /// <inheritdoc/>
            public override Interval<T> Intersecting => this.Overlap;

            /// <inheritdoc/>
            public override Interval<T> UpperDisjunct => this.SecondDisjunct;
        }

        /// <summary>
        /// Represents a starting relation of intervals, where one interval completely contains another, and
        /// the starting points are equal.
        /// </summary>
        public record Starting(Interval<T> Overlap, Interval<T> SecondDisjunct) : IntervalRelation<T>
        {
            /// <inheritdoc/>
            public override Interval<T> LowerDisjunct => Interval<T>.Empty;

            /// <inheritdoc/>
            public override Interval<T> Intersecting => this.Overlap;

            /// <inheritdoc/>
            public override Interval<T> UpperDisjunct => this.SecondDisjunct;
        }

        /// <summary>
        /// Represents a finishing relation of intervals, where one interval completely contains another, and
        /// the finishing points are equal.
        /// </summary>
        public record Finishing(Interval<T> FirstDisjunct, Interval<T> Overlap) : IntervalRelation<T>
        {
            /// <inheritdoc/>
            public override Interval<T> LowerDisjunct => this.FirstDisjunct;

            /// <inheritdoc/>
            public override Interval<T> Intersecting => this.Overlap;

            /// <inheritdoc/>
            public override Interval<T> UpperDisjunct => Interval<T>.Empty;
        }

        /// <summary>
        /// Represents an equal relation of intervals, where the two intervals are completely equal.
        /// </summary>
        public record Equal(Interval<T> Overlap) : IntervalRelation<T>
        {
            /// <inheritdoc/>
            public override Interval<T> LowerDisjunct => Interval<T>.Empty;

            /// <inheritdoc/>
            public override Interval<T> Intersecting => this.Overlap;

            /// <inheritdoc/>
            public override Interval<T> UpperDisjunct => Interval<T>.Empty;
        }
    }
}
