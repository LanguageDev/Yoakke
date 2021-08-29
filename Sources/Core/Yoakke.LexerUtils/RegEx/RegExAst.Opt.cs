// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.LexerUtils.FiniteAutomata;

namespace Yoakke.LexerUtils.RegEx
{
    public partial class RegExAst
    {
        /// <summary>
        /// Represents an optional construct.
        /// </summary>
        public class Opt : RegExAst
        {
            /// <summary>
            /// The optional sub-construct.
            /// </summary>
            public RegExAst Subexpr { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Opt"/> class.
            /// </summary>
            /// <param name="subexpr">The optional subexpression to match.</param>
            public Opt(RegExAst subexpr)
            {
                this.Subexpr = subexpr;
            }

            /// <inheritdoc/>
            public override bool Equals(RegExAst other) => other is Opt opt && this.Subexpr.Equals(opt.Subexpr);

            /// <inheritdoc/>
            public override int GetHashCode() => this.Subexpr.GetHashCode();

            /// <inheritdoc/>
            public override RegExAst Desugar() => new Opt(this.Subexpr.Desugar());

            /// <inheritdoc/>
            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                var (start, end) = this.Subexpr.ThompsonConstruct(denseNfa);
                denseNfa.AddTransition(start, Epsilon.Instance, end);
                return (start, end);
            }
        }
    }
}
