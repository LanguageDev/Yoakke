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
        /// Represents a sequence of other regex constructs.
        /// </summary>
        public class Seq : RegExAst
        {
            /// <summary>
            /// The first construct of the sequence.
            /// </summary>
            public RegExAst First { get; }

            /// <summary>
            /// The second construct of the sequence.
            /// </summary>
            public RegExAst Second { get; }

            public Seq(RegExAst first, RegExAst second)
            {
                this.First = first;
                this.Second = second;
            }

            public override bool Equals(RegExAst other) => other is Seq seq
                && this.First.Equals(seq.First)
                && this.Second.Equals(seq.Second);

            public override int GetHashCode() => HashCode.Combine(this.First, this.Second);

            public override RegExAst Desugar() => new Seq(this.First.Desugar(), this.Second.Desugar());

            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                var (firstStart, firstEnd) = this.First.ThompsonConstruct(denseNfa);
                var (secondStart, secondEnd) = this.Second.ThompsonConstruct(denseNfa);
                denseNfa.AddTransition(firstEnd, Epsilon.Default, secondStart);
                return (firstStart, secondEnd);
            }
        }
    }
}
