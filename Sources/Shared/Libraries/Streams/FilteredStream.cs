// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Streams;

/// <summary>
/// A filtered <see cref="IStream{T}"/> with a predicate.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class FilteredStream<T> : IStream<T>
{
    /// <inheritdoc/>
    public bool IsEnd => this.Underlying.IsEnd;

    /// <summary>
    /// The underlying stream this one reads from.
    /// </summary>
    public IStream<T> Underlying => this.underlying;

    /// <summary>
    /// The predicate used for filtering.
    /// </summary>
    public Predicate<T> Predicate { get; }

    private readonly PeekStream<T> underlying;

    /// <summary>
    /// Initializes a new instance of the <see cref="FilteredStream{T}"/> class.
    /// </summary>
    /// <param name="underlying">The underlying stream this one reads from.</param>
    /// <param name="predicate">The predicate used for filtering.</param>
    public FilteredStream(IStream<T> underlying, Predicate<T> predicate)
    {
        this.underlying = new PeekStream<T>(underlying);
        this.Predicate = predicate;
        this.UpdatePeek();
    }

    /// <inheritdoc/>
    public bool TryConsume([MaybeNullWhen(false)] out T item)
    {
        if (this.underlying.TryConsume(out item))
        {
            this.UpdatePeek();
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <inheritdoc/>
    public int Consume(int amount) => StreamExtensions.Consume(this, amount);

    private void UpdatePeek()
    {
        while (this.underlying.TryPeek(out var item))
        {
            if (this.Predicate(item)) break;
            this.underlying.Consume();
        }
    }
}
