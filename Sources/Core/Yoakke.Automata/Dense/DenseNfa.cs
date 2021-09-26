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
using Yoakke.Collections.Dense;
using Yoakke.Collections.Intervals;

namespace Yoakke.Automata.Dense
{
    /// <summary>
    /// A generic dense NFA implementation.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public class DenseNfa<TState, TSymbol> : IDenseNfa<TState, TSymbol>
    {
        private class TransitionCollection
            : IReadOnlyCollection<Transition<TState, Interval<TSymbol>>>, ICollection<Transition<TState, Interval<TSymbol>>>
        {
            public IEqualityComparer<TState> StateComparer { get; }

            public IntervalComparer<TSymbol> SymbolIntervalComparer { get; }

            public Dictionary<TState, DenseMap<TSymbol, HashSet<TState>>> TransitionMap { get; }

            public bool IsReadOnly => false;

            public int Count => this.TransitionMap.Values.Sum(v => v.Values.Sum(s => s.Count));

            public event EventHandler<Transition<TState, Interval<TSymbol>>>? Added;

            public TransitionCollection(IEqualityComparer<TState> stateComparer, IntervalComparer<TSymbol> symbolComparer)
            {
                this.TransitionMap = new(stateComparer);
                this.StateComparer = stateComparer;
                this.SymbolIntervalComparer = symbolComparer;
            }

            public void Clear() => this.TransitionMap.Clear();

            public void Add(Transition<TState, Interval<TSymbol>> item)
            {
                var onMap = this.GetTransitionsFrom(item.Source);
                onMap.Add(item.Symbol, new HashSet<TState>(this.StateComparer) { item.Destination });
                this.Added?.Invoke(this, item);
            }

            public bool Remove(Transition<TState, Interval<TSymbol>> item)
            {
                if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
                var anyRemoved = false;
                var intervals = onMap.GetIntervalsAndValues(item.Symbol).ToList();
                foreach (var (iv, toSet) in intervals)
                {
                    if (!toSet.Contains(item.Destination)) continue;
                    var commonIv = this.SymbolIntervalComparer.Intersection(iv, item.Symbol);
                    onMap.Remove(commonIv);
                    if (toSet.Count > 1)
                    {
                        var newToSet = toSet.ToHashSet(this.StateComparer);
                        newToSet.Remove(item.Destination);
                        onMap.Add(commonIv, newToSet);
                    }
                    anyRemoved = true;
                }
                return anyRemoved;
            }

            public bool Contains(Transition<TState, Interval<TSymbol>> item)
            {
                if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
                if (!onMap.ContainsKeys(item.Symbol)) return false;
                return onMap.GetValues(item.Symbol).All(toSet => toSet.Contains(item.Destination));
            }

            public void CopyTo(Transition<TState, Interval<TSymbol>>[] array, int arrayIndex)
            {
                foreach (var t in this) array[arrayIndex++] = t;
            }

            public IEnumerator<Transition<TState, Interval<TSymbol>>> GetEnumerator()
            {
                foreach (var (from, onMap) in this.TransitionMap)
                {
                    foreach (var (on, toSet) in onMap)
                    {
                        foreach (var to in toSet) yield return new(from, on, to);
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            public DenseMap<TSymbol, HashSet<TState>> GetTransitionsFrom(TState from)
            {
                if (!this.TransitionMap.TryGetValue(from, out var onMap))
                {
                    onMap = new(this.SymbolIntervalComparer);
                    this.TransitionMap.Add(from, onMap);
                }
                return onMap;
            }
        }

        private class EpsilonTransitionCollection
            : IReadOnlyCollection<EpsilonTransition<TState>>, ICollection<EpsilonTransition<TState>>
        {
            public IEqualityComparer<TState> StateComparer { get; }

            public Dictionary<TState, HashSet<TState>> EpsilonTransitionMap { get; }

            public bool IsReadOnly => false;

            public int Count => this.EpsilonTransitionMap.Values.Sum(v => v.Count);

            public event EventHandler<EpsilonTransition<TState>>? Added;

            public EpsilonTransitionCollection(IEqualityComparer<TState> stateComparer)
            {
                this.EpsilonTransitionMap = new(stateComparer);
                this.StateComparer = stateComparer;
            }

            public void Clear() => this.EpsilonTransitionMap.Clear();

            public void Add(EpsilonTransition<TState> item)
            {
                if (!this.EpsilonTransitionMap.TryGetValue(item.Source, out var toSet))
                {
                    toSet = new(this.StateComparer);
                    this.EpsilonTransitionMap.Add(item.Source, toSet);
                }
                toSet.Add(item.Destination);
                this.Added?.Invoke(this, item);
            }

            public bool Remove(EpsilonTransition<TState> item)
            {
                if (!this.EpsilonTransitionMap.TryGetValue(item.Source, out var toSet)) return false;
                return toSet.Remove(item.Destination);
            }

            public bool Contains(EpsilonTransition<TState> item)
            {
                if (!this.EpsilonTransitionMap.TryGetValue(item.Source, out var toSet)) return false;
                return toSet.Contains(item.Destination);
            }

            public void CopyTo(EpsilonTransition<TState>[] array, int arrayIndex)
            {
                foreach (var t in this) array[arrayIndex++] = t;
            }

            public IEnumerator<EpsilonTransition<TState>> GetEnumerator()
            {
                foreach (var (from, toSet) in this.EpsilonTransitionMap)
                {
                    foreach (var to in toSet) yield return new(from, to);
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        /// <inheritdoc/>
        public ICollection<TState> InitialStates => this.initialStates;

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyNfa<TState, TSymbol>.InitialStates => this.initialStates;

        /// <inheritdoc/>
        public ICollection<TState> AcceptingStates => this.acceptingStates;

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.AcceptingStates => this.acceptingStates;

        /// <inheritdoc/>
        public ICollection<TState> States => this.allStates;

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.States => this.allStates;

        /// <inheritdoc/>
        public ICollection<Transition<TState, Interval<TSymbol>>> Transitions => this.transitions;

        /// <inheritdoc/>
        IReadOnlyCollection<Transition<TState, Interval<TSymbol>>> IReadOnlyDenseFiniteAutomaton<TState, TSymbol>.Transitions => this.transitions;

        /// <inheritdoc/>
        public ICollection<EpsilonTransition<TState>> EpsilonTransitions => this.epsilonTransitions;

        /// <inheritdoc/>
        IReadOnlyCollection<EpsilonTransition<TState>> IReadOnlyNfa<TState, TSymbol>.EpsilonTransitions => this.epsilonTransitions;

        /// <inheritdoc/>
        public ICollection<Interval<TSymbol>> Alphabet => this.alphabet;

        /// <inheritdoc/>
        IReadOnlyCollection<Interval<TSymbol>> IReadOnlyDenseFiniteAutomaton<TState, TSymbol>.Alphabet => this.alphabet;

        /// <inheritdoc/>
        public IEqualityComparer<TState> StateComparer => this.transitions.StateComparer;

        /// <summary>
        /// The comparer used for alphabet symbol intervals.
        /// </summary>
        public IntervalComparer<TSymbol> SymbolIntervalComparer => this.transitions.SymbolIntervalComparer;

        private readonly TransitionCollection transitions;
        private readonly EpsilonTransitionCollection epsilonTransitions;
        private readonly ObservableCollection<TState> allStates;
        private readonly ObservableCollection<TState> acceptingStates;
        private readonly ObservableCollection<TState> initialStates;
        private readonly ObservableCollection<Interval<TSymbol>> alphabet;

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseNfa{TState, TSymbol}"/> class.
        /// </summary>
        public DenseNfa()
            : this(EqualityComparer<TState>.Default, IntervalComparer<TSymbol>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseNfa{TState, TSymbol}"/> class.
        /// </summary>
        /// <param name="stateComparer">The state comparer to use.</param>
        /// <param name="symbolIntervalComparer">The symbol interval comparer to use.</param>
        public DenseNfa(IEqualityComparer<TState> stateComparer, IntervalComparer<TSymbol> symbolIntervalComparer)
        {
            var (all, accepting, initial) = ObservableCollection<TState>.StateWithAcceptingAndInitial(() => new HashSet<TState>(stateComparer));
            this.allStates = all;
            this.initialStates = initial;
            this.acceptingStates = accepting;
            this.alphabet = new(new DenseSet<TSymbol>(symbolIntervalComparer));
            this.transitions = new(stateComparer, symbolIntervalComparer);
            this.epsilonTransitions = new(stateComparer);

            this.allStates.Removed += (sender, item) =>
            {
                // Remove both ways from transitions
                this.transitions.TransitionMap.Remove(item);
                foreach (var map in this.transitions.TransitionMap.Values)
                {
                    foreach (var toSet in map.Values) toSet.Remove(item);
                }
                // Remove both ways from epsilon transitions
                this.epsilonTransitions.EpsilonTransitionMap.Remove(item);
                foreach (var toSet in this.epsilonTransitions.EpsilonTransitionMap.Values) toSet.Remove(item);
            };
            this.allStates.Cleared += (sender, eventArgs) =>
            {
                this.transitions.Clear();
                this.epsilonTransitions.Clear();
            };

            this.transitions.Added += (sender, item) =>
            {
                this.alphabet.Add(item.Symbol);
                this.allStates.Add(item.Source);
                this.allStates.Add(item.Destination);
            };

            this.epsilonTransitions.Added += (sender, item) =>
            {
                this.allStates.Add(item.Source);
                this.allStates.Add(item.Destination);
            };

            this.alphabet.Removed += (sender, item) =>
            {
                foreach (var onMap in this.transitions.TransitionMap.Values) onMap.Remove(item);
            };
            this.alphabet.Cleared += (sender, eventArgs) => this.transitions.Clear();
        }

        /// <inheritdoc/>
        public string ToDot() => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool Accepts(IEnumerable<TSymbol> input) => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<TState> GetTransitions(TState from, TSymbol on) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void AddTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void AddTransition(TState from, Interval<TSymbol> on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, TSymbol on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, Interval<TSymbol> on, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public void AddEpsilonTransition(TState from, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveEpsilonTransition(TState from, TState to) => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<TState> EpsilonClosure(TState state) => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool RemoveUnreachable() => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<TState> ReachableStates() => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool EliminateEpsilonTransitions() => throw new NotImplementedException();

        /// <inheritdoc/>
        IDfa<TResultState, TSymbol> IReadOnlyNfa<TState, TSymbol>.Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner) =>
            this.Determinize(combiner);

        /// <inheritdoc/>
        public IDenseDfa<TResultState, TSymbol> Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner) => throw new NotImplementedException();
    }
}
