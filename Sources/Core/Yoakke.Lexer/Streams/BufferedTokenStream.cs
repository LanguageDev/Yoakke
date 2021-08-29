// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Yoakke.Collections;

namespace Yoakke.Lexer.Streams
{
    /// <summary>
    /// An <see cref="ITokenStream{TToken}"/> implementation with a backing buffer.
    /// Only assumes the underlying stream to implement <see cref="ITokenStream{TToken}.IsEnd"/> and
    /// <see cref="ITokenStream{TToken}.TryAdvance"/>.
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
        public bool TryPeek([MaybeNullWhen(false)] out TToken token) => this.TryLookAhead(0, out token);

        /// <inheritdoc/>
        public bool TryLookAhead(int offset, [MaybeNullWhen(false)] out TToken token)
        {
            while (this.peek.Count <= offset)
            {
                if (this.Underlying.TryAdvance(out var t)) this.peek.AddBack(t);
                else break;
            }
            if (this.peek.Count > offset)
            {
                token = this.peek[offset];
                return true;
            }
            else
            {
                token = default;
                return false;
            }
        }

        /// <inheritdoc/>
        public bool TryAdvance([MaybeNullWhen(false)] out TToken token)
        {
            if (!this.TryPeek(out token)) return false;
            this.peek.RemoveFront();
            return true;
        }

        /// <inheritdoc/>
        public int Advance(int amount)
        {
            if (amount == 0) return 0;

            this.TryLookAhead(amount - 1, out var _);
            var result = Math.Min(this.peek.Count, amount);
            for (var i = 0; i < result; ++i) this.peek.RemoveFront();
            return result;
        }

        /// <inheritdoc/>
        public void Defer(TToken token) => this.peek.AddFront(token);
    }
}
