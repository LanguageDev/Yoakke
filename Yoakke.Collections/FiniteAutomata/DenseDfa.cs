using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.FiniteAutomata
{
    /// <summary>
    /// A deterministic finite automaton with a dense representation, meaning it stores transitions with intervals.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type the automata steps on.</typeparam>
    public class DenseDfa<TSymbol> : IDeterministicFiniteAutomata<TSymbol>
    {
        public State InitalState { get; set; } = State.Invalid;
        IEnumerable<State> IFiniteAutomata<TSymbol>.AcceptingStates => AcceptingStates;
        public ISet<State> AcceptingStates { get; } = new HashSet<State>();
        public IEnumerable<State> States =>
            transitions.Keys.Concat(transitions.Values.SelectMany(t => t.Values)).Distinct();

        private Dictionary<State, IntervalMap<TSymbol, State>> transitions;
        private IComparer<TSymbol> comparer;
        private int stateCounter;

        /// <summary>
        /// Initializes a new <see cref="DenseDfa{TSymbol}"/> with the default comparer.
        /// </summary>
        public DenseDfa()
            : this(Comparer<TSymbol>.Default)
        {
            transitions = new Dictionary<State, IntervalMap<TSymbol, State>>();
        }

        /// <summary>
        /// Initializes a new <see cref="DenseDfa{TSymbol}"/> with the given comparer.
        /// </summary>
        /// <param name="comparer">The comparer to use.</param>
        public DenseDfa(IComparer<TSymbol> comparer)
        {
            this.comparer = comparer;
        }

        public bool IsAccepting(State state) => AcceptingStates.Contains(state);

        public IEnumerable<State> GetTransitions(State from, TSymbol on)
        {
            var to = GetTransition(from, on);
            if (to != null) yield return to.Value;
        }

        public State? GetTransition(State from, TSymbol on)
        {
            if (transitions.TryGetValue(from, out var map))
            {
                if (map.TryGetValue(on, out var state)) return state;
            }
            return null;
        }

        public IDeterministicFiniteAutomata<TSymbol> Minify()
        {
            // TODO
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new, unique state.
        /// </summary>
        /// <returns>The created state.</returns>
        public State NewState() => new State(stateCounter++);

        /// <summary>
        /// Adds a transition to this deterministic finite automaton.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="on">The symbol to transition on.</param>
        /// <param name="to">The state to transition to.</param>
        public void AddTransition(State from, TSymbol on, State to) =>
            AddTransition(from, Interval<TSymbol>.Singleton(on), to);

        /// <summary>
        /// Adds a transition to this deterministic finite automaton.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="on">The interval of symbols to transition on.</param>
        /// <param name="to">The state to transition to.</param>
        public void AddTransition(State from, Interval<TSymbol> on, State to)
        {
            if (!transitions.TryGetValue(from, out var onMap))
            {
                onMap = new IntervalMap<TSymbol, State>(comparer);
                transitions.Add(from, onMap);
            }
            // If unification is called, it means that we transition to multiple states for a given symbol,
            // which is illegal for DFAs.
            onMap.AddAndUpdate(on, to, (_, _) => throw new InvalidOperationException());
        }

        /// <summary>
        /// Retrieves the transition map for a given state.
        /// </summary>
        /// <param name="from">The state to get the transitions from.</param>
        /// <returns>The interval map of transitions.</returns>
        public IIntervalMap<TSymbol, State> TransitionsFrom(State from) => transitions[from];
    }
}
