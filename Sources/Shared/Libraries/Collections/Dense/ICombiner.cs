// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Dense;

/// <summary>
/// An object that can combine multiple values into one.
/// Can be used to combine overlapping values in a mapping for example.
/// </summary>
/// <typeparam name="T">The type of values to combine.</typeparam>
public interface ICombiner<T>
{
    /// <summary>
    /// Combines an existing value with a new one.
    /// </summary>
    /// <param name="existing">The existing value.</param>
    /// <param name="added">The new value.</param>
    /// <returns>The new, combined value of <paramref name="existing"/> and <paramref name="added"/>.</returns>
    public T Combine(T existing, T added);
}
