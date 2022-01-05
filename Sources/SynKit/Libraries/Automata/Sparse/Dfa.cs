// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using Yoakke.SynKit.Automata.Internal;
using Yoakke.Collections;
using Yoakke.Collections.Graphs;

namespace Yoakke.SynKit.Automata.Sparse;

/// <summary>
/// A generic sparse DFA implementation.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public sealed class Dfa<TState, TSymbol> : ISparseDfa<TState, TSymbol>
{
    private class TransitionCollection
        : IReadOnlyCollection<Transition<TState, TSymbol>>, ICollection<Transition<TState, TSymbol>>
    {
        public IEqualityComparer<TState> StateComparer { get; }

        public IEqualityComparer<TSymbol> SymbolComparer { get; }

        public Dictionary<TState, Dictionary<TSymbol, TState>> TransitionMap { get; }

        public bool IsReadOnly => false;

        public int Count => this.TransitionMap.Values.Sum(v => v.Count);

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
            onMap[item.Symbol] = item.Destination;
            this.Added?.Invoke(this, item);
        }

        public bool Remove(Transition<TState, TSymbol> item)
        {
            if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
            if (!onMap.TryGetValue(item.Symbol, out var gotTo)) return false;
            if (!this.StateComparer.Equals(item.Destination, gotTo)) return false;
            return onMap.Remove(item.Symbol);
        }

        public bool Contains(Transition<TState, TSymbol> item)
        {
            if (!this.TransitionMap.TryGetValue(item.Source, out var onMap)) return false;
            if (!onMap.TryGetValue(item.Symbol, out var gotTo)) return false;
            return this.StateComparer.Equals(item.Destination, gotTo);
        }

        public void CopyTo(Transition<TState, TSymbol>[] array, int arrayIndex)
        {
            foreach (var t in this) array[arrayIndex++] = t;
        }

        public IEnumerator<Transition<TState, TSymbol>> GetEnumerator()
        {
            foreach (var (from, onMap) in this.TransitionMap)
            {
                foreach (var (on, to) in onMap) yield return new(from, on, to);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        public Dictionary<TSymbol, TState> GetTransitionsFrom(TState from)
        {
            if (!this.TransitionMap.TryGetValue(from, out var onMap))
            {
                onMap = new(this.SymbolComparer);
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
    public ICollection<Transition<TState, TSymbol>> Transitions => this.transitions;

    /// <inheritdoc/>
    IReadOnlyCollection<Transition<TState, TSymbol>> IReadOnlySparseFiniteAutomaton<TState, TSymbol>.Transitions => this.transitions;

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
    public bool IsComplete =>
           this.alphabet.Count == 0
        || this.States.All(state => this.alphabet.All(symbol => this.TryGetTransition(state, symbol, out _)));

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
    private readonly ObservableCollection<TSymbol> alphabet;
    private TState initialState = default!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa{TState, TSymbol}"/> class.
    /// </summary>
    public Dfa()
        : this(EqualityComparer<TState>.Default, EqualityComparer<TSymbol>.Default)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dfa{TState, TSymbol}"/> class.
    /// </summary>
    /// <param name="stateComparer">The state comparer to use.</param>
    /// <param name="symbolComparer">The symbol comparer to use.</param>
    public Dfa(IEqualityComparer<TState> stateComparer, IEqualityComparer<TSymbol> symbolComparer)
    {
        var (all, accepting) = ObservableCollection<TState>.StateWithAccepting(() => new HashSet<TState>(stateComparer));
        this.allStates = all;
        this.acceptingStates = accepting;
        this.alphabet = new(new HashSet<TSymbol>(symbolComparer));
        this.transitions = new(stateComparer, symbolComparer);

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
            var on = string.Join(", ", group.Select(g => g.Symbol));
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
        return fromMap.TryGetValue(on, out to);
    }

    /// <inheritdoc/>
    public void AddTransition(TState from, TSymbol on, TState to) => this.Transitions.Add(new(from, on, to));

    /// <inheritdoc/>
    public bool RemoveTransition(TState from, TSymbol on, TState to) => this.Transitions.Remove(new(from, on, to));

    /// <inheritdoc/>
    public bool RemoveUnreachable() => TrivialImpl.RemoveUnreachable(this);

    /// <inheritdoc/>
    public bool Complete(TState trap)
    {
        if (this.alphabet.Count == 0) return true;

        var result = false;
        foreach (var state in this.States)
        {
            var onMap = this.transitions.GetTransitionsFrom(state);
            foreach (var symbol in this.alphabet)
            {
                if (onMap.ContainsKey(symbol)) continue;
                // NOTE: We get away with modification as this does not add a state directly
                onMap.Add(symbol, trap);
                result = true;
            }
        }

        // If we added any transitions to trap, we also need to wire trap into itself
        if (result)
        {
            this.States.Add(trap);
            var trapMap = this.transitions.GetTransitionsFrom(trap);
            foreach (var symbol in this.alphabet) trapMap.Add(symbol, trap);
        }
        return result;
    }

    /// <inheritdoc/>
    IDfa<TResultState, TSymbol> IReadOnlyDfa<TState, TSymbol>.Minimize<TResultState>(
        IStateCombiner<TState, TResultState> combiner,
        IEnumerable<(TState, TState)> differentiatePairs) => this.Minimize(combiner, differentiatePairs);

    /// <inheritdoc/>
    public ISparseDfa<TResultState, TSymbol> Minimize<TResultState>(
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
                var onSet = (this.transitions.TransitionMap.TryGetValue(s1, out var s1on) ? s1on.Keys : Enumerable.Empty<TSymbol>())
                        .Concat(this.transitions.TransitionMap.TryGetValue(s2, out var s2on) ? s2on.Keys : Enumerable.Empty<TSymbol>())
                        .ToHashSet(this.SymbolComparer);
                foreach (var on in onSet)
                {
                    var on1has = s1on.TryGetValue(on, out var to1);
                    var on2has = s2on.TryGetValue(on, out var to2);
                    if ((on1has && !on2has && equivalenceTable.IsDifferentFromTrap(to1))
                        || (!on1has && on2has && equivalenceTable.IsDifferentFromTrap(to2))) return true;
                    if (!on1has || !on2has) continue;
                    if (equivalenceTable.AreDifferent(to1, to2)) return true;
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
        var result = new Dfa<TResultState, TSymbol>(combiner.ResultComparer, this.SymbolComparer);

        // Now build the new transitions with the state equivalences
        foreach (var (from, on, to) in this.Transitions) result.AddTransition(stateMap[from], on, stateMap[to]);

        // Introduce the initial state and all the accepting states
        result.InitialState = stateMap[this.InitialState];
        foreach (var s in this.acceptingStates) result.AcceptingStates.Add(stateMap[s]);

        return result;
    }
}
