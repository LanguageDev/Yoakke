// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Yoakke.Lexer.Streams
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
        /// Retrieves the upcoming token without consuming it.
        /// </summary>
        /// <param name="token">The peeked token gets written here, if there was any.</param>
        /// <returns>True, if there was a token to peek.</returns>
        public bool TryPeek([MaybeNullWhen(false)] out TToken token);

        /// <summary>
        /// Peeks ahead a given amount of tokens without consuming them. With <paramref name="offset"/> set to 0
        /// this is equivalent to <see cref="TryPeek"/>.
        /// </summary>
        /// <param name="offset">The offset to look ahead.</param>
        /// <param name="token">The peeked token gets written here, if there was any.</param>
        /// <returns>True, if there was a token to peek.</returns>
        public bool TryLookAhead(int offset, [MaybeNullWhen(false)] out TToken token);

        /// <summary>
        /// Consumes the upcoming token in the stream.
        /// </summary>
        /// <param name="token">The consumed token gets written here, if there was any.</param>
        /// <returns>True, if there was a token to advance.</returns>
        public bool TryAdvance([MaybeNullWhen(false)] out TToken token);

        /// <summary>
        /// Consumes a given amount of tokens in the stream.
        /// </summary>
        /// <param name="amount">The number of tokens to advance.</param>
        /// <returns>The number of tokens actually consumed.</returns>
        public int Advance(int amount);

        /// <summary>
        /// Pushes a token to the front of the stream.
        /// This operation might not be supported by all streams.
        /// </summary>
        /// <param name="token">The token to push into the front.</param>
        public void Defer(TToken token);
    }
}
