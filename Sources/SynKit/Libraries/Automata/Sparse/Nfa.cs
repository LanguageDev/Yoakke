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
using Yoakke.Collections.Graphs;

namespace Yoakke.SynKit.Automata.Sparse;

/// <summary>
/// A generic sparse NFA implementation.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public sealed class Nfa<TState, TSymbol> : ISparseNfa<TState, TSymbol>
{
    private class TransitionCollection
        : IReadOnlyCollection<Transition<TState, TSymbol>>, ICollection<Transition<TState, TSymbol>>
    {
        public IEqualityComparer<TState> StateComparer { get; }

        public IEqualityComparer<TSymbol> SymbolComparer { get; }

        public Dictionary<TState, Dictionary<TSymbol, HashSet<TState>>> TransitionMap { get; }

        public bool IsReadOnly => false;

        public int Count => this.TransitionMap.Values.Sum(v => v.Values.Sum(s => s.Count));

        public event EventHandler<Transition<TState, TSymbol>>? Added;

        public TransitionCollection(IEqualityComparer<TState> stateComparer, IEqualityComparer<TSymbol> symbolComparer)
        {
            this.TransitionMap = new(stateComparer);
            this.StateComparer = stateComparer;
            this.SymbolComparer = symbolComparer;
        }

        public void Clear() => this.TransitionMap.Clear();

        public void Add(Transition<TState, TSymbol> item)
        {
            var onMap = this.GetTransitionsFrom(item.Source);
            if (!onMap.TryGetValue(item.Symbol, out var toSet))
            {
                toSet = new(this.StateComparer);
                onMap.Add(item.Symbol, toSet);
            }
            toSet.Add(item.Destination);
            this.Added?.Invoke(this, item);
        }

        public bool Remove(Transition<TState, TSymbol> item)
        {
            if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
            if (!onMap.TryGetValue(item.Symbol, out var toSet)) return false;
            return toSet.Remove(item.Destination);
        }

        public bool Contains(Transition<TState, TSymbol> item)
        {
            if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
            if (!onMap.TryGetValue(item.Symbol, out var toSet)) return false;
            return toSet.Contains(item.Destination);
        }

        public void CopyTo(Transition<TState, TSymbol>[] array, int arrayIndex)
        {
            foreach (var t in this) array[arrayIndex++] = t;
        }

        public IEnumerator<Transition<TState, TSymbol>> GetEnumerator()
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

        public Dictionary<TSymbol, HashSet<TState>> GetTransitionsFrom(TState from)
        {
            if (!this.TransitionMap.TryGetValue(from, out var onMap))
            {
                onMap = new(this.SymbolComparer);
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
    public ICollection<Transition<TState, TSymbol>> Transitions => this.transitions;

    /// <inheritdoc/>
    IReadOnlyCollection<Transition<TState, TSymbol>> IReadOnlySparseFiniteAutomaton<TState, TSymbol>.Transitions => this.transitions;

    /// <inheritdoc/>
    public ICollection<EpsilonTransition<TState>> EpsilonTransitions => this.epsilonTransitions;

    /// <inheritdoc/>
    IReadOnlyCollection<EpsilonTransition<TState>> IReadOnlyNfa<TState, TSymbol>.EpsilonTransitions => this.epsilonTransitions;

    /// <inheritdoc/>
    public ICollection<TSymbol> Alphabet => this.alphabet;

    /// <inheritdoc/>
    IReadOnlyCollection<TSymbol> IReadOnlySparseFiniteAutomaton<TState, TSymbol>.Alphabet => this.alphabet;

    /// <inheritdoc/>
    public IEqualityComparer<TState> StateComparer => this.transitions.StateComparer;

    /// <summary>
    /// The comparer used for alphabet symbols.
    /// </summary>
    public IEqualityComparer<TSymbol> SymbolComparer => this.transitions.SymbolComparer;

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
    private readonly ObservableCollection<TSymbol> alphabet;

    /// <summary>
    /// Initializes a new instance of the <see cref="Nfa{TState, TSymbol}"/> class.
    /// </summary>
    public Nfa()
        : this(EqualityComparer<TState>.Default, EqualityComparer<TSymbol>.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Nfa{TState, TSymbol}"/> class.
    /// </summary>
    /// <param name="stateComparer">The state comparer to use.</param>
    /// <param name="symbolComparer">The symbol comparer to use.</param>
    public Nfa(IEqualityComparer<TState> stateComparer, IEqualityComparer<TSymbol> symbolComparer)
    {
        var (all, accepting, initial) = ObservableCollection<TState>.StateWithAcceptingAndInitial(() => new HashSet<TState>(stateComparer));
        this.allStates = all;
        this.initialStates = initial;
        this.acceptingStates = accepting;
        this.alphabet = new(new HashSet<TSymbol>(symbolComparer));
        this.transitions = new(stateComparer, symbolComparer);
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
            var on = string.Join(", ", group.Select(g => g.Symbol));
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
            if (!onMap.TryGetValue(on, out var toSet)) continue;

            foreach (var s in toSet.SelectMany(this.EpsilonClosure))
            {
                if (touched.Add(s)) yield return s;
            }
        }
    }

    /// <inheritdoc/>
    public void AddTransition(TState from, TSymbol on, TState to) => this.Transitions.Add(new(from, on, to));

    /// <inheritdoc/>
    public bool RemoveTransition(TState from, TSymbol on, TState to) => this.Transitions.Remove(new(from, on, to));

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
            foreach (var (on, toV2Set) in fromV2Map)
            {
                if (!fromV1Map.TryGetValue(on, out var toV1Set))
                {
                    toV1Set = new(this.StateComparer);
                    fromV1Map.Add(on, toV1Set);
                }
                foreach (var s in toV2Set) toV1Set.Add(s);
            }
        });

    /// <inheritdoc/>
    IDfa<TResultState, TSymbol> IReadOnlyNfa<TState, TSymbol>.Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner) =>
        this.Determinize(combiner);

    /// <inheritdoc/>
    public ISparseDfa<TResultState, TSymbol> Determinize<TResultState>(IStateCombiner<TState, TResultState> combiner)
    {
        var result = new Dfa<TResultState, TSymbol>(combiner.ResultComparer, this.SymbolComparer);
        var visited = new HashSet<StateSet<TState>>();
        var stk = new Stack<StateSet<TState>>();
        var first = new StateSet<TState>(this.InitialStates.SelectMany(this.EpsilonClosure).ToHashSet(this.StateComparer));
        result.InitialState = combiner.Combine(first);
        stk.Push(first);
        while (stk.TryPop(out var top))
        {
            // Construct a transition map
            // Essentially from the current set of states we calculate what set of states we arrive at for a given symbol
            var resultTransitions = new Dictionary<TSymbol, HashSet<TState>>(this.SymbolComparer);
            foreach (var primState in top)
            {
                if (!this.transitions.TransitionMap.TryGetValue(primState, out var onMap)) continue;
                foreach (var (on, toSet) in onMap)
                {
                    if (!resultTransitions.TryGetValue(on, out var existing))
                    {
                        existing = new(this.StateComparer);
                        resultTransitions.Add(on, existing);
                    }
                    foreach (var to in toSet.SelectMany(this.EpsilonClosure)) existing.Add(to);
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
