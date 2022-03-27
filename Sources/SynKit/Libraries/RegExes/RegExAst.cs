// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.SynKit.Automata;

namespace Yoakke.SynKit.RegExes;

/// <summary>
/// Represents the most basic regex AST imaginable. All other regexes desugar to this.
/// </summary>
/// <typeparam name="TSymbol">The regex symbol type.</typeparam>
public abstract record class RegExAst<TSymbol>
{
    /// <summary>
    /// Constructs this node inside the given NFA.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <param name="nfa">The NFA to construct into.</param>
    /// <param name="stateCreator">A factory that can create unique states.</param>
    /// <returns>The starting and ending states of the built construct.</returns>
    public abstract (TState Start, TState End) ThompsonsConstruct<TState>(
        Nfa<TState, TSymbol> nfa,
        IStateCreator<TState> stateCreator);
}
