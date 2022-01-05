// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.SynKit.Automata.Dense;

namespace Yoakke.SynKit.Automata.RegExAst;

/// <summary>
/// Represents a regular expression AST node.
/// </summary>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public interface IRegExNode<TSymbol>
{
    /// <summary>
    /// Desugars this node into simpler constructs.
    /// </summary>
    /// <returns>The equivalent, possibly simpler regex node.</returns>
    public IRegExNode<TSymbol> Desugar();

    /// <summary>
    /// Constructs this node inside the given NFA. This can throw a <see cref="NotSupportedException"/>, in case this
    /// is preferred to be desugared into simpler elements with <see cref="Desugar"/>.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <param name="nfa">The NFA to construct into.</param>
    /// <param name="makeState">A function to create a unique state.</param>
    /// <returns>The starting and ending states of the built construct.</returns>
    public (TState Start, TState End) ThompsonsConstruct<TState>(INfa<TState, TSymbol> nfa, Func<TState> makeState);
}
