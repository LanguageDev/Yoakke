// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Automata.Sparse
{
    /// <summary>
    /// Represents a read-only, sparse DFA.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public interface IReadOnlySparseDfa<TState, TSymbol>
        : IReadOnlyDfa<TState, TSymbol>,
          IReadOnlySparseFiniteAutomaton<TState, TSymbol>
    {
    }
}
