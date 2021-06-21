// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.Lexer;
using Yoakke.Text;

namespace Yoakke.C.Syntax
{
    /// <summary>
    /// A C-specific <see cref="Token{TKind}"/>.
    /// </summary>
    public sealed class CToken : Token<CTokenType>
    {
        /// <summary>
        /// The original text the token originated from.
        /// </summary>
        public string OriginalText { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CToken"/> class.
        /// </summary>
        /// <param name="range">The <see cref="Range"/> of the <see cref="CToken"/> in the source.</param>
        /// <param name="text">The escaped text the <see cref="CToken"/> was parsed from.</param>
        /// <param name="originalText">The original text with line continuations, digraphs and trigraphs
        /// the <see cref="CToken"/> was parsed from.</param>
        /// <param name="kind">The <see cref="CTokenType"/> of the <see cref="CToken"/>.</param>
        public CToken(Text.Range range, string text, string originalText, CTokenType kind)
            : base(range, text, kind)
        {
            this.OriginalText = originalText;
        }

        public override bool Equals(Token<CTokenType>? other) =>
               other is CToken cToken
            && base.Equals(cToken)
            && this.OriginalText == cToken.OriginalText;

        public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), this.OriginalText);
    }
}
