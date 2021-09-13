// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// Factory methods for <see cref="UpperBound{T}"/>.
    /// </summary>
    public static class UpperBound
    {
        /// <summary>
        /// Creates an unbounded <see cref="UpperBound{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the bound value.</typeparam>
        /// <returns>An unbounded upper bound.</returns>
        public static UpperBound<T> Unbounded<T>() => UpperBound<T>.Unbounded.Instance;

        /// <summary>
        /// Creates an exclusive <see cref="UpperBound{T}"/>, given a bound value.
        /// </summary>
        /// <typeparam name="T">The type of the bound value.</typeparam>
        /// <param name="value">The bound value.</param>
        /// <returns>An upper bound, that exclusively does not contain <paramref name="value"/>.</returns>
        public static UpperBound<T> Exclusive<T>(T value) => new UpperBound<T>.Exclusive(value);

        /// <summary>
        /// Creates an inclusive <see cref="UpperBound{T}"/>, given a bound value.
        /// </summary>
        /// <typeparam name="T">The type of the bound value.</typeparam>
        /// <param name="value">The bound value.</param>
        /// <returns>An upper bound, that inclusively contains <paramref name="value"/>.</returns>
        public static UpperBound<T> Inclusive<T>(T value) => new UpperBound<T>.Inclusive(value);
    }
}
