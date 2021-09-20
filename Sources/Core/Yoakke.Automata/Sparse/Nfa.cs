// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;
using Yoakke.Automata.Internal;

namespace Yoakke.Automata.Sparse
{
    /// <summary>
    /// A generic sparse NFA implementation.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public sealed class Nfa<TState, TSymbol> : ISparseNfa<TState, TSymbol>
    {
#if false
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
            .Concat(this.AcceptingStates)
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
#endif

        // Utility to solve the cross-references between the different collections we expose
        private class TransitionCollection
            : IReadOnlyCollection<TState>, ICollection<TState>,
              IReadOnlyCollection<Transition<TState, TSymbol>>, ICollection<Transition<TState, TSymbol>>,
              IReadOnlyCollection<EpsilonTransition<TState>>, ICollection<EpsilonTransition<TState>>
        {
            /* Comparers */

            public IEqualityComparer<TState> StateComparer { get; }

            public IEqualityComparer<TSymbol> SymbolComparer { get; }

            /* All containers */

            public Dictionary<TState, Dictionary<TSymbol, HashSet<TState>>> TransitionMap { get; }

            public Dictionary<TState, HashSet<TState>> EpsilonTransitionMap { get; }

            public HashSet<TState> AllStates { get; }

            public HashSet<TState> InitialStates { get; }

            public HashSet<TState> AcceptingStates { get; }

            /* Count crud */

            int IReadOnlyCollection<TState>.Count => this.AllStates.Count;

            int ICollection<TState>.Count => (this as IReadOnlyCollection<TState>).Count;

            int IReadOnlyCollection<Transition<TState, TSymbol>>.Count =>
                this.TransitionMap.Values.Sum(v => v.Values.Sum(s => s.Count));

            int ICollection<Transition<TState, TSymbol>>.Count => (this as IReadOnlyCollection<Transition<TState, TSymbol>>).Count;

            int IReadOnlyCollection<EpsilonTransition<TState>>.Count =>
                this.EpsilonTransitionMap.Values.Sum(v => v.Count);

            int ICollection<EpsilonTransition<TState>>.Count => (this as IReadOnlyCollection<EpsilonTransition<TState>>).Count;

            public bool IsReadOnly => false;

            public TransitionCollection(
                IEqualityComparer<TState> stateComparer,
                IEqualityComparer<TSymbol> symbolComparer)
            {
                this.StateComparer = stateComparer;
                this.SymbolComparer = symbolComparer;
                this.TransitionMap = new(stateComparer);
                this.EpsilonTransitionMap = new(stateComparer);
                this.AllStates = new(stateComparer);
                this.InitialStates = new(stateComparer);
                this.AcceptingStates = new(stateComparer);
            }

            void ICollection<TState>.Clear()
            {
                this.TransitionMap.Clear();
                this.AllStates.Clear();
                this.InitialStates.Clear();
                this.AcceptingStates.Clear();
            }

            void ICollection<Transition<TState, TSymbol>>.Clear() => this.TransitionMap.Clear();

            void ICollection<EpsilonTransition<TState>>.Clear() => this.EpsilonTransitionMap.Clear();

            public void Add(TState item) => this.AllStates.Add(item);

            public void Add(Transition<TState, TSymbol> item) => this.AddBool(item);

            public bool AddBool(Transition<TState, TSymbol> item)
            {
                var onMap = this.GetTransitionsFrom(item.Source);
                if (!onMap.TryGetValue(item.Symbol, out var toSet))
                {
                    toSet = new(this.StateComparer);
                    onMap.Add(item.Symbol, toSet);
                }
                this.AllStates.Add(item.Source);
                this.AllStates.Add(item.Destination);
                return toSet.Add(item.Destination);
            }

            public void Add(EpsilonTransition<TState> item) => this.AddBool(item);

            public bool AddBool(EpsilonTransition<TState> item)
            {
                if (!this.EpsilonTransitionMap.TryGetValue(item.Source, out var toSet))
                {
                    toSet = new(this.StateComparer);
                    this.EpsilonTransitionMap.Add(item.Source, toSet);
                }
                this.AllStates.Add(item.Source);
                this.AllStates.Add(item.Destination);
                return toSet.Add(item.Destination);
            }

            public bool Remove(TState item)
            {
                if (!this.AllStates.Remove(item)) return false;
                this.AcceptingStates.Remove(item);
                this.InitialStates.Remove(item);
                // Remove both ways from transitions
                this.TransitionMap.Remove(item);
                foreach (var map in this.TransitionMap.Values)
                {
                    foreach (var toSet in map.Values) toSet.Remove(item);
                }
                return true;
            }

            public bool Remove(Transition<TState, TSymbol> item)
            {
                if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
                if (!onMap.TryGetValue(item.Symbol, out var toSet)) return false;
                return toSet.Remove(item.Destination);
            }

            public bool Remove(EpsilonTransition<TState> item)
            {
                if (!this.EpsilonTransitionMap.TryGetValue(item.Source, out var toSet)) return false;
                return toSet.Remove(item.Destination);
            }

            public bool Contains(TState item) => this.AllStates.Contains(item);

            public bool Contains(Transition<TState, TSymbol> item)
            {
                if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
                if (!onMap.TryGetValue(item.Symbol, out var toSet)) return false;
                return toSet.Contains(item.Destination);
            }

            public bool Contains(EpsilonTransition<TState> item)
            {
                if (!this.EpsilonTransitionMap.TryGetValue(item.Source, out var toSet)) return false;
                return toSet.Contains(item.Destination);
            }

            public void CopyTo(TState[] array, int arrayIndex) => this.AllStates.CopyTo(array, arrayIndex);

            public void CopyTo(Transition<TState, TSymbol>[] array, int arrayIndex)
            {
                foreach (var t in this as IEnumerable<Transition<TState, TSymbol>>) array[arrayIndex++] = t;
            }

            public void CopyTo(EpsilonTransition<TState>[] array, int arrayIndex)
            {
                foreach (var t in this as IEnumerable<EpsilonTransition<TState>>) array[arrayIndex++] = t;
            }

            IEnumerator<Transition<TState, TSymbol>> IEnumerable<Transition<TState, TSymbol>>.GetEnumerator()
            {
                foreach (var (from, onMap) in this.TransitionMap)
                {
                    foreach (var (on, toSet) in onMap)
                    {
                        foreach (var to in toSet) yield return new(from, on, to);
                    }
                }
            }

            IEnumerator<EpsilonTransition<TState>> IEnumerable<EpsilonTransition<TState>>.GetEnumerator()
            {
                foreach (var (from, toSet) in this.EpsilonTransitionMap)
                {
                    foreach (var to in toSet) yield return new(from, to);
                }
            }

            IEnumerator<TState> IEnumerable<TState>.GetEnumerator() => this.AllStates.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

            public Dictionary<TSymbol, HashSet<TState>> GetTransitionsFrom(TState from)
            {
                if (!this.TransitionMap.TryGetValue(from, out var onMap))
                {
                    onMap = new(this.SymbolComparer);
                    this.TransitionMap.Add(from, onMap);
                }
                return onMap;
            }
        }

        // Helper to expose the accepting states
        private class StateCollection : IReadOnlyCollection<TState>, ICollection<TState>
        {
            public int Count => this.transitions.AcceptingStates.Count;

            public bool IsReadOnly => false;

            private readonly TransitionCollection transitions;
            private readonly HashSet<TState> states;

            public StateCollection(TransitionCollection transitions, HashSet<TState> states)
            {
                this.transitions = transitions;
                this.states = states;
            }

            public void Add(TState item)
            {
                this.states.Add(item);
                this.transitions.Add(item);
            }

            public void Clear() => this.states.Clear();

            public bool Contains(TState item) => this.states.Contains(item);

            public void CopyTo(TState[] array, int arrayIndex) => this.states.CopyTo(array, arrayIndex);

            public bool Remove(TState item) => this.states.Remove(item);

            public IEnumerator<TState> GetEnumerator() => this.states.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => (this.states as IEnumerable).GetEnumerator();
        }

        /// <inheritdoc/>
        public ICollection<TState> InitialStates => this.initial;

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyNfa<TState, TSymbol>.InitialStates => this.initial;

        /// <inheritdoc/>
        public ICollection<TState> AcceptingStates => this.accepting;

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.AcceptingStates => this.accepting;

        /// <inheritdoc/>
        public ICollection<Transition<TState, TSymbol>> Transitions => this.transitions;

        /// <inheritdoc/>
        IReadOnlyCollection<Transition<TState, TSymbol>> IReadOnlySparseFiniteAutomaton<TState, TSymbol>.Transitions => this.transitions;

        /// <inheritdoc/>
        public ICollection<EpsilonTransition<TState>> EpsilonTransitions => this.transitions;

        /// <inheritdoc/>
        IReadOnlyCollection<EpsilonTransition<TState>> IReadOnlySparseNfa<TState, TSymbol>.EpsilonTransitions => this.transitions;

        /// <inheritdoc/>
        public ICollection<TState> States => this.transitions;

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.States => this.transitions;

        /// <inheritdoc/>
        public IEqualityComparer<TState> StateComparer => this.transitions.StateComparer;

        /// <summary>
        /// The comparer used for alphabet symbols.
        /// </summary>
        public IEqualityComparer<TSymbol> SymbolComparer => this.transitions.SymbolComparer;

        private readonly TransitionCollection transitions;
        private readonly StateCollection initial;
        private readonly StateCollection accepting;

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
            this.transitions = new(stateComparer, symbolComparer);
            this.initial = new(this.transitions, this.transitions.InitialStates);
            this.accepting = new(this.transitions, this.transitions.AcceptingStates);
        }

        /// <inheritdoc/>
        public string ToDot() => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Accepts(IEnumerable<TSymbol> input) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Accepts(TState initial, IEnumerable<TSymbol> input) => throw new NotImplementedException();

        /// <inheritdoc/>
        public StateSet<TState> GetTransitions(StateSet<TState> from, TSymbol on) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool AddTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public StateSet<TState> EpsilonClosure(TState state) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool AddEpsilonTransition(TState from, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveEpsilonTransition(TState from, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveUnreachable() => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveUnreachable(TState from) => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<TState> ReachableStates() => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<TState> ReachableStates(TState initial) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool EliminateEpsilonTransitions() => throw new NotImplementedException();

        /// <inheritdoc/>
        IDfa<TResultState, TSymbol> IReadOnlyNfa<TState, TSymbol>.Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner) =>
            this.Determinize(combiner);

        /// <inheritdoc/>
        public ISparseDfa<TResultState, TSymbol> Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner) => throw new NotImplementedException();
    }
}
