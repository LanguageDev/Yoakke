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
            get => storage == null ? 0 : storage.Length;
            set => SetCapacity(value);
        }
        public int Count { get; private set; }
        public int Head { get; private set; }
        public int Tail => ToStorageIndex(Head + Count);
        public T this[int index] 
        {
            get
            {
                CheckIndexBounds(index);
                return storage![ToStorageIndex(Head + index)]!;
            }
            set
            {
                CheckIndexBounds(index);
                storage![ToStorageIndex(Head + index)] = value;
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
            Count = 0;
            SetCapacity(capacity);
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
            EnsureExtraCapacity(1);
            Head = ToStorageIndex(Head - 1);
            storage![Head] = item;
            ++Count;
        }

        public void AddBack(T item)
        {
            EnsureExtraCapacity(1);
            storage![Tail] = item;
            ++Count;
        }

        public T RemoveFront()
        {
            CheckNonEmpty();
            var result = storage![Head];
            storage![Head] = default;
            Head = ToStorageIndex(Head + 1);
            --Count;
            return result!;
        }

        public T RemoveBack()
        {
            CheckNonEmpty();
            var index = ToStorageIndex(Tail - 1);
            var result = storage![index];
            storage[index] = default;
            --Count;
            return result!;
        }

        public void Clear()
        {
            var ((hstart, hlen), (tstart, tlen)) = GetSlices();
            for (int i = hstart; i < hstart + hlen; ++i) storage![i] = default;
            for (int i = tstart; i < tstart + tlen; ++i) storage![i] = default;
            Head = 0;
            Count = 0;
        }

        public IEnumerator<T> GetEnumerator()
        {
            var ((hstart, hlen), (tstart, tlen)) = GetSlices();
            for (int i = hstart; i < hstart + hlen; ++i) yield return storage![i]!;
            for (int i = tstart; i < tstart + tlen; ++i) yield return storage![i]!;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void EnsureExtraCapacity(int amount)
        {
            if (Count + amount <= Capacity) return;
            var nextCapacity = Capacity == 0 ? DefaultCapacity : Capacity * 2;
            for (; nextCapacity < Count + amount; nextCapacity *= 2) ;
            SetCapacity(nextCapacity);
        }

        private void SetCapacity(int capacity)
        {
            if (capacity < Count) throw new ArgumentOutOfRangeException(nameof(capacity));
            
            if (storage == null)
            {
                // There was nothing to copy over
                storage = new T[capacity];
                Head = 0;
                return;
            }

            if (capacity == storage.Length) return;

            // We need to copy elements over to a new array
            // We also use this opportunity to "reorient" the ring buffer and set the head to 0
            var newStorage = new T[capacity];
            var (head, tail) = GetSlices();
            Array.Copy(storage, head.Start, newStorage, 0, head.Length);
            Array.Copy(storage, tail.Start, newStorage, head.Length, tail.Length);
            Head = 0;
            storage = newStorage;
        }

        private ((int Start, int Length) Head, (int Start, int Length) Tail) GetSlices()
        {
            if (Head < Tail)
            {
                // One piece
                return ((Head, Count), (Head + Count, 0));
            }
            else
            {
                // Two pieces
                return ((Head, Capacity - Head), (0, Tail));
            }
        }

        private int ToStorageIndex(int index) => Mod(index, Capacity);

        private void CheckNonEmpty()
        {
            if (Count == 0) throw new InvalidOperationException();
        }

        private void CheckIndexBounds(int index)
        {
            if (index >= Count) throw new ArgumentOutOfRangeException(nameof(index));
        }

        private static int Mod(int a, int b)
        {
            int r = a % b;
            return r < 0 ? r + b : r;
        }
    }
}
