using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections.FiniteAutomata;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.RegEx
{
    partial class RegExAst
    {
        /// <summary>
        /// Represents match on a range of character literals.
        /// </summary>
        public class LiteralRange : RegExAst
        {
            /// <summary>
            /// True, if the range should be negated.
            /// </summary>
            public readonly bool Negate;
            /// <summary>
            /// The character ranges.
            /// </summary>
            public readonly IList<(char From, char To)> Ranges;

            public LiteralRange(bool negate, IList<(char From, char To)> ranges)
            {
                Negate = negate;
                Ranges = ranges;
            }

            public override bool Equals(RegExAst other) => other is LiteralRange r
                && Negate == r.Negate
                && Ranges.SequenceEqual(r.Ranges);
            public override int GetHashCode()
            {
                var hash = new HashCode();
                hash.Add(Negate);
                foreach (var r in Ranges) hash.Add(r);
                return hash.ToHashCode();
            }

            public override RegExAst Desugar() => this;

            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                // Build the range
                var range = new IntervalSet<char>();
                foreach (var (from, to) in Ranges)
                {
                    range.Add(new Interval<char>(LowerBound<char>.Inclusive(from), UpperBound<char>.Inclusive(to)));
                }
                if (Negate) range.Invert();
                // Write the transitions
                var start = denseNfa.NewState();
                var end = denseNfa.NewState();
                foreach (var iv in range) denseNfa.AddTransition(start, iv, end);
                return (start, end);
            }
        }
    }
}
