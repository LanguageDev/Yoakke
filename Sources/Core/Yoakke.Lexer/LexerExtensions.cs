// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Yoakke.Lexer.Streams;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Extensions for an <see cref="ILexer{TToken}"/>.
    /// </summary>
    public static class LexerExtensions
    {
        private class LexerAdapter<TToken> : ITokenStream<TToken>
            where TToken : IToken
        {
            public bool IsEnd => this.lexer.IsEnd;

            private readonly ILexer<TToken> lexer;

            public LexerAdapter(ILexer<TToken> lexer) => this.lexer = lexer;

            public bool TryAdvance([MaybeNullWhen(false)] out TToken token)
            {
                if (this.lexer.IsEnd)
                {
                    token = default;
                    return false;
                }
                else
                {
                    token = this.lexer.Next();
                    return true;
                }
            }

            public int Advance(int amount) => throw new NotSupportedException();

            public void Defer(TToken token) => throw new NotSupportedException();

            public bool TryLookAhead(int offset, [MaybeNullWhen(false)] out TToken token) => throw new NotSupportedException();

            public bool TryPeek([MaybeNullWhen(false)] out TToken token) => throw new NotSupportedException();
        }

        /// <summary>
        /// Adapts an <see cref="ILexer{TToken}"/> to be an <see cref="ITokenStream{TToken}"/>.
        /// </summary>
        /// <typeparam name="TToken">The exact <see cref="IToken"/> handled by the lexer.</typeparam>
        /// <param name="lexer">The <see cref="ILexer{TToken}"/> to adapt.</param>
        /// <returns>An <see cref="ITokenStream{TToken}"/> that supports all operations and reads from
        /// <paramref name="lexer"/>.</returns>
        public static ITokenStream<TToken> AsTokenStream<TToken>(this ILexer<TToken> lexer)
            where TToken : IToken => new BufferedTokenStream<TToken>(new LexerAdapter<TToken>(lexer));
    }
}
