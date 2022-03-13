using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Collections;

/// <summary>
/// Represents a list that can be compared by value.
/// </summary>
/// <typeparam name="T">The list element type.</typeparam>
public readonly struct ByValueList<T> : IReadOnlyList<T>, IList<T>,
                                        IEquatable<ByValueList<T>>, IEquatable<IReadOnlyList<T>>, IEquatable<IList<T>>
{
    /// <inheritdoc/>
    public int Count => this.underlying.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => true;

    /// <inheritdoc/>
    T IList<T>.this[int index]
    {
        get => this[index];
        set => throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public T this[int index] => this.underlying[index];

    private readonly IReadOnlyList<T> underlying;
    private readonly IEqualityComparer<T> comparer;

    /// <summary>
    /// Creates a new <see cref="ByValueList{T}"/> by wrapping an existing list.
    /// </summary>
    /// <param name="underlying">The underlying list to wrap.</param>
    /// <param name="comparer">The comparer to use.</param>
    public ByValueList(IReadOnlyList<T> underlying, IEqualityComparer<T>? comparer = null)
    {
        this.underlying = underlying;
        this.comparer = comparer ?? EqualityComparer<T>.Default;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
        if (obj is IList<T> l) return this.Equals(l);
        if (obj is IReadOnlyList<T> rl) return this.Equals(rl);
        return false;
    }

    /// <inheritdoc/>
    public bool Equals(ByValueList<T> other) => this.Equals(other, other.Count);

    /// <inheritdoc/>
    public bool Equals(IReadOnlyList<T> other) => this.Equals(other, other.Count);

    /// <inheritdoc/>
    public bool Equals(IList<T> other) => this.Equals(other, other.Count);

    private bool Equals(IEnumerable<T> other, int otherCount) =>
           this.Count == otherCount
        && this.underlying.SequenceEqual(other, this.comparer);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var h = default(HashCode);
        foreach (var item in this) h.Add(item, this.comparer);
        return h.ToHashCode();
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator() => this.underlying.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    /// <inheritdoc/>
    public bool Contains(T item) => this.underlying.Contains(item);

    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        for (var i = 0; i < this.Count; ++i)
        {
            if (this.comparer.Equals(this[i], item)) return i;
        }
        return -1;
    }

    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        if (arrayIndex + this.Count > array.Length) throw new ArgumentOutOfRangeException(nameof(array));
        for (var i = 0; i < this.Count; ++i) array[arrayIndex + i] = this[i];
    }

    /// <inheritdoc/>
    void IList<T>.Insert(int index, T item) => throw new NotSupportedException();

    /// <inheritdoc/>
    void IList<T>.RemoveAt(int index) => throw new NotSupportedException();

    /// <inheritdoc/>
    void ICollection<T>.Add(T item) => throw new NotSupportedException();

    /// <inheritdoc/>
    void ICollection<T>.Clear() => throw new NotSupportedException();

    /// <inheritdoc/>
    bool ICollection<T>.Remove(T item) => throw new NotSupportedException();

    /// <summary>
    /// Compares two <see cref="ByValueList{T}"/>s for equality.
    /// </summary>
    /// <param name="left">The first list to compare.</param>
    /// <param name="right">The second list to compare.</param>
    /// <returns>True, if <paramref name="left"/> and <paramref name="right"/> are equal; false otherwise.</returns>
    public static bool operator ==(ByValueList<T> left, ByValueList<T> right) => left.Equals(right);

    /// <summary>
    /// Compares two <see cref="ByValueList{T}"/>s for inequality.
    /// </summary>
    /// <param name="left">The first list to compare.</param>
    /// <param name="right">The second list to compare.</param>
    /// <returns>True, if <paramref name="left"/> and <paramref name="right"/> are not equal; false otherwise.</returns>
    public static bool operator !=(ByValueList<T> left, ByValueList<T> right) => !(left == right);
}
