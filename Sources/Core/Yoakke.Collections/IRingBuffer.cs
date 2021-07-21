// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.Collections
{
    /// <summary>
    /// Represents a generic collection of objects with efficient add and removal at the front and back.
    /// </summary>
    /// <typeparam name="T">The item type of the collection.</typeparam>
    public interface IRingBuffer<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Gets or sets the total number of elements the internal data structure can hold without resizing.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// The current offset of the head index (index to the first element).
        /// </summary>
        public int Head { get; }

        /// <summary>
        /// The current offset of the tail index (index after the last element).
        /// </summary>
        public int Tail { get; }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">he zero-based index of the element to get or set relative to the head.</param>
        public new T this[int index] { get; set; }

        /// <summary>
        /// Adds an item to the front of the buffer.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddFront(T item);

        /// <summary>
        /// Adds an item to the back of the buffer.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddBack(T item);

        /// <summary>
        /// Removes an item from the front of the buffer.
        /// </summary>
        /// <returns>The removed item.</returns>
        public T RemoveFront();

        /// <summary>
        /// Removes an item from the back of the buffer.
        /// </summary>
        /// <returns>The removed item.</returns>
        public T RemoveBack();

        /// <summary>
        /// Removes all items from the ring buffer.
        /// </summary>
        public void Clear();
    }
}
