using System;
using Yoakke.Collections.FiniteAutomata;

namespace Yoakke.Collections.RegEx
{
    partial class RegExAst
    {
        /// <summary>
        /// Represents that a construct should be repeated 1 or more times.
        /// </summary>
        public class Rep1 : RegExAst
        {
            /// <summary>
            /// The subconstruct to repeat.
            /// </summary>
            public readonly RegExAst Subexpr;

            public Rep1(RegExAst subexpr)
            {
                Subexpr = subexpr;
            }

            public override bool Equals(RegExAst other) => other is Rep1 r && Subexpr.Equals(r.Subexpr);
            public override int GetHashCode() => Subexpr.GetHashCode();

            public override RegExAst Desugar()
            {
                var sub = Subexpr.Desugar();
                return new Seq(sub, new Rep0(sub));
            }

            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa) =>
                throw new NotSupportedException();
        }
    }
}
