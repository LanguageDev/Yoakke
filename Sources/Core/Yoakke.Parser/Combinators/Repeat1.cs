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
    /// Uses a parser to parse 1 of more occurrences of an item.
    /// </summary>
    /// <typeparam name="TResult">The result type of a sub-parser.</typeparam>
    public class Repeat1<TResult> : IParser<IReadOnlyList<TResult>>
    {
        /// <summary>
        /// The item parser.
        /// </summary>
        public IParser<TResult> Item { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Repeat1{TResult}"/> class.
        /// </summary>
        /// <param name="item">The item parser.</param>
        public Repeat1(IParser<TResult> item)
        {
            this.Item = item;
        }

        /// <inheritdoc/>
        public ParseResult<IReadOnlyList<TResult>> Parse<TItem>(IPeekableStream<TItem> stream) =>
            Combinator.Parse(this, stream);

        /// <inheritdoc/>
        public ParseResult<IReadOnlyList<TResult>> ParsePeek<TItem>(IPeekableStream<TItem> stream, int offset)
        {
            var result = new List<TResult>();
            ParseError? error = null;
            while (true)
            {
                var item = this.Item.ParsePeek(stream, offset);
                if (item.IsError)
                {
                    error = error | item.Error;
                    break;
                }
                offset = item.Ok.Offset;
                result.Add(item.Ok.Value);
                error = error | item.Ok.FurthestError;
            }
            return result.Count == 0
                ? error!
                : ParseResult.Ok((IReadOnlyList<TResult>)result, offset, error);
        }
    }
}
