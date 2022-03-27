// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections;
using System.Collections.Generic;

namespace Yoakke.Collections;

/// <summary>
/// A wrapper for an enumerable to be treated as a collection.
/// </summary>
/// <typeparam name="T">The collection element type.</typeparam>
public sealed class EnumerableCollection<T> : IReadOnlyCollection<T>
{
    /// <inheritdoc/>
    public int Count => this.count;

    private readonly IEnumerable<T> enumerable;
    private readonly int count;

    /// <summary>
    /// Wraps up an enumerable as a collection.
    /// </summary>
    /// <param name="enumerable">The enumerable to wrap.</param>
    /// <param name="count">The count of the enumerable.</param>
    public EnumerableCollection(IEnumerable<T> enumerable, int count)
    {
        this.enumerable = enumerable;
        this.count = count;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => this.enumerable.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}
