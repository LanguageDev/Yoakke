using System;
using Yoakke.Collections.Compatibility;
using Yoakke.Collections.FiniteAutomata;

namespace Yoakke.Collections.RegEx
{
    partial class RegExAst
    {
        /// <summary>
        /// Represents that a regex construct should be repeated in some way.
        /// </summary>
        public class Quantified : RegExAst
        {
            /// <summary>
            /// The sub-construct to repeat.
            /// </summary>
            public readonly RegExAst Subexpr;
            /// <summary>
            /// The minimum number of repetitions. Can be zero.
            /// </summary>
            public readonly int AtLeast;
            /// <summary>
            /// The maximum number of repetitions, if any.
            /// </summary>
            public readonly int? AtMost;

            public Quantified(RegExAst subexpr, int atLeast, int? atMost)
            {
                if (atLeast < 0) throw new ArgumentOutOfRangeException(nameof(atLeast));
                if (atMost != null && atMost.Value < atLeast) throw new ArgumentOutOfRangeException(nameof(atMost));

                Subexpr = subexpr;
                AtLeast = atLeast;
                AtMost = atMost;
            }

            public override bool Equals(RegExAst other) => other is Quantified q 
                && Subexpr.Equals(q.Subexpr)
                && AtLeast == q.AtLeast
                && AtMost == q.AtMost;
            public override int GetHashCode() => HashCode.Combine(Subexpr, AtLeast, AtMost);

            public override RegExAst Desugar()
            {
                if (AtLeast == 0)
                {
                    // Basically just Rep0
                    if (AtMost == null) return new Rep0(Subexpr.Desugar());
                    // Epsilon-transition
                    if (AtMost == 0)
                    {
                        // TODO
                        throw new NotSupportedException();
                    }
                    // Basically just optional
                    if (AtMost == 1) return new Opt(Subexpr.Desugar());
                    // 0..AtMost repeat
                    var sub = Subexpr.Desugar();
                    var result = sub;
                    for (int i = 0; i < AtMost.Value; ++i) result = new Seq(result, sub);
                    return result;
                }
                else
                {
                    // Create the prefix
                    var sub = Subexpr.Desugar();
                    var result = sub;
                    for (int i = 0; i < AtLeast; ++i) result = new Seq(result, sub);
                    // Rep0 after the prefix
                    if (AtMost == null) return new Seq(result, new Rep0(sub));
                    // Just these
                    if (AtMost == AtLeast) return result;
                    // We need a sequence of optional constructs
                    RegExAst after = new Opt(sub);
                    for (int i = AtLeast + 1; i < AtMost.Value; ++i) after = new Seq(after, new Opt(sub));
                    return new Seq(result, after);
                }
            }

            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa) =>
                throw new NotSupportedException();
        }
    }
}
