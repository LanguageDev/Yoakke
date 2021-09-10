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
    /// Combinator extensions.
    /// </summary>
    public static class Combinator
    {
        /// <summary>
        /// Default implementation for <see cref="IParser{TResult}.Parse{TItem}(IPeekableStream{TItem})"/>.
        /// </summary>
        /// <typeparam name="TResult">The parsed value type.</typeparam>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <param name="parser">The parser to parse with.</param>
        /// <param name="stream">The stream to parse from.</param>
        /// <returns>The result of the parse.</returns>
        public static ParseResult<TResult> Parse<TResult, TItem>(IParser<TResult> parser, IPeekableStream<TItem> stream)
        {
            var result = parser.Parse(stream);
            if (result.IsOk) stream.Consume(result.Ok.Offset);
            return result;
        }
    }
}
