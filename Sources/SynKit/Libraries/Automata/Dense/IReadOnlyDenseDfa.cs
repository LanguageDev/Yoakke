// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata.Dense;

/// <summary>
/// Represents a read-only, dense DFA.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface IReadOnlyDenseDfa<TState, TSymbol>
    : IReadOnlyDfa<TState, TSymbol>,
      IReadOnlyDenseFiniteAutomaton<TState, TSymbol>
{
    /// <summary>
    /// Checks, if this DFA is complete over a given alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet to check completeness over.</param>
    /// <returns>True, if the DFA is complete over <paramref name="alphabet"/>.</returns>
    public bool IsComplete(IEnumerable<Interval<TSymbol>> alphabet);

    /// <summary>
    /// Minimizes this DFA into an equivalent one with the least amount of states possible.
    /// </summary>
    /// <typeparam name="TResultState">The state type of the resulting DFA.</typeparam>
    /// <param name="combiner">The state combiner to use.</param>
    /// <param name="differentiatePairs">The pairs of states that must not be merged in the minimization process.
    /// This can be useful if some states have associated values to them that we want to keep.</param>
    /// <returns>The minimized DFA.</returns>
    public new IDenseDfa<TResultState, TSymbol> Minimize<TResultState>(
        IStateCombiner<TState, TResultState> combiner,
        IEnumerable<(TState, TState)> differentiatePairs);
}
