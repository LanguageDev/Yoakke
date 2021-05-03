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
                First = first;
                Second = second;
            }

            public override RegExAst Desugar() => new Alt(First.Desugar(), Second.Desugar());

            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                var newStart = denseNfa.NewState();
                var newEnd = denseNfa.NewState();

                var (firstStart, firstEnd) = First.ThompsonConstruct(denseNfa);
                var (secondStart, secondEnd) = Second.ThompsonConstruct(denseNfa);
                
                denseNfa.AddTransition(newStart, Epsilon.Default, firstStart);
                denseNfa.AddTransition(newStart, Epsilon.Default, secondStart);

                denseNfa.AddTransition(firstEnd, Epsilon.Default, newEnd);
                denseNfa.AddTransition(secondEnd, Epsilon.Default, newEnd);

                return (newStart, newEnd);
            }
        }
    }
}
