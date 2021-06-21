// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.IO;
using System.Text;
using Yoakke.Collections;
using Yoakke.Text;
using Range = Yoakke.Text.Range;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Base-class to provide common functionality for lexers, if a custom solution is needed.
    /// </summary>
    /// <typeparam name="TKind">The token-kind of the <see cref="IToken{TKind}"/>s this <see cref="LexerBase{TKind}"/> produces.</typeparam>
    public abstract class LexerBase<TKind> : LexerBaseCommon, ILexer<TKind>
        where TKind : notnull
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LexerBase{T}"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> that reads the source text.</param>
        protected LexerBase(TextReader reader)
            : base(reader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexerBase{T}"/> class.
        /// </summary>
        /// <param name="source">The string to lex.</param>
        protected LexerBase(string source)
            : base(source)
        {
        }

        /// <summary>
        /// Skips characters in the input and builds a <see cref="IToken{T}"/> with a given factory function.
        /// </summary>
        /// <typeparam name="TToken">The exact token type to produce.</typeparam>
        /// <param name="length">The amount of characters to skip.</param>
        /// <param name="makeToken">The factory function that receives the source <see cref="Range"/> of the skipped characters
        /// and the skipped characters themselves concatenated as a string, and produces an <see cref="IToken{T}"/> from them.</param>
        /// <returns>The constructed <see cref="IToken{TKind}"/> returned from <paramref name="makeToken"/>.</returns>
        protected new TToken TakeToken<TToken>(int length, Func<Range, string, TToken> makeToken)
            where TToken : IToken<TKind>
        {
            var start = this.Position;
            var text = length == 0 ? string.Empty : this.Take(length);
            var range = new Range(start, this.Position);
            return makeToken(range, text);
        }

        /// <summary>
        /// Skips characters in the input and builds a <see cref="Token{T}"/> from the skipped characters.
        /// </summary>
        /// <param name="kind">The token-kind to build.</param>
        /// <param name="length">The amount of characters to skip.</param>
        /// <returns>The constructed <see cref="Token{T}"/>.</returns>
        protected Token<TKind> TakeToken(TKind kind, int length)
        {
            var start = this.Position;
            var text = length == 0 ? string.Empty : this.Take(length);
            var range = new Range(start, this.Position);
            return new Token<TKind>(range, text, kind);
        }

        IToken ILexer.Next() => this.Next();

        /// <summary>
        /// Lexes the next <see cref="IToken{T}"/> in the input.
        /// </summary>
        /// <returns>The lexed <see cref="IToken{T}"/>.</returns>
        public abstract IToken<TKind> Next();
    }
}
