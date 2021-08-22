// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.Utilities.FiniteAutomata;

namespace Yoakke.Utilities.RegEx
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

            /// <summary>
            /// Initializes a new instance of the <see cref="Seq"/> class.
            /// </summary>
            /// <param name="first">The construct to match first.</param>
            /// <param name="second">The construct to match if <paramref name="first"/> matched successfully.</param>
            public Seq(RegExAst first, RegExAst second)
            {
                this.First = first;
                this.Second = second;
            }

            /// <inheritdoc/>
            public override bool Equals(RegExAst other) => other is Seq seq
                && this.First.Equals(seq.First)
                && this.Second.Equals(seq.Second);

            /// <inheritdoc/>
            public override int GetHashCode() => (this.First, this.Second).GetHashCode();

            /// <inheritdoc/>
            public override RegExAst Desugar() => new Seq(this.First.Desugar(), this.Second.Desugar());

            /// <inheritdoc/>
            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                var (firstStart, firstEnd) = this.First.ThompsonConstruct(denseNfa);
                var (secondStart, secondEnd) = this.Second.ThompsonConstruct(denseNfa);
                denseNfa.AddTransition(firstEnd, Epsilon.Instance, secondStart);
                return (firstStart, secondEnd);
            }
        }
    }
}
