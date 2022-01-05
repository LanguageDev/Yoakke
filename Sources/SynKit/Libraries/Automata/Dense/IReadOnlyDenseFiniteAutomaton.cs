// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata.Dense;

/// <summary>
/// Represents a dense finite automaton that stores transitions as intervals.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface IReadOnlyDenseFiniteAutomaton<TState, TSymbol> : IReadOnlyFiniteAutomaton<TState, TSymbol>
{
    /// <summary>
    /// The alphabet of this automaton.
    /// </summary>
    public IReadOnlyCollection<Interval<TSymbol>> Alphabet { get; }

    /// <summary>
    /// The transitions of this automaton.
    /// </summary>
    public IReadOnlyCollection<Transition<TState, Interval<TSymbol>>> Transitions { get; }
}
