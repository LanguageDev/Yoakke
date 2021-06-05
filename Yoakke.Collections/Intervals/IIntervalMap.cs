using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// Represents a generic collection of disjunct intervals and associated values.
    /// </summary>
    /// <typeparam name="TKey">The interval value type.</typeparam>
    /// <typeparam name="TValue">The associated value type.</typeparam>
    public interface IIntervalMap<TKey, TValue> : IReadOnlyCollection<KeyValuePair<Interval<TKey>, TValue>>
    {
        /// <summary>
        /// The intervals that the values are associated to.
        /// </summary>
        public IEnumerable<Interval<TKey>> Intervals { get; }
        /// <summary>
        /// The associated values.
        /// </summary>
        public IEnumerable<TValue> Values { get; }

        /// <summary>
        /// Gets the associated value at a given point in the intervals.
        /// </summary>
        /// <param name="key">The key to locate in the intervals.</param>
        public TValue this[TKey key] { get; }

        /// <summary>
        /// Clears this interval map.
        /// </summary>
        public void Clear();
        /// <summary>
        /// Checks if the given key is covered by the intervals.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <returns>True, if an interval covers this key, false otherwise.</returns>
        public bool ContainsKey(TKey key);
        /// <summary>
        /// Gets the associated value at a given point in the intervals.
        /// </summary>
        /// <param name="key">The key to locate.</param>
        /// <param name="value">When the method returns true, it gets filled with the value associated with the keys containing interval.
        /// Otherwise it gets filled with default.</param>
        /// <returns>True, if the key is covered by an interval.</returns>
        public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue? value);
        /// <summary>
        /// Inserts an interval with a given associated value.
        /// The intersecting intervals get sliced up and the intersections associated values get unified with the specified update function.
        /// </summary>
        /// <param name="interval">The interval to insert.</param>
        /// <param name="value">The associated value to the interval.</param>
        /// <param name="updateFunc">The update function that receives the existing value and the newly inserted value and
        /// returns the new value to keep.</param>
        public void AddAndUpdate(Interval<TKey> interval, TValue value, Func<TValue, TValue, TValue> updateFunc);
        /// <summary>
        /// Merges the touching intervals with the same associated value.
        /// </summary>
        public void MergeTouching();
    }
}
