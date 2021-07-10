// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.Utilities.Compatibility;
using Yoakke.Utilities.FiniteAutomata;

namespace Yoakke.Utilities.RegEx
{
    public partial class RegExAst
    {
        /// <summary>
        /// Represents that a regex construct should be repeated in some way.
        /// </summary>
        public class Quantified : RegExAst
        {
            /// <summary>
            /// The sub-construct to repeat.
            /// </summary>
            public RegExAst Subexpr { get; }

            /// <summary>
            /// The minimum number of repetitions. Can be zero.
            /// </summary>
            public int AtLeast { get; }

            /// <summary>
            /// The maximum number of repetitions, if any.
            /// </summary>
            public int? AtMost { get; }

            public Quantified(RegExAst subexpr, int atLeast, int? atMost)
            {
                if (atLeast < 0) throw new ArgumentOutOfRangeException(nameof(atLeast));
                if (atMost != null && atMost.Value < atLeast) throw new ArgumentOutOfRangeException(nameof(atMost));

                this.Subexpr = subexpr;
                this.AtLeast = atLeast;
                this.AtMost = atMost;
            }

            /// <inheritdoc/>
            public override bool Equals(RegExAst other) => other is Quantified q
                && this.Subexpr.Equals(q.Subexpr)
                && this.AtLeast == q.AtLeast
                && this.AtMost == q.AtMost;

            /// <inheritdoc/>
            public override int GetHashCode() => HashCode.Combine(this.Subexpr, this.AtLeast, this.AtMost);

            /// <inheritdoc/>
            public override RegExAst Desugar()
            {
                if (this.AtLeast == 0)
                {
                    // Basically just Rep0
                    if (this.AtMost == null) return new Rep0(this.Subexpr.Desugar());
                    // Epsilon-transition
                    if (this.AtMost == 0)
                    {
                        // TODO
                        throw new NotSupportedException();
                    }
                    // Basically just optional
                    if (this.AtMost == 1) return new Opt(this.Subexpr.Desugar());
                    // 0..AtMost repeat
                    var sub = this.Subexpr.Desugar();
                    var result = sub;
                    for (var i = 0; i < this.AtMost.Value; ++i) result = new Seq(result, sub);
                    return result;
                }
                else
                {
                    // Create the prefix
                    var sub = this.Subexpr.Desugar();
                    var result = sub;
                    for (var i = 0; i < this.AtLeast; ++i) result = new Seq(result, sub);
                    // Rep0 after the prefix
                    if (this.AtMost == null) return new Seq(result, new Rep0(sub));
                    // Just these
                    if (this.AtMost == this.AtLeast) return result;
                    // We need a sequence of optional constructs
                    RegExAst after = new Opt(sub);
                    for (var i = this.AtLeast + 1; i < this.AtMost.Value; ++i) after = new Seq(after, new Opt(sub));
                    return new Seq(result, after);
                }
            }

            /// <inheritdoc/>
            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa) =>
                throw new NotSupportedException();
        }
    }
}
