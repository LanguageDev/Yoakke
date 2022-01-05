// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Streams;

/// <summary>
/// Represents some stream of items that can be consumed sequentially.
/// </summary>
/// <typeparam name="T">The stream element type.</typeparam>
public interface IStream<T>
{
    /// <summary>
    /// True, if the stream is out of items.
    /// </summary>
    public bool IsEnd { get; }

    /// <summary>
    /// Consumes the upcoming element in the stream.
    /// </summary>
    /// <param name="item">The consumed element gets written here, if there was any.</param>
    /// <returns>True, if there was an item to advance.</returns>
    public bool TryConsume([MaybeNullWhen(false)] out T item);

    /// <summary>
    /// Consumes a given amount of items in the stream.
    /// </summary>
    /// <param name="amount">The number of items to advance.</param>
    /// <returns>The number of items actually consumed.</returns>
    public int Consume(int amount);
}
