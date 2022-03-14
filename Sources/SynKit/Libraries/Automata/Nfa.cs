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
/// A generic NFA implementation.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public sealed class Nfa<TState, TSymbol> : IFiniteStateAutomaton<TState, TSymbol>
{
    private readonly struct UnionCombiner : IValueCombiner<HashSet<TState>>
    {
        public HashSet<TState> Combine(HashSet<TState> first, HashSet<TState> second) =>
            new(first.Concat(second), first.Comparer);
    }

    private sealed class TransitionCollection
        : IReadOnlyCollection<Transition<TState, Interval<TSymbol>>>,
          ICollection<Transition<TState, Interval<TSymbol>>>
    {
        public int Count => this.TransitionMap.Values.Sum(v => v.Values.Sum(s => s.Count));

        public bool IsReadOnly => false;

        public IEqualityComparer<TState> StateComparer { get; }

        public IntervalComparer<TSymbol> SymbolIntervalComparer { get; }

        public Dictionary<TState, IntervalMap<TSymbol, HashSet<TState>>> TransitionMap { get; }

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
            onMap.Add(
                item.Symbol,
                new HashSet<TState>(this.StateComparer) { item.Destination },
                default(UnionCombiner));
        }

        public IntervalMap<TSymbol, HashSet<TState>> GetTransitionsFrom(TState from)
        {
            if (!this.TransitionMap.TryGetValue(from, out var onMap))
            {
                onMap = new(this.SymbolIntervalComparer);
                this.TransitionMap.Add(from, onMap);
            }
            return onMap;
        }

        public bool Remove(Transition<TState, Interval<TSymbol>> item)
        {
            if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
            var anyRemoved = false;
            var intervals = onMap.Intersecting(item.Symbol).ToList();
            foreach (var kv in intervals)
            {
                var iv = kv.Key;
                var toSet = kv.Value;
                if (!toSet.Contains(item.Destination)) continue;
                var commonIv = this.SymbolIntervalComparer.Intersection(iv, item.Symbol);
                onMap.Remove(commonIv);
                if (toSet.Count > 1)
                {
                    var newToSet = new HashSet<TState>(toSet, this.StateComparer);
                    newToSet.Remove(item.Destination);
                    onMap.Add(commonIv, newToSet, default(UnionCombiner));
                }
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
                .All(toSet => toSet.Contains(item.Destination));
        }

        public void CopyTo(Transition<TState, Interval<TSymbol>>[] array, int arrayIndex)
        {
            if (arrayIndex + this.Count > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            foreach (var t in this) array[arrayIndex++] = t;
        }

        public IEnumerator<Transition<TState, Interval<TSymbol>>> GetEnumerator()
        {
            foreach (var fromOn in this.TransitionMap)
            {
                foreach (var onTo in fromOn.Value)
                {
                    foreach (var to in onTo.Value) yield return new(fromOn.Key, onTo.Key, to);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    private sealed class EpsilonTransitionCollection
        : IReadOnlyCollection<EpsilonTransition<TState>>,
          ICollection<EpsilonTransition<TState>>
    {
        public IEqualityComparer<TState> StateComparer { get; }

        public Dictionary<TState, HashSet<TState>> EpsilonTransitionMap { get; }

        public bool IsReadOnly => false;

        public int Count => this.EpsilonTransitionMap.Values.Sum(v => v.Count);

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
            foreach (var fromTo in this.EpsilonTransitionMap)
            {
                foreach (var to in fromTo.Value) yield return new(fromTo.Key, to);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    /// <inheritdoc/>
    public ICollection<TState> States => this.allStates;

    /// <summary>
    /// The initial states of the automaton.
    /// </summary>
    public ICollection<TState> InitialStates => this.initialStates;

    /// <inheritdoc/>
    IReadOnlyCollection<TState> IFiniteStateAutomaton<TState, TSymbol>.InitialStates => this.initialStates;

    /// <inheritdoc/>
    public ICollection<TState> AcceptingStates => this.acceptingStates;

    /// <inheritdoc/>
    public ICollection<Transition<TState, Interval<TSymbol>>> Transitions => this.transitions;

    /// <summary>
    /// The epsilon-tramsitions of the automaton.
    /// </summary>
    public ICollection<EpsilonTransition<TState>> EpsilonTransitions => this.epsilonTransitions;

    /// <inheritdoc/>
    IReadOnlyCollection<EpsilonTransition<TState>> IFiniteStateAutomaton<TState, TSymbol>.EpsilonTransitions =>
        this.epsilonTransitions;

    /// <inheritdoc/>
    public ICollection<Interval<TSymbol>> Alphabet => this.alphabet;

    /// <summary>
    /// The state comparer.
    /// </summary>
    public IEqualityComparer<TState> StateComparer { get; }

    /// <summary>
    /// The symbol interval comparer.
    /// </summary>
    public IntervalComparer<TSymbol> SymbolIntervalComparer { get; }

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
    private readonly ObservableCollection<EpsilonTransition<TState>> epsilonTransitions;
    private readonly ObservableCollection<TState> allStates;
    private readonly ObservableCollection<TState> acceptingStates;
    private readonly ObservableCollection<TState> initialStates;
    private readonly ObservableCollection<Interval<TSymbol>> alphabet;

    private readonly TransitionCollection transitionsRaw;
    private readonly EpsilonTransitionCollection epsilonTransitionsRaw;

    /// <summary>
    /// Initializes a new, empty <see cref="Nfa{TState, TSymbol}"/>.
    /// </summary>
    /// <param name="stateComparer">The state comparer.</param>
    /// <param name="symbolIntervalComparer">The symbol interval comparer.</param>
    public Nfa(
        IEqualityComparer<TState> stateComparer,
        IntervalComparer<TSymbol> symbolIntervalComparer)
    {
        this.StateComparer = stateComparer;
        this.SymbolIntervalComparer = symbolIntervalComparer;

        // Instantiate collections
        this.transitionsRaw = new(stateComparer, symbolIntervalComparer);
        this.epsilonTransitionsRaw = new(stateComparer);
        this.transitions = new(this.transitionsRaw);
        this.epsilonTransitions = new(this.epsilonTransitionsRaw);
        this.allStates = new(new HashSet<TState>(stateComparer));
        this.acceptingStates = new(new HashSet<TState>(stateComparer));
        this.initialStates = new(new HashSet<TState>(stateComparer));
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
        // Adding an epsilon transition is almost the same deal, but it only touches states (there is no symbol)
        this.epsilonTransitions.ItemAdded += (_, item) =>
        {
            this.allStates.Add(item.Source);
            this.allStates.Add(item.Destination);
        };
        // Adding a state affects nothing, but removing one affects all transitions containing that state
        // and accepting and initial states
        this.allStates.ItemRemoved += (_, item) =>
        {
            // Remove from initial
            this.initialStates.Remove(item);
            // Remove from accepting
            this.acceptingStates.Remove(item);
            // Remove both ways from transitions
            this.transitionsRaw.TransitionMap.Remove(item);
            foreach (var map in this.transitionsRaw.TransitionMap.Values)
            {
                foreach (var toSet in map.Values) toSet.Remove(item);
            }
            // Remove both ways from epsilon transitions
            this.epsilonTransitionsRaw.EpsilonTransitionMap.Remove(item);
            foreach (var toSet in this.epsilonTransitionsRaw.EpsilonTransitionMap.Values) toSet.Remove(item);
        };
        this.allStates.Cleared += (_, _) =>
        {
            this.initialStates.Clear();
            this.acceptingStates.Clear();
            this.transitions.Clear();
            this.epsilonTransitions.Clear();
        };
        // Adding to accepting states will add to all states, removing from it means nothing
        this.acceptingStates.ItemAdded += (_, item) => this.allStates.Add(item);
        // Adding to initial states will add to all states, removing from it means nothing
        this.initialStates.ItemAdded += (_, item) => this.allStates.Add(item);
        // Adding to the alphabet means nothing, but removing from it affects all transitions
        this.alphabet.ItemRemoved += (_, item) =>
        {
            foreach (var onMap in this.transitionsRaw.TransitionMap.Values) onMap.Remove(item);
        };
        this.alphabet.Cleared += (_, _) => this.transitions.Clear();
    }

    /// <summary>
    /// Initializes a new, empty <see cref="Nfa{TState, TSymbol}"/>.
    /// </summary>
    public Nfa()
        : this(EqualityComparer<TState>.Default, IntervalComparer<TSymbol>.Default)
    {
    }

    /// <inheritdoc/>
    public bool Accepts(IEnumerable<TSymbol> input) => throw new NotImplementedException();
}
