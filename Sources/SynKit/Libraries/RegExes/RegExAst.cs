// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Collections.Intervals;
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

    /// <summary>
    /// Represents the empty symbol.
    /// </summary>
    public sealed record class Epsilon : RegExAst<TSymbol>
    {
        /// <summary>
        /// A default instance to use.
        /// </summary>
        public static Epsilon Instance { get; } = new();

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
    /// Represents the alternation of two regex constructs.
    /// </summary>
    /// <param name="First">The first alternative.</param>
    /// <param name="Second">The second alternative.</param>
    public sealed record class Alternation(RegExAst<TSymbol> First, RegExAst<TSymbol> Second) : RegExAst<TSymbol>
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
    public sealed record class Sequence(RegExAst<TSymbol> First, RegExAst<TSymbol> Second) : RegExAst<TSymbol>
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
    public sealed record class Repetition(RegExAst<TSymbol> Element) : RegExAst<TSymbol>
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
    /// Represents a literal symbol.
    /// </summary>
    /// <param name="Symbol">The symbol.</param>
    public sealed record class Literal(TSymbol Symbol) : RegExAst<TSymbol>
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
    public sealed record class LiteralRange(bool Invert, IEnumerable<Interval<TSymbol>> Intervals) : RegExAst<TSymbol>
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

/// <summary>
/// Factory functions for regexes.
/// </summary>
public static class RegExAst
{
    /// <summary>
    /// Creates alternatives from the given regex nodes.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="alts">The elements to turn into alternatives.</param>
    /// <returns>A regex that has <paramref name="alts"/> as alternatives.</returns>
    public static RegExAst<TSymbol> Alternation<TSymbol>(params RegExAst<TSymbol>[] alts) =>
        Alternation(alts.AsEnumerable());

    /// <summary>
    /// Creates alternatives from the given regex nodes.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="alts">The elements to turn into alternatives.</param>
    /// <returns>A regex that has <paramref name="alts"/> as alternatives.</returns>
    public static RegExAst<TSymbol> Alternation<TSymbol>(IEnumerable<RegExAst<TSymbol>> alts) =>
        ConstructFolded(alts, (x, y) => new RegExAst<TSymbol>.Alternation(x, y));

    /// <summary>
    /// Creates a sequence from the given regex nodes.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="elements">The constructs to put in the sequence.</param>
    /// <returns>A regex that has <paramref name="elements"/> in sequence.</returns>
    public static RegExAst<TSymbol> Sequence<TSymbol>(params RegExAst<TSymbol>[] elements) =>
        Sequence(elements.AsEnumerable());

    /// <summary>
    /// Creates a sequence from the given regex nodes.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="elements">The constructs to put in the sequence.</param>
    /// <returns>A regex that has <paramref name="elements"/> in sequence.</returns>
    public static RegExAst<TSymbol> Sequence<TSymbol>(IEnumerable<RegExAst<TSymbol>> elements) =>
        ConstructFolded(elements, (x, y) => new RegExAst<TSymbol>.Sequence(x, y));

    /// <summary>
    /// Creates a repetition node from the given construct that repeats 0-or-more times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The construct to repeat.</param>
    /// <returns>A regex that repeats <paramref name="element"/> 0 or more times.</returns>
    public static RegExAst<TSymbol> Repeat0<TSymbol>(RegExAst<TSymbol> element) =>
        new RegExAst<TSymbol>.Repetition(element);

    /// <summary>
    /// Creates an optional from a given construct.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The construct to make optional.</param>
    /// <returns>A regex that makes <paramref name="element"/> optional.</returns>
    public static RegExAst<TSymbol> Option<TSymbol>(RegExAst<TSymbol> element) =>
        AtMost(element, 1);

    /// <summary>
    /// Creates a repetition node from the given construct that repeats 1-or-more times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The construct to repeat.</param>
    /// <returns>A regex that repeats <paramref name="element"/> 1 or more times.</returns>
    public static RegExAst<TSymbol> Repeat1<TSymbol>(RegExAst<TSymbol> element) =>
        Sequence(element, Repeat0(element));

    /// <summary>
    /// Creates a repetition node from the given construct that repeats exactly the given amount of times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The construct to repeat.</param>
    /// <param name="n">The amount to repeat.</param>
    /// <returns>A regex that repeats <paramref name="element"/> exactly <paramref name="n"/> times.</returns>
    public static RegExAst<TSymbol> Exactly<TSymbol>(RegExAst<TSymbol> element, int n)
    {
        if (n < 0) throw new ArgumentOutOfRangeException(nameof(n));
        if (n == 0) return Empty<TSymbol>();
        var result = element;
        for (var i = 1; i < n; ++i) result = Sequence(result, element);
        return result;
    }

    /// <summary>
    /// Creates a repetition node from the given construct that repeats at least the given amount of times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The construct to repeat.</param>
    /// <param name="n">The amount to repeat at least.</param>
    /// <returns>A regex that repeats <paramref name="element"/> at least <paramref name="n"/> times.</returns>
    public static RegExAst<TSymbol> AtLeast<TSymbol>(RegExAst<TSymbol> element, int n) =>
        Sequence(Exactly(element, n), Repeat0(element));

    /// <summary>
    /// Creates a repetition node from the given construct that repeats at most the given amount of times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The construct to repeat.</param>
    /// <param name="n">The amount to repeat at most.</param>
    /// <returns>A regex that repeats <paramref name="element"/> at most <paramref name="n"/> times.</returns>
    public static RegExAst<TSymbol> AtMost<TSymbol>(RegExAst<TSymbol> element, int n) =>
        Between(element, 0, n);

    /// <summary>
    /// Creates a repetition node from the given construct that repeats between given amounts of times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The construct to repeat.</param>
    /// <param name="from">The amount to repeat at least.</param>
    /// <param name="to">The amount to repeat at most.</param>
    /// <returns>A regex that repeats <paramref name="element"/> at least <paramref name="from"/>
    /// and at most <paramref name="to"/> times.</returns>
    public static RegExAst<TSymbol> Between<TSymbol>(RegExAst<TSymbol> element, int from, int to)
    {
        if (from < 0) throw new ArgumentOutOfRangeException(nameof(from));
        if (to < from) throw new ArgumentOutOfRangeException(nameof(to));

        var result = Exactly(element, from);
        for (var i = from + 1; i <= to; ++i) result = Alternation(result, Exactly(element, i));
        return result;
    }

    /// <summary>
    /// Creates a node that represents an empty word.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <returns>A construct that matches the emtpy word.</returns>
    public static RegExAst<TSymbol> Empty<TSymbol>() => RegExAst<TSymbol>.Epsilon.Instance;

    /// <summary>
    /// Creates a node that represents a single literal.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="value">The literal to match.</param>
    /// <returns>A construct that matches <paramref name="value"/>.</returns>
    public static RegExAst<TSymbol> Literal<TSymbol>(TSymbol value) => new RegExAst<TSymbol>.Literal(value);

    /// <summary>
    /// Creates a node that represents a range of literals.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="invert">True, if the ranges should be inverted.</param>
    /// <param name="intervals">The literal intervals.</param>
    /// <returns>A construct that matches any of the values covered by <paramref name="intervals"/> if
    /// <paramref name="invert"/> is false, othervise it matches any value not covered by
    /// <paramref name="intervals"/>.</returns>
    public static RegExAst<TSymbol> LiteralRange<TSymbol>(
        bool invert,
        IEnumerable<Interval<TSymbol>> intervals) => new RegExAst<TSymbol>.LiteralRange(invert, intervals);

    private static RegExAst<TSymbol> ConstructFolded<TSymbol>(
        IEnumerable<RegExAst<TSymbol>> elements,
        Func<RegExAst<TSymbol>, RegExAst<TSymbol>, RegExAst<TSymbol>> folder)
    {
        var enumerator = elements.GetEnumerator();
        if (!enumerator.MoveNext()) return Empty<TSymbol>();
        var first = enumerator.Current;
        while (enumerator.MoveNext()) first = folder(first, enumerator.Current);
        return first;
    }
}
