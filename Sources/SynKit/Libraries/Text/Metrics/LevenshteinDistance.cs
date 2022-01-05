// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.Text.Metrics;

/// <summary>
/// Implements Levenshtein distance.
/// See: https://en.wikipedia.org/wiki/Levenshtein_distance.
/// </summary>
public class LevenshteinDistance : IStringMetric
{
    /// <summary>
    /// A singleton <see cref="LevenshteinDistance"/> instance to use.
    /// </summary>
    public static readonly LevenshteinDistance Instance = new();

    /// <inheritdoc/>
    public int Distance(ReadOnlySpan<char> str1, ReadOnlySpan<char> str2)
    {
        var v0 = new int[str2.Length + 1];
        var v1 = new int[str2.Length + 1];

        for (var i = 0; i <= str2.Length; ++i) v0[i] = i;

        for (var i = 0; i < str1.Length; ++i)
        {
            v1[0] = i + 1;

            for (var j = 0; j < str2.Length; ++j)
            {
                var deletionCost = v0[j + 1] + 1;
                var insertionCost = v1[j] + 1;
                var substitutionCost = v0[j] + (str1[i] == str2[j] ? 0 : 1);

                v1[j + 1] = Math.Min(deletionCost, Math.Min(insertionCost, substitutionCost));
            }

            var tmp = v1;
            v1 = v0;
            v0 = tmp;
        }

        return v0[str2.Length];
    }
}
