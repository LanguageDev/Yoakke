// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Diagnostics.CodeAnalysis;
using Yoakke.Collections;

namespace Yoakke.Streams;

/// <summary>
/// An <see cref="IPeekableStream{T}"/> implementation that wraps an <see cref="IStream{T}"/>.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class BufferedStream<T> : IPeekableStream<T>
{
    /// <summary>
    /// The underlying stream.
    /// </summary>
    public IStream<T> Underlying { get; }

    /// <inheritdoc/>
    public bool IsEnd => this.Underlying.IsEnd && this.peek.Count == 0;

    private readonly RingBuffer<T> peek = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferedStream{T}"/> class.
    /// </summary>
    /// <param name="underlying">The underlying stream.</param>
    public BufferedStream(IStream<T> underlying)
    {
        this.Underlying = underlying;
    }

    /// <inheritdoc/>
    public bool TryPeek([MaybeNullWhen(false)] out T item) => this.TryLookAhead(0, out item);

    /// <inheritdoc/>
    public bool TryLookAhead(int offset, [MaybeNullWhen(false)] out T item)
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
    public bool TryConsume([MaybeNullWhen(false)] out T item)
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
    public void Defer(T item) => this.peek.AddFront(item);
}
