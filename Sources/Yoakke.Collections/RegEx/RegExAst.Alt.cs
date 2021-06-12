// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Collections.Compatibility;
using Yoakke.Collections.FiniteAutomata;

namespace Yoakke.Collections.RegEx
{
    public partial class RegExAst
    {
        /// <summary>
        /// Represents an alternative of other regex constructs.
        /// </summary>
        public class Alt : RegExAst
        {
            /// <summary>
            /// The first alternative construct.
            /// </summary>
            public readonly RegExAst First;

            /// <summary>
            /// The second alternative construct.
            /// </summary>
            public readonly RegExAst Second;

            public Alt(RegExAst first, RegExAst second)
            {
                this.First = first;
                this.Second = second;
            }

            public override bool Equals(RegExAst other) => other is Alt alt
                && this.First.Equals(alt.First)
                && this.Second.Equals(alt.Second);

            public override int GetHashCode() => HashCode.Combine(this.First, this.Second);

            public override RegExAst Desugar() => new Alt(this.First.Desugar(), this.Second.Desugar());

            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                var newStart = denseNfa.NewState();
                var newEnd = denseNfa.NewState();

                var (firstStart, firstEnd) = this.First.ThompsonConstruct(denseNfa);
                var (secondStart, secondEnd) = this.Second.ThompsonConstruct(denseNfa);

                denseNfa.AddTransition(newStart, Epsilon.Default, firstStart);
                denseNfa.AddTransition(newStart, Epsilon.Default, secondStart);

                denseNfa.AddTransition(firstEnd, Epsilon.Default, newEnd);
                denseNfa.AddTransition(secondEnd, Epsilon.Default, newEnd);

                return (newStart, newEnd);
            }
        }
    }
}
