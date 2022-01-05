// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;

namespace Yoakke.Streams;

/// <summary>
/// Extensions for stream objects.
/// </summary>
public static class StreamExtensions
{
    #region Adapters

    /// <summary>
    /// Converts a stream to an <see cref="IEnumerable{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <param name="stream">The stream to convert.</param>
    /// <returns><paramref name="stream"/> as an <see cref="IEnumerable{TItem}"/>.</returns>
    public static IEnumerable<TItem> ToEnumerable<TItem>(this IStream<TItem> stream)
    {
        while (stream.TryConsume(out var item)) yield return item;
    }

    /// <summary>
    /// Converts a stream to a <see cref="IPeekableStream{TItem}"/> by making it buffered.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <param name="stream">The stream to convert.</param>
    /// <returns><paramref name="stream"/> as an <see cref="IPeekableStream{TItem}"/>.</returns>
    public static IPeekableStream<TItem> ToBuffered<TItem>(this IStream<TItem> stream) => new BufferedStream<TItem>(stream);

    /// <summary>
    /// Filters a stream using a predicate.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <param name="stream">The stream to filter.</param>
    /// <param name="predicate">The predicate to use for filtering.</param>
    /// <returns><paramref name="stream"/> that only yields the items that the <paramref name="predicate"/> returns true
    /// for.</returns>
    public static IStream<TItem> Filter<TItem>(this IStream<TItem> stream, Predicate<TItem> predicate) =>
        new FilteredStream<TItem>(stream, predicate);

    #endregion

    #region Operations

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
    /// A default implementation for <see cref="IStream{TItem}.Consume(int)"/>.
    /// </summary>
    /// <typeparam name="TItem">The item type of the stream.</typeparam>
    /// <param name="stream">The stream to consume from.</param>
    /// <param name="amount">The amount to consume.</param>
    /// <returns>The amount that was actually consumed.</returns>
    public static int Consume<TItem>(IStream<TItem> stream, int amount)
    {
        if (amount == 0) return 0;
        var i = 0;
        for (; i < amount && stream.TryConsume(out _); ++i)
        {
            // Pass
        }
        return i;
    }

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

    #endregion
}
