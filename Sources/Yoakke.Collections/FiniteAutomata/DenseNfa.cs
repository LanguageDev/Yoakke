using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Collections.Compatibility;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.FiniteAutomata
{
    /// <summary>
    /// A nondeterministic finite automaton with a dense representation, meaning it stores transitions with intervals.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type the automata steps on.</typeparam>
    public class DenseNfa<TSymbol> : INondeterministicFiniteAutomata<TSymbol>
    {
        private class StateSetEqualityComparer : IEqualityComparer<SortedSet<State>>
        {
            public static readonly StateSetEqualityComparer Default = new();

            public bool Equals(SortedSet<State> x, SortedSet<State> y) => x.SequenceEqual(y);

            public int GetHashCode(SortedSet<State> obj)
            {
                var hash = new HashCode();
                foreach (var s in obj) hash.Add(s);
                return hash.ToHashCode();
            }
        }

        public State InitalState { get; set; } = State.Invalid;

        IEnumerable<State> IFiniteAutomata<TSymbol>.AcceptingStates => this.AcceptingStates;

        public ISet<State> AcceptingStates { get; } = new HashSet<State>();

        public IEnumerable<State> States => this.transitions.Keys
            .Concat(this.transitions.Values.SelectMany(t => t.Values.SelectMany(v => v)))
            .Concat(this.epsilon.Keys)
            .Concat(this.epsilon.Values.SelectMany(v => v))
            .Append(this.InitalState)
            .Distinct();

        private readonly Dictionary<State, IntervalMap<TSymbol, ISet<State>>> transitions;
        private readonly Dictionary<State, HashSet<State>> epsilon;
        private readonly IComparer<TSymbol> comparer;
        private int stateCounter;

        /// <summary>
        /// Initializes a new <see cref="DenseNfa{TSymbol}"/> with the default comparer.
        /// </summary>
        public DenseNfa()
            : this(Comparer<TSymbol>.Default)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="DenseNfa{TSymbol}"/> with the given comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use.</param>
        public DenseNfa(IComparer<TSymbol> comparer)
        {
            this.transitions = new Dictionary<State, IntervalMap<TSymbol, ISet<State>>>();
            this.epsilon = new Dictionary<State, HashSet<State>>();
            this.comparer = comparer;
        }

        public bool IsAccepting(State state) => this.AcceptingStates.Contains(state);

        public IEnumerable<State> GetTransitions(State from, TSymbol on)
        {
            var fromClosure = this.EpsilonClosure(from);
            return fromClosure.SelectMany(f =>
            {
                if (this.transitions.TryGetValue(f, out var map))
                {
                    if (map.TryGetValue(on, out var to)) return to.SelectMany(this.EpsilonClosure);
                }
                return Enumerable.Empty<State>();
            });
        }

        public IEnumerable<State> EpsilonClosure(State state)
        {
            var touched = new HashSet<State>();
            var stk = new Stack<State>();
            stk.Push(state);
            touched.Add(state);

            while (stk.TryPop(out var top))
            {
                yield return top!;

                if (this.epsilon.TryGetValue(top!, out var states))
                {
                    foreach (var s in states)
                    {
                        if (touched.Add(s)) stk.Push(s);
                    }
                }
            }
        }

        IDeterministicFiniteAutomata<TSymbol> INondeterministicFiniteAutomata<TSymbol>.Determinize() => this.Determinize();

        public DenseDfa<TSymbol> Determinize()
        {
            var dfa = new DenseDfa<TSymbol>();
            var stateMap = new Dictionary<SortedSet<State>, State>(StateSetEqualityComparer.Default);
            var stk = new Stack<(SortedSet<State>, State)>();

            // Deal with the initial state
            {
                var initialStates = this.EpsilonClosure(this.InitalState);
                dfa.InitalState = new State(initialStates);
                var nfaStates = new SortedSet<State>(initialStates);
                stateMap.Add(nfaStates, dfa.InitalState);
                stk.Push((nfaStates, dfa.InitalState));
                // If is an accepting, register it as so
                if (nfaStates.Any(this.IsAccepting)) dfa.AcceptingStates.Add(dfa.InitalState);
            }
            while (stk.TryPop(out var top))
            {
                var (nfaStates, dfaState) = top;

                // Collect where we can go to
                var resultingTransitions = new IntervalMap<TSymbol, SortedSet<State>>();

                foreach (var nfaState in nfaStates)
                {
                    if (this.transitions.TryGetValue(nfaState, out var trs))
                    {
                        foreach (var (iv, destStates) in trs)
                        {
                            var ds = new SortedSet<State>(destStates.SelectMany(this.EpsilonClosure));
                            resultingTransitions.AddAndUpdate(iv, ds, (existing, added) => new SortedSet<State>(existing.Concat(added)));
                        }
                    }
                }

                // Now resultingTransitions has all transitions from the set of NFA states
                foreach (var (on, to) in resultingTransitions)
                {
                    if (!stateMap.TryGetValue(to, out var dfaTo))
                    {
                        // New state, process
                        dfaTo = new State(to);
                        stateMap.Add(to, dfaTo);
                        stk.Push((to, dfaTo));
                        // If it's accepting, register it as so
                        if (to.Any(this.IsAccepting)) dfa.AcceptingStates.Add(dfaTo);
                    }
                    dfa.AddTransition(dfaState, on, dfaTo);
                }
            }
            return dfa;
        }

        public string ToDebugDOT()
        {
            var sb = new StringBuilder();
            sb.AppendLine("digraph DenseNfa {");
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
                foreach (var (iv, tos) in onMap)
                {
                    foreach (var to in tos) sb.AppendLine($"    {from} -> {to} [label=\"{iv}\"];");
                }
            }
            // Epsilon-transitions
            foreach (var (from, tos) in this.epsilon)
            {
                foreach (var to in tos) sb.AppendLine($"    {from} -> {to} [label=\"\u03B5\"];");
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
        /// Adds a transition to this nondeterministic finite automaton.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="on">The symbol to transition on.</param>
        /// <param name="to">The state to transition to.</param>
        public void AddTransition(State from, TSymbol on, State to) =>
            this.AddTransition(from, Interval<TSymbol>.Singleton(on), to);

        /// <summary>
        /// Adds a transition to this nondeterministic finite automaton.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="on">The interval of symbols to transition on.</param>
        /// <param name="to">The state to transition to.</param>
        public void AddTransition(State from, Interval<TSymbol> on, State to)
        {
            if (!this.transitions.TryGetValue(from, out var onMap))
            {
                onMap = new IntervalMap<TSymbol, ISet<State>>(this.comparer);
                this.transitions.Add(from, onMap);
            }
            onMap.AddAndUpdate(on, new HashSet<State> { to }, (existing, added) => existing.Concat(added).ToHashSet());
        }

        /// <summary>
        /// Adds an epsilon transition to this nondeterministic finite automaton.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="on">The epsilon symbol.</param>
        /// <param name="to">The state to transition to.</param>
        public void AddTransition(State from, Epsilon on, State to)
        {
            if (!this.epsilon.TryGetValue(from, out var toSet))
            {
                toSet = new HashSet<State>();
                this.epsilon.Add(from, toSet);
            }
            toSet.Add(to);
        }
    }
}
