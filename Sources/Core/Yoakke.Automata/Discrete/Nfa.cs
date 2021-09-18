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
        public TState InitialState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ICollection<TState> AcceptingStates => throw new NotImplementedException();

        public IEnumerable<TState> States => throw new NotImplementedException();

        TState IReadOnlyFiniteAutomaton<TState, TSymbol>.InitialState => throw new NotImplementedException();

        IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.AcceptingStates => throw new NotImplementedException();

        public bool Accepts(IEnumerable<TSymbol> input) => throw new NotImplementedException();
        public bool AddEpsilonTransition(TState from, TState to) => throw new NotImplementedException();
        public bool AddTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();
        public IDfa<StateSet<TState>, TSymbol> Determinize() => throw new NotImplementedException();
        public StateSet<TState> EpsilonClosure(TState state) => throw new NotImplementedException();
        public bool RemoveEpsilonTransitions() => throw new NotImplementedException();
        public bool RemoveTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();
        public void RemoveUnreachable() => throw new NotImplementedException();
        public bool RemoveUnreachable(TState from) => throw new NotImplementedException();
        public string ToDot() => throw new NotImplementedException();
        IReadOnlyDfa<TState, StateSet<TSymbol>> IReadOnlyNfa<TState, TSymbol>.Determinize() => throw new NotImplementedException();
        bool IFiniteAutomaton<TState, TSymbol>.RemoveUnreachable() => throw new NotImplementedException();
    }
}
