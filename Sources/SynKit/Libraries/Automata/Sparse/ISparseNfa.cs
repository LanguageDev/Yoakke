// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata.Sparse;

/// <summary>
/// Represents a sparse NFA.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface ISparseNfa<TState, TSymbol>
    : INfa<TState, TSymbol>,
      IReadOnlySparseNfa<TState, TSymbol>,
      ISparseFiniteAutomaton<TState, TSymbol>
{
}
