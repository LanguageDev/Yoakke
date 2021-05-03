using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// A generic interval set implementation.
    /// </summary>
    /// <typeparam name="T">The interval value type.</typeparam>
    public class IntervalSet<T> : IIntervalSet<T>
    {
        public int Count => intervals.Count;

        private List<Interval<T>> intervals = new List<Interval<T>>();
        private IComparer<T> comparer;

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

        public void Clear() => intervals.Clear();

        public bool Contains(T value)
        {
            var (from, to) = IntersectingIndexRange(Interval<T>.Singleton(value));
            return from != to;
        }

        public void Add(Interval<T> interval)
        {
            if (intervals.Count == 0)
            {
                // Empty set
                intervals.Add(interval);
                return;
            }
            // Non-empty set
            var (start, end) = TouchingIndexRange(interval);
            if (start == end)
            {
                // Touches nothing, just insert
                intervals.Insert(start, interval);
            }
            else if (end - start == 1)
            {
                // Intersects with a single entry
                var newLower = interval.Lower.CompareTo(intervals[start].Lower, comparer) < 0 ? interval.Lower : intervals[start].Lower;
                var newUpper = interval.Upper.CompareTo(intervals[start].Upper, comparer) > 0 ? interval.Upper : intervals[start].Upper;
                intervals[start] = new Interval<T>(newLower, newUpper);
            }
            else {
                // Intersecting with multiple intervals, we need to unify them
                var newLower = interval.Lower.CompareTo(intervals[start].Lower, comparer) < 0 ? interval.Lower : intervals[start].Lower;
                var newUpper = interval.Upper.CompareTo(intervals[end - 1].Upper, comparer) > 0 ? interval.Upper : intervals[end - 1].Upper;
                intervals.RemoveRange(start + 1, end - start - 1);
                intervals[start] = new Interval<T>(newLower, newUpper);
            }
        }

        public void Invert()
        {
            if (intervals.Count == 0)
            {
                // Inverse of the empty set is the full interval
                intervals.Add(Interval<T>.Full());
                return;
            }
            if (intervals[0].Lower.Type == BoundType.Unbounded && intervals[0].Upper.Type == BoundType.Unbounded)
            {
                // Inverse of full interval is the empty one
                intervals.Clear();
                return;
            }
            // Out interval set is neither empty, nor full, there are 3 cases:
            //  - Both ends are unbounded: for N intervals this creates N - 1 intervals when inverted
            //  - One end is unbounded: for N intervals this creates N intervals when inverted
            //  - Both ends are bounded: for N intervals this creates N + 1 intervals when inverted

            var lowerUnbounded = intervals[0].Lower.Type == BoundType.Unbounded;
            var upperUnbounded = intervals[intervals.Count - 1].Upper.Type == BoundType.Unbounded;

            if (lowerUnbounded && upperUnbounded)
            {
                var nIntervals = intervals.Count - 1;
                for (int i = 0; i < nIntervals; ++i)
                {
                    var lower = intervals[i].Upper.GetTouching().Value;
                    var upper = intervals[i + 1].Lower.GetTouching().Value;
                    intervals[i] = new Interval<T>(lower, upper);
                }
                intervals.RemoveAt(intervals.Count - 1);
            }
            else if (lowerUnbounded)
            {
                var nIntervals = intervals.Count;
                if (nIntervals > 1)
                {
                    var prevTouch = intervals[nIntervals - 1].Lower.GetTouching().Value;

                    // Modify the last one
                    var lastLower = intervals[nIntervals - 1].Upper.GetTouching().Value;
                    intervals[nIntervals - 1] = new Interval<T>(lastLower, UpperBound<T>.Unbounded());

                    for (int i = nIntervals - 2; i > 0; --i)
                    {
                        var loTouch = prevTouch;
                        var hiTouch = intervals[i].Upper.GetTouching().Value;
                        prevTouch = intervals[i].Lower.GetTouching().Value;
                        intervals[i] = new Interval<T>(hiTouch, loTouch);
                    }

                    // First one
                    var hTouch = intervals[0].Upper.GetTouching().Value;
                    intervals[0] = new Interval<T>(hTouch, prevTouch);
                }
                else
                {
                    // Modify the only one
                    var lower = intervals[0].Upper.GetTouching().Value;
                    intervals[0] = new Interval<T>(lower, UpperBound<T>.Unbounded());
                }
            }
            else if (upperUnbounded)
            {
                var nIntervals = intervals.Count;
                if (nIntervals > 1)
                {
                    var prevTouch = intervals[0].Upper.GetTouching().Value;

                    // Modify the first one
                    var firstUpper = intervals[0].Lower.GetTouching().Value;
                    intervals[0] = new Interval<T>(LowerBound<T>.Unbounded(), firstUpper);

                    for (int i = 1; i < nIntervals - 1; ++i)
                    {
                        var loTouch = intervals[i].Lower.GetTouching().Value;
                        var hiTouch = prevTouch;
                        prevTouch = intervals[i].Upper.GetTouching().Value;
                        intervals[i] = new Interval<T>(hiTouch, loTouch);
                    }

                    // Last one
                    var lTouch = intervals[nIntervals - 1].Lower.GetTouching().Value;
                    intervals[nIntervals - 1] = new Interval<T>(prevTouch, lTouch);
                }
                else
                {
                    // Modify the only one
                    var upper = intervals[0].Lower.GetTouching().Value;
                    intervals[0] = new Interval<T>(LowerBound<T>.Unbounded(), upper);
                }
            }
            else
            {
                // Bounded, meaning N + 1 entries
                var nIntervals = intervals.Count;

                // Add a last entry
                intervals.Add(new Interval<T>(intervals[intervals.Count - 1].Upper.GetTouching().Value, UpperBound<T>.Unbounded()));

                var prevTouch = intervals[0].Upper.GetTouching().Value;

                // Modify first one
                var firstUpper = intervals[0].Lower.GetTouching().Value;
                intervals[0] = new Interval<T>(LowerBound<T>.Unbounded(), firstUpper);

                for (int i = 1; i < nIntervals; ++i)
                {
                    var loTouch = intervals[i].Lower.GetTouching().Value;
                    var upper = prevTouch;
                    prevTouch = intervals[i].Upper.GetTouching().Value;
                    intervals[i] = new Interval<T>(upper, loTouch);
                }
            }
        }

        public IEnumerator<Interval<T>> GetEnumerator() => intervals.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private (int, int) IntersectingIndexRange(Interval<T> interval)
        {
            var (from, _) = intervals.BinarySearch(interval.Lower, iv => iv.Upper, (k1, k2) => k1.CompareTo(k2, comparer));
            var (to, _) = intervals.BinarySearch(from, interval.Upper, iv => iv.Lower, (k1, k2) => k1.CompareTo(k2, comparer));
            return (from, to);
        }

        private (int, int) TouchingIndexRange(Interval<T> interval)
        {
            var (from, to) = IntersectingIndexRange(interval);
            if (from != 0 && interval.Lower.IsTouching(intervals[from - 1].Upper, comparer)) from -= 1;
            if (to != intervals.Count && interval.Upper.IsTouching(intervals[to].Lower, comparer)) to += 1;
            return (from, to);
        }
    }
}
