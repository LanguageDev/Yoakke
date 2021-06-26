// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Utilities.Compatibility;
using Yoakke.Utilities.Intervals;

namespace Yoakke.Utilities.FiniteAutomata
{
    /// <summary>
    /// A deterministic finite automaton with a dense representation, meaning it stores transitions with intervals.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type the automata steps on.</typeparam>
    public class DenseDfa<TSymbol> : IDeterministicFiniteAutomata<TSymbol>
    {
        public State InitalState { get; set; } = State.Invalid;

        IEnumerable<State> IFiniteAutomata<TSymbol>.AcceptingStates => this.AcceptingStates;

        public ISet<State> AcceptingStates { get; } = new HashSet<State>();

        public IEnumerable<State> States =>
            this.transitions.Keys.Concat(this.transitions.Values.SelectMany(t => t.Values)).Append(this.InitalState).Distinct();

        private readonly Dictionary<State, IntervalMap<TSymbol, State>> transitions;
        private readonly IComparer<TSymbol> comparer;
        private int stateCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseDfa{TSymbol}"/> class.
        /// </summary>
        public DenseDfa()
            : this(Comparer<TSymbol>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DenseDfa{TSymbol}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer to use.</param>
        public DenseDfa(IComparer<TSymbol> comparer)
        {
            this.comparer = comparer;
            this.transitions = new Dictionary<State, IntervalMap<TSymbol, State>>();
        }

        public bool IsAccepting(State state) => this.AcceptingStates.Contains(state);

        public IEnumerable<State> GetTransitions(State from, TSymbol on)
        {
            var to = this.GetTransition(from, on);
            if (to is not null) yield return to;
        }

        public State? GetTransition(State from, TSymbol on) =>
            this.transitions.TryGetValue(from, out var map) && map.TryGetValue(on, out var state)
                ? state
                : null;

        public IDeterministicFiniteAutomata<TSymbol> Minify() =>
            // TODO
            throw new NotImplementedException();

        public string ToDebugDOT()
        {
            var sb = new StringBuilder();
            sb.AppendLine("digraph DenseDfa {");
            // Initial
            sb.AppendLine("    blank_node [label=\"\", shape=none, width=0, height=0];");
            sb.AppendLine($"    blank_node -> {this.InitalState};");
            // Accepting
            foreach (var state in this.AcceptingStates)
            {
                sb.AppendLine($"    {state} [peripheries=2];");
            }
            // Transitions
            foreach (var (from, onMap) in this.transitions)
            {
                foreach (var (iv, to) in onMap)
                {
                    sb.AppendLine($"    {from} -> {to} [label=\"{iv}\"];");
                }
            }
            sb.AppendLine("}");
            return sb.ToString();
        }

        /// <summary>
        /// Creates a new, unique state.
        /// </summary>
        /// <returns>The created state.</returns>
        public State NewState() => new(this.stateCounter++);

        /// <summary>
        /// Adds a transition to this deterministic finite automaton.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="on">The symbol to transition on.</param>
        /// <param name="to">The state to transition to.</param>
        public void AddTransition(State from, TSymbol on, State to) =>
            this.AddTransition(from, Interval<TSymbol>.Singleton(on), to);

        /// <summary>
        /// Adds a transition to this deterministic finite automaton.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="on">The interval of symbols to transition on.</param>
        /// <param name="to">The state to transition to.</param>
        public void AddTransition(State from, Interval<TSymbol> on, State to)
        {
            if (!this.transitions.TryGetValue(from, out var onMap))
            {
                onMap = new IntervalMap<TSymbol, State>(this.comparer);
                this.transitions.Add(from, onMap);
            }
            // If unification is called, it means that we transition to multiple states for a given symbol,
            // which is illegal for DFAs.
            onMap.AddAndUpdate(on, to, (_, _) => throw new InvalidOperationException());
        }

        /// <summary>
        /// Checks, if there are transitions from a given state.
        /// </summary>
        /// <param name="from">The state to get transitions from.</param>
        /// <param name="value">The transitions, if they are present at a state, null otherwise.</param>
        /// <returns>True, if there are transitions from the given state, false otherwise.</returns>
        public bool TryGetTransitionsFrom(State from, out IIntervalMap<TSymbol, State> value)
        {
            var result = this.transitions.TryGetValue(from, out var value1);
            value = value1;
            return result;
        }
    }
}
