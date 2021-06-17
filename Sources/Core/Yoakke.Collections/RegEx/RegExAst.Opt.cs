// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Collections.FiniteAutomata;

namespace Yoakke.Collections.RegEx
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

            public Opt(RegExAst subexpr)
            {
                this.Subexpr = subexpr;
            }

            public override bool Equals(RegExAst other) => other is Opt opt && this.Subexpr.Equals(opt.Subexpr);

            public override int GetHashCode() => this.Subexpr.GetHashCode();

            public override RegExAst Desugar() => new Opt(this.Subexpr.Desugar());

            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                var (start, end) = this.Subexpr.ThompsonConstruct(denseNfa);
                denseNfa.AddTransition(start, Epsilon.Default, end);
                return (start, end);
            }
        }
    }
}
