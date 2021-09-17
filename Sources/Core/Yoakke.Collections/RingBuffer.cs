// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections
{
    /// <summary>
    /// A ring-buffer backed <see cref="IDeque{T}"/> implementation.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public class RingBuffer<T> : IDeque<T>
    {
        private const int DefaultCapacity = 8;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public int Count { get; private set; }

        /// <summary>
        /// Gets or sets the capacity of the backing array.
        /// </summary>
        public int Capacity
        {
            get => this.storage.Length;
            set
            {
                // If we try to reduce it below the current count, throw
                if (value < this.Capacity) throw new ArgumentOutOfRangeException(nameof(value));

                // If capacity equals, early-return
                if (value == this.Capacity) return;

                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// The current head-index, which is the position of the first element.
        /// </summary>
        public int Head { get; private set; }

        /// <summary>
        /// The current tail-index, which is the position of the index after the last element.
        /// </summary>
        public int Tail => (this.Head + this.Count) % this.Capacity;

        /// <inheritdoc/>
        public T this[int index]
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }

        /// <inheritdoc/>
        T IReadOnlyList<T>.this[int index] => this[index];

        private T[] storage = Array.Empty<T>();

        /// <summary>
        /// Initializes a new instance of the <see cref="RingBuffer{T}"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new ring buffer can initially store.</param>
        public RingBuffer(int capacity)
        {
            this.Capacity = capacity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RingBuffer{T}"/> class that is empty and has the default capacity.
        /// </summary>
        public RingBuffer()
            : this(DefaultCapacity)
        {
        }

        /// <inheritdoc/>
        public void Clear() => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Add(T item) => this.AddBack(item);

        /// <inheritdoc/>
        public void AddFront(T item) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void AddBack(T item) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void Insert(int index, T item) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Remove(T item)
        {
            var index = this.IndexOf(item);

            if (index == -1) return false;

            this.RemoveAt(index);
            return true;
        }

        /// <inheritdoc/>
        public T RemoveFront() => throw new NotImplementedException();

        /// <inheritdoc/>
        public T RemoveBack() => throw new NotImplementedException();

        /// <inheritdoc/>
        public void RemoveAt(int index) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Contains(T item) => this.IndexOf(item) != -1;

        /// <inheritdoc/>
        public int IndexOf(T item)
        {
            var (first, second) = this.AsMemory();
            var index = 0;
            for (var i = 0; i < first.Length; ++i, ++index)
            {
                if (EqualityComparer<T>.Default.Equals(item, first.Span[i])) return index;
            }
            for (var i = 0; i < second.Length; ++i, ++index)
            {
                if (EqualityComparer<T>.Default.Equals(item, second.Span[i])) return index;
            }
            return -1;
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            var (first, second) = this.AsMemory();
            first.CopyTo(array.AsMemory(arrayIndex));
            second.CopyTo(array.AsMemory(arrayIndex + first.Length));
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            var (first, second) = this.AsMemory();
            for (var i = 0; i < first.Length; ++i) yield return first.Span[i];
            for (var i = 0; i < second.Length; ++i) yield return second.Span[i];
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private (Memory<T> First, Memory<T> Second) AsMemory()
        {
            // For empty buffer, just return an empty pair
            if (this.Count == 0) return (Memory<T>.Empty, Memory<T>.Empty);

            // For non-empty buffers we can assum that if tail <= head, then the buffer is split in 2
            return this.Tail > this.Head
                ? (this.storage.AsMemory(this.Head, this.Count), Memory<T>.Empty)
                : (this.storage.AsMemory(this.Head, this.Capacity - this.Head), this.storage.AsMemory(0, this.Tail));
        }
    }
}
