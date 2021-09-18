// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Automata.Internal;

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
            .Concat(this.transitions.Values.SelectMany(t => t.Values.SelectMany(v => v)))
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
        private readonly Dictionary<TState, Dictionary<TSymbol, HashSet<TState>>> transitions;
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
        public string ToDot()
        {
            var helper = new DotWriter<TState, TSymbol>(this);
            helper.WriteStart();

            // Go through each transition
            foreach (var (from, onMap) in this.transitions)
            {
                // For a bit more dense repr, we group by to states so we can draw a single arrow between 2 states
                var epsilon = new HashSet<TState>(this.StateComparer);
                if (this.epsilonTransitions.TryGetValue(from, out var e))
                {
                    foreach (var s in e) epsilon.Add(s);
                }
                var toOnGroup = onMap
                    .SelectMany(kv => kv.Value.Select(v => (Key: kv.Key, Value: v)))
                    .GroupBy(kv => kv.Value, this.StateComparer);
                foreach (var group in toOnGroup)
                {
                    var to = group.Key;
                    var ons = string.Join(", ", group.Select(kv => kv.Key));
                    if (epsilon.Remove(to)) ons = $"{ons}, ε";
                    helper.WriteTransition(from, ons, to);
                }
                foreach (var to in epsilon) helper.WriteTransition(from, "ε", to);
            }

            helper.WriteEnd();
            return helper.Code;
        }

        /// <inheritdoc/>
        public bool Accepts(TState initial, IEnumerable<TSymbol> input)
        {
            var currentState = new StateSet<TState>(new[] { initial });
            foreach (var symbol in input)
            {
                currentState = this.GetTransitions(currentState, symbol);
                if (currentState.Count == 0) return false;
            }
            return currentState.Any(s => this.acceptingStates.Contains(s));
        }

        /// <inheritdoc/>
        public StateSet<TState> GetTransitions(StateSet<TState> from, TSymbol on)
        {
            var set = new HashSet<TState>(this.StateComparer);
            foreach (var fromState in from.SelectMany(this.EpsilonClosureAsSet))
            {
                if (!this.transitions.TryGetValue(fromState, out var onMap)) continue;
                if (!onMap.TryGetValue(on, out var toSet)) continue;

                foreach (var s in toSet) this.EpsilonClosure(set, s);
            }
            return new(set);
        }

        /// <inheritdoc/>
        public bool AddTransition(TState from, TSymbol on, TState to)
        {
            if (!this.transitions.TryGetValue(from, out var onMap))
            {
                onMap = new(this.SymbolComparer);
                this.transitions.Add(from, onMap);
            }
            if (!onMap.TryGetValue(on, out var toSet))
            {
                toSet = new(this.StateComparer);
                onMap.Add(on, toSet);
            }
            return toSet.Add(to);
        }

        /// <inheritdoc/>
        public bool AddEpsilonTransition(TState from, TState to)
        {
            if (!this.epsilonTransitions.TryGetValue(from, out var toSet))
            {
                toSet = new(this.StateComparer);
                this.epsilonTransitions.Add(from, toSet);
            }
            return toSet.Add(to);
        }

        /// <inheritdoc/>
        public StateSet<TState> EpsilonClosure(TState state) => new(this.EpsilonClosureAsSet(state));

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, TSymbol on, TState to)
        {
            if (!this.transitions.TryGetValue(from, out var onMap)) return false;
            if (onMap.TryGetValue(on, out var toSet)) return false;
            return toSet.Remove(to);
        }

        /// <inheritdoc/>
        public bool RemoveEpsilonTransition(TState from, TState to)
        {
            if (this.epsilonTransitions.TryGetValue(from, out var toSet)) return false;
            return toSet.Remove(to);
        }

        /// <inheritdoc/>
        public INfa<TResultState, TSymbol> EliminateEpsilonTransitions<TResultState>(IStateCombiner<TState, TResultState> combiner)
        {
            var stateSetComparer = new StateSetEqualityComparer<TState>(this.StateComparer);
            var result = new Nfa<TResultState, TSymbol>(combiner.ResultComparer, this.SymbolComparer);
            var visited = new HashSet<StateSet<TState>>(new StateSetEqualityComparer<TState>(this.StateComparer));
            var stk = new Stack<StateSet<TState>>();
            var first = this.EpsilonClosure(this.InitialState);
            result.InitialState = combiner.Combine(first);
            stk.Push(first);
            while (stk.TryPop(out var top))
            {
                // Construct a transition map
                // Essentially from the current set of states we calculate what set of states we arrive at for a given symbol
                var resultTransitions = new Dictionary<TSymbol, HashSet<StateSet<TState>>>(this.SymbolComparer);
                foreach (var primState in top)
                {
                    if (!this.transitions.TryGetValue(primState, out var onMap)) continue;
                    foreach (var (on, toSet) in onMap)
                    {
                        if (!resultTransitions.TryGetValue(on, out var existing))
                        {
                            existing = new(stateSetComparer);
                            resultTransitions.Add(on, existing);
                        }
                        foreach (var to in toSet.Select(this.EpsilonClosureAsSet)) existing.Add(new(to));
                    }
                }
                // Add the transitions
                var from = combiner.Combine(top);
                foreach (var (on, toSet) in resultTransitions)
                {
                    foreach (var to in toSet)
                    {
                        if (visited.Add(to)) stk.Push(to);
                        result.AddTransition(from, on, combiner.Combine(to));
                    }
                }
                // Register as accepting, if any
                if (top.Any(s => this.AcceptingStates.Contains(s))) result.AcceptingStates.Add(from);
            }
            return result;
        }

        /// <inheritdoc/>
        IReadOnlyNfa<TResultState, TSymbol> IReadOnlyNfa<TState, TSymbol>.EliminateEpsilonTransitions<TResultState>(IStateCombiner<TState, TResultState> combiner) =>
            this.EliminateEpsilonTransitions(combiner);

        /// <inheritdoc/>
        public bool RemoveUnreachable(TState from)
        {
            var touched = this.ReachableStates(from);

            // Prune transitions that are not in this set
            var result = false;
            var untouchedStates = this.transitions.Keys.Except(touched, this.StateComparer);
            foreach (var untouched in untouchedStates)
            {
                if (this.transitions.Remove(untouched) || this.epsilonTransitions.Remove(untouched)) result = true;
            }
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<TState> ReachableStates(TState initial)
        {
            var touched = new HashSet<TState>(this.StateComparer);
            var stk = new Stack<TState>();
            stk.Push(initial);
            touched.Add(initial);
            while (stk.TryPop(out var top))
            {
                yield return top;
                if (!this.transitions.TryGetValue(top, out var onMap)) continue;
                foreach (var toSet in onMap.Values)
                {
                    foreach (var to in toSet)
                    {
                        if (touched.Add(to)) stk.Push(to);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IDfa<TResultState, TSymbol> Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner)
        {
            var result = new Dfa<TResultState, TSymbol>(combiner.ResultComparer, this.SymbolComparer);
            var visited = new HashSet<StateSet<TState>>(new StateSetEqualityComparer<TState>(this.StateComparer));
            var stk = new Stack<StateSet<TState>>();
            var first = this.EpsilonClosure(this.InitialState);
            result.InitialState = combiner.Combine(first);
            stk.Push(first);
            while (stk.TryPop(out var top))
            {
                // Construct a transition map
                // Essentially from the current set of states we calculate what set of states we arrive at for a given symbol
                var resultTransitions = new Dictionary<TSymbol, HashSet<TState>>(this.SymbolComparer);
                foreach (var primState in top)
                {
                    if (!this.transitions.TryGetValue(primState, out var onMap)) continue;
                    foreach (var (on, toSet) in onMap)
                    {
                        if (!resultTransitions.TryGetValue(on, out var existing))
                        {
                            existing = new(this.StateComparer);
                            resultTransitions.Add(on, existing);
                        }
                        foreach (var to in toSet.SelectMany(this.EpsilonClosureAsSet)) existing.Add(to);
                    }
                }
                // Add the transitions
                var from = combiner.Combine(top);
                foreach (var (on, toHashSet) in resultTransitions)
                {
                    var toSet = new StateSet<TState>(toHashSet);
                    if (visited.Add(toSet)) stk.Push(toSet);
                    var to = combiner.Combine(toSet);
                    result.AddTransition(from, on, to);
                }
                // Register as accepting, if any
                if (top.Any(s => this.AcceptingStates.Contains(s))) result.AcceptingStates.Add(from);
            }
            return result;
        }

        /// <inheritdoc/>
        IReadOnlyDfa<TResultState, TSymbol> IReadOnlyNfa<TState, TSymbol>.Determinize<TResultState>(
            IStateCombiner<TState, TResultState> combiner) => this.Determinize(combiner);

        private HashSet<TState> EpsilonClosureAsSet(TState state)
        {
            var set = new HashSet<TState>(this.StateComparer);
            this.EpsilonClosure(set, state);
            return new(set);
        }

        private void EpsilonClosure(HashSet<TState> touched, TState state)
        {
            var stk = new Stack<TState>();
            if (!touched.Add(state)) return;
            stk.Push(state);
            while (stk.TryPop(out var top))
            {
                if (!this.epsilonTransitions.TryGetValue(top, out var onSet)) continue;
                foreach (var to in onSet)
                {
                    if (touched.Add(to)) stk.Push(to);
                }
            }
        }
    }
}
