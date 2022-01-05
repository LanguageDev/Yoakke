// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata.RegExAst;

/// <summary>
/// Makes a regex construct optional.
/// </summary>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public record RegExOptNode<TSymbol>(IRegExNode<TSymbol> Element) : IRegExNode<TSymbol>
{
    /// <inheritdoc/>
    public IRegExNode<TSymbol> Desugar() => new RegExOptNode<TSymbol>(this.Element.Desugar());

    /// <inheritdoc/>
    public (TState Start, TState End) ThompsonsConstruct<TState>(INfa<TState, TSymbol> nfa, Func<TState> makeState)
    {
        var (start, end) = this.Element.ThompsonsConstruct(nfa, makeState);

        nfa.AddEpsilonTransition(start, end);

        return (start, end);
    }
}
