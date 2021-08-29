// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Represents a stream of <see cref="IToken"/>s that can be read and (optionally) written sequentially.
    /// </summary>
    /// <typeparam name="TToken">The exact type of <see cref="IToken"/>s produced.</typeparam>
    public interface ITokenStream<TToken>
        where TToken : IToken
    {
        /// <summary>
        /// True, if the stream is out of tokens.
        /// </summary>
        public bool IsEnd { get; }

        /// <summary>
        /// Returns the upcoming token without consuming it.
        /// </summary>
        /// <returns>The next token in the stream.</returns>
        public TToken Peek();

        /// <summary>
        /// Peeks ahead a given amount of tokens without consuming them. With <paramref name="offset"/> set to 0
        /// this is equivalent to <see cref="Peek"/>.
        /// </summary>
        /// <param name="offset">The offset to look ahead.</param>
        /// <returns>The token <paramref name="offset"/> amount over.</returns>
        public TToken LookAhead(int offset);

        /// <summary>
        /// Consumes the upcoming token in the stream.
        /// </summary>
        /// <returns>The consumed token.</returns>
        public TToken Advance();

        /// <summary>
        /// Consumes a given amount of tokens in the stream.
        /// </summary>
        /// <param name="amount">The number of tokens to advance.</param>
        public void Advance(int amount);

        /// <summary>
        /// Pushes a token to the front of the stream.
        /// This operation might not be supported by all streams.
        /// </summary>
        /// <param name="token">The token to push into the front.</param>
        public void Defer(TToken token);
    }
}
