using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Collections;

/// <summary>
/// Represents a set that can be compared by value.
/// </summary>
/// <typeparam name="T">The set element type.</typeparam>
public readonly struct ByValueSet<T> : ISet<T>,
                                       IEquatable<ByValueSet<T>>, IEquatable<ISet<T>>
{
    /// <inheritdoc/>
    public int Count => this.underlying.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <summary>
    /// The comparer used.
    /// </summary>
    public IEqualityComparer<T> Comparer { get; }

    private readonly ISet<T> underlying;

    /// <summary>
    /// Creates a new <see cref="ByValueSet{T}"/> by wrapping an existing set.
    /// </summary>
    /// <param name="underlying">The underlying set to wrap.</param>
    /// <param name="comparer">The comparer to use.</param>
    public ByValueSet(ISet<T> underlying, IEqualityComparer<T>? comparer)
    {
        this.underlying = underlying;
        this.Comparer = comparer ?? EqualityComparer<T>.Default;
    }

    /// <summary>
    /// Creates a new <see cref="ByValueSet{T}"/> from a sequence of values.
    /// </summary>
    /// <param name="values">The values for the set to contain.</param>
    /// <param name="comparer">The comparer to use.</param>
    public ByValueSet(IEnumerable<T> values, IEqualityComparer<T>? comparer)
        : this(new HashSet<T>(values, comparer), comparer)
    {
    }

    /// <inheritdoc/>
    public override string ToString() => $"{{{string.Join(", ", this.underlying)}}}";

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ISet<T> s && this.Equals(s);

    /// <inheritdoc/>
    public bool Equals(ByValueSet<T> other) => this.SetEquals(other.underlying);

    /// <inheritdoc/>
    public bool Equals(ISet<T> other) => this.SetEquals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashUtils.CombineXor(this.underlying, this.Comparer);

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => this.underlying.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <inheritdoc/>
    public bool Contains(T item) => this.underlying.Contains(item);

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (arrayIndex + this.Count > array.Length) throw new ArgumentOutOfRangeException(nameof(array));

        var i = 0;
        foreach (var item in this)
        {
            array[arrayIndex + i] = item;
            ++i;
        }
    }

    /// <inheritdoc/>
    public bool IsProperSubsetOf(IEnumerable<T> other) => this.underlying.IsProperSubsetOf(other);

    /// <inheritdoc/>
    public bool IsProperSupersetOf(IEnumerable<T> other) => this.underlying.IsProperSupersetOf(other);

    /// <inheritdoc/>
    public bool IsSubsetOf(IEnumerable<T> other) => this.underlying.IsSubsetOf(other);

    /// <inheritdoc/>
    public bool IsSupersetOf(IEnumerable<T> other) => this.underlying.IsSupersetOf(other);

    /// <inheritdoc/>
    public bool Overlaps(IEnumerable<T> other) => this.underlying.Overlaps(other);

    /// <inheritdoc/>
    public bool SetEquals(IEnumerable<T> other) => this.underlying.SetEquals(other);

    /// <inheritdoc/>
    bool ISet<T>.Add(T item) => throw new NotSupportedException();

    /// <inheritdoc/>
    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

    /// <inheritdoc/>
    void ICollection<T>.Add(T item) => throw new NotSupportedException();

    /// <inheritdoc/>
    void ICollection<T>.Clear() => throw new NotSupportedException();

    /// <inheritdoc/>
    void ISet<T>.IntersectWith(IEnumerable<T> other) => throw new NotSupportedException();

    /// <inheritdoc/>
    void ISet<T>.SymmetricExceptWith(IEnumerable<T> other) => throw new NotSupportedException();

    /// <inheritdoc/>
    void ISet<T>.UnionWith(IEnumerable<T> other) => throw new NotSupportedException();

    /// <inheritdoc/>
    void ISet<T>.ExceptWith(IEnumerable<T> other) => throw new NotSupportedException();

    /// <summary>
    /// Compares two <see cref="ByValueSet{T}"/>s for equality.
    /// </summary>
    /// <param name="left">The first set to compare.</param>
    /// <param name="right">The second set to compare.</param>
    /// <returns>True, if <paramref name="left"/> and <paramref name="right"/> are equal; false otherwise.</returns>
    public static bool operator ==(ByValueSet<T> left, ByValueSet<T> right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="ByValueSet{T}"/>s for inequality.
    /// </summary>
    /// <param name="left">The first set to compare.</param>
    /// <param name="right">The second set to compare.</param>
    /// <returns>True, if <paramref name="left"/> and <paramref name="right"/> are not equal; false otherwise.</returns>
    public static bool operator !=(ByValueSet<T> left, ByValueSet<T> right) => !(left == right);
}
