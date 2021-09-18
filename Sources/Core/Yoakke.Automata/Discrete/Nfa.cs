// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Automata.Discrete
{
    /// <summary>
    /// A generic NFA implementation.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public class Nfa<TState, TSymbol> : INfa<TState, TSymbol>
    {
        /// <inheritdoc/>
        public TState InitialState { get; set; } = default!;

        /// <inheritdoc/>
        TState IReadOnlyFiniteAutomaton<TState, TSymbol>.InitialState => this.InitialState;

        /// <inheritdoc/>
        public ICollection<TState> AcceptingStates => this.acceptingStates;

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.AcceptingStates => this.acceptingStates;

        /// <inheritdoc/>
        public IEnumerable<TState> States => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEqualityComparer<TState> StateComparer { get; }

        /// <summary>
        /// The comparer used for alphabet symbols.
        /// </summary>
        public IEqualityComparer<TSymbol> SymbolComparer { get; }

        private readonly HashSet<TState> acceptingStates;
        private readonly Dictionary<TState, Dictionary<TSymbol, TState>> transitions;
        private readonly Dictionary<TState, HashSet<TState>> epsilonTransitions;

        public bool Accepts(IEnumerable<TSymbol> input) => throw new NotImplementedException();
        public bool AddEpsilonTransition(TState from, TState to) => throw new NotImplementedException();
        public bool AddTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();
        public IDfa<StateSet<TState>, TSymbol> Determinize() => throw new NotImplementedException();
        public StateSet<TState> EpsilonClosure(TState state) => throw new NotImplementedException();
        public StateSet<TState> GetTransitions(StateSet<TState> from, TSymbol on) => throw new NotImplementedException();
        public bool RemoveEpsilonTransitions() => throw new NotImplementedException();
        public bool RemoveTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();
        public void RemoveUnreachable() => throw new NotImplementedException();
        public bool RemoveUnreachable(TState from) => throw new NotImplementedException();
        public string ToDot() => throw new NotImplementedException();
        public IDfa<TResultState, TSymbol> Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner) => throw new NotImplementedException();
        IReadOnlyDfa<TResultState, TSymbol> IReadOnlyNfa<TState, TSymbol>.Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner) => throw new NotImplementedException();
        public bool Accepts(TState initial, IEnumerable<TSymbol> input) => throw new NotImplementedException();
    }
}
