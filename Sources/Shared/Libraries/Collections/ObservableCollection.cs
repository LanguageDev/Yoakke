// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;

namespace Yoakke.Collections;

/// <summary>
/// A collection wrapper that exposes changes through events.
/// </summary>
/// <typeparam name="T">The collection element type.</typeparam>
public sealed class ObservableCollection<T> : IReadOnlyCollection<T>, ICollection<T>
{
    /// <inheritdoc/>
    public int Count => this.underlying.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => this.underlying.IsReadOnly;

    /// <summary>
    /// An event that happens when an item is added.
    /// </summary>
    public event EventHandler<T>? ItemAdded;

    /// <summary>
    /// An event that happens when an item is removed.
    /// </summary>
    public event EventHandler<T>? ItemRemoved;

    /// <summary>
    /// An event that happens when the collection is cleared.
    /// </summary>
    public event EventHandler? Cleared;

    private readonly ICollection<T> underlying;

    /// <summary>
    /// Creates an <see cref="ObservableCollection{T}"/> by wrapping up an existing collection.
    /// </summary>
    /// <param name="underlying">The collection to wrap up.</param>
    public ObservableCollection(ICollection<T> underlying)
    {
        this.underlying = underlying;
    }

    /// <inheritdoc/>
    public void Add(T item)
    {
        this.underlying.Add(item);
        this.ItemAdded?.Invoke(this, item);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        if (this.underlying.Remove(item))
        {
            this.ItemRemoved?.Invoke(this, item);
            return true;
        }
        return false;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        this.underlying.Clear();
        this.Cleared?.Invoke(this, null);
    }

    /// <inheritdoc/>
    public bool Contains(T item) => this.underlying.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex) => this.underlying.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => this.underlying.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
