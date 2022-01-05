// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.SynKit.Automata.Dense;
using Yoakke.Collections.Dense;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata.RegExAst;

/// <summary>
/// A literal range match node.
/// </summary>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public record RegExRangeNode<TSymbol>(bool Invert, IReadOnlyCollection<Interval<TSymbol>> Ranges) : IRegExNode<TSymbol>
{
    /// <inheritdoc/>
    public IRegExNode<TSymbol> Desugar() => this;

    /// <inheritdoc/>
    public (TState Start, TState End) ThompsonsConstruct<TState>(INfa<TState, TSymbol> nfa, Func<TState> makeState)
    {
        if (nfa is not IDenseNfa<TState, TSymbol> denseNfa) throw new NotSupportedException("Only dense NFAs support literal ranges.");

        // Construct the set
        var set = new DenseSet<TSymbol>(denseNfa.SymbolIntervalComparer);
        foreach (var iv in this.Ranges) set.Add(iv);

        // Invert, if needed
        if (this.Invert) set.Complement();

        // Add transitions
        var start = makeState();
        var end = makeState();

        foreach (var iv in set) denseNfa.AddTransition(start, iv, end);

        return (start, end);
    }
}
