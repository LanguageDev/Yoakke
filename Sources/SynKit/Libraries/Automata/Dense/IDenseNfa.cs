// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata.Dense;

/// <summary>
/// Represents a dense NFA.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface IDenseNfa<TState, TSymbol>
    : INfa<TState, TSymbol>,
      IReadOnlyDenseNfa<TState, TSymbol>,
      IDenseFiniteAutomaton<TState, TSymbol>
{
}
