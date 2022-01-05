// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.Text.Metrics;

/// <summary>
/// Interface for any string distance metric.
/// </summary>
public interface IStringMetric
{
    /// <summary>
    /// Calculates the distance (inverse similarity) between two strings.
    /// </summary>
    /// <param name="str1">The first string to compare.</param>
    /// <param name="str2">The second string to compare.</param>
    /// <returns>A number describing how dissimilar the two strings are. 0 means that the two strings are
    /// identical, bigger numbers mean bigger difference.</returns>
    public int Distance(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2);
}
