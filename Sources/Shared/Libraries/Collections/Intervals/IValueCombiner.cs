// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// A value combiner to combine multiple entries.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public interface IValueCombiner<T>
{
    /// <summary>
    /// Combines the entries into a single entry.
    /// </summary>
    /// <param name="first">The first entry to combine.</param>
    /// <param name="second">The second entry to combine.</param>
    /// <returns>The combined entry of <paramref name="first"/> and <paramref name="second"/>.</returns>
    public T Combine(T first, T second);
}
