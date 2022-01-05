// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections;

/// <summary>
/// A generic double-ended queue interface.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface IDeque<T> : IReadOnlyList<T>, IList<T>
{
    /// <summary>
    /// Adds an item to the front of the queue.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void AddFront(T item);

    /// <summary>
    /// Adds an item to the back of the queue.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public void AddBack(T item);

    /// <summary>
    /// Removes an item from the front of the queue.
    /// </summary>
    /// <returns>The removed item.</returns>
    public T RemoveFront();

    /// <summary>
    /// Removes an item from the back of the queue.
    /// </summary>
    /// <returns>The removed item.</returns>
    public T RemoveBack();
}
