// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Streams
{
    /// <summary>
    /// Extensions for stream objects.
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Consumes the upcoming item in the stream.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <param name="stream">The <see cref="IStream{TItem}"/> to peek in.</param>
        /// <returns>The consumed item.</returns>
        public static TItem Consume<TItem>(this IStream<TItem> stream) => stream.TryConsume(out var item)
            ? item
            : throw new InvalidOperationException("The stream had no more items.");

        /// <summary>
        /// Retrieves the upcoming item without consuming it.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <param name="stream">The <see cref="IStream{TItem}"/> to peek in.</param>
        /// <returns>The next item in the stream.</returns>
        public static TItem Peek<TItem>(this IPeekableStream<TItem> stream) => stream.LookAhead(0);

        /// <summary>
        /// Peeks ahead a given amount of items without consuming them. With <paramref name="offset"/> set to 0
        /// this is equivalent to <see cref="Peek"/>.
        /// </summary>
        /// <typeparam name="TItem">The item type of the stream.</typeparam>
        /// <param name="stream">The <see cref="IPeekableStream{TItem}"/> to peek in.</param>
        /// <param name="offset">The offset to look ahead.</param>
        /// <returns>The peeked item.</returns>
        public static TItem LookAhead<TItem>(this IPeekableStream<TItem> stream, int offset) => stream.TryLookAhead(offset, out var item)
            ? item
            : throw new InvalidOperationException("The stream had no more items.");
    }
}
