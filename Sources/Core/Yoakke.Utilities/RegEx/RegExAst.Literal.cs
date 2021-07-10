// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Utilities.FiniteAutomata;

namespace Yoakke.Utilities.RegEx
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
            public char Char { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="Literal"/> class.
            /// </summary>
            /// <param name="char">The character literal to match.</param>
            public Literal(char @char)
            {
                this.Char = @char;
            }

            /// <inheritdoc/>
            public override bool Equals(RegExAst other) => other is Literal lit && this.Char == lit.Char;

            /// <inheritdoc/>
            public override int GetHashCode() => this.Char.GetHashCode();

            /// <inheritdoc/>
            public override RegExAst Desugar() => this;

            /// <inheritdoc/>
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
