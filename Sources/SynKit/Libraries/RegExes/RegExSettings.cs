// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.RegExes;

/// <summary>
/// Settings for regexes.
/// </summary>
public sealed class RegExSettings
{
    /// <summary>
    /// The default settings.
    /// </summary>
    public static RegExSettings Default { get; } = new();

    /// <summary>
    /// The meta-sequences meanings.
    /// </summary>
    public IDictionary<object, RegExAst<char>> MetaSequences { get; init; } = new Dictionary<object, RegExAst<char>>
    {
        // By default '.' consumes any single character
        { ".", RegExAst.LiteralRange(true, Enumerable.Empty<Interval<char>>()) },
    };

    /// <summary>
    /// The POSIX named character classes.
    /// </summary>
    public IDictionary<string, IReadOnlyList<Interval<char>>> NamedCharacterClasses { get; init; } = new Dictionary<string, IReadOnlyList<Interval<char>>>
    {
        { "xdigit", new[] { Interval.Inclusive('0', '9'), Interval.Inclusive('a', 'f'), Interval.Inclusive('A', 'F') } },
    };
}
