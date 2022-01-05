// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata.RegExAst;

/// <summary>
/// Makes a regex construct repeated 1 or more times.
/// </summary>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public record RegExRep1Node<TSymbol>(IRegExNode<TSymbol> Element) : IRegExNode<TSymbol>
{
    /// <inheritdoc/>
    public IRegExNode<TSymbol> Desugar()
    {
        var elementDesugared = this.Element.Desugar();
        return new RegExSeqNode<TSymbol>(elementDesugared, new RegExRep0Node<TSymbol>(elementDesugared));
    }

    /// <inheritdoc/>
    public (TState Start, TState End) ThompsonsConstruct<TState>(INfa<TState, TSymbol> nfa, Func<TState> makeState) =>
        throw new NotSupportedException("Element must be desugared.");
}
