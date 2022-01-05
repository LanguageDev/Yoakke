// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Intervals;

/// <summary>
/// Factory methods and constants for generic intervals.
/// </summary>
internal static class Interval
{
    /// <summary>
    /// The valid infinity strings without sign.
    /// </summary>
    internal static readonly IReadOnlyList<string> InfinityStrings = new[]
    {
            "∞", "oo", "infinity", "infty", string.Empty,
        };
}
