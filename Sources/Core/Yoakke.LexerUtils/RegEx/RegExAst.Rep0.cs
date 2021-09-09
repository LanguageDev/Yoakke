// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.LexerUtils.FiniteAutomata;

namespace Yoakke.LexerUtils.RegEx
{
    public partial class RegExAst
    {
        /// <summary>
        /// Represents that a construct should be repeated 0 or more times.
        /// </summary>
        public class Rep0 : RegExAst
        {
            /// <summary>
            /// The subconstruct to repeat.
            /// </summary>
            public RegExAst Subexpr { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Rep0"/> class.
            /// </summary>
            /// <param name="subexpr">The subexpression to match 0 or more times.</param>
            public Rep0(RegExAst subexpr)
            {
                this.Subexpr = subexpr;
            }

            /// <inheritdoc/>
            public override bool Equals(RegExAst other) => other is Rep0 r && this.Subexpr.Equals(r.Subexpr);

            /// <inheritdoc/>
            public override int GetHashCode() => this.Subexpr.GetHashCode();

            /// <inheritdoc/>
            public override RegExAst Desugar() => new Rep0(this.Subexpr.Desugar());

            /// <inheritdoc/>
            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                var (start, end) = this.Subexpr.ThompsonConstruct(denseNfa);

                var newStart = denseNfa.NewState();
                var newEnd = denseNfa.NewState();

                denseNfa.AddTransition(newStart, Epsilon.Instance, start);
                denseNfa.AddTransition(end, Epsilon.Instance, newEnd);
                denseNfa.AddTransition(end, Epsilon.Instance, start);
                denseNfa.AddTransition(newStart, Epsilon.Instance, newEnd);

                return (newStart, newEnd);
            }
        }
    }
}
