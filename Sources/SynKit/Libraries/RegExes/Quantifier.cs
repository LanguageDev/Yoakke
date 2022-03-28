// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.RegExes;

/// <summary>
/// A quantifier for regexes.
/// </summary>
public abstract record class Quantifier
{
    /// <summary>
    /// Marks an element as optional.
    /// </summary>
    public sealed record class Optional : Quantifier
    {
        /// <summary>
        /// The default instance to use.
        /// </summary>
        public static Optional Instance { get; } = new();
    }

    /// <summary>
    /// Marks an element for repetition 1 or more times.
    /// </summary>
    public sealed record class OneOrMore : Quantifier
    {
        /// <summary>
        /// The default instance to use.
        /// </summary>
        public static OneOrMore Instance { get; } = new();
    }

    /// <summary>
    /// Marks an element for repetition 0 or more times.
    /// </summary>
    public sealed record class ZeroOrMore : Quantifier
    {
        /// <summary>
        /// The default instance to use.
        /// </summary>
        public static ZeroOrMore Instance { get; } = new();
    }

    /// <summary>
    /// Marks an element for repetition exactly a given amount of times.
    /// </summary>
    /// <param name="Amount">The exact amount of repetition.</param>
    public sealed record class Exactly(int Amount) : Quantifier;

    /// <summary>
    /// Marks an element for repetition at least a given amount of times.
    /// </summary>
    /// <param name="Amount">The minimum amount of repetition.</param>
    public sealed record class AtLeast(int Amount) : Quantifier;

    /// <summary>
    /// Marks an element for repetition at most a given amount of times.
    /// </summary>
    /// <param name="Amount">The maximum amount of repetition.</param>
    public sealed record class AtMost(int Amount) : Quantifier;

    /// <summary>
    /// Marks an element for repetition between a minimum and maximum amount.
    /// </summary>
    /// <param name="Min">The minimum amount to repeat.</param>
    /// <param name="Max">The maximum amount to repeat.</param>
    public sealed record class Between(int Min, int Max) : Quantifier;
}
