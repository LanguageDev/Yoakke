// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata.RegExAst;

/// <summary>
/// Makes a regex construct repeated 0 or more times.
/// </summary>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public record RegExRep0Node<TSymbol>(IRegExNode<TSymbol> Element) : IRegExNode<TSymbol>
{
    /// <inheritdoc/>
    public IRegExNode<TSymbol> Desugar() => new RegExRep0Node<TSymbol>(this.Element.Desugar());

    /// <inheritdoc/>
    public (TState Start, TState End) ThompsonsConstruct<TState>(INfa<TState, TSymbol> nfa, Func<TState> makeState)
    {
        var start = makeState();
        var end = makeState();

        var (elementStart, elementEnd) = this.Element.ThompsonsConstruct(nfa, makeState);

        nfa.AddEpsilonTransition(start, end);
        nfa.AddEpsilonTransition(start, elementStart);
        nfa.AddEpsilonTransition(elementEnd, end);
        nfa.AddEpsilonTransition(elementEnd, elementStart);

        return (start, end);
    }
}
