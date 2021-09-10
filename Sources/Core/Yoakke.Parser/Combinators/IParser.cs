// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Streams;

namespace Yoakke.Parser.Combinators
{
    /// <summary>
    /// Represents the base for all parsers.
    /// </summary>
    /// <typeparam name="TResult">The parsed type.</typeparam>
    public interface IParser<TResult>
    {
        /// <summary>
        /// Parses an element and consumes from the underlying stream, if succeeded.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <param name="stream">The stream to parse from.</param>
        /// <returns>The result of the parse.</returns>
        public ParseResult<TResult> Parse<TItem>(IPeekableStream<TItem> stream);

        /// <summary>
        /// Parses from the given offset, without consuming anything.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <param name="stream">The stream to parse from.</param>
        /// <param name="offset">The offset to start parsing from.</param>
        /// <returns>The result of the parse.</returns>
        public ParseResult<TResult> ParsePeek<TItem>(IPeekableStream<TItem> stream, int offset);
    }
}
