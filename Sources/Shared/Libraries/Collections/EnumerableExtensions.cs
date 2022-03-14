// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections;

/// <summary>
/// Extensions for <see cref="IEnumerable{T}"/>s.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Turns a single value into an enumerable.
    /// </summary>
    /// <typeparam name="T">The enumerable element type.</typeparam>
    /// <param name="value">The value to wrap up.</param>
    /// <returns>An enumerable only containing <paramref name="value"/>.</returns>
    public static IEnumerable<T> Singleton<T>(T value)
    {
        yield return value;
    }
}
