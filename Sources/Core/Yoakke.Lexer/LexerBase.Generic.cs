// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.IO;
using System.Text;
using Yoakke.Utilities;
using Yoakke.Text;
using Range = Yoakke.Text.Range;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Base-class to provide common functionality for lexers, if a custom solution is needed.
    /// </summary>
    /// <typeparam name="TToken">The exact type of token this <see cref="LexerBase{TToken}"/> produces.</typeparam>
    public abstract class LexerBase<TToken> : LexerBaseCommon, ILexer<TToken>
        where TToken : IToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LexerBase{TToken}"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> that reads the source text.</param>
        protected LexerBase(TextReader reader)
            : base(reader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexerBase{TToken}"/> class.
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
        protected TToken TakeToken(int length, Func<Range, string, TToken> makeToken) =>
            base.TakeToken<TToken>(length, makeToken);

        IToken ILexer.Next() => this.Next();

        /// <summary>
        /// Lexes the next <see cref="TToken"/> in the input.
        /// </summary>
        /// <returns>The lexed <see cref="TToken"/>.</returns>
        public abstract TToken Next();
    }
}
