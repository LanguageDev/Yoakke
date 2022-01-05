// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Streams;

/// <summary>
/// An <see cref="IPeekableStream{T}"/> wrapper for <see cref="IStream{T}"/> that only supports a single peek ahead.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class PeekStream<T> : IPeekableStream<T>
{
    /// <inheritdoc/>
    public bool IsEnd => !this.hasPeek;

    /// <summary>
    /// The underlying stream.
    /// </summary>
    public IStream<T> Underlying { get; }

    private T? peekedItem;
    private bool hasPeek;

    /// <summary>
    /// Initializes a new instance of the <see cref="PeekStream{T}"/> class.
    /// </summary>
    /// <param name="underlying">The underlying stream.</param>
    public PeekStream(IStream<T> underlying)
    {
        this.Underlying = underlying;
        this.UpdatePeek();
    }

    /// <inheritdoc/>
    public bool TryPeek([MaybeNullWhen(false)] out T item)
    {
        item = this.peekedItem;
        return this.hasPeek;
    }

    /// <inheritdoc/>
    public bool TryLookAhead(int offset, [MaybeNullWhen(false)] out T item)
    {
        if (offset != 0) throw new ArgumentOutOfRangeException(nameof(offset), "A peek stream can only look ahead to the next item.");
        return this.TryPeek(out item);
    }

    /// <inheritdoc/>
    public int Consume(int amount) => StreamExtensions.Consume(this, amount);

    /// <inheritdoc/>
    public bool TryConsume([MaybeNullWhen(false)] out T item)
    {
        if (this.hasPeek)
        {
            item = this.peekedItem!;
            this.UpdatePeek();
            return true;
        }
        else
        {
            item = default;
            return false;
        }
    }

    /// <inheritdoc/>
    public void Defer(T item) => throw new NotSupportedException();

    private void UpdatePeek()
    {
        this.hasPeek = this.Underlying.TryConsume(out var item);
        this.peekedItem = item;
    }
}
