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
    /// Uses a parser to parse an optional element.
    /// </summary>
    /// <typeparam name="TResult">The result type of the sub-parser.</typeparam>
    public class Optional<TResult> : IParser<TResult?>
    {
        /// <summary>
        /// The item parser.
        /// </summary>
        public IParser<TResult> Item { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Optional{TResult}"/> class.
        /// </summary>
        /// <param name="item">The item parser.</param>
        public Optional(IParser<TResult> item)
        {
            this.Item = item;
        }

        /// <inheritdoc/>
        public ParseResult<TResult?> Parse<TItem>(IPeekableStream<TItem> stream) =>
            Combinator.Parse(this, stream);

        /// <inheritdoc/>
        public ParseResult<TResult?> ParsePeek<TItem>(IPeekableStream<TItem> stream, int offset)
        {
            var result = this.Item.ParsePeek(stream, offset);
            return result.IsOk
                ? ParseResult.Ok((TResult?)result.Ok.Value, result.Ok.Offset, result.Ok.FurthestError)
                : ParseResult.Ok(default(TResult?), offset, result.FurthestError);
        }
    }
}
