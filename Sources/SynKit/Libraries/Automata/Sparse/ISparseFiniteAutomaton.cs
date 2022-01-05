// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata.Sparse;

/// <summary>
/// Represents a sparse finite automaton that stores all transitions separately.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface ISparseFiniteAutomaton<TState, TSymbol>
    : IReadOnlySparseFiniteAutomaton<TState, TSymbol>,
      IFiniteAutomaton<TState, TSymbol>
{
    /// <summary>
    /// The alphabet of this automaton.
    /// </summary>
    public new ICollection<TSymbol> Alphabet { get; }

    /// <summary>
    /// The transitions of this automaton.
    /// </summary>
    public new ICollection<Transition<TState, TSymbol>> Transitions { get; }
}
