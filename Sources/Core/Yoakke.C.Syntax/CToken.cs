// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.Lexer;
using Yoakke.Text;

namespace Yoakke.C.Syntax
{
    /// <summary>
    /// The token produced by lexing C source code.
    /// </summary>
    public sealed class CToken : IToken<CTokenType>, IEquatable<CToken>
    {
        public Text.Range Range { get; }

        public string Text { get; }

        public CTokenType Kind { get; }

        /// <summary>
        /// The original text value without escaped digraph, trigraphs and line-continuations.
        /// </summary>
        public string OriginalText { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CToken"/> class.
        /// </summary>
        /// <param name="range">The <see cref="Text.Range"/> of the <see cref="CToken"/> in the source.</param>
        /// <param name="text">The text the <see cref="CToken"/>.</param>
        /// <param name="originalText">The original text the <see cref="CToken"/> was parsed from.</param>
        /// <param name="kind">The <see cref="CTokenType"/> of the <see cref="CToken"/>.</param>
        public CToken(Text.Range range, string text, string originalText, CTokenType kind)
        {
            this.Range = range;
            this.Text = text;
            this.Kind = kind;
            this.OriginalText = originalText;
        }

        public bool Equals(IToken? other) => this.Equals(other as CToken);

        public bool Equals(IToken<CTokenType>? other) => this.Equals(other as CToken);

        public bool Equals(CToken? other) =>
               other is not null
            && this.Range == other.Range
            && this.Text == other.Text
            && this.Kind == other.Kind;
    }
}
