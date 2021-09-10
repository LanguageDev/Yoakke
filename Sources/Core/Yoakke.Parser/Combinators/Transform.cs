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
    /// A parser that transforms the succeeding result with a function.
    /// </summary>
    /// <typeparam name="TParsed">The parsed item type.</typeparam>
    /// <typeparam name="TResult">The result type.</typeparam>
    public class Transform<TParsed, TResult> : IParser<TResult>
    {
        /// <summary>
        /// The item parser.
        /// </summary>
        public IParser<TParsed> Item { get; }

        /// <summary>
        /// The transformer function.
        /// </summary>
        public Func<TParsed, TResult> Transformer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transform{TParsed, TResult}"/> class.
        /// </summary>
        /// <param name="item">The item parser.</param>
        /// <param name="transformer">The transformer function.</param>
        public Transform(IParser<TParsed> item, Func<TParsed, TResult> transformer)
        {
            this.Item = item;
            this.Transformer = transformer;
        }

        /// <inheritdoc/>
        public ParseResult<TResult> Parse<TItem>(IPeekableStream<TItem> stream) =>
            Combinator.Parse(this, stream);

        /// <inheritdoc/>
        public ParseResult<TResult> ParsePeek<TItem>(IPeekableStream<TItem> stream, int offset)
        {
            var result = this.Item.ParsePeek(stream, offset);
            if (result.IsOk)
            {
                var transformed = this.Transformer(result.Ok.Value);
                return ParseResult.Ok(transformed, result.Ok.Offset, result.Ok.FurthestError);
            }
            else
            {
                return result.Error;
            }
        }
    }
}
