// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Automata.RegExAst;

/// <summary>
/// Factory methods for <see cref="IRegExNode{TSymbol}"/>s.
/// </summary>
public static class RegEx
{
    /// <summary>
    /// Constructs a literal match node.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="symbol">The symbol to match.</param>
    /// <returns>The literal node constructed.</returns>
    public static IRegExNode<TSymbol> Lit<TSymbol>(TSymbol symbol) => new RegExLitNode<TSymbol>(symbol);

    /// <summary>
    /// Constructs an any literal match (wildcard) node.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <returns>The literal node constructed.</returns>
    public static IRegExNode<TSymbol> Any<TSymbol>() => RegExAnyNode<TSymbol>.Instance;

    /// <summary>
    /// Constructs a literal range match node.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="negate">True, if the range should be negated.</param>
    /// <param name="ranges">The ranges of literals.</param>
    /// <returns>The literal range node constructed.</returns>
    public static IRegExNode<TSymbol> Range<TSymbol>(bool negate, params Interval<TSymbol>[] ranges) =>
        Range(negate, ranges as IEnumerable<Interval<TSymbol>>);

    /// <summary>
    /// Constructs a literal range match node.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="negate">True, if the range should be negated.</param>
    /// <param name="ranges">The ranges of literals.</param>
    /// <returns>The literal range node constructed.</returns>
    public static IRegExNode<TSymbol> Range<TSymbol>(bool negate, IEnumerable<Interval<TSymbol>> ranges) =>
        new RegExRangeNode<TSymbol>(negate, ranges.ToList());

    /// <summary>
    /// Constructs a literal range match node.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="ranges">The ranges of literals.</param>
    /// <returns>The literal range node constructed.</returns>
    public static IRegExNode<TSymbol> Range<TSymbol>(params Interval<TSymbol>[] ranges) => Range(false, ranges);

    /// <summary>
    /// Constructs a literal range match node.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="ranges">The ranges of literals.</param>
    /// <returns>The literal range node constructed.</returns>
    public static IRegExNode<TSymbol> Range<TSymbol>(IEnumerable<Interval<TSymbol>> ranges) => Range(false, ranges);

    /// <summary>
    /// Constructs a literal range match node.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="ranges">The ranges of literals.</param>
    /// <returns>The literal range node constructed.</returns>
    public static IRegExNode<TSymbol> NotRange<TSymbol>(params Interval<TSymbol>[] ranges) => Range(true, ranges);

    /// <summary>
    /// Constructs a literal range match node.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="ranges">The ranges of literals.</param>
    /// <returns>The literal range node constructed.</returns>
    public static IRegExNode<TSymbol> NotRange<TSymbol>(IEnumerable<Interval<TSymbol>> ranges) => Range(true, ranges);

    /// <summary>
    /// Constructs an alternative of regex constructs.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="first">The first alternative.</param>
    /// <param name="rest">The remaining alternatives.</param>
    /// <returns>The alternative node constructed.</returns>
    public static IRegExNode<TSymbol> Or<TSymbol>(IRegExNode<TSymbol> first, params IRegExNode<TSymbol>[] rest) =>
        Or(first, rest as IEnumerable<IRegExNode<TSymbol>>);

    /// <summary>
    /// Constructs an alternative of regex constructs.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="first">The first alternative.</param>
    /// <param name="rest">The remaining alternatives.</param>
    /// <returns>The alternative node constructed.</returns>
    public static IRegExNode<TSymbol> Or<TSymbol>(IRegExNode<TSymbol> first, IEnumerable<IRegExNode<TSymbol>> rest)
    {
        var result = first;
        foreach (var second in rest) result = new RegExOrNode<TSymbol>(result, second);
        return result;
    }

    /// <summary>
    /// Constructs a sequence of regex constructs.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="first">The first element in the sequence.</param>
    /// <param name="rest">The remaining sequence elements.</param>
    /// <returns>The sequence node constructed.</returns>
    public static IRegExNode<TSymbol> Seq<TSymbol>(IRegExNode<TSymbol> first, params IRegExNode<TSymbol>[] rest) =>
        Seq(first, rest as IEnumerable<IRegExNode<TSymbol>>);

    /// <summary>
    /// Constructs a sequence of regex constructs.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="first">The first element in the sequence.</param>
    /// <param name="rest">The remaining sequence elements.</param>
    /// <returns>The sequence node constructed.</returns>
    public static IRegExNode<TSymbol> Seq<TSymbol>(IRegExNode<TSymbol> first, IEnumerable<IRegExNode<TSymbol>> rest)
    {
        var result = first;
        foreach (var second in rest) result = new RegExSeqNode<TSymbol>(result, second);
        return result;
    }

    /// <summary>
    /// Constructs a 0-or-more repetition node from <paramref name="element"/>.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The element to construct the repetition from.</param>
    /// <returns>The repetition node constructed.</returns>
    public static IRegExNode<TSymbol> Rep0<TSymbol>(IRegExNode<TSymbol> element) => new RegExRep0Node<TSymbol>(element);

    /// <summary>
    /// Constructs a 1-or-more repetition node from <paramref name="element"/>.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The element to construct the repetition from.</param>
    /// <returns>The repetition node constructed.</returns>
    public static IRegExNode<TSymbol> Rep1<TSymbol>(IRegExNode<TSymbol> element) => new RegExRep1Node<TSymbol>(element);

    /// <summary>
    /// Constructs an optional node from <paramref name="element"/>.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="element">The element to construct the optional from.</param>
    /// <returns>The optional node constructed.</returns>
    public static IRegExNode<TSymbol> Opt<TSymbol>(IRegExNode<TSymbol> element) => new RegExOptNode<TSymbol>(element);

    /// <summary>
    /// Constructs a node repeated exactly N times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="n">The number of repetitions.</param>
    /// <param name="element">The element to construct the repetition from.</param>
    /// <returns>The repetition node constructed.</returns>
    public static IRegExNode<TSymbol> Exactly<TSymbol>(int n, IRegExNode<TSymbol> element) => new RegExRepBetweenNode<TSymbol>(n, n, element);

    /// <summary>
    /// Constructs a node repeated at least N times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="n">The minimum number of repetitions.</param>
    /// <param name="element">The element to construct the repetition from.</param>
    /// <returns>The repetition node constructed.</returns>
    public static IRegExNode<TSymbol> AtLeast<TSymbol>(int n, IRegExNode<TSymbol> element) => new RegExRepBetweenNode<TSymbol>(n, null, element);

    /// <summary>
    /// Constructs a node repeated at most N times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="n">The maximum number of repetitions.</param>
    /// <param name="element">The element to construct the repetition from.</param>
    /// <returns>The repetition node constructed.</returns>
    public static IRegExNode<TSymbol> AtMost<TSymbol>(int n, IRegExNode<TSymbol> element) => new RegExRepBetweenNode<TSymbol>(0, n, element);

    /// <summary>
    /// Constructs a node repeated between N and M times.
    /// </summary>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    /// <param name="n">The minimum number of repetitions.</param>
    /// <param name="m">The maximum number of repetitions.</param>
    /// <param name="element">The element to construct the repetition from.</param>
    /// <returns>The repetition node constructed.</returns>
    public static IRegExNode<TSymbol> Between<TSymbol>(int n, int m, IRegExNode<TSymbol> element) => new RegExRepBetweenNode<TSymbol>(n, m, element);
}
