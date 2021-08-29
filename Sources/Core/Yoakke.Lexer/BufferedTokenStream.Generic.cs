// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections;

namespace Yoakke.Lexer
{
    /// <summary>
    /// An <see cref="ITokenStream{TToken}"/> implementation with a backing buffer.
    /// Only assumes the underlying stream to implement <see cref="ITokenStream{TToken}.IsEnd"/> and
    /// <see cref="ITokenStream{TToken}.Advance()"/>.
    /// </summary>
    /// <typeparam name="TToken">The exact type of <see cref="IToken"/>.</typeparam>
    public class BufferedTokenStream<TToken> : ITokenStream<TToken>
        where TToken : IToken
    {
        /// <summary>
        /// The underlying stream.
        /// </summary>
        public ITokenStream<TToken> Underlying { get; }

        /// <inheritdoc/>
        public bool IsEnd => this.Underlying.IsEnd && this.peek.Count == 0;

        private readonly RingBuffer<TToken> peek = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedTokenStream{TToken}"/> class.
        /// </summary>
        /// <param name="underlying">The underlying stream.</param>
        public BufferedTokenStream(ITokenStream<TToken> underlying)
        {
            this.Underlying = underlying;
        }

        /// <inheritdoc/>
        public TToken Peek() => this.LookAhead(0);

        /// <inheritdoc/>
        public TToken LookAhead(int offset)
        {
            for (; this.peek.Count <= offset; this.peek.AddBack(this.Underlying.Advance()))
            {
                // Pass
            }
            return this.peek[offset];
        }

        /// <inheritdoc/>
        public TToken Advance()
        {
            var t = this.Peek();
            this.peek.RemoveFront();
            return t;
        }

        /// <inheritdoc/>
        public void Advance(int amount)
        {
            if (amount == 0) return;
            this.LookAhead(amount - 1);
            for (var i = 0; i < amount; ++i) this.peek.RemoveFront();
        }

        /// <inheritdoc/>
        public void Defer(TToken token) => this.peek.AddFront(token);
    }
}
