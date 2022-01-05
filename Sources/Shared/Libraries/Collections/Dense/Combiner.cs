// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Dense;

/// <summary>
/// Utilities for <see cref="ICombiner{T}"/>s.
/// </summary>
/// <typeparam name="T">The type of values to combine.</typeparam>
public static class Combiner<T>
{
    private class OverrideCombiner : ICombiner<T>
    {
        public T Combine(T existing, T added) => added;
    }

    private class LambdaCombiner : ICombiner<T>
    {
        private readonly Combine combine;

        public LambdaCombiner(Combine combine)
        {
            this.combine = combine;
        }

        public T Combine(T existing, T added) => this.combine(existing, added);
    }

    /// <summary>
    /// Combines an existing value with a new one.
    /// </summary>
    /// <param name="existing">The existing value.</param>
    /// <param name="added">The new value.</param>
    /// <returns>The new, combined value of the existing and the new one.</returns>
    public delegate T Combine(T existing, T added);

    /// <summary>
    /// A default instance, that simply overrides the existing value with the new one.
    /// </summary>
    public static ICombiner<T> Default { get; } = new OverrideCombiner();

    /// <summary>
    /// Creates a combiner from the given <see cref="Combine"/> delegate.
    /// </summary>
    /// <param name="combine">The delegate to use for combination.</param>
    /// <returns>The combiner created from <paramref name="combine"/>.</returns>
    public static ICombiner<T> Create(Combine combine) => new LambdaCombiner(combine);
}
