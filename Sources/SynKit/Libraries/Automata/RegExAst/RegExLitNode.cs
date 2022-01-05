// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata.RegExAst;

/// <summary>
/// A literal match regex node.
/// </summary>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public record RegExLitNode<TSymbol>(TSymbol Symbol) : IRegExNode<TSymbol>
{
    /// <inheritdoc/>
    public IRegExNode<TSymbol> Desugar() => this;

    /// <inheritdoc/>
    public (TState Start, TState End) ThompsonsConstruct<TState>(INfa<TState, TSymbol> nfa, Func<TState> makeState)
    {
        var start = makeState();
        var end = makeState();

        nfa.AddTransition(start, this.Symbol, end);

        return (start, end);
    }
}
