// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Collections;

namespace Yoakke.SynKit.Automata.Internal;

/// <summary>
/// A helper to determine state equivalence.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
internal class EquivalenceTable<TState, TSymbol>
{
    private readonly IReadOnlyDfa<TState, TSymbol> automaton;
    private readonly HashSet<(TState, TState)> table;
    private readonly HashSet<TState> trapTable;
    private readonly List<TState> states;

    /// <summary>
    /// Initializes a new instance of the <see cref="EquivalenceTable{TState, TSymbol}"/> class.
    /// </summary>
    /// <param name="automaton">The automaton to generate the equivalence table for.</param>
    public EquivalenceTable(IReadOnlyDfa<TState, TSymbol> automaton)
    {
        this.automaton = automaton;
        var tupleComparer = new TupleEqualityComparer<TState, TState>(automaton.StateComparer, automaton.StateComparer);
        this.table = new(tupleComparer);
        this.trapTable = new(automaton.StateComparer);
        this.states = automaton.States.ToList();
    }

    /// <summary>
    /// Checks, if 2 states are differentiated by the table.
    /// </summary>
    /// <param name="s1">The first state.</param>
    /// <param name="s2">The second state.</param>
    /// <returns>True, if <paramref name="s1"/> and <paramref name="s2"/> are considered different.</returns>
    public bool AreDifferent(TState s1, TState s2) => this.table.Contains((s1, s2));

    /// <summary>
    /// Checks, if a state is differentiated from the trap state.
    /// </summary>
    /// <param name="s">The state.</param>
    /// <returns>True, if <paramref name="s"/> and the implicit trap state are considered different.</returns>
    public bool IsDifferentFromTrap(TState s) => this.trapTable.Contains(s);

    /// <summary>
    /// Plots the initial differentiated states into the equivalence table.
    /// </summary>
    /// <param name="differentiatePairs">The extra pair of states to differentiate initially.</param>
    public void Initialize(IEnumerable<(TState, TState)> differentiatePairs)
    {
        // First, all (accepting, non-accepting) pairs get plotted in the table
        for (var i = 0; i < this.states.Count; ++i)
        {
            var s1 = this.states[i];
            var s1accepting = this.automaton.AcceptingStates.Contains(s1);
            for (var j = 0; j < i; ++j)
            {
                var s2 = this.states[j];
                var s2accepting = this.automaton.AcceptingStates.Contains(s2);
                if (s1accepting != s2accepting) this.Plot(s1, s2);
            }
        }

        // Plot trap vs accepting
        foreach (var s in this.automaton.AcceptingStates) this.PlotTrap(s);

        // Then we plot the custom pairs too
        foreach (var (s1, s2) in differentiatePairs) this.Plot(s1, s2);
    }

    /// <summary>
    /// Fills the equivalence table.
    /// </summary>
    /// <param name="differentStates">A function, that determines if the two states should be considered different.
    /// It should return true on different states.</param>
    /// <param name="differentTrap">A function, that determines if a state should be considered different from the trap state.
    /// It should return true if the state is different from the trap.</param>
    public void Fill(Func<TState, TState, bool> differentStates, Func<TState, bool> differentTrap)
    {
        while (true)
        {
            var changed = false;
            for (var i = 0; i < this.states.Count; ++i)
            {
                var s1 = this.states[i];
                // Compare to other states
                for (var j = 0; j < i; ++j)
                {
                    var s2 = this.states[j];

                    if (this.AreDifferent(s1, s2)) continue;
                    if (differentStates(s1, s2))
                    {
                        this.Plot(s1, s2);
                        changed = true;
                    }
                }
                // Compare to trap
                if (!this.IsDifferentFromTrap(s1) && differentTrap(s1))
                {
                    this.PlotTrap(s1);
                    changed = true;
                }
            }
            if (!changed) break;
        }
    }

    /// <summary>
    /// Builds a state mapping from the equivalence table.
    /// </summary>
    /// <typeparam name="TResultState">The result state type.</typeparam>
    /// <param name="combiner">The state combiner to use.</param>
    /// <returns>The mapping from the old states to the new states.</returns>
    public Dictionary<TState, TResultState> BuildStateMap<TResultState>(IStateCombiner<TState, TResultState> combiner)
    {
        var stateMap = new Dictionary<TState, TResultState>(this.automaton.StateComparer);
        for (var i = 0; i < this.states.Count; ++i)
        {
            var s1 = this.states[i];
            var equivalentSet = new HashSet<TState>(this.automaton.StateComparer) { s1 };
            for (var j = 0; j < this.states.Count; ++j)
            {
                if (i == j) continue;
                var s2 = this.states[j];
                if (!this.AreDifferent(s1, s2)) equivalentSet.Add(s2);
            }
            stateMap.Add(s1, combiner.Combine(equivalentSet));
        }
        return stateMap;
    }

    private void Plot(TState s1, TState s2)
    {
        this.table.Add((s1, s2));
        this.table.Add((s2, s1));
    }

    private void PlotTrap(TState s) => this.trapTable.Add(s);
}
