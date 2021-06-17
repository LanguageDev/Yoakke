// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Collections.FiniteAutomata;

namespace Yoakke.Collections.RegEx
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

            public Rep0(RegExAst subexpr)
            {
                this.Subexpr = subexpr;
            }

            public override bool Equals(RegExAst other) => other is Rep0 r && this.Subexpr.Equals(r.Subexpr);

            public override int GetHashCode() => this.Subexpr.GetHashCode();

            public override RegExAst Desugar() => new Rep0(this.Subexpr.Desugar());

            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                var (start, end) = this.Subexpr.ThompsonConstruct(denseNfa);

                var newStart = denseNfa.NewState();
                var newEnd = denseNfa.NewState();

                denseNfa.AddTransition(newStart, Epsilon.Default, start);
                denseNfa.AddTransition(end, Epsilon.Default, newEnd);
                denseNfa.AddTransition(end, Epsilon.Default, start);
                denseNfa.AddTransition(newStart, Epsilon.Default, newEnd);

                return (newStart, newEnd);
            }
        }
    }
}
