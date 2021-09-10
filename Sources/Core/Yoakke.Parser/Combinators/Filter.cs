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
    /// Filters a successful result with a predicate.
    /// </summary>
    /// <typeparam name="TResult">The presult type of the parse.</typeparam>
    public class Filter<TResult> : IParser<TResult>
    {
        /// <summary>
        /// The item parser.
        /// </summary>
        public IParser<TResult> Item { get; }

        /// <summary>
        /// The predicate that filters the result.
        /// </summary>
        public Predicate<TResult> Predicate { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Filter{TResult}"/> class.
        /// </summary>
        /// <param name="item">The item parser.</param>
        /// <param name="predicate">The predicate that filters the result.</param>
        public Filter(IParser<TResult> item, Predicate<TResult> predicate)
        {
            this.Item = item;
            this.Predicate = predicate;
        }

        /// <inheritdoc/>
        public ParseResult<TResult> Parse<TItem>(IPeekableStream<TItem> stream) =>
            Combinator.Parse(this, stream);

        /// <inheritdoc/>
        public ParseResult<TResult> ParsePeek<TItem>(IPeekableStream<TItem> stream, int offset)
        {
            var result = this.Item.ParsePeek(stream, offset);
            if (result.IsError) return result;
            if (!this.Predicate(result.Ok.Value)) return ParseResult.Error("filtered item", null, this.Item.ToString());
            return result;
        }
    }
}
