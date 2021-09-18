// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Automata
{
    /// <summary>
    /// Represents a generic, readonly deterministic finite automaton.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public interface IReadOnlyDfa<TState, TSymbol> : IReadOnlyFiniteAutomaton<TState, TSymbol>
    {
        /// <summary>
        /// Checks, if this DFA is complete over a given alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet to check completeness over.</param>
        /// <returns>True, if this DFA is complete over <paramref name="alphabet"/>, otherwise false.</returns>
        public bool IsComplete(IEnumerable<TSymbol> alphabet);
    }
}
