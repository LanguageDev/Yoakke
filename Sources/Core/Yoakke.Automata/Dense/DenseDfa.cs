// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Yoakke.Collections.Dense;
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
        // Utility to solve the cross-references between the different collections we expose
        private class TransitionCollection
            : IReadOnlyCollection<TState>, ICollection<TState>,
              IReadOnlyCollection<Transition<TState, Interval<TSymbol>>>, ICollection<Transition<TState, Interval<TSymbol>>>
        {
            /* Comparers */

            public IEqualityComparer<TState> StateComparer { get; }

            public IntervalComparer<TSymbol> SymbolIntervalComparer { get; }

            /* All containers */

            public Dictionary<TState, DenseMap<TSymbol, TState>> TransitionMap { get; }

            public HashSet<TState> AllStates { get; }

            public HashSet<TState> AcceptingStates { get; }

            private TState initialState = default!;

            public TState InitialState
            {
                get => this.initialState;
                set
                {
                    this.initialState = value;
                    this.AllStates.Add(value);
                }
            }

            /* Count crud */

            int IReadOnlyCollection<TState>.Count => this.AllStates.Count;

            int ICollection<TState>.Count => (this as IReadOnlyCollection<TState>).Count;

            int IReadOnlyCollection<Transition<TState, Interval<TSymbol>>>.Count => this.TransitionMap.Values.Sum(v => v.Count);

            int ICollection<Transition<TState, Interval<TSymbol>>>.Count => (this as IReadOnlyCollection<Transition<TState, Interval<TSymbol>>>).Count;

            public bool IsReadOnly => false;

            public TransitionCollection(
                IEqualityComparer<TState> stateComparer,
                IntervalComparer<TSymbol> symbolComparer)
            {
                this.StateComparer = stateComparer;
                this.SymbolIntervalComparer = symbolComparer;
                this.TransitionMap = new(stateComparer);
                this.AllStates = new(stateComparer);
                this.AcceptingStates = new(stateComparer);
            }

            void ICollection<TState>.Clear()
            {
                this.TransitionMap.Clear();
                this.AllStates.Clear();
                this.AcceptingStates.Clear();
                this.initialState = default!;
            }

            void ICollection<Transition<TState, Interval<TSymbol>>>.Clear() => this.TransitionMap.Clear();

            public void Add(TState item) => this.AllStates.Add(item);

            public void Add(Transition<TState, Interval<TSymbol>> item)
            {
                var onMap = this.GetTransitionsFrom(item.Source);
                onMap.Add(item.Symbol, item.Destination);
                this.AllStates.Add(item.Source);
                this.AllStates.Add(item.Destination);
            }

            public bool Remove(TState item)
            {
                if (!this.AllStates.Remove(item)) return false;
                this.AcceptingStates.Remove(item);
                if (this.StateComparer.Equals(this.initialState, item)) this.initialState = default!;
                // Remove both ways from transitions
                this.TransitionMap.Remove(item);
                foreach (var map in this.TransitionMap.Values)
                {
                    var symbolToRemove = map
                        .Where(kv => this.StateComparer.Equals(kv.Value, item))
                        .Select(kv => kv.Key)
                        .GetEnumerator();
                    if (symbolToRemove.MoveNext()) map.Remove(symbolToRemove.Current);
                }
                return true;
            }

            public bool Remove(Transition<TState, Interval<TSymbol>> item)
            {
                if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
                var anyRemoved = false;
                var intervals = onMap.GetIntervalsAndValues(item.Symbol).ToList();
                foreach (var (iv, value) in intervals)
                {
                    if (!this.StateComparer.Equals(value, item.Destination)) continue;
                    var commonIv = this.SymbolIntervalComparer.Intersection(iv, item.Symbol);
                    onMap.Remove(commonIv);
                    anyRemoved = true;
                }
                return anyRemoved;
            }

            public bool Contains(TState item) => this.AllStates.Contains(item);

            public bool Contains(Transition<TState, Interval<TSymbol>> item)
            {
                if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
                if (!onMap.ContainsKeys(item.Symbol)) return false;
                return onMap.GetValues(item.Symbol).All(v => this.StateComparer.Equals(v, item.Destination));
            }

            public void CopyTo(TState[] array, int arrayIndex) => this.AllStates.CopyTo(array, arrayIndex);

            public void CopyTo(Transition<TState, Interval<TSymbol>>[] array, int arrayIndex)
            {
                foreach (var t in this as IEnumerable<Transition<TState, Interval<TSymbol>>>) array[arrayIndex++] = t;
            }

            IEnumerator<Transition<TState, Interval<TSymbol>>> IEnumerable<Transition<TState, Interval<TSymbol>>>.GetEnumerator()
            {
                foreach (var (from, onMap) in this.TransitionMap)
                {
                    foreach (var (on, to) in onMap) yield return new(from, on, to);
                }
            }

            IEnumerator<TState> IEnumerable<TState>.GetEnumerator() => this.AllStates.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

            public DenseMap<TSymbol, TState> GetTransitionsFrom(TState from)
            {
                if (!this.TransitionMap.TryGetValue(from, out var onMap))
                {
                    onMap = new(this.SymbolIntervalComparer);
                    this.TransitionMap.Add(from, onMap);
                }
                return onMap;
            }
        }

        // Helper to expose the accepting states
        private class AcceptingCollection : IReadOnlyCollection<TState>, ICollection<TState>
        {
            public int Count => this.transitions.AcceptingStates.Count;

            public bool IsReadOnly => false;

            private readonly TransitionCollection transitions;

            public AcceptingCollection(TransitionCollection transitions)
            {
                this.transitions = transitions;
            }

            public void Add(TState item)
            {
                this.transitions.AcceptingStates.Add(item);
                this.transitions.Add(item);
            }

            public void Clear() => this.transitions.AcceptingStates.Clear();

            public bool Contains(TState item) => this.transitions.AcceptingStates.Contains(item);

            public void CopyTo(TState[] array, int arrayIndex) => this.transitions.AcceptingStates.CopyTo(array, arrayIndex);

            public bool Remove(TState item) => this.transitions.AcceptingStates.Remove(item);

            public IEnumerator<TState> GetEnumerator() => this.transitions.AcceptingStates.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => (this.transitions.AcceptingStates as IEnumerable).GetEnumerator();
        }

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
