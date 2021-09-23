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
using Yoakke.Collections;
using Yoakke.Collections.Graphs;

namespace Yoakke.Automata.Sparse
{
    /// <summary>
    /// A generic sparse NFA implementation.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public sealed class Nfa<TState, TSymbol> : ISparseNfa<TState, TSymbol>
    {
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

            public void Add(Transition<TState, TSymbol> item)
            {
                var onMap = this.GetTransitionsFrom(item.Source);
                if (!onMap.TryGetValue(item.Symbol, out var toSet))
                {
                    toSet = new(this.StateComparer);
                    onMap.Add(item.Symbol, toSet);
                }
                this.AllStates.Add(item.Source);
                this.AllStates.Add(item.Destination);
                toSet.Add(item.Destination);
            }

            public void Add(EpsilonTransition<TState> item)
            {
                if (!this.EpsilonTransitionMap.TryGetValue(item.Source, out var toSet))
                {
                    toSet = new(this.StateComparer);
                    this.EpsilonTransitionMap.Add(item.Source, toSet);
                }
                this.AllStates.Add(item.Source);
                this.AllStates.Add(item.Destination);
                toSet.Add(item.Destination);
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
        IReadOnlyCollection<EpsilonTransition<TState>> IReadOnlyNfa<TState, TSymbol>.EpsilonTransitions => this.transitions;

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
        public string ToDot()
        {
            var writer = new DotWriter<TState, TSymbol>(this.StateComparer);
            writer.WriteStart("NFA");

            // Accepting states
            writer.WriteAcceptingStates(this.AcceptingStates);
            // Non-accepting states
            writer.WriteStates(this.States.Except(this.AcceptingStates, this.StateComparer));
            // Initial states
            writer.WriteInitialStates(this.InitialStates);

            // Transitions
            var tupleComparer = new TupleEqualityComparer<TState, TState>(this.StateComparer, this.StateComparer);
            var transitions = this.Transitions.GroupBy(t => (t.Source, t.Destination), tupleComparer);
            var remainingEpsilon = this.transitions.EpsilonTransitionMap
                .ToDictionary(kv => kv.Key, kv => kv.Value.ToHashSet(this.StateComparer), this.StateComparer);
            foreach (var group in transitions)
            {
                var from = group.Key.Item1;
                var to = group.Key.Item2;
                var on = string.Join(", ", group.Select(g => g.Symbol));
                if (this.EpsilonTransitions.Contains(new(from, to)))
                {
                    remainingEpsilon[from].Remove(to);
                    on = $"{on}, ε";
                }
                writer.WriteTransition(from, on, to);
            }

            // Epsilon-transitions
            foreach (var (from, toSet) in remainingEpsilon)
            {
                foreach (var to in toSet) writer.WriteTransition(from, "ε", to);
            }

            writer.WriteEnd();
            return writer.Code.ToString();
        }

        /// <inheritdoc/>
        public bool Accepts(IEnumerable<TSymbol> input)
        {
            var currentState = new StateSet<TState>(this.InitialStates, this.StateComparer);
            foreach (var symbol in input)
            {
                currentState = this.GetTransitions(currentState, symbol);
                if (currentState.Count == 0) return false;
            }
            return currentState.Overlaps(this.AcceptingStates);
        }

        /// <inheritdoc/>
        public IEnumerable<TState> GetTransitions(TState from, TSymbol on)
        {
            var touched = new HashSet<TState>(this.StateComparer);
            foreach (var fromState in this.EpsilonClosure(from))
            {
                if (!this.transitions.TransitionMap.TryGetValue(fromState, out var onMap)) continue;
                if (!onMap.TryGetValue(on, out var toSet)) continue;

                foreach (var s in toSet.SelectMany(this.EpsilonClosure))
                {
                    if (touched.Add(s)) yield return s;
                }
            }
        }

        /// <inheritdoc/>
        public void AddTransition(TState from, TSymbol on, TState to) => this.Transitions.Add(new(from, on, to));

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, TSymbol on, TState to) => this.Transitions.Remove(new(from, on, to));

        /// <inheritdoc/>
        public void AddEpsilonTransition(TState from, TState to) => this.EpsilonTransitions.Add(new(from, to));

        /// <inheritdoc/>
        public bool RemoveEpsilonTransition(TState from, TState to) => this.EpsilonTransitions.Remove(new(from, to));

        /// <inheritdoc/>
        public IEnumerable<TState> EpsilonClosure(TState state) => BreadthFirst.Search(
            state,
            state => this.transitions.EpsilonTransitionMap.TryGetValue(state, out var eps)
                ? eps
                : Enumerable.Empty<TState>(),
            this.StateComparer);

        /// <inheritdoc/>
        public bool RemoveUnreachable()
        {
            var unreachable = this.transitions.AllStates.Except(this.ReachableStates(), this.StateComparer);
            var result = false;
            foreach (var state in unreachable)
            {
                if (this.transitions.Remove(state)) result = true;
            }
            return result;
        }

        /// <inheritdoc/>
        public IEnumerable<TState> ReachableStates()
        {
            IEnumerable<TState> GetNeighbors(TState from) =>
                (this.transitions.TransitionMap.TryGetValue(from, out var onMap)
                    ? onMap.Values.SelectMany(x => x)
                    : Enumerable.Empty<TState>()).Concat(
                    this.transitions.EpsilonTransitionMap.TryGetValue(from, out var toSet)
                        ? toSet
                        : Enumerable.Empty<TState>());

            var touched = new HashSet<TState>(this.StateComparer);
            foreach (var initial in this.InitialStates)
            {
                if (!touched.Add(initial)) continue;
                foreach (var s in BreadthFirst.Search(initial, GetNeighbors, this.StateComparer))
                {
                    touched.Add(s);
                    yield return s;
                }
            }
        }

        /// <inheritdoc/>
        public bool EliminateEpsilonTransitions()
        {
            if (this.EpsilonTransitions.Count == 0) return false;

            foreach (var (v1, v2) in this.EpsilonTransitions)
            {
                // For each v1 --[Epsilon]--> v2, we look at the outgoing transitions from v2
                // and add them to v1
                // So for each v2 --[x]--> vx we add a v1 --[x]--> vx
                if (this.transitions.TransitionMap.TryGetValue(v2, out var fromV2Map))
                {
                    var fromV1Map = this.transitions.GetTransitionsFrom(v1);
                    foreach (var (on, toV2Set) in fromV2Map)
                    {
                        if (!fromV1Map.TryGetValue(on, out var toV1Set))
                        {
                            toV1Set = new(this.StateComparer);
                            fromV1Map.Add(on, toV1Set);
                        }
                        foreach (var s in toV2Set) toV1Set.Add(s);
                    }
                }
                // If v1 is a starting state, we need to make v2 one aswell
                if (this.InitialStates.Contains(v1)) this.InitialStates.Add(v2);
                // If v2 is a final state, v1 needs to be aswell
                if (this.AcceptingStates.Contains(v2)) this.AcceptingStates.Add(v1);
            }

            this.EpsilonTransitions.Clear();

            return true;
        }

        /// <inheritdoc/>
        IDfa<TResultState, TSymbol> IReadOnlyNfa<TState, TSymbol>.Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner) =>
            this.Determinize(combiner);

        /// <inheritdoc/>
        public ISparseDfa<TResultState, TSymbol> Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner)
        {
            var result = new Dfa<TResultState, TSymbol>(combiner.ResultComparer, this.SymbolComparer);
            var visited = new HashSet<StateSet<TState>>();
            var stk = new Stack<StateSet<TState>>();
            var first = new StateSet<TState>(this.InitialStates.SelectMany(this.EpsilonClosure).ToHashSet(this.StateComparer));
            result.InitialState = combiner.Combine(first);
            stk.Push(first);
            while (stk.TryPop(out var top))
            {
                // Construct a transition map
                // Essentially from the current set of states we calculate what set of states we arrive at for a given symbol
                var resultTransitions = new Dictionary<TSymbol, HashSet<TState>>(this.SymbolComparer);
                foreach (var primState in top)
                {
                    if (!this.transitions.TransitionMap.TryGetValue(primState, out var onMap)) continue;
                    foreach (var (on, toSet) in onMap)
                    {
                        if (!resultTransitions.TryGetValue(on, out var existing))
                        {
                            existing = new(this.StateComparer);
                            resultTransitions.Add(on, existing);
                        }
                        foreach (var to in toSet.SelectMany(this.EpsilonClosure)) existing.Add(to);
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
    }
}
