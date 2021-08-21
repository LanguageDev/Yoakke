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
        /// Represents an alternative of other regex constructs.
        /// </summary>
        public class Alt : RegExAst
        {
            /// <summary>
            /// The first alternative construct.
            /// </summary>
            public RegExAst First { get; }

            /// <summary>
            /// The second alternative construct.
            /// </summary>
            public RegExAst Second { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Alt"/> class.
            /// </summary>
            /// <param name="first">The first alternative construct to try.</param>
            /// <param name="second">The second alternative construct to try.</param>
            public Alt(RegExAst first, RegExAst second)
            {
                this.First = first;
                this.Second = second;
            }

            /// <inheritdoc/>
            public override bool Equals(RegExAst other) => other is Alt alt
                && this.First.Equals(alt.First)
                && this.Second.Equals(alt.Second);

            /// <inheritdoc/>
            public override int GetHashCode() => HashCode.Combine(this.First, this.Second);

            /// <inheritdoc/>
            public override RegExAst Desugar() => new Alt(this.First.Desugar(), this.Second.Desugar());

            /// <inheritdoc/>
            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                var newStart = denseNfa.NewState();
                var newEnd = denseNfa.NewState();

                var (firstStart, firstEnd) = this.First.ThompsonConstruct(denseNfa);
                var (secondStart, secondEnd) = this.Second.ThompsonConstruct(denseNfa);

                denseNfa.AddTransition(newStart, Epsilon.Instance, firstStart);
                denseNfa.AddTransition(newStart, Epsilon.Instance, secondStart);

                denseNfa.AddTransition(firstEnd, Epsilon.Instance, newEnd);
                denseNfa.AddTransition(secondEnd, Epsilon.Instance, newEnd);

                return (newStart, newEnd);
            }
        }
    }
}
