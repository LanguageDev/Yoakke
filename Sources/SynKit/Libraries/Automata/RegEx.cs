// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Factory methods and utilities for regex AST.
/// </summary>
public static class RegEx
{
    /// <summary>
    /// Creates alternatives from the given regex nodes.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="alts">The elements to turn into alternatives.</param>
    /// <returns>A regex that has <paramref name="alts"/> as alternatives.</returns>
    public static RegExNode<TSymbol> Alternation<TSymbol>(params RegExNode<TSymbol>[] alts) =>
        Alternation(alts.AsEnumerable());

    /// <summary>
    /// Creates alternatives from the given regex nodes.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="alts">The elements to turn into alternatives.</param>
    /// <returns>A regex that has <paramref name="alts"/> as alternatives.</returns>
    public static RegExNode<TSymbol> Alternation<TSymbol>(IEnumerable<RegExNode<TSymbol>> alts) =>
        ConstructFolded(alts, (x, y) => new RegExNode<TSymbol>.Alt(x, y));

    /// <summary>
    /// Creates a sequence from the given regex nodes.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="elements">The constructs to put in the sequence.</param>
    /// <returns>A regex that has <paramref name="elements"/> in sequence.</returns>
    public static RegExNode<TSymbol> Sequence<TSymbol>(params RegExNode<TSymbol>[] elements) =>
        Sequence(elements.AsEnumerable());

    /// <summary>
    /// Creates a sequence from the given regex nodes.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="elements">The constructs to put in the sequence.</param>
    /// <returns>A regex that has <paramref name="elements"/> in sequence.</returns>
    public static RegExNode<TSymbol> Sequence<TSymbol>(IEnumerable<RegExNode<TSymbol>> elements) =>
        ConstructFolded(elements, (x, y) => new RegExNode<TSymbol>.Seq(x, y));

    /// <summary>
    /// Creates a repetition node from the given construct that repeats 0-or-more times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The construct to repeat.</param>
    /// <returns>A regex that repeats <paramref name="element"/> 0 or more times.</returns>
    public static RegExNode<TSymbol> Repeat0<TSymbol>(RegExNode<TSymbol> element) =>
        new RegExNode<TSymbol>.Rep(element);

    /// <summary>
    /// Creates an optional from a given construct.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The construct to make optional.</param>
    /// <returns>A regex that makes <paramref name="element"/> optional.</returns>
    public static RegExNode<TSymbol> Option<TSymbol>(RegExNode<TSymbol> element) =>
        AtMost(element, 1);

    /// <summary>
    /// Creates a repetition node from the given construct that repeats 1-or-more times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The construct to repeat.</param>
    /// <returns>A regex that repeats <paramref name="element"/> 1 or more times.</returns>
    public static RegExNode<TSymbol> Repeat1<TSymbol>(RegExNode<TSymbol> element) =>
        Sequence(element, Repeat0(element));

    /// <summary>
    /// Creates a repetition node from the given construct that repeats exactly the given amount of times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The construct to repeat.</param>
    /// <param name="n">The amount to repeat.</param>
    /// <returns>A regex that repeats <paramref name="element"/> exactly <paramref name="n"/> times.</returns>
    public static RegExNode<TSymbol> Exactly<TSymbol>(RegExNode<TSymbol> element, int n)
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
    public static RegExNode<TSymbol> AtLeast<TSymbol>(RegExNode<TSymbol> element, int n) =>
        Sequence(Exactly(element, n), Repeat0(element));

    /// <summary>
    /// Creates a repetition node from the given construct that repeats at most the given amount of times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The construct to repeat.</param>
    /// <param name="n">The amount to repeat at most.</param>
    /// <returns>A regex that repeats <paramref name="element"/> at most <paramref name="n"/> times.</returns>
    public static RegExNode<TSymbol> AtMost<TSymbol>(RegExNode<TSymbol> element, int n) =>
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
    public static RegExNode<TSymbol> Between<TSymbol>(RegExNode<TSymbol> element, int from, int to)
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
    public static RegExNode<TSymbol> Empty<TSymbol>() => RegExNode<TSymbol>.Eps.Instance;

    /// <summary>
    /// Creates a node that represents a single literal.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="value">The literal to match.</param>
    /// <returns>A construct that matches <paramref name="value"/>.</returns>
    public static RegExNode<TSymbol> Literal<TSymbol>(TSymbol value) => new RegExNode<TSymbol>.Lit(value);

    /// <summary>
    /// Creates a node that represents a range of literals.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="invert">True, if the ranges should be inverted.</param>
    /// <param name="intervals">The literal intervals.</param>
    /// <returns>A construct that matches any of the values covered by <paramref name="intervals"/> if
    /// <paramref name="invert"/> is false, othervise it matches any value not covered by
    /// <paramref name="intervals"/>.</returns>
    public static RegExNode<TSymbol> Range<TSymbol>(
        bool invert,
        IEnumerable<Interval<TSymbol>> intervals) => new RegExNode<TSymbol>.Range(invert, intervals);

    private static RegExNode<TSymbol> ConstructFolded<TSymbol>(
        IEnumerable<RegExNode<TSymbol>> elements,
        Func<RegExNode<TSymbol>, RegExNode<TSymbol>, RegExNode<TSymbol>> folder)
    {
        var enumerator = elements.GetEnumerator();
        if (!enumerator.MoveNext()) return Empty<TSymbol>();
        var first = enumerator.Current;
        while (enumerator.MoveNext()) first = folder(first, enumerator.Current);
        return first;
    }
}
