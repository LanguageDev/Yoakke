// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// A combiner that can combine states and produce new ones for finite automata algorithms.
/// </summary>
/// <typeparam name="TState">The original state type to combine.</typeparam>
/// <typeparam name="TResultState">The combined state type.</typeparam>
public interface IStateCombiner<TState, TResultState>
{
    /// <summary>
    /// Retrieves the comparer to be used by the result type.
    /// </summary>
    public IEqualityComparer<TResultState> ResultComparer { get; }

    /// <summary>
    /// Combines the given <paramref name="states"/> into a <typeparamref name="TResultState"/>.
    /// </summary>
    /// <param name="states">The sequence of states to combine.</param>
    /// <returns>The combined states.</returns>
    public TResultState Combine(IEnumerable<TState> states);
}
