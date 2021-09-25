// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Yoakke.Automata.Internal;
using Yoakke.Collections;
using Yoakke.Collections.Graphs;

namespace Yoakke.Automata.Sparse
{
    /// <summary>
    /// A generic sparse DFA implementation.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public sealed class Dfa<TState, TSymbol> : ISparseDfa<TState, TSymbol>
    {
        // Utility to solve the cross-references between the different collections we expose
        private class TransitionCollection
            : IReadOnlyCollection<TState>, ICollection<TState>,
              IReadOnlyCollection<TSymbol>, ICollection<TSymbol>,
              IReadOnlyCollection<Transition<TState, TSymbol>>, ICollection<Transition<TState, TSymbol>>
        {
            /* Comparers */

            public IEqualityComparer<TState> StateComparer { get; }

            public IEqualityComparer<TSymbol> SymbolComparer { get; }

            /* All containers */

            public TState InitialState
            {
                get => this.initialState;
                set
                {
                    this.initialState = value;
                    this.allStates.Add(value);
                }
            }

            private readonly Dictionary<TState, Dictionary<TSymbol, TState>> transitionMap;
            private readonly HashSet<TState> allStates;
            private readonly HashSet<TState> acceptingStates;
            private readonly HashSet<TSymbol> alphabet;

            private TState initialState = default!;

            /* Count crud */

            int IReadOnlyCollection<TState>.Count => this.allStates.Count;

            int ICollection<TState>.Count => (this as IReadOnlyCollection<TState>).Count;

            int IReadOnlyCollection<TSymbol>.Count => this.alphabet.Count;

            int ICollection<TSymbol>.Count => (this as IReadOnlyCollection<TSymbol>).Count;

            int IReadOnlyCollection<Transition<TState, TSymbol>>.Count => this.transitionMap.Values.Sum(v => v.Count);

            int ICollection<Transition<TState, TSymbol>>.Count => (this as IReadOnlyCollection<Transition<TState, TSymbol>>).Count;

            public bool IsReadOnly => false;

            public TransitionCollection(
                IEqualityComparer<TState> stateComparer,
                IEqualityComparer<TSymbol> symbolComparer)
            {
                this.StateComparer = stateComparer;
                this.SymbolComparer = symbolComparer;
                this.transitionMap = new(stateComparer);
                this.allStates = new(stateComparer);
                this.acceptingStates = new(stateComparer);
                this.alphabet = new(symbolComparer);
            }

            void ICollection<TState>.Clear()
            {
                this.transitionMap.Clear();
                this.allStates.Clear();
                this.acceptingStates.Clear();
                this.initialState = default!;
            }

            void ICollection<TSymbol>.Clear()
            {
                this.transitionMap.Clear();
                this.alphabet.Clear();
            }

            void ICollection<Transition<TState, TSymbol>>.Clear() => this.transitionMap.Clear();

            public void Add(TState item) => this.allStates.Add(item);

            public void Add(TSymbol item) => this.alphabet.Add(item);

            public void Add(Transition<TState, TSymbol> item)
            {
                var onMap = this.GetTransitionsFrom(item.Source);
                onMap[item.Symbol] = item.Destination;
                this.allStates.Add(item.Source);
                this.allStates.Add(item.Destination);
                this.alphabet.Add(item.Symbol);
            }

            public bool Remove(TState item)
            {
                if (!this.allStates.Remove(item)) return false;
                this.acceptingStates.Remove(item);
                if (this.StateComparer.Equals(this.initialState, item)) this.initialState = default!;
                // Remove both ways from transitions
                this.transitionMap.Remove(item);
                foreach (var map in this.transitionMap.Values)
                {
                    var symbolToRemove = map
                        .Where(kv => this.StateComparer.Equals(kv.Value, item))
                        .Select(kv => kv.Key)
                        .GetEnumerator();
                    if (symbolToRemove.MoveNext()) map.Remove(symbolToRemove.Current);
                }
                return true;
            }

            public bool Remove(TSymbol item)
            {
                var removed = false;
                foreach (var (_, onMap) in this.transitionMap)
                {
                    if (onMap.Remove(item)) removed = true;
                }
                return removed;
            }

            public bool Remove(Transition<TState, TSymbol> item)
            {
                if (!this.transitionMap.TryGetValue(item.Source, out var onMap)) return false;
                if (!onMap.TryGetValue(item.Symbol, out var gotTo)) return false;
                if (!this.StateComparer.Equals(item.Destination, gotTo)) return false;
                return onMap.Remove(item.Symbol);
            }

            public bool Contains(TState item) => this.allStates.Contains(item);

            public bool Contains(TSymbol item) => this.alphabet.Contains(item);

            public bool Contains(Transition<TState, TSymbol> item)
            {
                if (!this.transitionMap.TryGetValue(item.Source, out var onMap)) return false;
                if (!onMap.TryGetValue(item.Symbol, out var gotTo)) return false;
                return this.StateComparer.Equals(item.Destination, gotTo);
            }

            public void CopyTo(TState[] array, int arrayIndex) => this.allStates.CopyTo(array, arrayIndex);

            public void CopyTo(TSymbol[] array, int arrayIndex) => this.alphabet.CopyTo(array, arrayIndex);

            public void CopyTo(Transition<TState, TSymbol>[] array, int arrayIndex)
            {
                foreach (var t in this as IEnumerable<Transition<TState, TSymbol>>) array[arrayIndex++] = t;
            }

            IEnumerator<TState> IEnumerable<TState>.GetEnumerator() => this.allStates.GetEnumerator();

            IEnumerator<TSymbol> IEnumerable<TSymbol>.GetEnumerator() => this.alphabet.GetEnumerator();

            IEnumerator<Transition<TState, TSymbol>> IEnumerable<Transition<TState, TSymbol>>.GetEnumerator()
            {
                foreach (var (from, onMap) in this.transitionMap)
                {
                    foreach (var (on, to) in onMap) yield return new(from, on, to);
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => throw new NotSupportedException();

            public Dictionary<TSymbol, TState> GetTransitionsFrom(TState from)
            {
                if (!this.transitionMap.TryGetValue(from, out var onMap))
                {
                    onMap = new(this.SymbolComparer);
                    this.transitionMap.Add(from, onMap);
                }
                return onMap;
            }
        }

        // Helper to expose the accepting states
        private class AcceptingCollection : IReadOnlyCollection<TState>, ICollection<TState>
        {
            public int Count => this.transitions.acceptingStates.Count;

            public bool IsReadOnly => false;

            private readonly TransitionCollection transitions;

            public AcceptingCollection(TransitionCollection transitions)
            {
                this.transitions = transitions;
            }

            public void Add(TState item)
            {
                this.transitions.acceptingStates.Add(item);
                this.transitions.Add(item);
            }

            public void Clear() => this.transitions.acceptingStates.Clear();

            public bool Contains(TState item) => this.transitions.acceptingStates.Contains(item);

            public void CopyTo(TState[] array, int arrayIndex) => this.transitions.acceptingStates.CopyTo(array, arrayIndex);

            public bool Remove(TState item) => this.transitions.acceptingStates.Remove(item);

            public IEnumerator<TState> GetEnumerator() => this.transitions.acceptingStates.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => (this.transitions.acceptingStates as IEnumerable).GetEnumerator();
        }

        /// <inheritdoc/>
        public TState InitialState
        {
            get => this.transitions.InitialState;
            set => this.transitions.InitialState = value;
        }

        /// <inheritdoc/>
        TState IReadOnlyDfa<TState, TSymbol>.InitialState => this.InitialState;

        /// <inheritdoc/>
        public ICollection<TState> AcceptingStates => this.accepting;

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.AcceptingStates => this.accepting;

        /// <inheritdoc/>
        public ICollection<Transition<TState, TSymbol>> Transitions => this.transitions;

        /// <inheritdoc/>
        IReadOnlyCollection<Transition<TState, TSymbol>> IReadOnlySparseFiniteAutomaton<TState, TSymbol>.Transitions => this.transitions;

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
        private readonly AcceptingCollection accepting;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dfa{TState, TSymbol}"/> class.
        /// </summary>
        public Dfa()
            : this(EqualityComparer<TState>.Default, EqualityComparer<TSymbol>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dfa{TState, TSymbol}"/> class.
        /// </summary>
        /// <param name="stateComparer">The state comparer to use.</param>
        /// <param name="symbolComparer">The symbol comparer to use.</param>
        public Dfa(IEqualityComparer<TState> stateComparer, IEqualityComparer<TSymbol> symbolComparer)
        {
            this.transitions = new(stateComparer, symbolComparer);
            this.accepting = new(this.transitions);
        }

        /// <inheritdoc/>
        public string ToDot()
        {
            var writer = new DotWriter<TState, TSymbol>(this.StateComparer);
            writer.WriteStart("DFA");

            // Accepting states
            writer.WriteAcceptingStates(this.AcceptingStates);
            // Non-accepting states
            writer.WriteStates(this.States.Except(this.AcceptingStates, this.StateComparer));
            // Initial states
            writer.WriteInitialStates(new[] { this.InitialState });

            // Transitions
            var tupleComparer = new TupleEqualityComparer<TState, TState>(this.StateComparer, this.StateComparer);
            var transitions = this.Transitions.GroupBy(t => (t.Source, t.Destination), tupleComparer);
            foreach (var group in transitions)
            {
                var from = group.Key.Item1;
                var to = group.Key.Item2;
                var on = string.Join(", ", group.Select(g => g.Symbol));
                writer.WriteTransition(from, on, to);
            }

            writer.WriteEnd();
            return writer.Code.ToString();
        }

        /// <inheritdoc/>
        public bool Accepts(IEnumerable<TSymbol> input)
        {
            var currentState = this.InitialState;
            foreach (var symbol in input)
            {
                if (!this.TryGetTransition(currentState, symbol, out var destinationState)) return false;
                currentState = destinationState;
            }
            return this.AcceptingStates.Contains(currentState);
        }

        /// <inheritdoc/>
        public bool TryGetTransition(TState from, TSymbol on, [MaybeNullWhen(false)] out TState to)
        {
            if (!this.transitions.transitionMap.TryGetValue(from, out var fromMap))
            {
                to = default;
                return false;
            }
            return fromMap.TryGetValue(on, out to);
        }

        /// <inheritdoc/>
        public void AddTransition(TState from, TSymbol on, TState to) => this.Transitions.Add(new(from, on, to));

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, TSymbol on, TState to) => this.Transitions.Remove(new(from, on, to));

        /// <inheritdoc/>
        public IEnumerable<TState> ReachableStates() => BreadthFirst.Search(
            this.InitialState,
            state => this.transitions.transitionMap.TryGetValue(state, out var transitions)
                ? transitions.Values
                : Enumerable.Empty<TState>(),
            this.StateComparer);

        /// <inheritdoc/>
        public bool RemoveUnreachable()
        {
            var unreachable = this.States.Except(this.ReachableStates(), this.StateComparer).ToList();
            var result = false;
            foreach (var state in unreachable)
            {
                if (this.States.Remove(state)) result = true;
            }
            return result;
        }

        /// <inheritdoc/>
        public bool IsComplete(IEnumerable<TSymbol> alphabet) =>
               !alphabet.Any()
            || this.States.All(state => alphabet.All(symbol => this.TryGetTransition(state, symbol, out _)));

        /// <inheritdoc/>
        public bool Complete(IEnumerable<TSymbol> alphabet, TState trap)
        {
            if (!alphabet.Any()) return true;

            var result = false;
            foreach (var state in this.States)
            {
                var onMap = this.transitions.GetTransitionsFrom(state);
                foreach (var symbol in alphabet)
                {
                    if (onMap.ContainsKey(symbol)) continue;
                    // NOTE: We get away with modification as this does not add a state directly
                    onMap.Add(symbol, trap);
                    result = true;
                }
            }

            // If we added any transitions to trap, we also need to wire trap into itself
            if (result)
            {
                this.States.Add(trap);
                var trapMap = this.transitions.GetTransitionsFrom(trap);
                foreach (var symbol in alphabet) trapMap.Add(symbol, trap);
            }
            return result;
        }

        /// <inheritdoc/>
        IDfa<TResultState, TSymbol> IReadOnlyDfa<TState, TSymbol>.Minimize<TResultState>(
            IStateCombiner<TState, TResultState> combiner,
            IEnumerable<(TState, TState)> differentiatePairs) => this.Minimize(combiner, differentiatePairs);

        /// <inheritdoc/>
        public ISparseDfa<TResultState, TSymbol> Minimize<TResultState>(
            IStateCombiner<TState, TResultState> combiner,
            IEnumerable<(TState, TState)> differentiatePairs)
        {
            var equivalenceTable = new EquivalenceTable<TState, TSymbol>(this);

            // Initially fill the table
            equivalenceTable.Initialize(differentiatePairs);

            // Fill until no change
            equivalenceTable.Fill((s1, s2) =>
            {
                var onSet = (this.transitions.transitionMap.TryGetValue(s1, out var s1on) ? s1on.Keys : Enumerable.Empty<TSymbol>())
                    .Concat(this.transitions.transitionMap.TryGetValue(s2, out var s2on) ? s2on.Keys : Enumerable.Empty<TSymbol>())
                    .ToHashSet(this.SymbolComparer);
                foreach (var on in onSet)
                {
                    var on1has = s1on.TryGetValue(on, out var to1);
                    var on2has = s2on.TryGetValue(on, out var to2);
                    if ((on1has && !on2has && this.AcceptingStates.Contains(to1))
                    || (!on1has && on2has && this.AcceptingStates.Contains(to2))) return true;
                    if (!on1has || !on2has) continue;
                    if (equivalenceTable.AreDifferent(to1, to2)) return true;
                }
                return false;
            });

            // Create a state mapping of old-state -> equivalent-state
            var stateMap = equivalenceTable.BuildStateMap(combiner);

            // Create the result
            var result = new Dfa<TResultState, TSymbol>(combiner.ResultComparer, this.SymbolComparer);

            // Now build the new transitions with the state equivalences
            foreach (var (from, on, to) in this.Transitions) result.AddTransition(stateMap[from], on, stateMap[to]);

            // Introduce the initial state and all the accepting states
            result.InitialState = stateMap[this.InitialState];
            foreach (var s in this.transitions.acceptingStates) result.AcceptingStates.Add(stateMap[s]);

            return result;
        }
    }
}
