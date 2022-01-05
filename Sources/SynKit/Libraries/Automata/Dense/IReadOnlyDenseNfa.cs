// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata.Dense;

/// <summary>
/// Represents a read-only, dense NFA.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface IReadOnlyDenseNfa<TState, TSymbol>
    : IReadOnlyNfa<TState, TSymbol>,
      IReadOnlyDenseFiniteAutomaton<TState, TSymbol>
{
    /// <summary>
    /// The comparer for symbol intervals.
    /// </summary>
    public IntervalComparer<TSymbol> SymbolIntervalComparer { get; }

    /// <summary>
    /// Constructs an equivalent DFA from this NFA.
    /// </summary>
    /// <typeparam name="TResultState">The result state type.</typeparam>
    /// <param name="combiner">The state combiner to use.</param>
    /// <returns>The constructed DFA.</returns>
    public new IDenseDfa<TResultState, TSymbol> Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner);
}
