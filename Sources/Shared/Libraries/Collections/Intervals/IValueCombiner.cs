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
    /// <param name="existing">The existing entry to combine.</param>
    /// <param name="added">The newly added entry to combine.</param>
    /// <returns>The combined entry of <paramref name="existing"/> and <paramref name="added"/>.</returns>
    public T Combine(T existing, T added);
}
