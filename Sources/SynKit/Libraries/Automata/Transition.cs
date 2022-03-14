// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Represents a generic transition in a state machine.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
/// <param name="Source">The source state.</param>
/// <param name="Symbol">The symbol the transition triggers on.</param>
/// <param name="Destination">The destination state.</param>
public readonly record struct Transition<TState, TSymbol>(TState Source, TSymbol Symbol, TState Destination)
{
    /// <summary>
    /// Converts the transition to a transition with a singleton symbol interval.
    /// </summary>
    /// <param name="t">The transition to convert.</param>
    public static implicit operator Transition<TState, Interval<TSymbol>>(Transition<TState, TSymbol> t) =>
        new(t.Source, Interval.Singleton(t.Symbol), t.Destination);
}
