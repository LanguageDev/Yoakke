// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Yoakke.SynKit.Automata.Internal;
using Yoakke.Collections;
using Yoakke.Collections.Dense;
using Yoakke.Collections.Graphs;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata.Dense;

/// <summary>
/// A generic dense DFA implementation.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public sealed class DenseDfa<TState, TSymbol> : IDenseDfa<TState, TSymbol>
{
    private class TransitionCollection
        : IReadOnlyCollection<Transition<TState, Interval<TSymbol>>>, ICollection<Transition<TState, Interval<TSymbol>>>
    {
        public IEqualityComparer<TState> StateComparer { get; }

        public IntervalComparer<TSymbol> SymbolIntervalComparer { get; }

        public Dictionary<TState, DenseMap<TSymbol, TState>> TransitionMap { get; }

        public bool IsReadOnly => false;

        public int Count => this.TransitionMap.Values.Sum(v => v.Count);

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
            onMap.Add(item.Symbol, item.Destination);
            this.Added?.Invoke(this, item);
        }

        public bool Remove(Transition<TState, Interval<TSymbol>> item)
        {
            if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
            var anyRemoved = false;
            var intervals = onMap.GetIntervalsAndValues(item.Symbol).ToList();
            foreach (var (iv, value) in intervals)
            {
                if (!this.StateComparer.Equals(value, item.Destination)) continue;
                var commonIv = this.SymbolIntervalComparer.Intersection(iv, item.Symbol);
                onMap.Remove(commonIv);
                anyRemoved = true;
            }
            return anyRemoved;
        }

        public bool Contains(Transition<TState, Interval<TSymbol>> item)
        {
            if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
            if (!onMap.ContainsKeys(item.Symbol)) return false;
            return onMap.GetValues(item.Symbol).All(v => this.StateComparer.Equals(v, item.Destination));
        }

        public void CopyTo(Transition<TState, Interval<TSymbol>>[] array, int arrayIndex)
        {
            foreach (var t in this) array[arrayIndex++] = t;
        }

        public IEnumerator<Transition<TState, Interval<TSymbol>>> GetEnumerator()
        {
            foreach (var (from, onMap) in this.TransitionMap)
            {
                foreach (var (on, to) in onMap) yield return new(from, on, to);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public DenseMap<TSymbol, TState> GetTransitionsFrom(TState from)
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
    public ICollection<Interval<TSymbol>> Alphabet => this.alphabet;

    /// <inheritdoc/>
    IReadOnlyCollection<Interval<TSymbol>> IReadOnlyDenseFiniteAutomaton<TState, TSymbol>.Alphabet => this.alphabet;

    /// <inheritdoc/>
    public IEqualityComparer<TState> StateComparer => this.transitions.StateComparer;

    /// <summary>
    /// The comparer used for alphabet symbol intervals.
    /// </summary>
    public IntervalComparer<TSymbol> SymbolIntervalComparer => this.transitions.SymbolIntervalComparer;

    /// <inheritdoc/>
    public IEnumerable<TState> ReachableStates => BreadthFirst.Search(
        this.InitialState,
        state => this.transitions.TransitionMap.TryGetValue(state, out var transitions)
            ? transitions.Values
            : Enumerable.Empty<TState>(),
        this.StateComparer);

    private readonly TransitionCollection transitions;
    private readonly ObservableCollection<TState> allStates;
    private readonly ObservableCollection<TState> acceptingStates;
    private readonly ObservableCollection<Interval<TSymbol>> alphabet;
    private TState initialState = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="DenseDfa{TState, TSymbol}"/> class.
    /// </summary>
    public DenseDfa()
        : this(EqualityComparer<TState>.Default, IntervalComparer<TSymbol>.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DenseDfa{TState, TSymbol}"/> class.
    /// </summary>
    /// <param name="stateComparer">The state comparer to use.</param>
    /// <param name="symbolIntervalComparer">The symbol interval comparer to use.</param>
    public DenseDfa(IEqualityComparer<TState> stateComparer, IntervalComparer<TSymbol> symbolIntervalComparer)
    {
        var (all, accepting) = ObservableCollection<TState>.StateWithAccepting(() => new HashSet<TState>(stateComparer));
        this.transitions = new(stateComparer, symbolIntervalComparer);
        this.allStates = all;
        this.acceptingStates = accepting;
        this.alphabet = new(new DenseSet<TSymbol>(symbolIntervalComparer));
        this.transitions = new(stateComparer, symbolIntervalComparer);

        this.allStates.Removed += (sender, item) =>
        {
            if (this.StateComparer.Equals(item, this.initialState)) this.initialState = default!;
        // Remove both ways from transitions
        this.transitions.TransitionMap.Remove(item);
            foreach (var map in this.transitions.TransitionMap.Values)
            {
                var symbolToRemove = map
                        .Where(kv => this.StateComparer.Equals(kv.Value, item))
                        .Select(kv => kv.Key)
                        .GetEnumerator();
                if (symbolToRemove.MoveNext()) map.Remove(symbolToRemove.Current);
            }
        };
        this.allStates.Cleared += (sender, eventArgs) => this.transitions.Clear();

        this.transitions.Added += (sender, item) =>
        {
            this.alphabet.Add(item.Symbol);
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
        writer.WriteStart("DFA");

        // Accepting states
        writer.WriteAcceptingStates(this.AcceptingStates);
        // Non-accepting states
        writer.WriteStates(this.States.Except(this.AcceptingStates, this.StateComparer));
        // Initial states
        writer.WriteInitialStates(new[] { this.InitialState });

        // Transitions
        var tupleComparer = new TupleEqualityComparer<TState, TState>(this.StateComparer, this.StateComparer);
        var transitionsByState = this.Transitions.GroupBy(t => (t.Source, t.Destination), tupleComparer);
        foreach (var group in transitionsByState)
        {
            var from = group.Key.Item1;
            var to = group.Key.Item2;
            var on = string.Join(" U ", group.Select(g => g.Symbol));
            writer.WriteTransition(from, on, to);
        }

        writer.WriteEnd();
        return writer.Code.ToString();
    }

    /// <inheritdoc/>
    public bool Accepts(IEnumerable<TSymbol> input) => TrivialImpl.Accepts(this, input);

    /// <inheritdoc/>
    public bool TryGetTransition(TState from, TSymbol on, [MaybeNullWhen(false)] out TState to)
    {
        if (!this.transitions.TransitionMap.TryGetValue(from, out var fromMap))
        {
            to = default;
            return false;
        }
        var values = fromMap.GetValues(on).GetEnumerator();
        if (values.MoveNext())
        {
            to = values.Current;
            return true;
        }
        else
        {
            to = default;
            return false;
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
    public bool RemoveUnreachable() => TrivialImpl.RemoveUnreachable(this);

    /// <inheritdoc/>
    public bool IsComplete(IEnumerable<Interval<TSymbol>> alphabet) =>
           !alphabet.Any()
        || this.States.All(state => this.transitions.TransitionMap.TryGetValue(state, out var onMap)
                                 && alphabet.All(symbolIv => onMap.ContainsKeys(symbolIv)));

    /// <inheritdoc/>
    public bool Complete(IEnumerable<Interval<TSymbol>> alphabet, TState trap) => throw new NotImplementedException();

    /// <inheritdoc/>
    IDfa<TResultState, TSymbol> IReadOnlyDfa<TState, TSymbol>.Minimize<TResultState>(
        IStateCombiner<TState, TResultState> combiner,
        IEnumerable<(TState, TState)> differentiatePairs) => this.Minimize(combiner, differentiatePairs);

    /// <inheritdoc/>
    public IDenseDfa<TResultState, TSymbol> Minimize<TResultState>(
        IStateCombiner<TState, TResultState> combiner,
        IEnumerable<(TState, TState)> differentiatePairs)
    {
        var equivalenceTable = new EquivalenceTable<TState, TSymbol>(this);

        // Initially fill the table
        equivalenceTable.Initialize(differentiatePairs);

        // Fill until no change
        equivalenceTable.Fill(
            (s1, s2) =>
            {
            // We need to cut up the transition symbols into sections
            // For that we build a transition map of (interval -> destination[1..2])
            var onMap = new DenseMap<TSymbol, TState[]>(
                this.SymbolIntervalComparer,
                Combiner<TState[]>.Create((existing, added) => existing.Concat(added).ToArray()));
                if (this.transitions.TransitionMap.TryGetValue(s1, out var s1on))
                {
                    foreach (var (iv, to) in s1on) onMap.Add(iv, new[] { to });
                }
                if (this.transitions.TransitionMap.TryGetValue(s2, out var s2on))
                {
                    foreach (var (iv, to) in s2on) onMap.Add(iv, new[] { to });
                }

                foreach (var (iv, to) in onMap)
                {
                    Debug.Assert(to.Length == 1 || to.Length == 2, "At least one state has to map from the specified intervals.");
                    if (to.Length == 1 && equivalenceTable.IsDifferentFromTrap(to[0])) return true;
                    if (to.Length == 1) continue;
                    if (equivalenceTable.AreDifferent(to[0], to[1])) return true;
                }
                return false;
            },
            state =>
            {
                if (!this.transitions.TransitionMap.TryGetValue(state, out var onSet)) return false;
                foreach (var (_, to) in onSet)
                {
                    if (equivalenceTable.IsDifferentFromTrap(to)) return true;
                }
                return false;
            });

        // Create a state mapping of old-state -> equivalent-state
        var stateMap = equivalenceTable.BuildStateMap(combiner);

        // Create the result
        var result = new DenseDfa<TResultState, TSymbol>(combiner.ResultComparer, this.SymbolIntervalComparer);

        // Now build the new transitions with the state equivalences
        foreach (var (from, on, to) in this.Transitions) result.AddTransition(stateMap[from], on, stateMap[to]);

        // Introduce the initial state and all the accepting states
        result.InitialState = stateMap[this.InitialState];
        foreach (var s in this.acceptingStates) result.AcceptingStates.Add(stateMap[s]);

        return result;
    }
}
