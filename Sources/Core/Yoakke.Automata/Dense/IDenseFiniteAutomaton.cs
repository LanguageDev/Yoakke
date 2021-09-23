// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.Automata.Dense
{
    /// <summary>
    /// Represents a dense finite automaton that stores all transitions as intervals.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public interface IDenseFiniteAutomaton<TState, TSymbol>
        : IReadOnlyDenseFiniteAutomaton<TState, TSymbol>,
          IFiniteAutomaton<TState, TSymbol>
    {
        /// <summary>
        /// The transitions of this automaton.
        /// </summary>
        public new ICollection<Transition<TState, Interval<TSymbol>>> Transitions { get; }
    }
}
