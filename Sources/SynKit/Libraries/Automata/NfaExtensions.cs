// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.SynKit.Automata.Dense;
using Yoakke.SynKit.Automata.Sparse;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Extensions for NFAs.
/// </summary>
public static class NfaExtensions
{
    /// <summary>
    /// Gets all transitions that are valid from a set of states on an input.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="nfa">The NFA to get the transitions for.</param>
    /// <param name="from">The set of states to transition from.</param>
    /// <param name="on">The symbol to transition on.</param>
    /// <returns>The set of states that are valid from the starting set on the input.</returns>
    public static StateSet<TState> GetTransitions<TState, TSymbol>(
        this IReadOnlyNfa<TState, TSymbol> nfa,
        StateSet<TState> from,
        TSymbol on) => new(from.SelectMany(s => nfa.GetTransitions(s, on)), nfa.StateComparer);

    /// <summary>
    /// Constructs an equivalent DFA from this NFA.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="nfa">The NFA to determinize.</param>
    /// <returns>The constructed DFA.</returns>
    public static IDfa<StateSet<TState>, TSymbol> Determinize<TState, TSymbol>(this IReadOnlyNfa<TState, TSymbol> nfa) =>
        nfa.Determinize(StateCombiner<TState>.ToSetCombiner(nfa.StateComparer));

    /// <summary>
    /// Constructs an equivalent DFA from this NFA.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="nfa">The NFA to determinize.</param>
    /// <returns>The constructed DFA.</returns>
    public static ISparseDfa<StateSet<TState>, TSymbol> Determinize<TState, TSymbol>(this IReadOnlySparseNfa<TState, TSymbol> nfa) =>
        nfa.Determinize(StateCombiner<TState>.ToSetCombiner(nfa.StateComparer));

    /// <summary>
    /// Constructs an equivalent DFA from this NFA.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="nfa">The NFA to determinize.</param>
    /// <returns>The constructed DFA.</returns>
    public static IDenseDfa<StateSet<TState>, TSymbol> Determinize<TState, TSymbol>(this IReadOnlyDenseNfa<TState, TSymbol> nfa) =>
        nfa.Determinize(StateCombiner<TState>.ToSetCombiner(nfa.StateComparer));
}
