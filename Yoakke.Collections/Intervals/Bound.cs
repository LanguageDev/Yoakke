using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// Represents some bound of an interval.
    /// </summary>
    /// <typeparam name="T">The type of the interval values.</typeparam>
    public readonly struct Bound<T>
    {
        /// <summary>
        /// The type of this bound.
        /// </summary>
        public readonly BoundType Type;

        /// <summary>
        /// The associated value of this bound. Only valid when the bound is not unbounded.
        /// </summary>
        public T Value
        {
            get
            {
                if (Type == BoundType.Unbounded) throw new InvalidOperationException();
                return value;
            }
        }

        private readonly T value;

        /// <summary>
        /// Initializes a new <see cref="Bound{T}"/>.
        /// </summary>
        /// <param name="type">The type of the bound.</param>
        /// <param name="value">The associated value of the bound, if any.</param>
        public Bound(BoundType type, T value)
        {
            Type = type;
            this.value = value;
        }

        /// <summary>
        /// Checks, if this is an unbounded interval bound.
        /// </summary>
        /// <returns>True, if this is an unbounded interval bound, false otherwise.</returns>
        public bool IsUnbounded() => Type == BoundType.Unbounded;

        /// <summary>
        /// Checks, if this is an interval bound that includes it's associated value.
        /// </summary>
        /// <param name="value">The intervals associated value is written here, if this is an inclusive bound.</param>
        /// <returns>True, if this is an inclusive bound.</returns>
        public bool IsInclusive(out T value)
        {
            if (Type == BoundType.Inclusive)
            {
                value = this.value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Checks, if this is an interval bound that excludes it's associated value.
        /// </summary>
        /// <param name="value">The intervals associated value is written here, if this is an exclusive bound.</param>
        /// <returns>True, if this is an exclusive bound.</returns>
        public bool IsExclusive(out T value)
        {
            if (Type == BoundType.Exclusive)
            {
                value = this.value;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}
