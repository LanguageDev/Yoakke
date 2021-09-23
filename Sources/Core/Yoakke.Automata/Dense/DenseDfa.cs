// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.Automata.Dense
{
    /// <summary>
    /// A generic dense DFA implementation.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public class DenseDfa<TState, TSymbol> : IDenseDfa<TState, TSymbol>
    {
        /// <inheritdoc/>
        public TState InitialState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        TState IReadOnlyDfa<TState, TSymbol>.InitialState => this.InitialState;

        /// <inheritdoc/>
        public ICollection<TState> AcceptingStates => throw new NotImplementedException();

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.AcceptingStates => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICollection<Transition<TState, Interval<TSymbol>>> Transitions => throw new NotImplementedException();

        /// <inheritdoc/>
        IReadOnlyCollection<Transition<TState, Interval<TSymbol>>> IReadOnlyDenseFiniteAutomaton<TState, TSymbol>.Transitions => throw new NotImplementedException();

        /// <inheritdoc/>
        public ICollection<TState> States => throw new NotImplementedException();

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.States => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEqualityComparer<TState> StateComparer => throw new NotImplementedException();

        /// <inheritdoc/>
        public string ToDot() => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Accepts(IEnumerable<TSymbol> input) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool TryGetTransition(TState from, TSymbol on, [MaybeNullWhen(false)] out TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void AddTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void AddTransition(TState from, Interval<TSymbol> on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, Interval<TSymbol> on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<TState> ReachableStates() => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveUnreachable() => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsComplete(IEnumerable<Interval<TSymbol>> alphabet) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Complete(IEnumerable<Interval<TSymbol>> alphabet, TState trap) => throw new NotImplementedException();

        /// <inheritdoc/>
        IDfa<TResultState, TSymbol> IReadOnlyDfa<TState, TSymbol>.Minimize<TResultState>(
            IStateCombiner<TState, TResultState> combiner,
            IEnumerable<(TState, TState)> differentiatePairs) => this.Minimize(combiner, differentiatePairs);

        /// <inheritdoc/>
        public IDenseDfa<TResultState, TSymbol> Minimize<TResultState>(
            IStateCombiner<TState, TResultState> combiner,
            IEnumerable<(TState, TState)> differentiatePairs)
        {
            throw new NotImplementedException();
        }
    }
}
