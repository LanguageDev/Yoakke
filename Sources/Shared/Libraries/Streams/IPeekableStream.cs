// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Streams;

/// <summary>
///  Represents some stream of items that can be peeked ahead and consumed sequentially.
/// </summary>
/// <typeparam name="T">The stream element type.</typeparam>
public interface IPeekableStream<T> : IStream<T>
{
    /// <summary>
    /// Retrieves the upcoming item without consuming it.
    /// </summary>
    /// <param name="item">The peeked item gets written here, if there was any.</param>
    /// <returns>True, if there was an item to peek.</returns>
    public bool TryPeek([MaybeNullWhen(false)] out T item);

    /// <summary>
    /// Peeks ahead a given amount of items without consuming them. With <paramref name="offset"/> set to 0
    /// this is equivalent to <see cref="TryPeek"/>.
    /// </summary>
    /// <param name="offset">The offset to look ahead.</param>
    /// <param name="item">The peeked item gets written here, if there was any.</param>
    /// <returns>True, if there was an item to peek.</returns>
    public bool TryLookAhead(int offset, [MaybeNullWhen(false)] out T item);

    /// <summary>
    /// Pushes an item to the front of the stream.
    /// This operation might not be supported by all streams.
    /// </summary>
    /// <param name="item">The item to push into the front.</param>
    public void Defer(T item);
}
