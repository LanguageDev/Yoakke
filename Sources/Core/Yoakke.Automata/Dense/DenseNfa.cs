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
    /// A generic dense NFA implementation.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public class DenseNfa<TState, TSymbol> : IDenseNfa<TState, TSymbol>
    {
        /// <inheritdoc/>
        public ICollection<TState> InitialStates => throw new NotImplementedException();

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyNfa<TState, TSymbol>.InitialStates => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICollection<TState> AcceptingStates => throw new NotImplementedException();

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.AcceptingStates => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICollection<TState> States => throw new NotImplementedException();

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.States => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICollection<Transition<TState, Interval<TSymbol>>> Transitions => throw new NotImplementedException();

        /// <inheritdoc/>
        IReadOnlyCollection<Transition<TState, Interval<TSymbol>>> IReadOnlyDenseFiniteAutomaton<TState, TSymbol>.Transitions => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICollection<EpsilonTransition<TState>> EpsilonTransitions => throw new NotImplementedException();

        /// <inheritdoc/>
        IReadOnlyCollection<EpsilonTransition<TState>> IReadOnlyNfa<TState, TSymbol>.EpsilonTransitions => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICollection<Interval<TSymbol>> Alphabet => throw new NotImplementedException();

        /// <inheritdoc/>
        IReadOnlyCollection<Interval<TSymbol>> IReadOnlyDenseFiniteAutomaton<TState, TSymbol>.Alphabet => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEqualityComparer<TState> StateComparer => throw new NotImplementedException();

        /// <summary>
        /// The comparer used for alphabet symbol intervals.
        /// </summary>
        public IntervalComparer<TSymbol> SymbolIntervalComparer => throw new NotImplementedException();

        /// <inheritdoc/>
        public string ToDot() => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Accepts(IEnumerable<TSymbol> input) => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<TState> GetTransitions(TState from, TSymbol on) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void AddTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void AddTransition(TState from, Interval<TSymbol> on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, Interval<TSymbol> on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void AddEpsilonTransition(TState from, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveEpsilonTransition(TState from, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<TState> EpsilonClosure(TState state) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveUnreachable() => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<TState> ReachableStates() => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool EliminateEpsilonTransitions() => throw new NotImplementedException();

        /// <inheritdoc/>
        IDfa<TResultState, TSymbol> IReadOnlyNfa<TState, TSymbol>.Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner) =>
            this.Determinize(combiner);

        /// <inheritdoc/>
        public IDenseDfa<TResultState, TSymbol> Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner) => throw new NotImplementedException();
    }
}
