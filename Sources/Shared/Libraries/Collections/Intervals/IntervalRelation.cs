// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// Represents the relation of two intervals.
/// </summary>
/// <typeparam name="T">The type of the endpoint value.</typeparam>
public abstract record IntervalRelation<T>
{
    /// <summary>
    /// The lower disjunct part of the relation.
    /// </summary>
    public virtual Interval<T> LowerDisjunct => Interval<T>.Empty;

    /// <summary>
    /// The intersecting part of the relation.
    /// </summary>
    public virtual Interval<T> Intersecting => Interval<T>.Empty;

    /// <summary>
    /// The upper disjunct part of the relation.
    /// </summary>
    public virtual Interval<T> UpperDisjunct => Interval<T>.Empty;

    // Alternatives

    /// <summary>
    /// Represents a disjunct relation of intervals, where the two intervals share no elements and are not touching.
    /// </summary>
    public sealed record Disjunct : IntervalRelation<T>
    {
        /// <inheritdoc/>
        public override Interval<T> LowerDisjunct { get; }

        /// <inheritdoc/>
        public override Interval<T> UpperDisjunct { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Disjunct"/> class.
        /// </summary>
        /// <param name="lower">The lower disjunct interval.</param>
        /// <param name="upper">The upper disjunct interval.</param>
        public Disjunct(Interval<T> lower, Interval<T> upper)
        {
            this.LowerDisjunct = lower;
            this.UpperDisjunct = upper;
        }
    }

    /// <summary>
    /// Represents a touching relation of intervals, where the two intervals share no elements, but their endpoints touch.
    /// </summary>
    public sealed record Touching : IntervalRelation<T>
    {
        /// <inheritdoc/>
        public override Interval<T> LowerDisjunct { get; }

        /// <inheritdoc/>
        public override Interval<T> UpperDisjunct { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Touching"/> class.
        /// </summary>
        /// <param name="lower">The lower disjunct interval.</param>
        /// <param name="upper">The upper disjunct interval.</param>
        public Touching(Interval<T> lower, Interval<T> upper)
        {
            this.LowerDisjunct = lower;
            this.UpperDisjunct = upper;
        }
    }

    /// <summary>
    /// Represents an overlapping relation of intervals, where the two intervals share elements,
    /// but neither contain the other.
    /// </summary>
    public sealed record Overlapping : IntervalRelation<T>
    {
        /// <inheritdoc/>
        public override Interval<T> LowerDisjunct { get; }

        /// <inheritdoc/>
        public override Interval<T> Intersecting { get; }

        /// <inheritdoc/>
        public override Interval<T> UpperDisjunct { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Overlapping"/> class.
        /// </summary>
        /// <param name="lower">The lower disjunct interval.</param>
        /// <param name="intersecting">The intersecting interval.</param>
        /// <param name="upper">The upper disjunct interval.</param>
        public Overlapping(Interval<T> lower, Interval<T> intersecting, Interval<T> upper)
        {
            this.LowerDisjunct = lower;
            this.Intersecting = intersecting;
            this.UpperDisjunct = upper;
        }
    }

    /// <summary>
    /// Represents a containing relation of intervals, where one interval completely contains another, but none
    /// of the endpoints are equal.
    /// </summary>
    public sealed record Containing : IntervalRelation<T>
    {
        /// <inheritdoc/>
        public override Interval<T> LowerDisjunct { get; }

        /// <inheritdoc/>
        public override Interval<T> Intersecting { get; }

        /// <inheritdoc/>
        public override Interval<T> UpperDisjunct { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Containing"/> class.
        /// </summary>
        /// <param name="lower">The lower disjunct interval.</param>
        /// <param name="intersecting">The intersecting interval.</param>
        /// <param name="upper">The upper disjunct interval.</param>
        public Containing(Interval<T> lower, Interval<T> intersecting, Interval<T> upper)
        {
            this.LowerDisjunct = lower;
            this.Intersecting = intersecting;
            this.UpperDisjunct = upper;
        }
    }

    /// <summary>
    /// Represents a starting relation of intervals, where one interval completely contains another, and
    /// the starting points are equal.
    /// </summary>
    public sealed record Starting : IntervalRelation<T>
    {
        /// <inheritdoc/>
        public override Interval<T> Intersecting { get; }

        /// <inheritdoc/>
        public override Interval<T> UpperDisjunct { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Starting"/> class.
        /// </summary>
        /// <param name="intersecting">The intersecting interval.</param>
        /// <param name="upper">The upper disjunct interval.</param>
        public Starting(Interval<T> intersecting, Interval<T> upper)
        {
            this.Intersecting = intersecting;
            this.UpperDisjunct = upper;
        }
    }

    /// <summary>
    /// Represents a finishing relation of intervals, where one interval completely contains another, and
    /// the finishing points are equal.
    /// </summary>
    public sealed record Finishing : IntervalRelation<T>
    {
        /// <inheritdoc/>
        public override Interval<T> LowerDisjunct { get; }

        /// <inheritdoc/>
        public override Interval<T> Intersecting { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Finishing"/> class.
        /// </summary>
        /// <param name="lower">The lower disjunct interval.</param>
        /// <param name="intersecting">The intersecting interval.</param>
        public Finishing(Interval<T> lower, Interval<T> intersecting)
        {
            this.LowerDisjunct = lower;
            this.Intersecting = intersecting;
        }
    }

    /// <summary>
    /// Represents an equal relation of intervals, where the two intervals are completely equal.
    /// </summary>
    public sealed record Equal : IntervalRelation<T>
    {
        /// <inheritdoc/>
        public override Interval<T> Intersecting { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Equal"/> class.
        /// </summary>
        /// <param name="intersecting">The intersecting interval.</param>
        public Equal(Interval<T> intersecting)
        {
            this.Intersecting = intersecting;
        }
    }
}
