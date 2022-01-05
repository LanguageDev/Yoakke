// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata.Sparse;

/// <summary>
/// Represents a read-only, sparse NFA.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface IReadOnlySparseNfa<TState, TSymbol>
    : IReadOnlyNfa<TState, TSymbol>,
      IReadOnlySparseFiniteAutomaton<TState, TSymbol>
{
    /// <summary>
    /// Constructs an equivalent DFA from this NFA.
    /// </summary>
    /// <typeparam name="TResultState">The result state type.</typeparam>
    /// <param name="combiner">The state combiner to use.</param>
    /// <returns>The constructed DFA.</returns>
    public new ISparseDfa<TResultState, TSymbol> Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner);
}
