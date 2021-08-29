// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Text;

namespace Yoakke.Lexer.Streams
{
    /// <summary>
    /// A general character stream to read from for lexers.
    /// </summary>
    public interface ICharStream
    {
        /// <summary>
        /// The current <see cref="Text.Position"/> the stream is at.
        /// </summary>
        public Position Position { get; }

        /// <summary>
        /// True, if the stream is out of characters.
        /// </summary>
        public bool IsEnd { get; }

        /// <summary>
        /// Retrieves the upcoming character without consuming it.
        /// </summary>
        /// <param name="ch">The peeked character gets written here, if there was any.</param>
        /// <returns>True, if there was a character to peek.</returns>
        public bool TryPeek(out char ch);

        /// <summary>
        /// Peeks ahead a given amount of characters without consuming them. With <paramref name="offset"/> set to 0
        /// this is equivalent to <see cref="TryPeek"/>.
        /// </summary>
        /// <param name="offset">The offset to look ahead.</param>
        /// <param name="ch">The peeked character gets written here, if there was any.</param>
        /// <returns>True, if there was a character to peek.</returns>
        public bool TryLookAhead(int offset, out char ch);

        /// <summary>
        /// Consumes the upcoming character in the stream.
        /// </summary>
        /// <param name="ch">The consumed character gets written here, if there was any.</param>
        /// <returns>True, if there was a character to advance.</returns>
        public bool TryAdvance(out char ch);

        /// <summary>
        /// Consumes a given amount of characters in the stream.
        /// </summary>
        /// <param name="amount">The number of characters to advance.</param>
        /// <returns>The number of characters actually consumed.</returns>
        public int Advance(int amount);
    }
}
