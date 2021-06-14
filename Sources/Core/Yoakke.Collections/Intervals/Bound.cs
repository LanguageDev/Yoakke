// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Diagnostics.CodeAnalysis;

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
                if (this.Type == BoundType.Unbounded) throw new InvalidOperationException();
                return this.value!;
            }
        }

        private readonly T? value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bound{T}"/> struct.
        /// </summary>
        /// <param name="type">The type of the bound.</param>
        /// <param name="value">The associated value of the bound, if any.</param>
        public Bound(BoundType type, T? value)
        {
            this.Type = type;
            this.value = value;
        }

        /// <summary>
        /// Checks, if this is an unbounded interval bound.
        /// </summary>
        /// <returns>True, if this is an unbounded interval bound, false otherwise.</returns>
        public bool IsUnbounded() => this.Type == BoundType.Unbounded;

        /// <summary>
        /// Checks, if this is an interval bound that includes it's associated value.
        /// </summary>
        /// <param name="value">The intervals associated value is written here, if this is an inclusive bound.</param>
        /// <returns>True, if this is an inclusive bound.</returns>
        public bool IsInclusive([MaybeNullWhen(false)] out T? value)
        {
            if (this.Type == BoundType.Inclusive)
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
        public bool IsExclusive([MaybeNullWhen(false)] out T? value)
        {
            if (this.Type == BoundType.Exclusive)
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
