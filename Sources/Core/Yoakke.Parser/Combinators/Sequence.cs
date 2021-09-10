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
    /// Combines two other parsers into a sequence.
    /// </summary>
    /// <typeparam name="TLeftResult">The first parser result.</typeparam>
    /// <typeparam name="TRightResult">The second parser result.</typeparam>
    public class Sequence<TLeftResult, TRightResult> : IParser<(TLeftResult, TRightResult)>
    {
        /// <summary>
        /// The first parser to invoke.
        /// </summary>
        public IParser<TLeftResult> Left { get; }

        /// <summary>
        /// The second parser to invoke.
        /// </summary>
        public IParser<TRightResult> Right { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence{TLeft, TRight}"/> class.
        /// </summary>
        /// <param name="left">The first parser to invoke.</param>
        /// <param name="right">The second parser to invoke.</param>
        public Sequence(IParser<TLeftResult> left, IParser<TRightResult> right)
        {
            this.Left = left;
            this.Right = right;
        }

        /// <inheritdoc/>
        public ParseResult<(TLeftResult, TRightResult)> Parse<TItem>(IPeekableStream<TItem> stream) =>
            Combinator.Parse(this, stream);

        /// <inheritdoc/>
        public ParseResult<(TLeftResult, TRightResult)> ParsePeek<TItem>(IPeekableStream<TItem> stream, int offset)
        {
            // Invoke first
            var first = this.Left.ParsePeek(stream, offset);
            if (first.IsError) return first.Error;
            var firstOk = first.Ok;
            // Invoke second
            var second = this.Right.ParsePeek(stream, firstOk.Offset);
            if (second.IsError) return (firstOk.FurthestError | second.Error)!;
            var secondOk = second.Ok;
            // Combine
            var value = (firstOk.Value, secondOk.Value);
            var error = firstOk.FurthestError | secondOk.FurthestError;
            return ParseResult.Ok(value, secondOk.Offset, error);
        }
    }
}
