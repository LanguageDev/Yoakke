using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                if (!readCurrent) throw new InvalidOperationException();
                return buffer[0];
            }
        }
        object IEnumerator.Current => Current;

        private IEnumerator<T> underlying;
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
            underlying.Dispose();
            buffer = null;
        }

        public void Reset()
        {
            underlying.Reset();
            buffer.Clear();
            readCurrent = false;
        }

        public bool MoveNext()
        {
            if (readCurrent)
            {
                // Not the first read
                // If the buffer is empty, means we have already passed the last element
                if (buffer.Count == 0) return false;
                // Otherwise we consume the current
                buffer.RemoveFront();
            }
            else
            {
                // This is the first read
                readCurrent = true;
            }
            return TryPeek(0, out var _);
        }

        public bool TryPeek(int amount, [MaybeNullWhen(false)] out T? item)
        {
            while (buffer.Count <= amount)
            {
                if (underlying.MoveNext())
                {
                    buffer.AddBack(underlying.Current);
                }
                else
                {
                    if (!readCurrent && amount == buffer.Count)
                    {
                        // Before first MoveNext, amount == buffer.Count is fine here
                        item = buffer[amount - 1];
                        return true;
                    }
                    // Not enough items remaining
                    item = default;
                    return false;
                }
            }
            item = buffer[amount];
            return true;
        }

        public T Peek(int amount)
        {
            if (!TryPeek(amount, out var item)) throw new ArgumentOutOfRangeException(nameof(item));
            return item;
        }
    }
}
