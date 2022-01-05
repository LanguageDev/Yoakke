// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Dense;

/// <summary>
/// A set interface that stores the contained elements as intervals.
/// </summary>
/// <typeparam name="T">The set element type.</typeparam>
public interface IDenseSet<T> : IReadOnlyDenseSet<T>, ICollection<Interval<T>>
{
    /// <summary>
    /// Determines if the set contains a full interval of items.
    /// </summary>
    /// <param name="interval">The interval to check if the set contains.</param>
    /// <returns>True if all elements of <paramref name="interval"/> are contained in the set,
    /// otherwise false.</returns>
    public new bool Contains(Interval<T> interval);

    /// <summary>
    /// Adds an interval of elements to this set.
    /// </summary>
    /// <param name="interval">The interval of elements to add.</param>
    /// <returns>True, if there was at least one new, unique element added.</returns>
    public new bool Add(Interval<T> interval);

    /// <summary>
    /// Inverts this set, meaning that any value that was contained before won't be contained and vice versa.
    /// </summary>
    public void Complement();

    /// <summary>
    /// Removes all intervals of elements in the specified collection from the current set.
    /// </summary>
    /// <param name="other">The collection of intervals to remove from the set.</param>
    public void ExceptWith(IEnumerable<Interval<T>> other);

    /// <summary>
    /// Modifies the current set so that it contains only elements that are also in a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    public void IntersectWith(IEnumerable<Interval<T>> other);

    /// <summary>
    /// Modifies the current set so that it contains only elements that are present either in the current set
    /// or in the specified collection, but not both.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    public void SymmetricExceptWith(IEnumerable<Interval<T>> other);

    /// <summary>
    /// Modifies the current set so that it contains all elements that are present in the current set, in the
    /// specified collection, or in both.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    public void UnionWith(IEnumerable<Interval<T>> other);
}
