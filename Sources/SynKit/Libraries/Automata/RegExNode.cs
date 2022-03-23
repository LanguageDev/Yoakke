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
    /// Represents the alternation of two regex constructs.
    /// </summary>
    /// <param name="First">The first alternative.</param>
    /// <param name="Second">The second alternative.</param>
    public sealed record class Alt(RegExNode<TSymbol> First, RegExNode<TSymbol> Second) : RegExNode<TSymbol>;

    /// <summary>
    /// Represents the sequence of two regex constructs.
    /// </summary>
    /// <param name="First">The first in the sequence.</param>
    /// <param name="Second">The second in the sequence.</param>
    public sealed record class Seq(RegExNode<TSymbol> First, RegExNode<TSymbol> Second) : RegExNode<TSymbol>;

    /// <summary>
    /// Represents the 0-or-more repetition of a regex construct.
    /// </summary>
    /// <param name="Element">The repeated element.</param>
    public sealed record class Rep0(RegExNode<TSymbol> Element) : RegExNode<TSymbol>;

    /// <summary>
    /// Represents the 1-or-more repetition of a regex construct.
    /// </summary>
    /// <param name="Element">The repeated element.</param>
    public sealed record class Rep1(RegExNode<TSymbol> Element) : RegExNode<TSymbol>;

    /// <summary>
    /// Represents an exact amount of repetition for a regex construct.
    /// </summary>
    /// <param name="Element">The repeated element.</param>
    /// <param name="Count">The amount to repeat.</param>
    public sealed record class RepExact(RegExNode<TSymbol> Element, int Count) : RegExNode<TSymbol>;

    /// <summary>
    /// Represents repetition for a regex construct for at least a given amount.
    /// </summary>
    /// <param name="Element">The repeated element.</param>
    /// <param name="Count">The amount to repeat at least.</param>
    public sealed record class RepAtLeast(RegExNode<TSymbol> Element, int Count) : RegExNode<TSymbol>;

    /// <summary>
    /// Represents repetition for a regex construct for at most a given amount.
    /// </summary>
    /// <param name="Element">The repeated element.</param>
    /// <param name="Count">The amount to repeat at most.</param>
    public sealed record class RepAtMost(RegExNode<TSymbol> Element, int Count) : RegExNode<TSymbol>;

    /// <summary>
    /// Represents repetition for a regex construct between 2 amounts.
    /// </summary>
    /// <param name="Element">The repeated element.</param>
    /// <param name="Min">The amount to repeat at least (inclusive).</param>
    /// <param name="Max">The amount to repeat at most (inclusive).</param>
    public sealed record class RepBetween(RegExNode<TSymbol> Element, int Min, int Max) : RegExNode<TSymbol>;

    /// <summary>
    /// Represents the empty symbol.
    /// </summary>
    public sealed record class Eps : RegExNode<TSymbol>
    {
        /// <summary>
        /// A default instance to use.
        /// </summary>
        public static Eps Instance { get; } = new();
    }

    /// <summary>
    /// Represents a literal symbol.
    /// </summary>
    /// <param name="Symbol">The symbol.</param>
    public sealed record class Lit(TSymbol Symbol) : RegExNode<TSymbol>;

    /// <summary>
    /// Represents a literal symbol range.
    /// </summary>
    /// <param name="Invert">True, if the ranges should be inverted.</param>
    /// <param name="Intervals">The intervals covering the included symbols.</param>
    public sealed record class Range(bool Invert, IEnumerable<Interval<TSymbol>> Intervals) : RegExNode<TSymbol>;

    /// <summary>
    /// Represents a character class.
    /// </summary>
    /// <param name="Identifier">The object that identifies the character class.</param>
    public sealed record class Class(object Identifier)
    {
        /// <summary>
        /// The character class usually represented by a dot.
        /// </summary>
        public static Class Dot { get; } = new(".");
    }
}
