// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Base class for regex constructs.
/// </summary>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public abstract record class RegExNode<TSymbol>
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

    /// <summary>
    /// Represents the alternation of two regex constructs.
    /// </summary>
    /// <param name="First">The first alternative.</param>
    /// <param name="Second">The second alternative.</param>
    public sealed record class Alt(RegExNode<TSymbol> First, RegExNode<TSymbol> Second) : RegExNode<TSymbol>
    {
        /// <inheritdoc/>
        public override (TState Start, TState End) ThompsonsConstruct<TState>(
            Nfa<TState, TSymbol> nfa,
            IStateCreator<TState> stateCreator)
        {
            var start = stateCreator.Create();
            var end = stateCreator.Create();

            var (firstStart, firstEnd) = this.First.ThompsonsConstruct(nfa, stateCreator);
            var (secondStart, secondEnd) = this.Second.ThompsonsConstruct(nfa, stateCreator);

            nfa.EpsilonTransitions.Add(start, firstStart);
            nfa.EpsilonTransitions.Add(start, secondStart);

            nfa.EpsilonTransitions.Add(firstEnd, end);
            nfa.EpsilonTransitions.Add(secondEnd, end);

            return (start, end);
        }
    }

    /// <summary>
    /// Represents the sequence of two regex constructs.
    /// </summary>
    /// <param name="First">The first in the sequence.</param>
    /// <param name="Second">The second in the sequence.</param>
    public sealed record class Seq(RegExNode<TSymbol> First, RegExNode<TSymbol> Second) : RegExNode<TSymbol>
    {
        /// <inheritdoc/>
        public override (TState Start, TState End) ThompsonsConstruct<TState>(
            Nfa<TState, TSymbol> nfa,
            IStateCreator<TState> stateCreator)
        {
            var (firstStart, firstEnd) = this.First.ThompsonsConstruct(nfa, stateCreator);
            var (secondStart, secondEnd) = this.Second.ThompsonsConstruct(nfa, stateCreator);

            nfa.EpsilonTransitions.Add(firstEnd, secondStart);

            return (firstStart, secondEnd);
        }
    }

    /// <summary>
    /// Represents the 0-or-more repetition of a regex construct.
    /// </summary>
    /// <param name="Element">The repeated element.</param>
    public sealed record class Rep(RegExNode<TSymbol> Element) : RegExNode<TSymbol>
    {
        /// <inheritdoc/>
        public override (TState Start, TState End) ThompsonsConstruct<TState>(
            Nfa<TState, TSymbol> nfa,
            IStateCreator<TState> stateCreator)
        {
            var start = stateCreator.Create();
            var end = stateCreator.Create();

            var (elementStart, elementEnd) = this.Element.ThompsonsConstruct(nfa, stateCreator);

            nfa.EpsilonTransitions.Add(start, end);
            nfa.EpsilonTransitions.Add(start, elementStart);
            nfa.EpsilonTransitions.Add(elementEnd, end);
            nfa.EpsilonTransitions.Add(elementEnd, elementStart);

            return (start, end);
        }
    }

    /// <summary>
    /// Represents the empty symbol.
    /// </summary>
    public sealed record class Eps : RegExNode<TSymbol>
    {
        /// <summary>
        /// A default instance to use.
        /// </summary>
        public static Eps Instance { get; } = new();

        /// <inheritdoc/>
        public override (TState Start, TState End) ThompsonsConstruct<TState>(
            Nfa<TState, TSymbol> nfa,
            IStateCreator<TState> stateCreator)
        {
            var start = stateCreator.Create();
            nfa.States.Add(start);
            return (start, start);
        }
    }

    /// <summary>
    /// Represents a literal symbol.
    /// </summary>
    /// <param name="Symbol">The symbol.</param>
    public sealed record class Lit(TSymbol Symbol) : RegExNode<TSymbol>
    {
        /// <inheritdoc/>
        public override (TState Start, TState End) ThompsonsConstruct<TState>(
            Nfa<TState, TSymbol> nfa,
            IStateCreator<TState> stateCreator)
        {
            var start = stateCreator.Create();
            var end = stateCreator.Create();

            nfa.Transitions.Add(start, this.Symbol, end);

            return (start, end);
        }
    }

    /// <summary>
    /// Represents a literal symbol range.
    /// </summary>
    /// <param name="Invert">True, if the ranges should be inverted.</param>
    /// <param name="Intervals">The intervals covering the included symbols.</param>
    public sealed record class Range(bool Invert, IEnumerable<Interval<TSymbol>> Intervals) : RegExNode<TSymbol>
    {
        /// <inheritdoc/>
        public override (TState Start, TState End) ThompsonsConstruct<TState>(
            Nfa<TState, TSymbol> nfa,
            IStateCreator<TState> stateCreator)
        {
            // Construct the set
            var set = new IntervalSet<TSymbol>(nfa.SymbolIntervalComparer);
            foreach (var iv in this.Intervals) set.Add(iv);

            // Invert, if needed
            if (this.Invert) set.Complement();

            // Add transitions
            var start = stateCreator.Create();
            var end = stateCreator.Create();

            foreach (var iv in set) nfa.Transitions.Add(start, iv, end);

            return (start, end);
        }
    }
}
