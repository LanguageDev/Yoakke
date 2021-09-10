// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Streams;

namespace Yoakke.Parser
{
    /// <summary>
    /// Parser combinator logic implementation.
    /// </summary>
    public static class Combinator
    {
        /// <summary>
        /// Represents a generic parser function. Any function can be used for parsing, that conforms this signature.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <typeparam name="TResult">The parsed value type.</typeparam>
        /// <param name="stream">The stream to parse from.</param>
        /// <param name="offset">The offset to start parsing from.</param>
        /// <returns>The result of the parse.</returns>
        public delegate ParseResult<TResult> Parser<TItem, TResult>(IPeekableStream<TItem> stream, int offset);

        /// <summary>
        /// Applies a parser to some input, consuming the parsed tokens on success.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <typeparam name="TResult">The parsed value type.</typeparam>
        /// <param name="parser">The parser to apply.</param>
        /// <param name="stream">The stream to parse from.</param>
        /// <returns>The result of parsing with <paramref name="parser"/> on <paramref name="stream"/>.</returns>
        public static ParseResult<TResult> Parse<TItem, TResult>(this Parser<TItem, TResult> parser, IPeekableStream<TItem> stream)
        {
            var result = parser(stream, 0);
            if (result.IsOk) stream.Consume(result.Ok.Offset);
            return result;
        }

        #region Core Combinators

        /// <summary>
        /// Constructs an alternative of parsers.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <typeparam name="TResult">The parsed value type.</typeparam>
        /// <param name="left">The first alternative parser to combine.</param>
        /// <param name="right">The second alternative parser to combine.</param>
        /// <returns>A new parser that tries both <paramref name="left"/> and <paramref name="right"/>, and returns
        /// the succeeding result that got further.</returns>
        public static Parser<TItem, TResult> Alt<TItem, TResult>(Parser<TItem, TResult> left, Parser<TItem, TResult> right) =>
            (stream, offset) => left(stream, offset) | right(stream, offset);

        /// <summary>
        /// Constructs a sequence of parsers.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <typeparam name="TLeftResult">The parsed value type of <paramref name="left"/>.</typeparam>
        /// <typeparam name="TRightResult">The parsed value type of <paramref name="right"/>.</typeparam>
        /// <param name="left">The first parser to combine.</param>
        /// <param name="right">The second parser to combine.</param>
        /// <returns>A new parser that first tries to parse <paramref name="left"/>, and if that succeeds,
        /// parses <paramref name="right"/>, and returns their combined results in a tuple.</returns>
        public static Parser<TItem, (TLeftResult, TRightResult)> Seq<TItem, TLeftResult, TRightResult>(
            Parser<TItem, TLeftResult> left,
            Parser<TItem, TRightResult> right) =>
            (stream, offset) =>
            {
                // Invoke first
                var first = left(stream, offset);
                if (first.IsError) return first.Error;
                var firstOk = first.Ok;
                // Invoke second
                var second = right(stream, firstOk.Offset);
                if (second.IsError) return (firstOk.FurthestError | second.Error)!;
                var secondOk = second.Ok;
                // Combine
                var value = (firstOk.Value, secondOk.Value);
                var error = firstOk.FurthestError | secondOk.FurthestError;
                return ParseResult.Ok(value, secondOk.Offset, error);
            };

        #endregion
    }
}
