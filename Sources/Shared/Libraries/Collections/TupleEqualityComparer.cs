// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections;

/// <summary>
/// A tuple equality comparer with custom element comparers.
/// </summary>
/// <typeparam name="T1">The first element type.</typeparam>
/// <typeparam name="T2">The second element type.</typeparam>
public class TupleEqualityComparer<T1, T2> : IEqualityComparer<(T1, T2)>
{
    /// <summary>
    /// The comparer of the first element.
    /// </summary>
    public IEqualityComparer<T1> FirstComparer { get; }

    /// <summary>
    /// The comparer of the second element.
    /// </summary>
    public IEqualityComparer<T2> SecondComparer { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TupleEqualityComparer{T1, T2}"/> class.
    /// </summary>
    /// <param name="firstComparer">The first comparer to use.</param>
    /// <param name="secondComparer">The second comparer to use.</param>
    public TupleEqualityComparer(IEqualityComparer<T1> firstComparer, IEqualityComparer<T2> secondComparer)
    {
        this.FirstComparer = firstComparer;
        this.SecondComparer = secondComparer;
    }

    /// <inheritdoc/>
    public bool Equals((T1, T2) x, (T1, T2) y) =>
           this.FirstComparer.Equals(x.Item1, y.Item1)
        && this.SecondComparer.Equals(x.Item2, y.Item2);

    /// <inheritdoc/>
    public int GetHashCode((T1, T2) obj)
    {
        var h = default(HashCode);
        h.Add(obj.Item1, this.FirstComparer);
        h.Add(obj.Item2, this.SecondComparer);
        return h.ToHashCode();
    }
}
