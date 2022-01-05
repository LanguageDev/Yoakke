// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata.Internal;

/// <summary>
/// Represents an observable collection that wraps an existing collection to publish modification events.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
internal class ObservableCollection<T> : IReadOnlyCollection<T>, ICollection<T>
{
    /// <summary>
    /// Creates 'all' and 'accepting' state collections that are connected.
    /// </summary>
    /// <param name="makeCollection">A factory to create an underlying collection.</param>
    /// <returns>The created state collections.</returns>
    public static (ObservableCollection<T> All, ObservableCollection<T> Accepting)
        StateWithAccepting(Func<ICollection<T>> makeCollection)
    {
        var all = new ObservableCollection<T>(makeCollection());
        var accepting = new ObservableCollection<T>(makeCollection());

        all.Removed += (sender, item) => accepting.Remove(item);
        all.Cleared += (sender, eventArgs) => accepting.Clear();

        accepting.Added += (sender, item) => all.Add(item);

        return (all, accepting);
    }

    /// <summary>
    /// Creates 'all', 'accepting' and 'initial' state collections that are connected.
    /// </summary>
    /// <param name="makeCollection">A factory to create an underlying collection.</param>
    /// <returns>The created state collections.</returns>
    public static (ObservableCollection<T> All, ObservableCollection<T> Accepting, ObservableCollection<T> Initial)
        StateWithAcceptingAndInitial(Func<ICollection<T>> makeCollection)
    {
        var (all, accepting) = StateWithAccepting(makeCollection);
        var initial = new ObservableCollection<T>(makeCollection());

        all.Removed += (sender, item) => initial.Remove(item);
        all.Cleared += (sender, eventArgs) => initial.Clear();

        initial.Added += (sender, item) => all.Add(item);

        return (all, accepting, initial);
    }

    /// <inheritdoc/>
    public int Count => this.items.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// Happens, when an item is added.
    /// </summary>
    public event EventHandler<T>? Added;

    /// <summary>
    /// Happens, when an item is removed.
    /// </summary>
    public event EventHandler<T>? Removed;

    /// <summary>
    /// Happens, when all items are removed.
    /// </summary>
    public event EventHandler? Cleared;

    private readonly ICollection<T> items;

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableCollection{T}"/> class.
    /// </summary>
    /// <param name="collection">The collection wrap.</param>
    public ObservableCollection(ICollection<T> collection)
    {
        this.items = collection;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        this.items.Clear();
        this.Cleared?.Invoke(this, default);
    }

    /// <inheritdoc/>
    public void Add(T item)
    {
        this.items.Add(item);
        this.Added?.Invoke(this, item);
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        if (!this.items.Remove(item)) return false;

        this.Removed?.Invoke(this, item);
        return true;
    }

    /// <inheritdoc/>
    public bool Contains(T item) => this.items.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex) => this.items.CopyTo(array, arrayIndex);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => this.items.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => (this.items as IEnumerable).GetEnumerator();
}
