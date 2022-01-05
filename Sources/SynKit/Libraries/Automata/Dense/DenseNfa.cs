// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;
using Yoakke.SynKit.Automata.Internal;
using Yoakke.Collections;
using Yoakke.Collections.Dense;
using Yoakke.Collections.Graphs;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata.Dense;

/// <summary>
/// A generic dense NFA implementation.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public sealed class DenseNfa<TState, TSymbol> : IDenseNfa<TState, TSymbol>
{
    private class TransitionCollection
        : IReadOnlyCollection<Transition<TState, Interval<TSymbol>>>, ICollection<Transition<TState, Interval<TSymbol>>>
    {
        public IEqualityComparer<TState> StateComparer { get; }

        public IntervalComparer<TSymbol> SymbolIntervalComparer { get; }

        public Dictionary<TState, DenseMap<TSymbol, HashSet<TState>>> TransitionMap { get; }

        public bool IsReadOnly => false;

        public int Count => this.TransitionMap.Values.Sum(v => v.Values.Sum(s => s.Count));

        public event EventHandler<Transition<TState, Interval<TSymbol>>>? Added;

        public TransitionCollection(IEqualityComparer<TState> stateComparer, IntervalComparer<TSymbol> symbolComparer)
        {
            this.TransitionMap = new(stateComparer);
            this.StateComparer = stateComparer;
            this.SymbolIntervalComparer = symbolComparer;
        }

        public void Clear() => this.TransitionMap.Clear();

        public void Add(Transition<TState, Interval<TSymbol>> item)
        {
            var onMap = this.GetTransitionsFrom(item.Source);
            onMap.Add(item.Symbol, new HashSet<TState>(this.StateComparer) { item.Destination });
            this.Added?.Invoke(this, item);
        }

        public bool Remove(Transition<TState, Interval<TSymbol>> item)
        {
            if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
            var anyRemoved = false;
            var intervals = onMap.GetIntervalsAndValues(item.Symbol).ToList();
            foreach (var (iv, toSet) in intervals)
            {
                if (!toSet.Contains(item.Destination)) continue;
                var commonIv = this.SymbolIntervalComparer.Intersection(iv, item.Symbol);
                onMap.Remove(commonIv);
                if (toSet.Count > 1)
                {
                    var newToSet = toSet.ToHashSet(this.StateComparer);
                    newToSet.Remove(item.Destination);
                    onMap.Add(commonIv, newToSet);
                }
                anyRemoved = true;
            }
            return anyRemoved;
        }

        public bool Contains(Transition<TState, Interval<TSymbol>> item)
        {
            if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
            if (!onMap.ContainsKeys(item.Symbol)) return false;
            return onMap.GetValues(item.Symbol).All(toSet => toSet.Contains(item.Destination));
        }

        public void CopyTo(Transition<TState, Interval<TSymbol>>[] array, int arrayIndex)
        {
            foreach (var t in this) array[arrayIndex++] = t;
        }

        public IEnumerator<Transition<TState, Interval<TSymbol>>> GetEnumerator()
        {
            foreach (var (from, onMap) in this.TransitionMap)
            {
                foreach (var (on, toSet) in onMap)
                {
                    foreach (var to in toSet) yield return new(from, on, to);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public DenseMap<TSymbol, HashSet<TState>> GetTransitionsFrom(TState from)
        {
            if (!this.TransitionMap.TryGetValue(from, out var onMap))
            {
                onMap = new(
                    this.SymbolIntervalComparer,
                    Combiner<HashSet<TState>>.Create((a, b) => a.Concat(b).ToHashSet(this.StateComparer)));
                this.TransitionMap.Add(from, onMap);
            }
            return onMap;
        }
    }

    private class EpsilonTransitionCollection
        : IReadOnlyCollection<EpsilonTransition<TState>>, ICollection<EpsilonTransition<TState>>
    {
        public IEqualityComparer<TState> StateComparer { get; }

        public Dictionary<TState, HashSet<TState>> EpsilonTransitionMap { get; }

        public bool IsReadOnly => false;

        public int Count => this.EpsilonTransitionMap.Values.Sum(v => v.Count);

        public event EventHandler<EpsilonTransition<TState>>? Added;

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
            this.Added?.Invoke(this, item);
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
            foreach (var (from, toSet) in this.EpsilonTransitionMap)
            {
                foreach (var to in toSet) yield return new(from, to);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    /// <inheritdoc/>
    public ICollection<TState> InitialStates => this.initialStates;

    /// <inheritdoc/>
    IReadOnlyCollection<TState> IReadOnlyNfa<TState, TSymbol>.InitialStates => this.initialStates;

    /// <inheritdoc/>
    public ICollection<TState> AcceptingStates => this.acceptingStates;

    /// <inheritdoc/>
    IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.AcceptingStates => this.acceptingStates;

    /// <inheritdoc/>
    public ICollection<TState> States => this.allStates;

    /// <inheritdoc/>
    IReadOnlyCollection<TState> IReadOnlyFiniteAutomaton<TState, TSymbol>.States => this.allStates;

    /// <inheritdoc/>
    public ICollection<Transition<TState, Interval<TSymbol>>> Transitions => this.transitions;

    /// <inheritdoc/>
    IReadOnlyCollection<Transition<TState, Interval<TSymbol>>> IReadOnlyDenseFiniteAutomaton<TState, TSymbol>.Transitions => this.transitions;

    /// <inheritdoc/>
    public ICollection<EpsilonTransition<TState>> EpsilonTransitions => this.epsilonTransitions;

    /// <inheritdoc/>
    IReadOnlyCollection<EpsilonTransition<TState>> IReadOnlyNfa<TState, TSymbol>.EpsilonTransitions => this.epsilonTransitions;

    /// <inheritdoc/>
    public ICollection<Interval<TSymbol>> Alphabet => this.alphabet;

    /// <inheritdoc/>
    IReadOnlyCollection<Interval<TSymbol>> IReadOnlyDenseFiniteAutomaton<TState, TSymbol>.Alphabet => this.alphabet;

    /// <inheritdoc/>
    public IEqualityComparer<TState> StateComparer => this.transitions.StateComparer;

    /// <inheritdoc/>
    public IntervalComparer<TSymbol> SymbolIntervalComparer => this.transitions.SymbolIntervalComparer;

    /// <inheritdoc/>
    public IEnumerable<TState> ReachableStates
    {
        get
        {
            IEnumerable<TState> GetNeighbors(TState from) =>
                (this.transitions.TransitionMap.TryGetValue(from, out var onMap)
                    ? onMap.Values.SelectMany(x => x)
                    : Enumerable.Empty<TState>()).Concat(
                    this.epsilonTransitions.EpsilonTransitionMap.TryGetValue(from, out var toSet)
                        ? toSet
                        : Enumerable.Empty<TState>());

            var touched = new HashSet<TState>(this.StateComparer);
            foreach (var initial in this.InitialStates)
            {
                if (!touched.Add(initial)) continue;
                foreach (var s in BreadthFirst.Search(initial, GetNeighbors, this.StateComparer))
                {
                    touched.Add(s);
                    yield return s;
                }
            }
        }
    }

    private readonly TransitionCollection transitions;
    private readonly EpsilonTransitionCollection epsilonTransitions;
    private readonly ObservableCollection<TState> allStates;
    private readonly ObservableCollection<TState> acceptingStates;
    private readonly ObservableCollection<TState> initialStates;
    private readonly ObservableCollection<Interval<TSymbol>> alphabet;

    /// <summary>
    /// Initializes a new instance of the <see cref="DenseNfa{TState, TSymbol}"/> class.
    /// </summary>
    public DenseNfa()
        : this(EqualityComparer<TState>.Default, IntervalComparer<TSymbol>.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DenseNfa{TState, TSymbol}"/> class.
    /// </summary>
    /// <param name="stateComparer">The state comparer to use.</param>
    /// <param name="symbolIntervalComparer">The symbol interval comparer to use.</param>
    public DenseNfa(IEqualityComparer<TState> stateComparer, IntervalComparer<TSymbol> symbolIntervalComparer)
    {
        var (all, accepting, initial) = ObservableCollection<TState>.StateWithAcceptingAndInitial(() => new HashSet<TState>(stateComparer));
        this.allStates = all;
        this.initialStates = initial;
        this.acceptingStates = accepting;
        this.alphabet = new(new DenseSet<TSymbol>(symbolIntervalComparer));
        this.transitions = new(stateComparer, symbolIntervalComparer);
        this.epsilonTransitions = new(stateComparer);

        this.allStates.Removed += (sender, item) =>
        {
        // Remove both ways from transitions
        this.transitions.TransitionMap.Remove(item);
            foreach (var map in this.transitions.TransitionMap.Values)
            {
                foreach (var toSet in map.Values) toSet.Remove(item);
            }
        // Remove both ways from epsilon transitions
        this.epsilonTransitions.EpsilonTransitionMap.Remove(item);
            foreach (var toSet in this.epsilonTransitions.EpsilonTransitionMap.Values) toSet.Remove(item);
        };
        this.allStates.Cleared += (sender, eventArgs) =>
        {
            this.transitions.Clear();
            this.epsilonTransitions.Clear();
        };

        this.transitions.Added += (sender, item) =>
        {
            this.alphabet.Add(item.Symbol);
            this.allStates.Add(item.Source);
            this.allStates.Add(item.Destination);
        };

        this.epsilonTransitions.Added += (sender, item) =>
        {
            this.allStates.Add(item.Source);
            this.allStates.Add(item.Destination);
        };

        this.alphabet.Removed += (sender, item) =>
        {
            foreach (var onMap in this.transitions.TransitionMap.Values) onMap.Remove(item);
        };
        this.alphabet.Cleared += (sender, eventArgs) => this.transitions.Clear();
    }

    /// <inheritdoc/>
    public string ToDot()
    {
        var writer = new DotWriter<TState>(this.StateComparer);
        writer.WriteStart("NFA");

        // Accepting states
        writer.WriteAcceptingStates(this.AcceptingStates);
        // Non-accepting states
        writer.WriteStates(this.States.Except(this.AcceptingStates, this.StateComparer));
        // Initial states
        writer.WriteInitialStates(this.InitialStates);

        // Transitions
        var tupleComparer = new TupleEqualityComparer<TState, TState>(this.StateComparer, this.StateComparer);
        var transitionsByState = this.Transitions.GroupBy(t => (t.Source, t.Destination), tupleComparer);
        var remainingEpsilon = this.epsilonTransitions.EpsilonTransitionMap
            .ToDictionary(kv => kv.Key, kv => kv.Value.ToHashSet(this.StateComparer), this.StateComparer);
        foreach (var group in transitionsByState)
        {
            var from = group.Key.Item1;
            var to = group.Key.Item2;
            var on = string.Join(" U ", group.Select(g => g.Symbol));
            if (this.EpsilonTransitions.Contains(new(from, to)))
            {
                remainingEpsilon[from].Remove(to);
                on = $"{on}, ε";
            }
            writer.WriteTransition(from, on, to);
        }

        // Epsilon-transitions
        foreach (var (from, toSet) in remainingEpsilon)
        {
            foreach (var to in toSet) writer.WriteTransition(from, "ε", to);
        }

        writer.WriteEnd();
        return writer.Code.ToString();
    }

    /// <inheritdoc/>
    public bool Accepts(IEnumerable<TSymbol> input) => TrivialImpl.Accepts(this, input);

    /// <inheritdoc/>
    public IEnumerable<TState> GetTransitions(TState from, TSymbol on)
    {
        var touched = new HashSet<TState>(this.StateComparer);
        foreach (var fromState in this.EpsilonClosure(from))
        {
            if (!this.transitions.TransitionMap.TryGetValue(fromState, out var onMap)) continue;
            var values = onMap.GetValues(on).GetEnumerator();
            if (!values.MoveNext()) continue;

            foreach (var s in values.Current.SelectMany(this.EpsilonClosure))
            {
                if (touched.Add(s)) yield return s;
            }
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
    public void AddEpsilonTransition(TState from, TState to) => this.EpsilonTransitions.Add(new(from, to));

    /// <inheritdoc/>
    public bool RemoveEpsilonTransition(TState from, TState to) => this.EpsilonTransitions.Remove(new(from, to));

    /// <inheritdoc/>
    public IEnumerable<TState> EpsilonClosure(TState state) => BreadthFirst.Search(
        state,
        state => this.epsilonTransitions.EpsilonTransitionMap.TryGetValue(state, out var eps)
            ? eps
            : Enumerable.Empty<TState>(),
        this.StateComparer);

    /// <inheritdoc/>
    public bool RemoveUnreachable() => TrivialImpl.RemoveUnreachable(this);

    /// <inheritdoc/>
    public bool EliminateEpsilonTransitions() => TrivialImpl.EliminateEpsilonTransitions(
        this,
        (v1, v2) =>
        {
            if (!this.transitions.TransitionMap.TryGetValue(v2, out var fromV2Map)) return;
            var fromV1Map = this.transitions.GetTransitionsFrom(v1);
            foreach (var (on, toV2Set) in fromV2Map) fromV1Map.Add(on, toV2Set);
        });

    /// <inheritdoc/>
    IDfa<TResultState, TSymbol> IReadOnlyNfa<TState, TSymbol>.Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner) =>
        this.Determinize(combiner);

    /// <inheritdoc/>
    public IDenseDfa<TResultState, TSymbol> Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner)
    {
        var result = new DenseDfa<TResultState, TSymbol>(combiner.ResultComparer, this.SymbolIntervalComparer);
        var visited = new HashSet<StateSet<TState>>();
        var stk = new Stack<StateSet<TState>>();
        var first = new StateSet<TState>(this.InitialStates.SelectMany(this.EpsilonClosure).ToHashSet(this.StateComparer));
        result.InitialState = combiner.Combine(first);
        stk.Push(first);
        while (stk.TryPop(out var top))
        {
            // Construct a transition map
            // Essentially from the current set of states we calculate what set of states we arrive at for a given symbol
            var resultTransitions = new DenseMap<TSymbol, HashSet<TState>>(
                this.SymbolIntervalComparer,
                Combiner<HashSet<TState>>.Create((a, b) => a.Concat(b).ToHashSet(this.StateComparer)));
            foreach (var primState in top)
            {
                if (!this.transitions.TransitionMap.TryGetValue(primState, out var onMap)) continue;
                foreach (var (on, toSet) in onMap)
                {
                    foreach (var to in toSet.SelectMany(this.EpsilonClosure))
                    {
                        resultTransitions.Add(on, new HashSet<TState>(this.StateComparer) { to });
                    }
                }
            }
            // Add the transitions
            var from = combiner.Combine(top);
            foreach (var (on, toHashSet) in resultTransitions)
            {
                var toSet = new StateSet<TState>(toHashSet);
                if (visited.Add(toSet)) stk.Push(toSet);
                var to = combiner.Combine(toSet);
                result.AddTransition(from, on, to);
            }
            // Register as accepting, if any
            if (top.Any(s => this.AcceptingStates.Contains(s))) result.AcceptingStates.Add(from);
        }
        return result;
    }
}
