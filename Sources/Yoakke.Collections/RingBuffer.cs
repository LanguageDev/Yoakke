// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;

namespace Yoakke.Collections
{
    /// <summary>
    /// A ring buffer implementation backed by an array.
    /// </summary>
    /// <typeparam name="T">The item type of the collection.</typeparam>
    public class RingBuffer<T> : IRingBuffer<T>
    {
        private const int DefaultCapacity = 16;

        public int Capacity
        {
            get => this.storage == null ? 0 : this.storage.Length;
            set => this.SetCapacity(value);
        }

        public int Count { get; private set; }

        public int Head { get; private set; }

        public int Tail => this.ToStorageIndex(this.Head + this.Count);

        public T this[int index]
        {
            get
            {
                this.CheckIndexBounds(index);
                return this.storage![this.ToStorageIndex(this.Head + index)]!;
            }

            set
            {
                this.CheckIndexBounds(index);
                this.storage![this.ToStorageIndex(this.Head + index)] = value;
            }
        }

        T IReadOnlyList<T>.this[int index] => this[index];

        private T?[]? storage;

        /// <summary>
        /// Initializes a new instance of the <see cref="RingBuffer{T}"/> class that is empty and has the specified initial capacity.
        /// </summary>
        /// <param name="capacity">The number of elements that the new ring buffer can initially store.</param>
        public RingBuffer(int capacity)
        {
            this.Count = 0;
            this.SetCapacity(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RingBuffer{T}"/> class that is empty and has the default capacity.
        /// </summary>
        public RingBuffer()
            : this(DefaultCapacity)
        {
        }

        public void AddFront(T item)
        {
            this.EnsureExtraCapacity(1);
            this.Head = this.ToStorageIndex(this.Head - 1);
            this.storage![this.Head] = item;
            ++this.Count;
        }

        public void AddBack(T item)
        {
            this.EnsureExtraCapacity(1);
            this.storage![this.Tail] = item;
            ++this.Count;
        }

        public T RemoveFront()
        {
            this.CheckNonEmpty();
            var result = this.storage![this.Head];
            this.storage![this.Head] = default;
            this.Head = this.ToStorageIndex(this.Head + 1);
            --this.Count;
            return result!;
        }

        public T RemoveBack()
        {
            this.CheckNonEmpty();
            var index = this.ToStorageIndex(this.Tail - 1);
            var result = this.storage![index];
            this.storage[index] = default;
            --this.Count;
            return result!;
        }

        public void Clear()
        {
            var ((hstart, hlen), (tstart, tlen)) = this.GetSlices();
            for (var i = hstart; i < hstart + hlen; ++i) this.storage![i] = default;
            for (var i = tstart; i < tstart + tlen; ++i) this.storage![i] = default;
            this.Head = 0;
            this.Count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var ((hstart, hlen), (tstart, tlen)) = this.GetSlices();
            for (var i = hstart; i < hstart + hlen; ++i) yield return this.storage![i]!;
            for (var i = tstart; i < tstart + tlen; ++i) yield return this.storage![i]!;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        private void EnsureExtraCapacity(int amount)
        {
            if (this.Count + amount <= this.Capacity) return;
            var nextCapacity = this.Capacity == 0 ? DefaultCapacity : this.Capacity * 2;
            for (; nextCapacity < this.Count + amount; nextCapacity *= 2) ;
            this.SetCapacity(nextCapacity);
        }

        private void SetCapacity(int capacity)
        {
            if (capacity < this.Count) throw new ArgumentOutOfRangeException(nameof(capacity));

            if (this.storage == null)
            {
                // There was nothing to copy over
                this.storage = new T[capacity];
                this.Head = 0;
                return;
            }

            if (capacity == this.storage.Length) return;

            // We need to copy elements over to a new array
            // We also use this opportunity to "reorient" the ring buffer and set the head to 0
            var newStorage = new T[capacity];
            var (head, tail) = this.GetSlices();
            Array.Copy(this.storage, head.Start, newStorage, 0, head.Length);
            Array.Copy(this.storage, tail.Start, newStorage, head.Length, tail.Length);
            this.Head = 0;
            this.storage = newStorage;
        }

        private ((int Start, int Length) Head, (int Start, int Length) Tail) GetSlices()
        {
            if (this.Head < this.Tail)
            {
                // One piece
                return ((this.Head, this.Count), (this.Head + this.Count, 0));
            }
            else
            {
                // Two pieces
                return ((this.Head, this.Capacity - this.Head), (0, this.Tail));
            }
        }

        private int ToStorageIndex(int index) => Mod(index, this.Capacity);

        private void CheckNonEmpty()
        {
            if (this.Count == 0) throw new InvalidOperationException();
        }

        private void CheckIndexBounds(int index)
        {
            if (index >= this.Count) throw new ArgumentOutOfRangeException(nameof(index));
        }

        private static int Mod(int a, int b)
        {
            var r = a % b;
            return r < 0 ? r + b : r;
        }
    }
}
