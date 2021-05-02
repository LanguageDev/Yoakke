using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            public static readonly StateSetEqualityComparer Default = new StateSetEqualityComparer();

            public bool Equals(SortedSet<State> x, SortedSet<State> y) => x.SequenceEqual(y);

            public int GetHashCode([DisallowNull] SortedSet<State> obj)
            {
                var hash = new HashCode();
                foreach (var s in obj) hash.Add(s);
                return hash.ToHashCode();
            }
        }

        public State InitalState { get; set; } = State.Invalid;
        IEnumerable<State> IFiniteAutomata<TSymbol>.AcceptingStates => AcceptingStates;
        public ISet<State> AcceptingStates { get; } = new HashSet<State>();
        public IEnumerable<State> States => transitions.Keys
            .Concat(transitions.Values.SelectMany(t => t.Values.SelectMany(v => v)))
            .Concat(epsilon.Keys)
            .Concat(epsilon.Values.SelectMany(v => v))
            .Distinct();

        private Dictionary<State, IntervalMap<TSymbol, ISet<State>>> transitions;
        private Dictionary<State, HashSet<State>> epsilon;
        private IComparer<TSymbol> comparer;
        private int stateCounter;

        /// <summary>
        /// Initializes a new <see cref="DenseNfa{TSymbol}"/> with the default comparer.
        /// </summary>
        public DenseNfa()
            : this(Comparer<TSymbol>.Default)
        {
            transitions = new Dictionary<State, IntervalMap<TSymbol, ISet<State>>>();
        }

        /// <summary>
        /// Initializes a new <see cref="DenseNfa{TSymbol}"/> with the given comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use.</param>
        public DenseNfa(IComparer<TSymbol> comparer)
        {
            this.comparer = comparer;
        }

        public bool IsAccepting(State state) => AcceptingStates.Contains(state);

        public IEnumerable<State> GetTransitions(State from, TSymbol on)
        {
            var fromClosure = EpsilonClosure(from);
            return fromClosure.SelectMany(f => 
            { 
                if (transitions.TryGetValue(f, out var map))
                {
                    if (map.TryGetValue(on, out var to)) return to.SelectMany(EpsilonClosure);
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
                yield return top;

                if (epsilon.TryGetValue(top, out var states))
                {
                    foreach (var s in states)
                    {
                        if (touched.Add(s)) stk.Push(s);
                    }
                }
            }
        }

        IDeterministicFiniteAutomata<TSymbol> INondeterministicFiniteAutomata<TSymbol>.Determinize() => Determinize();

        public DenseDfa<TSymbol> Determinize()
        {
            var dfa = new DenseDfa<TSymbol>();
            var stateMap = new Dictionary<SortedSet<State>, State>(StateSetEqualityComparer.Default);
            var stk = new Stack<(SortedSet<State>, State)>();

            // Deal with the initial state
            {
                var initialStates = EpsilonClosure(InitalState);
                dfa.InitalState = dfa.NewState();
                var nfaStates = new SortedSet<State>(initialStates);
                stateMap.Add(nfaStates, dfa.InitalState);
                stk.Push((nfaStates, dfa.InitalState));
                // If is an accepting, register it as so
                if (nfaStates.Any(IsAccepting)) dfa.AcceptingStates.Add(dfa.InitalState);
            }
            while (stk.TryPop(out var top))
            {
                var (nfaStates, dfaState) = top;

                // Collect where we can go to
                var resultingTransitions = new IntervalMap<TSymbol, SortedSet<State>>();

                foreach (var nfaState in nfaStates)
                {
                    if (transitions.TryGetValue(nfaState, out var trs))
                    {
                        foreach (var (iv, destStates) in trs)
                        {
                            var ds = new SortedSet<State>(destStates.SelectMany(EpsilonClosure));
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
                        dfaTo = dfa.NewState();
                        stateMap.Add(to, dfaTo);
                        stk.Push((to, dfaTo));
                        // If it's accepting, register it as so
                        if (to.Any(IsAccepting)) dfa.AcceptingStates.Add(dfaTo);
                    }
                    dfa.AddTransition(dfaState, on, dfaTo);
                }
            }
            return dfa;
        }

        /// <summary>
        /// Creates a new, unique state.
        /// </summary>
        /// <returns>The created state.</returns>
        public State NewState() => new State(stateCounter++);

        /// <summary>
        /// Adds a transition to this nondeterministic finite automaton.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="on">The symbol to transition on.</param>
        /// <param name="to">The state to transition to.</param>
        public void AddTransition(State from, TSymbol on, State to) =>
            AddTransition(from, Interval<TSymbol>.Singleton(on), to);

        /// <summary>
        /// Adds a transition to this nondeterministic finite automaton.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="on">The interval of symbols to transition on.</param>
        /// <param name="to">The state to transition to.</param>
        public void AddTransition(State from, Interval<TSymbol> on, State to)
        {
            if (!transitions.TryGetValue(from, out var onMap))
            {
                onMap = new IntervalMap<TSymbol, ISet<State>>(comparer);
                transitions.Add(from, onMap);
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
            if (!epsilon.TryGetValue(from, out var toSet))
            {
                toSet = new HashSet<State>();
                epsilon.Add(from, toSet);
            }
            toSet.Add(to);
        }
    }
}
