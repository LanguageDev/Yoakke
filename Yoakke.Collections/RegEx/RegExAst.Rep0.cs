using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections.FiniteAutomata;

namespace Yoakke.Collections.RegEx
{
    partial class RegExAst
    {
        /// <summary>
        /// Represents that a construct should be repeated 0 or more times.
        /// </summary>
        public class Rep0 : RegExAst
        {
            /// <summary>
            /// The subconstruct to repeat.
            /// </summary>
            public readonly RegExAst Subexpr;

            public Rep0(RegExAst subexpr)
            {
                Subexpr = subexpr;
            }

            public override bool Equals(RegExAst other) => other is Rep0 r && Subexpr.Equals(r.Subexpr);
            public override int GetHashCode() => Subexpr.GetHashCode();

            public override RegExAst Desugar() => new Rep0(Subexpr.Desugar());

            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                var (start, end) = Subexpr.ThompsonConstruct(denseNfa);

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
