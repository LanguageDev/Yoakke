// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Lexer.Streams
{
    /// <summary>
    /// Extensions for <see cref="ITokenStream{TToken}"/>.
    /// </summary>
    public static class TokenStreamExtensions
    {
        /// <summary>
        /// Retrieves the upcoming token without consuming it.
        /// </summary>
        /// <typeparam name="TToken">The exact token type of the stream.</typeparam>
        /// <param name="stream">The <see cref="ITokenStream{TToken}"/> to peek in.</param>
        /// <returns>The next token in the stream.</returns>
        public static TToken Peek<TToken>(this ITokenStream<TToken> stream)
            where TToken : IToken => stream.TryPeek(out var token)
            ? token
            : throw new InvalidOperationException("The stream had no more tokens.");

        /// <summary>
        /// Peeks ahead a given amount of tokens without consuming them. With <paramref name="offset"/> set to 0
        /// this is equivalent to <see cref="Peek"/>.
        /// </summary>
        /// <typeparam name="TToken">The exact token type of the stream.</typeparam>
        /// <param name="stream">The <see cref="ITokenStream{TToken}"/> to peek in.</param>
        /// <param name="offset">The offset to look ahead.</param>
        /// <returns>The peeked token.</returns>
        public static TToken LookAhead<TToken>(this ITokenStream<TToken> stream, int offset)
            where TToken : IToken => stream.TryLookAhead(offset, out var token)
            ? token
            : throw new InvalidOperationException("The stream had no more tokens.");

        /// <summary>
        /// Consumes the upcoming token in the stream.
        /// </summary>
        /// <typeparam name="TToken">The exact token type of the stream.</typeparam>
        /// <param name="stream">The <see cref="ITokenStream{TToken}"/> to peek in.</param>
        /// <returns>The consumed token.</returns>
        public static TToken Advance<TToken>(this ITokenStream<TToken> stream)
            where TToken : IToken => stream.TryAdvance(out var token)
            ? token
            : throw new InvalidOperationException("The stream had no more tokens.");
    }
}
