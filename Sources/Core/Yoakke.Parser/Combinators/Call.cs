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
    /// Calls a function as a parser.
    /// </summary>
    /// <typeparam name="TItem">The item type of the parsed stream.</typeparam>
    /// <typeparam name="TResult">The parse result type.</typeparam>
    public class Call<TItem, TResult> : IParser<TResult>
    {
        /// <summary>
        /// The parser function.
        /// </summary>
        public Func<IPeekableStream<TItem>, int, ParseResult<TResult>> Func { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Call{TItem, TResult}"/> class.
        /// </summary>
        /// <param name="func">The parser function.</param>
        public Call(Func<IPeekableStream<TItem>, int, ParseResult<TResult>> func)
        {
            this.Func = func;
        }

        /// <inheritdoc/>
        public ParseResult<TResult> Parse<TItemV>(IPeekableStream<TItemV> stream) =>
            Combinator.Parse(this, stream);

        /// <inheritdoc/>
        public ParseResult<TResult> ParsePeek<TItemV>(IPeekableStream<TItemV> stream, int offset)
        {
            if (typeof(TItem) != typeof(TItemV)) throw new InvalidOperationException("Different stream element type");
            return this.Func((IPeekableStream<TItem>)stream, offset);
        }
    }
}
