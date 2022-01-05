// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata.Dense;

/// <summary>
/// Represents a dense DFA.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface IDenseDfa<TState, TSymbol>
    : IDfa<TState, TSymbol>,
      IReadOnlyDenseDfa<TState, TSymbol>,
      IDenseFiniteAutomaton<TState, TSymbol>
{
    /// <summary>
    /// Completes this DFA over the given alphabet.
    /// </summary>
    /// <param name="alphabet">The alphabet to complete over.</param>
    /// <param name="trap">A default trap state to transition to.</param>
    /// <returns>True, if this DFA was not completed and needed completion, false otherwise.</returns>
    public bool Complete(IEnumerable<Interval<TSymbol>> alphabet, TState trap);
}
