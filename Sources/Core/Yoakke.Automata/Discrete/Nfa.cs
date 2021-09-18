// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
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
        public IEnumerable<TState> States => this.transitions.Keys
            .Concat(this.transitions.Values.SelectMany(t => t.Values))
            .Concat(this.epsilonTransitions.Keys)
            .Concat(this.epsilonTransitions.Values.SelectMany(v => v))
            .Append(this.InitialState)
            .Distinct(this.StateComparer);

        /// <inheritdoc/>
        public IEqualityComparer<TState> StateComparer { get; }

        /// <summary>
        /// The comparer used for alphabet symbols.
        /// </summary>
        public IEqualityComparer<TSymbol> SymbolComparer { get; }

        private readonly HashSet<TState> acceptingStates;
        private readonly Dictionary<TState, Dictionary<TSymbol, TState>> transitions;
        private readonly Dictionary<TState, HashSet<TState>> epsilonTransitions;

        /// <summary>
        /// Initializes a new instance of the <see cref="Nfa{TState, TSymbol}"/> class.
        /// </summary>
        public Nfa()
            : this(EqualityComparer<TState>.Default, EqualityComparer<TSymbol>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Nfa{TState, TSymbol}"/> class.
        /// </summary>
        /// <param name="stateComparer">The state comparer to use.</param>
        /// <param name="symbolComparer">The symbol comparer to use.</param>
        public Nfa(IEqualityComparer<TState> stateComparer, IEqualityComparer<TSymbol> symbolComparer)
        {
            this.StateComparer = stateComparer;
            this.SymbolComparer = symbolComparer;

            this.acceptingStates = new(stateComparer);
            this.transitions = new(stateComparer);
            this.epsilonTransitions = new(stateComparer);
        }

        /// <inheritdoc/>
        public string ToDot() => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Accepts(TState initial, IEnumerable<TSymbol> input) => throw new NotImplementedException();

        /// <inheritdoc/>
        public StateSet<TState> GetTransitions(StateSet<TState> from, TSymbol on) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool AddTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool AddEpsilonTransition(TState from, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public StateSet<TState> EpsilonClosure(TState state) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveEpsilonTransitions() => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveUnreachable(TState from) => throw new NotImplementedException();

        /// <inheritdoc/>
        public IDfa<TResultState, TSymbol> Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner) => throw new NotImplementedException();

        /// <inheritdoc/>
        IReadOnlyDfa<TResultState, TSymbol> IReadOnlyNfa<TState, TSymbol>.Determinize<TResultState>(
            IStateCombiner<TState, TResultState> combiner) => this.Determinize(combiner);
    }
}
