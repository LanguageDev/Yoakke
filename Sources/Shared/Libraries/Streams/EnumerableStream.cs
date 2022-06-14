// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Streams;

/// <summary>
/// An <see cref="IStream{T}"/> wrapper for an <see cref="IEnumerable{T}"/>.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class EnumerableStream<T> : IStream<T>
{
    /// <inheritdoc/>
    public bool IsEnd { get; private set; }

    private readonly IEnumerator<T> enumerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumerableStream{T}"/> class.
    /// </summary>
    /// <param name="items">The enumerable to wrap.</param>
    public EnumerableStream(IEnumerable<T> items)
    {
        this.enumerator = items.GetEnumerator();
        this.IsEnd = !this.enumerator.MoveNext();
    }

    /// <inheritdoc/>
    public bool TryConsume([MaybeNullWhen(false)] out T item)
    {
        if (this.IsEnd)
        {
            item = default;
            return false;
        }
        else
        {
            item = this.enumerator.Current;
            this.IsEnd = !this.enumerator.MoveNext();
            return true;
        }
    }

    /// <inheritdoc/>
    public int Consume(int amount) => StreamExtensions.Consume(this, amount);
}
