// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Automata
{
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

        /// <summary>
        /// Completes this DFA over the given alphabet.
        /// </summary>
        /// <param name="alphabet">The alphabet to complete over.</param>
        /// <param name="trap">A default trap state to transition to.</param>
        /// <returns>True, if this DFA was not completed and needed completion, false otherwise.</returns>
        public bool Complete(IEnumerable<TSymbol> alphabet, TState trap);
    }
}
