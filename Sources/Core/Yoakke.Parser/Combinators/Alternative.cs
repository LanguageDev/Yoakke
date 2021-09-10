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
    /// Combines two other parsers into an alternative.
    /// </summary>
    /// <typeparam name="TResult">The parser result.</typeparam>
    public class Alternative<TResult> : IParser<TResult>
    {
        /// <summary>
        /// The first parser to invoke.
        /// </summary>
        public IParser<TResult> Left { get; }

        /// <summary>
        /// The second parser to invoke.
        /// </summary>
        public IParser<TResult> Right { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Alternative{TResult}"/> class.
        /// </summary>
        /// <param name="left">The first parser to invoke.</param>
        /// <param name="right">The second parser to invoke.</param>
        public Alternative(IParser<TResult> left, IParser<TResult> right)
        {
            this.Left = left;
            this.Right = right;
        }

        /// <inheritdoc/>
        public ParseResult<TResult> Parse<TItem>(IPeekableStream<TItem> stream) => Combinator.Parse(this, stream);

        /// <inheritdoc/>
        public ParseResult<TResult> ParsePeek<TItem>(IPeekableStream<TItem> stream, int offset)
        {
            var first = this.Left.ParsePeek(stream, offset);
            var second = this.Right.ParsePeek(stream, offset);
            return first | second;
        }
    }
}
