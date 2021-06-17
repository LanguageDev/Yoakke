// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Collections.Compatibility;
using Yoakke.Collections.FiniteAutomata;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.RegEx
{
    public partial class RegExAst
    {
        /// <summary>
        /// Represents match on a range of character literals.
        /// </summary>
        public class LiteralRange : RegExAst
        {
            /// <summary>
            /// True, if the range should be negated.
            /// </summary>
            public bool Negate { get; }

            /// <summary>
            /// The character ranges.
            /// </summary>
            public IList<(char From, char To)> Ranges { get; }

            public LiteralRange(bool negate, IList<(char From, char To)> ranges)
            {
                this.Negate = negate;
                this.Ranges = ranges;
            }

            public override bool Equals(RegExAst other) => other is LiteralRange r
                && this.Negate == r.Negate
                && this.Ranges.SequenceEqual(r.Ranges);

            public override int GetHashCode()
            {
                var hash = default(HashCode);
                hash.Add(this.Negate);
                foreach (var r in this.Ranges) hash.Add(r);
                return hash.ToHashCode();
            }

            public override RegExAst Desugar() => this;

            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                // Build the range
                var range = new IntervalSet<char>();
                foreach (var (from, to) in this.Ranges)
                {
                    range.Add(new Interval<char>(LowerBound<char>.Inclusive(from), UpperBound<char>.Inclusive(to)));
                }
                if (this.Negate) range.Invert();
                // Write the transitions
                var start = denseNfa.NewState();
                var end = denseNfa.NewState();
                foreach (var iv in range) denseNfa.AddTransition(start, iv, end);
                return (start, end);
            }
        }
    }
}