using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections.Intervals;
using Yoakke.Collections.Compatibility;

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
            transitions.Keys.Concat(transitions.Values.SelectMany(t => t.Values)).Append(InitalState).Distinct();

        private readonly Dictionary<State, IntervalMap<TSymbol, State>> transitions;
        private readonly IComparer<TSymbol> comparer;
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
            if (to != null) yield return to;
        }

        public State GetTransition(State from, TSymbol on)
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

        public string ToDebugDOT()
        {
            var sb = new StringBuilder();
            sb.AppendLine("digraph DenseDfa {");
            // Initial
            sb.AppendLine("    blank_node [label=\"\", shape=none, width=0, height=0];");
            sb.AppendLine($"    blank_node -> {InitalState};");
            // Accepting
            foreach (var state in AcceptingStates)
            {
                sb.AppendLine($"    {state} [peripheries=2];");
            }
            // Transitions
            foreach (var (from, onMap) in transitions)
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
        public State NewState() => new(stateCounter++);

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
        /// Checks, if there are transitions from a given state.
        /// </summary>
        /// <param name="from">The state to get transitions from.</param>
        /// <param name="value">The transitions, if they are present at a state, null otherwise.</param>
        /// <returns>True, if there are transitions from the given state, false otherwise.</returns>
        public bool TryGetTransitionsFrom(State from, out IIntervalMap<TSymbol, State> value)
        {
            var result = transitions.TryGetValue(from, out var value1);
            value = value1;
            return result;
        }
    }
}
