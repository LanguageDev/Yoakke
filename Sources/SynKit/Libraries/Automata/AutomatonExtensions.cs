// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Linq;
using Yoakke.Collections;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Extension functions related to automata.
/// </summary>
public static class AutomatonExtensions
{
    private struct StateSetCombiner<TState> : IStateCombiner<TState, ByValueSet<TState>>
    {
        private readonly IEqualityComparer<TState> stateComparer;

        public StateSetCombiner(IEqualityComparer<TState> stateComparer)
        {
            this.stateComparer = stateComparer;
        }

        public IEqualityComparer<ByValueSet<TState>> ResultComparer =>
            EqualityComparer<ByValueSet<TState>>.Default;

        public ByValueSet<TState> Combine(IEnumerable<TState> states) => new(states, this.stateComparer);
    }

    private struct StateSetUnionCombiner<TState> : IStateCombiner<ByValueSet<TState>, ByValueSet<TState>>
    {
        public IEqualityComparer<ByValueSet<TState>> ResultComparer =>
            EqualityComparer<ByValueSet<TState>>.Default;

        public ByValueSet<TState> Combine(IEnumerable<ByValueSet<TState>> states)
        {
            var enumerator = states.GetEnumerator();
            if (!enumerator.MoveNext()) return new(Enumerable.Empty<TState>(), comparer: null);
            var comparer = enumerator.Current.Comparer;
            var result = new HashSet<TState>(enumerator.Current, comparer);
            while (enumerator.MoveNext())
            {
                foreach (var item in enumerator.Current) result.Add(item);
            }
            return new(result, comparer);
        }
    }

    /// <summary>
    /// Adds a singleton value to an interval set. Useful for DFA alphabets.
    /// </summary>
    /// <typeparam name="T">The added value type.</typeparam>
    /// <param name="intervalSet">The set to add to.</param>
    /// <param name="value">The singleton value to add.</param>
    public static void Add<T>(this ICollection<Interval<T>> intervalSet, T value) =>
        intervalSet.Add(Interval.Singleton(value));

    /// <summary>
    /// Adds a transition to the transition collection.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="transitions">The collection of transitions to add to.</param>
    /// <param name="from">The source state.</param>
    /// <param name="on">The symbol to transition on.</param>
    /// <param name="to">The destination state.</param>
    public static void Add<TState, TSymbol>(
        this ICollection<Transition<TState, Interval<TSymbol>>> transitions,
        TState from,
        TSymbol on,
        TState to) => transitions.Add(from, Interval.Singleton(on), to);

    /// <summary>
    /// Adds a transition to the transition collection.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="transitions">The collection of transitions to add to.</param>
    /// <param name="from">The source state.</param>
    /// <param name="on">The interval of symbols to transition on.</param>
    /// <param name="to">The destination state.</param>
    public static void Add<TState, TSymbol>(
        this ICollection<Transition<TState, Interval<TSymbol>>> transitions,
        TState from,
        Interval<TSymbol> on,
        TState to) => transitions.Add(new(from, on, to));

    /// <summary>
    /// Adds an epsilon-transition to the epsilon-transition collection.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="transitions">The collection of epsilon transitions to add to.</param>
    /// <param name="from">The source state.</param>
    /// <param name="to">The destination state.</param>
    public static void Add<TState>(
        this ICollection<EpsilonTransition<TState>> transitions,
        TState from,
        TState to) => transitions.Add(new(from, to));

    /// <summary>
    /// Minimizes the DFA.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <typeparam name="TResultState">The resulting state type.</typeparam>
    /// <param name="dfa">The DFA to minimize.</param>
    /// <param name="stateCombiner">The state combiner to use.</param>
    /// <returns>The equivalent minimal DFA.</returns>
    public static Dfa<TResultState, TSymbol> Minimize<TState, TSymbol, TResultState>(
        this Dfa<TState, TSymbol> dfa,
        IStateCombiner<TState, TResultState> stateCombiner) =>
        dfa.Minimize(Enumerable.Empty<(TState, TState)>(), stateCombiner);

    /// <summary>
    /// Minimizes the DFA.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="dfa">The DFA to minimize.</param>
    /// <param name="differentiate">The pair of states to differentiate.</param>
    /// <returns>The equivalent minimal DFA.</returns>
    public static Dfa<ByValueSet<TState>, TSymbol> Minimize<TState, TSymbol>(
        this Dfa<TState, TSymbol> dfa,
        IEnumerable<(TState, TState)> differentiate) =>
        dfa.Minimize(differentiate, new StateSetCombiner<TState>(dfa.StateComparer));

    /// <summary>
    /// Minimizes the DFA.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="dfa">The DFA to minimize.</param>
    /// <returns>The equivalent minimal DFA.</returns>
    public static Dfa<ByValueSet<TState>, TSymbol> Minimize<TState, TSymbol>(
        this Dfa<TState, TSymbol> dfa) =>
        dfa.Minimize(Enumerable.Empty<(TState, TState)>(), new StateSetCombiner<TState>(dfa.StateComparer));

    /// <summary>
    /// Minimizes the DFA.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="dfa">The DFA to minimize.</param>
    /// <param name="differentiate">The pair of states to differentiate.</param>
    /// <returns>The equivalent minimal DFA.</returns>
    public static Dfa<ByValueSet<TState>, TSymbol> Minimize<TState, TSymbol>(
        this Dfa<ByValueSet<TState>, TSymbol> dfa,
        IEnumerable<(ByValueSet<TState>, ByValueSet<TState>)> differentiate) =>
        dfa.Minimize(differentiate, default(StateSetUnionCombiner<TState>));

    /// <summary>
    /// Minimizes the DFA.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="dfa">The DFA to minimize.</param>
    /// <returns>The equivalent minimal DFA.</returns>
    public static Dfa<ByValueSet<TState>, TSymbol> Minimize<TState, TSymbol>(
        this Dfa<ByValueSet<TState>, TSymbol> dfa) =>
        dfa.Minimize(Enumerable.Empty<(ByValueSet<TState>, ByValueSet<TState>)>(), default(StateSetUnionCombiner<TState>));

    /// <summary>
    /// Determinizes the given NFA.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="nfa">The NFA to determinize.</param>
    /// <returns>The equivalent DFA.</returns>
    public static Dfa<ByValueSet<TState>, TSymbol> Determinize<TState, TSymbol>(
        this Nfa<TState, TSymbol> nfa) =>
        nfa.Determinize(new StateSetCombiner<TState>(nfa.StateComparer));
}
