// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Represents a generic deterministic finite automaton.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface IDfa<TState, TSymbol> : IReadOnlyDfa<TState, TSymbol>, IFiniteAutomaton<TState, TSymbol>
{
    /// <summary>
    /// The initial state of the automaton.
    /// </summary>
    public new TState InitialState { get; set; }
}
