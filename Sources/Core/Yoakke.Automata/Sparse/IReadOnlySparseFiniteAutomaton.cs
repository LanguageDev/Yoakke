// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Automata.Sparse
{
    /// <summary>
    /// Represents a sparse finite automaton that stores all transitions separately.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public interface IReadOnlySparseFiniteAutomaton<TState, TSymbol> : IReadOnlyFiniteAutomaton<TState, TSymbol>
    {
        /// <summary>
        /// The alphabed of this automaton.
        /// </summary>
        public IReadOnlyCollection<TSymbol> Alphabet { get; }

        /// <summary>
        /// The transitions of this automaton.
        /// </summary>
        public IReadOnlyCollection<Transition<TState, TSymbol>> Transitions { get; }
    }
}
