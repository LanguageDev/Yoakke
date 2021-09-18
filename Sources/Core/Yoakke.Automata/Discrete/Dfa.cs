// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Yoakke.Automata.Internal;
using Yoakke.Collections;

namespace Yoakke.Automata.Discrete
{
    /// <summary>
    /// A generic DFA implementation.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    public class Dfa<TState, TSymbol> : IDfa<TState, TSymbol>
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
            .Concat(this.transitions.Values.SelectMany(t => t.Select(v => v.Value)))
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
            this.StateComparer = stateComparer;
            this.SymbolComparer = symbolComparer;

            this.acceptingStates = new(stateComparer);
            this.transitions = new(stateComparer);
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
                var toOnGroup = onMap.GroupBy(kv => kv.Value, this.StateComparer);
                foreach (var group in toOnGroup)
                {
                    var to = group.Key;
                    var ons = string.Join(", ", group.Select(kv => kv.Key));
                    helper.WriteTransition(from, ons, to);
                }
            }

            helper.WriteEnd();
            return helper.Code;
        }

        /// <inheritdoc/>
        public bool Accepts(TState initial, IEnumerable<TSymbol> input)
        {
            var currentState = initial;
            foreach (var symbol in input)
            {
                if (!this.TryGetTransition(currentState, symbol, out var destinationState)) return false;
                currentState = destinationState;
            }
            return this.acceptingStates.Contains(currentState);
        }

        /// <inheritdoc/>
        public bool IsComplete(IEnumerable<TSymbol> alphabet) =>
               !alphabet.Any()
            || this.States.All(state =>
               {
                   if (!this.transitions.TryGetValue(state, out var onMap)) return false;
                   return alphabet.All(symbol => onMap.ContainsKey(symbol));
               });

        /// <inheritdoc/>
        public bool TryGetTransition(TState from, TSymbol on, [MaybeNullWhen(false)] out TState to)
        {
            if (!this.transitions.TryGetValue(from, out var fromMap))
            {
                to = default;
                return false;
            }
            return fromMap.TryGetValue(on, out to);
        }

        /// <inheritdoc/>
        public bool AddTransition(TState from, TSymbol on, TState to)
        {
            var onMap = this.GetTransitionsFrom(from);
            if (onMap.ContainsKey(on)) return false;
            onMap.Add(on, to);
            return true;
        }

        /// <inheritdoc/>
        public bool RemoveTransition(TState from, TSymbol on, TState to)
        {
            if (!this.transitions.TryGetValue(from, out var onMap)) return false;
            if (!onMap.TryGetValue(on, out var existingTo)) return false;
            if (!this.StateComparer.Equals(to, existingTo)) return false;
            return onMap.Remove(on);
        }

        /// <inheritdoc/>
        public bool RemoveUnreachable(TState from)
        {
            var touched = this.ReachableStates(from);

            // Prune transitions that are not in this set
            var result = false;
            var untouchedStates = this.transitions.Keys.Except(touched);
            foreach (var untouched in untouchedStates)
            {
                if (this.transitions.Remove(untouched)) result = true;
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
                foreach (var to in onMap.Values)
                {
                    if (touched.Add(to)) stk.Push(to);
                }
            }
        }

        /// <inheritdoc/>
        public bool Complete(IEnumerable<TSymbol> alphabet, TState trap)
        {
            if (!alphabet.Any()) return false;

            var result = false;
            foreach (var state in this.States.ToList())
            {
                var onMap = this.GetTransitionsFrom(state);
                foreach (var symbol in alphabet)
                {
                    if (TryAdd(onMap, symbol, trap)) result = true;
                }
            }
            // If we added any transitions to trap, we also need to wire trap into itself
            if (result)
            {
                var onTrap = this.GetTransitionsFrom(trap);
                foreach (var symbol in alphabet) TryAdd(onTrap, symbol, trap);
            }
            return result;
        }

        /// <inheritdoc/>
        public IDfa<TResultState, TSymbol> Minimize<TResultState>(
            IStateCombiner<TState, TResultState> combiner,
            IEnumerable<(TState, TState)> differentiatePairs)
        {
            var result = new Dfa<TResultState, TSymbol>(combiner.ResultComparer, this.SymbolComparer);
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
                        if (!this.transitions.TryGetValue(s1, out var s1on)
                         || !this.transitions.TryGetValue(s2, out var s2on)) continue;

                        foreach (var (on, to1) in s1on)
                        {
                            if (!s2on.TryGetValue(on, out var to2)) continue;
                            if (table.Contains((to1, to2)))
                            {
                                changed = true;
                                Plot(s1, s2);
                                break;
                            }
                        }
                    }
                }
                if (!changed) break;
            }

            // Now we have a table that is empty, where two states are equivalent
            // Create a state mapping of old-state -> equivalent-state
            var stateMap = new Dictionary<TState, TResultState>(this.StateComparer);
            for (var i = 0; i < states.Count; ++i)
            {
                var s1 = states[i];
                var equivalentLists = new List<TState> { s1 };
                for (var j = 0; j < states.Count; ++j)
                {
                    if (i == j) continue;
                    var s2 = states[j];
                    if (!table.Contains((s1, s2))) equivalentLists.Add(s2);
                }
                stateMap.Add(s1, combiner.Combine(equivalentLists));
            }

            // Now build the new transitions with the state equivalences
            foreach (var (from, onMap) in this.transitions)
            {
                foreach (var (on, to) in onMap) result.AddTransition(stateMap[from], on, stateMap[to]);
            }

            // Introduce the initial state and all the accepting states
            result.InitialState = stateMap[this.InitialState];
            foreach (var s in this.acceptingStates) result.AcceptingStates.Add(stateMap[s]);

            return result;
        }

        /// <inheritdoc/>
        IReadOnlyDfa<TResultState, TSymbol> IReadOnlyDfa<TState, TSymbol>.Minimize<TResultState>(
            IStateCombiner<TState, TResultState> combiner,
            IEnumerable<(TState, TState)> differentiatePairs) => this.Minimize(combiner, differentiatePairs);

        private Dictionary<TSymbol, TState> GetTransitionsFrom(TState state)
        {
            if (!this.transitions.TryGetValue(state, out var onState))
            {
                onState = new(this.SymbolComparer);
                this.transitions.Add(state, onState);
            }
            return onState;
        }

        private static bool TryAdd<TKey, TValue>(Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            if (dict.ContainsKey(key)) return false;
            dict.Add(key, value);
            return true;
        }
    }
}
