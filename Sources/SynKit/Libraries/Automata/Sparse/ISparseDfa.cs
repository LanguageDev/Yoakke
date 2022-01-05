// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata.Sparse;

/// <summary>
/// Represents a sparse DFA.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface ISparseDfa<TState, TSymbol>
    : IDfa<TState, TSymbol>,
      IReadOnlySparseDfa<TState, TSymbol>,
      ISparseFiniteAutomaton<TState, TSymbol>
{
    /// <summary>
    /// Completes this DFA over its alphabet.
    /// </summary>
    /// <param name="trap">A default trap state to transition to.</param>
    /// <returns>True, if this DFA was not completed and needed completion, false otherwise.</returns>
    public bool Complete(TState trap);
}
