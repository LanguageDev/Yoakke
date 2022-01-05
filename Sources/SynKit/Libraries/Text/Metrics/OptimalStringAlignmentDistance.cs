// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.Text.Metrics;

/// <summary>
/// Implements Optimal String Alignment distance.
/// See: https://en.wikipedia.org/wiki/Damerau%E2%80%93Levenshtein_distance#Optimal_string_alignment_distance.
/// </summary>
public class OptimalStringAlignmentDistance : IStringMetric
{
    /// <summary>
    /// A singleton <see cref="OptimalStringAlignmentDistance"/> instance to use.
    /// </summary>
    public static readonly OptimalStringAlignmentDistance Instance = new();

    /// <inheritdoc/>
    public int Distance(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2)
    {
        var d = new int[str1.Length + 1, str2.Length + 1];

        for (var i = 0; i <= str1.Length; ++i) d[i, 0] = i;
        for (var j = 0; j <= str2.Length; ++j) d[0, j] = j;

        for (var i = 1; i <= str1.Length; ++i)
        {
            for (var j = 1; j <= str2.Length; ++j)
            {
                var deletionCost = d[i - 1, j] + 1;
                var insertionCost = d[i, j - 1] + 1;
                var substitutionCost = d[i - 1, j - 1] + (str1[i - 1] == str2[j - 1] ? 0 : 1);

                d[i, j] = Math.Min(deletionCost, Math.Min(insertionCost, substitutionCost));

                if (i > 1 && j > 1 && str1[i - 1] == str2[j - 2] && str1[i - 2] == str2[j - 1])
                {
                    var transpositionCost = d[i - 2, j - 2] + 1;
                    d[i, j] = Math.Min(d[i, j], transpositionCost);
                }
            }
        }

        return d[str1.Length, str2.Length];
    }
}
