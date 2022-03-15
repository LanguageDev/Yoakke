// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Collections;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// A generic DFA implementation.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public sealed class Dfa<TState, TSymbol> : IFiniteStateAutomaton<TState, TSymbol>
{
    private readonly struct GraphNodeAdapter : GraphSearch.INeighborSelector<TState>
    {
        private readonly Dfa<TState, TSymbol> dfa;

        public GraphNodeAdapter(Dfa<TState, TSymbol> dfa)
        {
            this.dfa = dfa;
        }

        public IEnumerable<TState> GetNeighbors(TState node) =>
            this.dfa.transitionsRaw.TransitionMap.TryGetValue(node, out var onMap)
                ? onMap.Values
                : Enumerable.Empty<TState>();
    }

    private readonly struct OverwriteCombiner : IValueCombiner<TState>
    {
        public TState Combine(TState existing, TState added) => added;
    }

    private sealed class TransitionCollection
        : IReadOnlyCollection<Transition<TState, Interval<TSymbol>>>, ICollection<Transition<TState, Interval<TSymbol>>>
    {
        public IEqualityComparer<TState> StateComparer { get; }

        public IntervalComparer<TSymbol> SymbolIntervalComparer { get; }

        public Dictionary<TState, IntervalMap<TSymbol, TState>> TransitionMap { get; }

        public bool IsReadOnly => false;

        public int Count => this.TransitionMap.Values
            .Sum(v => (v as ICollection<KeyValuePair<Interval<TSymbol>, TState>>).Count);

        public TransitionCollection(
            IEqualityComparer<TState> stateComparer,
            IntervalComparer<TSymbol> symbolComparer)
        {
            this.TransitionMap = new(stateComparer);
            this.StateComparer = stateComparer;
            this.SymbolIntervalComparer = symbolComparer;
        }

        public void Clear() => this.TransitionMap.Clear();

        public void Add(Transition<TState, Interval<TSymbol>> item)
        {
            var onMap = this.GetTransitionsFrom(item.Source);
            onMap.Add(item.Symbol, item.Destination, default(OverwriteCombiner));
        }

        public bool Remove(Transition<TState, Interval<TSymbol>> item)
        {
            if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
            var anyRemoved = false;
            var intervals = onMap.Intersecting(item.Symbol).ToList();
            foreach (var fromOn in intervals)
            {
                if (!this.StateComparer.Equals(fromOn.Value, item.Destination)) continue;
                var commonIv = this.SymbolIntervalComparer.Intersection(fromOn.Key, item.Symbol);
                onMap.Remove(commonIv);
                anyRemoved = true;
            }
            return anyRemoved;
        }

        public bool Contains(Transition<TState, Interval<TSymbol>> item)
        {
            if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
            if (!onMap.ContainsKeys(item.Symbol)) return false;
            return onMap
                .Intersecting(item.Symbol)
                .Select(kv => kv.Value)
                .All(v => this.StateComparer.Equals(v, item.Destination));
        }

        public void CopyTo(Transition<TState, Interval<TSymbol>>[] array, int arrayIndex)
        {
            foreach (var t in this) array[arrayIndex++] = t;
        }

        public IEnumerator<Transition<TState, Interval<TSymbol>>> GetEnumerator()
        {
            foreach (var fromOn in this.TransitionMap)
            {
                foreach (var onTo in fromOn.Value) yield return new(fromOn.Key, onTo.Key, onTo.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public IntervalMap<TSymbol, TState> GetTransitionsFrom(TState from)
        {
            if (!this.TransitionMap.TryGetValue(from, out var onMap))
            {
                onMap = new(this.SymbolIntervalComparer);
                this.TransitionMap.Add(from, onMap);
            }
            return onMap;
        }
    }

    /// <inheritdoc/>
    public ICollection<TState> States => this.allStates;

    /// <summary>
    /// The initial states of the automaton.
    /// </summary>
    public TState InitialState
    { 
        get => this.initialState;
        set
        {
            this.initialState = value;
            this.allStates.Add(value);
        }
    }

    /// <inheritdoc/>
    IReadOnlyCollection<TState> IFiniteStateAutomaton<TState, TSymbol>.InitialStates =>
        new EnumerableCollection<TState>(EnumerableExtensions.Singleton(this.InitialState), 1);

    /// <inheritdoc/>
    public ICollection<TState> AcceptingStates => this.acceptingStates;

    /// <inheritdoc/>
    public ICollection<Transition<TState, Interval<TSymbol>>> Transitions => this.transitions;

    /// <inheritdoc/>
    IReadOnlyCollection<EpsilonTransition<TState>> IFiniteStateAutomaton<TState, TSymbol>.EpsilonTransitions =>
        Array.Empty<EpsilonTransition<TState>>();

    /// <inheritdoc/>
    public ICollection<Interval<TSymbol>> Alphabet => this.alphabet;

    /// <inheritdoc/>
    public IEnumerable<TState> ReachableStates => GraphSearch.DepthFirst(
        root: this.InitialState,
        comparer: this.StateComparer,
        nodeAdapter: new GraphNodeAdapter(this));

    /// <summary>
    /// True, if this DFA is complete over its <see cref="Alphabet"/>.
    /// </summary>
    public bool IsComplete => throw new NotImplementedException();

    /// <summary>
    /// The state comparer.
    /// </summary>
    public IEqualityComparer<TState> StateComparer => this.transitionsRaw.StateComparer;

    /// <summary>
    /// The symbol interval comparer.
    /// </summary>
    public IntervalComparer<TSymbol> SymbolIntervalComparer => this.transitionsRaw.SymbolIntervalComparer;

    /// <summary>
    /// The symbol comparer.
    /// </summary>
    public IComparer<TSymbol> SymbolComparer =>
        this.SymbolIntervalComparer.EndpointComparer.ValueComparer;

    /// <summary>
    /// The symbol equality comparer.
    /// </summary>
    public IEqualityComparer<TSymbol> SymbolEqualityComparer =>
        this.SymbolIntervalComparer.EndpointComparer.ValueEqualityComparer;

    private readonly ObservableCollection<Transition<TState, Interval<TSymbol>>> transitions;
    private readonly ObservableCollection<TState> allStates;
    private readonly ObservableCollection<TState> acceptingStates;
    private readonly ObservableCollection<Interval<TSymbol>> alphabet;
    private TState initialState = default!;

    private readonly TransitionCollection transitionsRaw;

    /// <summary>
    /// Initializes a new, empty <see cref="Dfa{TState, TSymbol}"/>.
    /// </summary>
    /// <param name="stateComparer">The state comparer.</param>
    /// <param name="symbolIntervalComparer">The symbol interval comparer.</param>
    public Dfa(
        IEqualityComparer<TState> stateComparer,
        IntervalComparer<TSymbol> symbolIntervalComparer)
    {
        // Instantiate collections
        this.transitionsRaw = new(stateComparer, symbolIntervalComparer);
        this.transitions = new(this.transitionsRaw);
        this.allStates = new(new HashSet<TState>(stateComparer));
        this.acceptingStates = new(new HashSet<TState>(stateComparer));
        this.alphabet = new(new IntervalSet<TSymbol>(symbolIntervalComparer));

        // Connect them up
        // Adding a transition means we add the source and destination states to all states
        // and adding the symbol to the alphabet
        this.transitions.ItemAdded += (_, item) =>
        {
            this.allStates.Add(item.Source);
            this.allStates.Add(item.Destination);
            this.alphabet.Add(item.Symbol);
        };
        // Removing from the transitions affects nothing
        // Adding a state affects nothing, but removing one affects all transitions containing that state
        // and accepting and initial states
        this.allStates.ItemRemoved += (_, item) =>
        {
            // Remove from initial
            if (this.StateComparer.Equals(item, this.initialState)) this.initialState = default!;
            // Remove from accepting
            this.acceptingStates.Remove(item);
            // Remove both ways from transitions
            this.transitionsRaw.TransitionMap.Remove(item);
            foreach (var map in this.transitionsRaw.TransitionMap.Values)
            {
                var symbolToRemove = map
                        .Where(kv => this.StateComparer.Equals(kv.Value, item))
                        .Select(kv => kv.Key)
                        .GetEnumerator();
                if (symbolToRemove.MoveNext()) map.Remove(symbolToRemove.Current);
            }
        };
        this.allStates.Cleared += (_, _) =>
        {
            this.initialState = default!;
            this.acceptingStates.Clear();
            this.transitions.Clear();
        };
        // Adding to accepting states will add to all states, removing from it means nothing
        this.acceptingStates.ItemAdded += (_, item) => this.allStates.Add(item);
        // Adding to the alphabet means nothing, but removing from it affects all transitions
        this.alphabet.ItemRemoved += (_, item) =>
        {
            foreach (var onMap in this.transitionsRaw.TransitionMap.Values) onMap.Remove(item);
        };
        this.alphabet.Cleared += (_, _) => this.transitions.Clear();
    }

    /// <summary>
    /// Initializes a new, empty <see cref="Dfa{TState, TSymbol}"/>.
    /// </summary>
    public Dfa()
        : this(EqualityComparer<TState>.Default, IntervalComparer<TSymbol>.Default)
    {
    }

    /// <inheritdoc/>
    public bool Accepts(IEnumerable<TSymbol> input) => throw new NotImplementedException();

    /// <summary>
    /// Completes this DFA under its <see cref="Alphabet"/>.
    /// </summary>
    /// <returns>True, if the DFA was not complete and needed completion.</returns>
    public bool Complete() => throw new NotImplementedException();

    /// <summary>
    /// Minimizes this DFA into a new one.
    /// </summary>
    /// <typeparam name="TResultState">The type of the minimized states.</typeparam>
    /// <param name="differentiate">The pairs of thahtes that have to stay different during minimization.
    /// An empty sequence will mean that only the accepting and non-accepting states will be differentiated strictly.</param>
    /// <param name="stateCombiner">The state combiner to use when combining equivalent states.</param>
    /// <returns></returns>
    public Dfa<TResultState, TSymbol> Minimize<TResultState>(
        IEnumerable<(TState, TState)> differentiate,
        IStateCombiner<TState, TResultState> stateCombiner) => throw new NotImplementedException();
}
