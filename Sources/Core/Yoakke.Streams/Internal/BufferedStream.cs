// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Yoakke.Collections;

namespace Yoakke.Streams.Internal
{
    /// <summary>
    /// An <see cref="IPeekableStream{TStream}"/> implementation that wraps an <see cref="IStream{TStream}"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type.</typeparam>
    /// <typeparam name="TStream">The underlying stream type.</typeparam>
    internal class BufferedStream<TItem, TStream> : IPeekableStream<TItem>
        where TStream : IStream<TItem>
    {
        /// <summary>
        /// The underlying stream.
        /// </summary>
        public TStream Underlying { get; }

        /// <inheritdoc/>
        public bool IsEnd => this.Underlying.IsEnd && this.peek.Count == 0;

        private readonly RingBuffer<TItem> peek = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedStream{TItem, TStream}"/> class.
        /// </summary>
        /// <param name="underlying">The underlying stream.</param>
        public BufferedStream(TStream underlying)
        {
            this.Underlying = underlying;
        }

        /// <inheritdoc/>
        public bool TryPeek([MaybeNullWhen(false)] out TItem item) => this.TryLookAhead(0, out item);

        /// <inheritdoc/>
        public bool TryLookAhead(int offset, [MaybeNullWhen(false)] out TItem item)
        {
            while (this.peek.Count <= offset)
            {
                if (this.Underlying.TryConsume(out var i)) this.peek.AddBack(i);
                else break;
            }
            if (this.peek.Count > offset)
            {
                item = this.peek[offset];
                return true;
            }
            else
            {
                item = default;
                return false;
            }
        }

        /// <inheritdoc/>
        public bool TryConsume([MaybeNullWhen(false)] out TItem item)
        {
            if (!this.TryPeek(out item)) return false;
            this.peek.RemoveFront();
            return true;
        }

        /// <inheritdoc/>
        public int Consume(int amount)
        {
            if (amount == 0) return 0;

            this.TryLookAhead(amount - 1, out var _);
            var result = Math.Min(this.peek.Count, amount);
            for (var i = 0; i < result; ++i) this.peek.RemoveFront();
            return result;
        }

        /// <inheritdoc/>
        public void Defer(TItem item) => this.peek.AddFront(item);
    }
}
