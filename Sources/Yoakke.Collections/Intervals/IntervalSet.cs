// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections;
using System.Collections.Generic;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// A generic interval set implementation.
    /// </summary>
    /// <typeparam name="T">The interval value type.</typeparam>
    public class IntervalSet<T> : IIntervalSet<T>
    {
        public int Count => this.intervals.Count;

        private readonly List<Interval<T>> intervals = new();
        private readonly IComparer<T> comparer;

        /// <summary>
        /// Initializes an empty <see cref="IntervalSet{T}"/> with the default comparer.
        /// </summary>
        public IntervalSet()
            : this(Comparer<T>.Default)
        {
        }

        /// <summary>
        /// Initializes an empty <see cref="IntervalSet{T}"/> with the given comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use.</param>
        public IntervalSet(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        public void Clear() => this.intervals.Clear();

        public bool Contains(T value)
        {
            var (from, to) = this.IntersectingIndexRange(Interval<T>.Singleton(value));
            return from != to;
        }

        public void Add(Interval<T> interval)
        {
            if (this.intervals.Count == 0)
            {
                // Empty set
                this.intervals.Add(interval);
                return;
            }
            // Non-empty set
            var (start, end) = this.TouchingIndexRange(interval);
            if (start == end)
            {
                // Touches nothing, just insert
                this.intervals.Insert(start, interval);
            }
            else if (end - start == 1)
            {
                // Intersects with a single entry
                var newLower = interval.Lower.CompareTo(this.intervals[start].Lower, this.comparer) < 0 ? interval.Lower : this.intervals[start].Lower;
                var newUpper = interval.Upper.CompareTo(this.intervals[start].Upper, this.comparer) > 0 ? interval.Upper : this.intervals[start].Upper;
                this.intervals[start] = new Interval<T>(newLower, newUpper);
            }
            else
            {
                // Intersecting with multiple intervals, we need to unify them
                var newLower = interval.Lower.CompareTo(this.intervals[start].Lower, this.comparer) < 0 ? interval.Lower : this.intervals[start].Lower;
                var newUpper = interval.Upper.CompareTo(this.intervals[end - 1].Upper, this.comparer) > 0 ? interval.Upper : this.intervals[end - 1].Upper;
                this.intervals.RemoveRange(start + 1, end - start - 1);
                this.intervals[start] = new Interval<T>(newLower, newUpper);
            }
        }

        public void Invert()
        {
            if (this.intervals.Count == 0)
            {
                // Inverse of the empty set is the full interval
                this.intervals.Add(Interval<T>.Full());
                return;
            }
            if (this.intervals[0].Lower.Type == BoundType.Unbounded && this.intervals[0].Upper.Type == BoundType.Unbounded)
            {
                // Inverse of full interval is the empty one
                this.intervals.Clear();
                return;
            }

            // Out interval set is neither empty, nor full, there are 3 cases:
            //  - Both ends are unbounded: for N intervals this creates N - 1 intervals when inverted
            //  - One end is unbounded: for N intervals this creates N intervals when inverted
            //  - Both ends are bounded: for N intervals this creates N + 1 intervals when inverted
            var lowerUnbounded = this.intervals[0].Lower.Type == BoundType.Unbounded;
            var upperUnbounded = this.intervals[this.intervals.Count - 1].Upper.Type == BoundType.Unbounded;

            if (lowerUnbounded && upperUnbounded)
            {
                var nIntervals = this.intervals.Count - 1;
                for (int i = 0; i < nIntervals; ++i)
                {
                    var lower = this.intervals[i].Upper.GetTouching()!.Value;
                    var upper = this.intervals[i + 1].Lower.GetTouching()!.Value;
                    this.intervals[i] = new Interval<T>(lower, upper);
                }
                this.intervals.RemoveAt(this.intervals.Count - 1);
            }
            else if (lowerUnbounded)
            {
                var nIntervals = this.intervals.Count;
                if (nIntervals > 1)
                {
                    var prevTouch = this.intervals[nIntervals - 1].Lower.GetTouching()!.Value;

                    // Modify the last one
                    var lastLower = this.intervals[nIntervals - 1].Upper.GetTouching()!.Value;
                    this.intervals[nIntervals - 1] = new Interval<T>(lastLower, UpperBound<T>.Unbounded());

                    for (int i = nIntervals - 2; i > 0; --i)
                    {
                        var loTouch = prevTouch;
                        var hiTouch = this.intervals[i].Upper.GetTouching()!.Value;
                        prevTouch = this.intervals[i].Lower.GetTouching()!.Value;
                        this.intervals[i] = new Interval<T>(hiTouch, loTouch);
                    }

                    // First one
                    var hTouch = this.intervals[0].Upper.GetTouching()!.Value;
                    this.intervals[0] = new Interval<T>(hTouch, prevTouch);
                }
                else
                {
                    // Modify the only one
                    var lower = this.intervals[0].Upper.GetTouching()!.Value;
                    this.intervals[0] = new Interval<T>(lower, UpperBound<T>.Unbounded());
                }
            }
            else if (upperUnbounded)
            {
                var nIntervals = this.intervals.Count;
                if (nIntervals > 1)
                {
                    var prevTouch = this.intervals[0].Upper.GetTouching()!.Value;

                    // Modify the first one
                    var firstUpper = this.intervals[0].Lower.GetTouching()!.Value;
                    this.intervals[0] = new Interval<T>(LowerBound<T>.Unbounded(), firstUpper);

                    for (int i = 1; i < nIntervals - 1; ++i)
                    {
                        var loTouch = this.intervals[i].Lower.GetTouching()!.Value;
                        var hiTouch = prevTouch;
                        prevTouch = this.intervals[i].Upper.GetTouching()!.Value;
                        this.intervals[i] = new Interval<T>(hiTouch, loTouch);
                    }

                    // Last one
                    var lTouch = this.intervals[nIntervals - 1].Lower.GetTouching()!.Value;
                    this.intervals[nIntervals - 1] = new Interval<T>(prevTouch, lTouch);
                }
                else
                {
                    // Modify the only one
                    var upper = this.intervals[0].Lower.GetTouching()!.Value;
                    this.intervals[0] = new Interval<T>(LowerBound<T>.Unbounded(), upper);
                }
            }
            else
            {
                // Bounded, meaning N + 1 entries
                var nIntervals = this.intervals.Count;

                // Add a last entry
                this.intervals.Add(new Interval<T>(this.intervals[this.intervals.Count - 1].Upper.GetTouching()!.Value, UpperBound<T>.Unbounded()));

                var prevTouch = this.intervals[0].Upper.GetTouching()!.Value;

                // Modify first one
                var firstUpper = this.intervals[0].Lower.GetTouching()!.Value;
                this.intervals[0] = new Interval<T>(LowerBound<T>.Unbounded(), firstUpper);

                for (int i = 1; i < nIntervals; ++i)
                {
                    var loTouch = this.intervals[i].Lower.GetTouching()!.Value;
                    var upper = prevTouch;
                    prevTouch = this.intervals[i].Upper.GetTouching()!.Value;
                    this.intervals[i] = new Interval<T>(upper, loTouch);
                }
            }
        }

        public IEnumerator<Interval<T>> GetEnumerator() => this.intervals.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private (int, int) IntersectingIndexRange(Interval<T> interval)
        {
            var (from, _) = this.intervals.BinarySearch(interval.Lower, iv => iv.Upper, (k1, k2) => k1.CompareTo(k2, this.comparer));
            var (to, _) = this.intervals.BinarySearch(from, interval.Upper, iv => iv.Lower, (k1, k2) => k1.CompareTo(k2, this.comparer));
            return (from, to);
        }

        private (int, int) TouchingIndexRange(Interval<T> interval)
        {
            var (from, to) = this.IntersectingIndexRange(interval);
            if (from != 0 && interval.Lower.IsTouching(this.intervals[from - 1].Upper, this.comparer)) from -= 1;
            if (to != this.intervals.Count && interval.Upper.IsTouching(this.intervals[to].Lower, this.comparer)) to += 1;
            return (from, to);
        }
    }
}
