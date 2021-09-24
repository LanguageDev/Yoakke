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
using Yoakke.Collections.Dense;
using Yoakke.Collections.Graphs;
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
        public ICollection<Transition<TState, Interval<TSymbol>>> Transitions => this.transitions;

        /// <inheritdoc/>
        IReadOnlyCollection<Transition<TState, Interval<TSymbol>>> IReadOnlyDenseFiniteAutomaton<TState, TSymbol>.Transitions => this.transitions;

        /// <inheritdoc/>
        public ICollection<TState> States => this.transitions;

        /// <inheritdoc/>
        IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.States => this.transitions;

        /// <inheritdoc/>
        public IEqualityComparer<TState> StateComparer => this.transitions.StateComparer;

        /// <summary>
        /// The comparer used for alphabet symbol intervals.
        /// </summary>
        public IntervalComparer<TSymbol> SymbolIntervalComparer => this.transitions.SymbolIntervalComparer;

        private readonly TransitionCollection transitions;
        private readonly AcceptingCollection accepting;

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseDfa{TState, TSymbol}"/> class.
        /// </summary>
        public DenseDfa()
            : this(EqualityComparer<TState>.Default, IntervalComparer<TSymbol>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseDfa{TState, TSymbol}"/> class.
        /// </summary>
        /// <param name="stateComparer">The state comparer to use.</param>
        /// <param name="symbolIntervalComparer">The symbol interval comparer to use.</param>
        public DenseDfa(IEqualityComparer<TState> stateComparer, IntervalComparer<TSymbol> symbolIntervalComparer)
        {
            this.transitions = new(stateComparer, symbolIntervalComparer);
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
                var on = string.Join(" U ", group.Select(g => g.Symbol));
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
            if (!this.transitions.TransitionMap.TryGetValue(from, out var fromMap))
            {
                to = default;
                return false;
            }
            var values = fromMap.GetValues(on).GetEnumerator();
            if (values.MoveNext())
            {
                to = values.Current;
                return true;
            }
            else
            {
                to = default;
                return false;
            }
        }

        /// <inheritdoc/>
        public void AddTransition(TState from, TSymbol on, TState to) => this.Transitions.Add(new(from, on, to));

        /// <inheritdoc/>
        public void AddTransition(TState from, Interval<TSymbol> on, TState to) => this.Transitions.Add(new(from, on, to));

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, TSymbol on, TState to) => this.Transitions.Remove(new(from, on, to));

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, Interval<TSymbol> on, TState to) => this.Transitions.Remove(new(from, on, to));

        /// <inheritdoc/>
        public IEnumerable<TState> ReachableStates() => BreadthFirst.Search(
            this.InitialState,
            state => this.transitions.TransitionMap.TryGetValue(state, out var transitions)
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
        public bool IsComplete(IEnumerable<Interval<TSymbol>> alphabet) =>
               !alphabet.Any()
            || this.States.All(state => this.transitions.TransitionMap.TryGetValue(state, out var onMap)
                                     && alphabet.All(symbolIv => onMap.ContainsKeys(symbolIv)));

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
            var result = new DenseDfa<TResultState, TSymbol>(combiner.ResultComparer, this.SymbolIntervalComparer);
            var tupleComparer = new TupleEqualityComparer<TState, TState>(this.StateComparer, this.StateComparer);
            var table = new HashSet<(TState, TState)>(tupleComparer);

            void Plot(TState s1, TState s2)
            {
                table!.Add((s1, s2));
                table!.Add((s2, s1));
            }

            // First, all (accepting, non-accepting) pairs get plotted in the table
            var states = this.States.ToList();
            for (var i = 0; i < states.Count; ++i)
            {
                var s1 = states[i];
                for (var j = 0; j < i; ++j)
                {
                    var s2 = states[j];
                    if (this.AcceptingStates.Contains(s1) != this.AcceptingStates.Contains(s2)) Plot(s1, s2);
                }
            }

            // Then we plot the custom pairs too
            foreach (var (s1, s2) in differentiatePairs) Plot(s1, s2);

            // Now for each (p, q) pair of states
            //   If (p, q) is unplotted and exists a symbol X, so that (delta(p, X), delta(q, X)) is not empty
            //     plot (p, q)
            // Repeat until there is no change
            while (true)
            {
                var changed = false;
                for (var i = 0; i < states.Count; ++i)
                {
                    var s1 = states[i];
                    for (var j = 0; j < i; ++j)
                    {
                        var s2 = states[j];

                        if (table.Contains((s1, s2))) continue;

                        // TODO: Look in sparse DFA for correctness alert
                        if (!this.transitions.TransitionMap.TryGetValue(s1, out var s1on)
                         || !this.transitions.TransitionMap.TryGetValue(s2, out var s2on)) continue;

                        var onSet = new DenseSet<TSymbol>(this.SymbolIntervalComparer);
                        foreach (var iv in s1on.Keys) onSet.Add(iv);
                        foreach (var iv in s2on.Keys) onSet.Add(iv);

                        // TODO: Finish
                    }
                }
                if (!changed) break;
            }

            // TODO: Finish

            return result;
        }
    }
}
