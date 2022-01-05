// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Streams;

/// <summary>
/// An <see cref="IStream{T}"/> that works on a <see cref="Memory{T}"/>.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class MemoryStream<T> : IPeekableStream<T>
{
    /// <inheritdoc/>
    public bool IsEnd => this.Index >= this.Memory.Length;

    /// <summary>
    /// The memory that is being read.
    /// </summary>
    public ReadOnlyMemory<T> Memory { get; }

    /// <summary>
    /// The current index in the memory.
    /// </summary>
    public int Index { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryStream{T}"/> class.
    /// </summary>
    /// <param name="memory">The memory to read from.</param>
    public MemoryStream(ReadOnlyMemory<T> memory)
    {
        this.Memory = memory;
    }

    /// <inheritdoc/>
    public bool TryPeek([MaybeNullWhen(false)] out T item) => this.TryLookAhead(0, out item);

    /// <inheritdoc/>
    public bool TryLookAhead(int offset, [MaybeNullWhen(false)] out T item)
    {
        var index = this.Index + offset;
        if (index >= this.Memory.Length)
        {
            item = default;
            return false;
        }
        else
        {
            item = this.Memory.Span[index];
            return true;
        }
    }

    /// <inheritdoc/>
    public bool TryConsume([MaybeNullWhen(false)] out T item)
    {
        if (!this.TryPeek(out item)) return false;
        ++this.Index;
        return true;
    }

    /// <inheritdoc/>
    public int Consume(int amount)
    {
        var nextIndex = Math.Min(this.Index + amount, this.Memory.Length);
        var diff = nextIndex - this.Index;
        this.Index = nextIndex;
        return diff;
    }

    /// <inheritdoc/>
    public void Defer(T item) => throw new NotSupportedException();
}
