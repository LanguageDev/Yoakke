// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;

namespace Yoakke.Collections.Values;

/// <summary>
/// A wrapper around <see cref="IReadOnlyList{T}"/>s to have value-based equality.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public sealed class ReadOnlyValueList<T> : IReadOnlyValueList<T>
{
    /// <inheritdoc/>
    public T this[int index] => this.Underlying[index];

    /// <inheritdoc/>
    public int Count => this.Underlying.Count;

    /// <summary>
    /// The underlying wrapped <see cref="IReadOnlyList{T}"/>.
    /// </summary>
    public IReadOnlyList<T> Underlying { get; }

    /// <summary>
    /// The <see cref="IEqualityComparer{T}"/> to use.
    /// </summary>
    public IEqualityComparer<T> Comparer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyValueList{T}"/> class.
    /// </summary>
    /// <param name="underlying">The underlying <see cref="IReadOnlyList{T}"/> to wrap.</param>
    /// <param name="comparer">The <see cref="IEqualityComparer{T}"/> to use.</param>
    public ReadOnlyValueList(IReadOnlyList<T> underlying, IEqualityComparer<T> comparer)
    {
        this.Underlying = underlying;
        this.Comparer = comparer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlyValueList{T}"/> class.
    /// </summary>
    /// <param name="underlying">The underlying <see cref="IReadOnlyList{T}"/> to wrap.</param>
    public ReadOnlyValueList(IReadOnlyList<T> underlying)
        : this(underlying, EqualityComparer<T>.Default)
    {
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => this.Equals(obj as IReadOnlyList<T>);

    /// <inheritdoc/>
    public bool Equals(IReadOnlyList<T>? other)
    {
        if (other is null || this.Count != other.Count) return false;
        for (var i = 0; i < this.Count; ++i)
        {
            if (!this.Comparer.Equals(this[i], other[i])) return false;
        }
        return true;
    }

    /// <inheritdoc/>
    public bool Equals(IReadOnlyValueList<T>? other) => this.Equals(other as IReadOnlyList<T>);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = default(HashCode);
        for (var i = 0; i < this.Count; ++i) hash.Add(this[i], this.Comparer);
        return hash.ToHashCode();
    }

    /// <inheritdoc/>
    public override string ToString() => $"[ {string.Join(", ", this.Underlying)} ]";

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => this.Underlying.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => (this.Underlying as IEnumerable).GetEnumerator();
}
