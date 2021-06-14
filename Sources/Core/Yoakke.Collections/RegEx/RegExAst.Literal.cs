// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.Collections.FiniteAutomata;

namespace Yoakke.Collections.RegEx
{
    public partial class RegExAst
    {
        /// <summary>
        /// Represents a single literal character to match.
        /// </summary>
        public class Literal : RegExAst
        {
            /// <summary>
            /// The character to match.
            /// </summary>
            public readonly char Char;

            public Literal(char @char)
            {
                this.Char = @char;
            }

            public override bool Equals(RegExAst other) => other is Literal lit && this.Char == lit.Char;

            public override int GetHashCode() => this.Char.GetHashCode();

            public override RegExAst Desugar() => this;

            public override (State Start, State End) ThompsonConstruct(DenseNfa<char> denseNfa)
            {
                var start = denseNfa.NewState();
                var end = denseNfa.NewState();

                denseNfa.AddTransition(start, this.Char, end);

                return (start, end);
            }
        }
    }
}
