// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata.RegExAst;

/// <summary>
/// Merges two alternative regex nodes so either are accepted.
/// </summary>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public record RegExOrNode<TSymbol>(IRegExNode<TSymbol> First, IRegExNode<TSymbol> Second) : IRegExNode<TSymbol>
{
    /// <inheritdoc/>
    public IRegExNode<TSymbol> Desugar() => new RegExOrNode<TSymbol>(this.First.Desugar(), this.Second.Desugar());

    /// <inheritdoc/>
    public (TState Start, TState End) ThompsonsConstruct<TState>(INfa<TState, TSymbol> nfa, Func<TState> makeState)
    {
        var start = makeState();
        var end = makeState();

        var (firstStart, firstEnd) = this.First.ThompsonsConstruct(nfa, makeState);
        var (secondStart, secondEnd) = this.Second.ThompsonsConstruct(nfa, makeState);

        nfa.AddEpsilonTransition(start, firstStart);
        nfa.AddEpsilonTransition(start, secondStart);

        nfa.AddEpsilonTransition(firstEnd, end);
        nfa.AddEpsilonTransition(secondEnd, end);

        return (start, end);
    }
}
