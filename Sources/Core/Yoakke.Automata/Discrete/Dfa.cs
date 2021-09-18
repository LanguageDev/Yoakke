// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            .Distinct();

        /// <summary>
        /// The comparer used for states.
        /// </summary>
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
            var result = new StringBuilder();
            result
                .AppendLine("digraph dfa {")
                // Left-to-right layout
                .AppendLine("    rankdir=LR;");
            // Double-circle accepting states
            if (this.acceptingStates.Count > 0)
            {
                var acceptingStates = string.Join(" ", this.acceptingStates.Select(s => $"\"{s}\""));
                result.AppendLine($"    node [shape = doublecircle]; {acceptingStates};");
            }
            // Rest are simple circle
            result.AppendLine($"    node [shape = circle];");
            // Go through each transition
            foreach (var (from, onMap) in this.transitions)
            {
                // For a bit more dense repr, we group by to states so we can draw a single arrow between 2 states
                var toOnGroup = onMap.GroupBy(kv => kv.Value, this.StateComparer);
                foreach (var group in toOnGroup)
                {
                    var to = group.Key;
                    var ons = string.Join(", ", group.Select(kv => kv.Key));
                    result.AppendLine($"    \"{from}\" -> \"{to}\" [label = \"{ons}\"];");
                }
            }
            result.Append("}");
            return result.ToString();
        }

        /// <inheritdoc/>
        public bool Accepts(IEnumerable<TSymbol> input)
        {
            var currentState = this.InitialState;
            foreach (var symbol in input)
            {
                if (!this.transitions.TryGetValue(currentState, out var transitionsFromCurrent)) return false;
                if (!transitionsFromCurrent.TryGetValue(symbol, out var destinationState)) return false;
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
        public void RemoveUnreachable() => throw new NotImplementedException();

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
        public IDfa<StateSet<TState>, TSymbol> Minimize(IEnumerable<TState> differentiate) =>
            this.Minimize(Enumerable.Empty<TState>());

        /// <inheritdoc/>
        public IDfa<StateSet<TState>, TSymbol> Minimize() => throw new NotImplementedException();

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
