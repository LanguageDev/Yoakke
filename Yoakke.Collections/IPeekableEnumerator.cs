using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections
{
    /// <summary>
    /// Supports iteration with peeking over a generic collection.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    public interface IPeekableEnumerator<T> : IEnumerator<T>
    {
        /// <summary>
        /// Returns a value that indicates whether there is an element a given amount ahead in this enumerator.
        /// </summary>
        /// <param name="amount">The number of elements to peek forward from the current.</param>
        /// <param name="item">If there are enough elements to peek ahead, then the element at that position, otherwise the default value.</param>
        /// <returns>True, if there is an element a given amount ahead in this enumerator, false otherwise.</returns>
        public bool TryPeek(int amount, [MaybeNullWhen(false)] out T? item);

        /// <summary>
        /// Peeks a given amount of elements forward.
        /// </summary>
        /// <param name="amount">The number of elements to peek forward from the current.</param>
        /// <returns>The number of elements to peek in front of the current.</returns>
        public T Peek(int amount)
        {
            if (!TryPeek(amount, out var item)) throw new ArgumentOutOfRangeException(nameof(item));
            return item;
        }
    }
}
