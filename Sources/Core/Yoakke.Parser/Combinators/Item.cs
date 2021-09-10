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
    /// Parses a single item from the stream.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    public class Item<TItem> : IParser<TItem>
    {
        /// <summary>
        /// A default instance to use.
        /// </summary>
        public static readonly Item<TItem> Instance = new();

        /// <inheritdoc/>
        public ParseResult<TItem> Parse<TItemV>(IPeekableStream<TItemV> stream) =>
            Combinator.Parse(this, stream);

        /// <inheritdoc/>
        public ParseResult<TItem> ParsePeek<TItemV>(IPeekableStream<TItemV> stream, int offset)
        {
            if (typeof(TItem) != typeof(TItemV)) throw new InvalidOperationException("Different stream element type");
            if (stream.TryLookAhead(offset, out var item)) return ParseResult.Ok((TItem)(object)item!, offset + 1, null);
            else return ParseResult.Error("token", null, "item");
        }
    }
}
