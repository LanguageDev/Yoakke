// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Automata
{
    /// <summary>
    /// Extensions for NFAs.
    /// </summary>
    public static class NfaExtensions
    {
        /// <summary>
        /// Constructs an equivalent DFA from this NFA.
        /// </summary>
        /// <typeparam name="TState">The state type.</typeparam>
        /// <typeparam name="TSymbol">The symbol type.</typeparam>
        /// <param name="nfa">The NFA to determinize.</param>
        /// <returns>The constructed DFA.</returns>
        public static IDfa<StateSet<TState>, TSymbol> Determinize<TState, TSymbol>(this IReadOnlyNfa<TState, TSymbol> nfa) =>
            nfa.Determinize(StateCombiner<TState>.ToSetCombiner(nfa.StateComparer));
    }
}
