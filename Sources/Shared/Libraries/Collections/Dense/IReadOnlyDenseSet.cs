// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Dense;

/// <summary>
/// A read-only set interface that stores the contained elements as intervals.
/// </summary>
/// <typeparam name="T">The set element type.</typeparam>
public interface IReadOnlyDenseSet<T> : IReadOnlyCollection<Interval<T>>
{
    /// <summary>
    /// Determines if the set contains a full interval of items.
    /// </summary>
    /// <param name="interval">The interval to check if the set contains.</param>
    /// <returns>True if all elements of <paramref name="interval"/> are contained in the set,
    /// otherwise false.</returns>
    public bool Contains(Interval<T> interval);

    /// <summary>
    /// Determines whether the current set is a proper (strict) subset of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>True if the current set is a proper subset of other, otherwise false.</returns>
    public bool IsProperSubsetOf(IEnumerable<Interval<T>> other);

    /// <summary>
    /// Determines whether the current set is a proper (strict) superset of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>True if the collection is a proper superset of other, otherwise false.</returns>
    public bool IsProperSupersetOf(IEnumerable<Interval<T>> other);

    /// <summary>
    /// Determine whether the current set is a subset of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>True if the current set is a subset of other, otherwise false.</returns>
    public bool IsSubsetOf(IEnumerable<Interval<T>> other);

    /// <summary>
    /// Determine whether the current set is a subset of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <param name="proper">True gets written here, if the subset relation is a proper subset relation,
    /// otherwise false.</param>
    /// <returns>True if the current set is a subset of other, otherwise false.</returns>
    public bool IsSubsetOf(IEnumerable<Interval<T>> other, out bool proper);

    /// <summary>
    /// Determine whether the current set is a super set of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>True if the current set is a subset of other, otherwise false.</returns>
    public bool IsSupersetOf(IEnumerable<Interval<T>> other);

    /// <summary>
    /// Determine whether the current set is a super set of a specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <param name="proper">True gets written here, if the superset relation is a proper superset relation,
    /// otherwise false.</param>
    /// <returns>True if the current set is a subset of other, otherwise false.</returns>
    public bool IsSupersetOf(IEnumerable<Interval<T>> other, out bool proper);

    /// <summary>
    /// Checks, if set overlaps with any elements of an interval.
    /// </summary>
    /// <param name="interval">The interval to check if the set overlaps.</param>
    /// <returns>True, if <paramref name="interval"/> contains elements of this set.</returns>
    public bool Overlaps(Interval<T> interval);

    /// <summary>
    /// Determines whether the current set overlaps with the specified collection.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>True, if the current set and other share at least one common element, otherwise false.</returns>
    public bool Overlaps(IEnumerable<Interval<T>> other);

    /// <summary>
    /// Determines whether the current set and the specified collection contain the same elements.
    /// </summary>
    /// <param name="other">The collection to compare to the current set.</param>
    /// <returns>True if the current set is equal to other, otherwise false.</returns>
    public bool SetEquals(IEnumerable<Interval<T>> other);
}
