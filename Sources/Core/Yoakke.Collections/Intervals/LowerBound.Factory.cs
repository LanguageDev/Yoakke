// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals
{
    /// <summary>
    /// Factory methods for <see cref="LowerBound{T}"/>.
    /// </summary>
    public static class LowerBound
    {
        /// <summary>
        /// Creates an unbounded <see cref="LowerBound{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of the bound value.</typeparam>
        /// <returns>An unbounded lower bound.</returns>
        public static LowerBound<T> Unbounded<T>() => LowerBound<T>.Unbounded.Instance;

        /// <summary>
        /// Creates an exclusive <see cref="LowerBound{T}"/>, given a bound value.
        /// </summary>
        /// <typeparam name="T">The type of the bound value.</typeparam>
        /// <param name="value">The bound value.</param>
        /// <returns>A lower bound, that exclusively does not contain <paramref name="value"/>.</returns>
        public static LowerBound<T> Exclusive<T>(T value) => new LowerBound<T>.Exclusive(value);

        /// <summary>
        /// Creates an inclusive <see cref="LowerBound{T}"/>, given a bound value.
        /// </summary>
        /// <typeparam name="T">The type of the bound value.</typeparam>
        /// <param name="value">The bound value.</param>
        /// <returns>A lower bound, that inclusively contains <paramref name="value"/>.</returns>
        public static LowerBound<T> Inclusive<T>(T value) => new LowerBound<T>.Inclusive(value);
    }
}
