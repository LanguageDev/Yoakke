// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Extension functions related to automata.
/// </summary>
public static class AutomatonExtensions
{
    /// <summary>
    /// Adds a transition to the transition collection.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="transitions">The collection of transitions to add to.</param>
    /// <param name="from">The sourc state.</param>
    /// <param name="on">The symbol to transition on.</param>
    /// <param name="to">The destination state.</param>
    public static void Add<TState, TSymbol>(
        this ICollection<Transition<TState, Interval<TSymbol>>> transitions,
        TState from,
        TSymbol on,
        TState to) => transitions.Add(from, Interval.Singleton(on), to);

    /// <summary>
    /// Adds a transition to the transition collection.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="transitions">The collection of transitions to add to.</param>
    /// <param name="from">The sourc state.</param>
    /// <param name="on">The interval of symbols to transition on.</param>
    /// <param name="to">The destination state.</param>
    public static void Add<TState, TSymbol>(
        this ICollection<Transition<TState, Interval<TSymbol>>> transitions,
        TState from,
        Interval<TSymbol> on,
        TState to) => transitions.Add(new(from, on, to));

    /// <summary>
    /// Adds an epsilon-transition to the epsilon-transition collection.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="transitions">The collection of epsilon transitions to add to.</param>
    /// <param name="from">The sourc state.</param>
    /// <param name="to">The destination state.</param>
    public static void Add<TState, TSymbol>(
        this ICollection<EpsilonTransition<TState>> transitions,
        TState from,
        TState to) => transitions.Add(new(from, to));
}
