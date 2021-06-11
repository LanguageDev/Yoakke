using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Collections
{
    /// <summary>
    /// A generic peekable enumerator backed by a ring buffer.
    /// </summary>
    /// <typeparam name="T">The type of objects to enumerate.</typeparam>
    public class PeekableEnumerator<T> : IPeekableEnumerator<T>
    {
        public T Current
        {
            get
            {
                if (!this.readCurrent) throw new InvalidOperationException();
                return this.buffer[0];
            }
        }

        object IEnumerator.Current => this.Current!;

        private readonly IEnumerator<T> underlying;
        private RingBuffer<T> buffer;
        private bool readCurrent;

        public PeekableEnumerator(IEnumerator<T> underlying)
        {
            this.underlying = underlying;
            this.buffer = new RingBuffer<T>();
            this.readCurrent = false;
        }

        public void Dispose()
        {
            this.underlying.Dispose();
            this.buffer.Clear();
            this.buffer = null!;
        }

        public void Reset()
        {
            this.underlying.Reset();
            this.buffer.Clear();
            this.readCurrent = false;
        }

        public bool MoveNext()
        {
            if (this.readCurrent)
            {
                // Not the first read
                // If the buffer is empty, means we have already passed the last element
                if (this.buffer.Count == 0) return false;
                // Otherwise we consume the current
                this.buffer.RemoveFront();
            }
            else
            {
                // This is the first read
                this.readCurrent = true;
            }
            return this.TryPeek(0, out var _);
        }

        public bool TryPeek(int amount, [MaybeNullWhen(false)] out T? item)
        {
            while (this.buffer.Count <= amount)
            {
                if (this.underlying.MoveNext())
                {
                    this.buffer.AddBack(this.underlying.Current);
                }
                else
                {
                    if (!this.readCurrent && amount == this.buffer.Count)
                    {
                        // Before first MoveNext, amount == buffer.Count is fine here
                        item = this.buffer[amount - 1];
                        return true;
                    }
                    // Not enough items remaining
                    item = default;
                    return false;
                }
            }
            item = this.buffer[amount];
            return true;
        }

        public T Peek(int amount)
        {
            if (!this.TryPeek(amount, out var item)) throw new ArgumentOutOfRangeException(nameof(amount));
            return item!;
        }
    }
}
