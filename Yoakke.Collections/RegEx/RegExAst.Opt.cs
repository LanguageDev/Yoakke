using Yoakke.Collections.FiniteAutomata;

namespace Yoakke.Collections.RegEx
{
    partial class RegExAst
    {
        /// <summary>
        /// Represents an optional construct.
        /// </summary>
        public class Opt : RegExAst
        {
            /// <summary>
            /// The optional sub-construct.
            /// </summary>
            public readonly RegExAst Subexpr;

            public Opt(RegExAst subexpr)
            {
                Subexpr = subexpr;
            }

            public override bool Equals(RegExAst other) => other is Opt opt && Subexpr.Equals(opt.Subexpr);
            public override int GetHashCode() => Subexpr.GetHashCode();

            public override RegExAst Desugar() => new Opt(Subexpr.Desugar());

            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                var (start, end) = Subexpr.ThompsonConstruct(denseNfa);
                denseNfa.AddTransition(start, Epsilon.Default, end);
                return (start, end);
            }
        }
    }
}
