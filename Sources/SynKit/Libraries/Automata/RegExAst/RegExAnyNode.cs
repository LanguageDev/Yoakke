// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.SynKit.Automata.Dense;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata.RegExAst;

/// <summary>
/// A node matching any character.
/// </summary>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public record RegExAnyNode<TSymbol> : IRegExNode<TSymbol>
{
    /// <summary>
    /// The instance to use.
    /// </summary>
    public static RegExAnyNode<TSymbol> Instance { get; } = new();

    private RegExAnyNode()
    {
    }

    /// <inheritdoc/>
    public IRegExNode<TSymbol> Desugar() => this;

    /// <inheritdoc/>
    public (TState Start, TState End) ThompsonsConstruct<TState>(INfa<TState, TSymbol> nfa, Func<TState> makeState)
    {
        if (nfa is not IDenseNfa<TState, TSymbol> denseNfa) throw new NotSupportedException("Only dense NFAs support wildcards.");

        var start = makeState();
        var end = makeState();

        denseNfa.AddTransition(start, Interval<TSymbol>.Full, end);

        return (start, end);
    }
}
