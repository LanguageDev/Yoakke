// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.Text.Metrics
{
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
        public int Distance(ReadOnlySpan<char> a, ReadOnlySpan<char> b)
        {
            var d = new int[a.Length + 1, b.Length + 1];

            for (var i = 0; i <= a.Length; ++i) d[i, 0] = i;
            for (var j = 0; j <= b.Length; ++j) d[0, j] = j;

            for (var i = 1; i <= a.Length; ++i)
            {
                for (var j = 1; j <= b.Length; ++j)
                {
                    var deletionCost = d[i - 1, j] + 1;
                    var insertionCost = d[i, j - 1] + 1;
                    var substitutionCost = d[i - 1, j - 1] + (a[i - 1] == b[j - 1] ? 0 : 1);

                    d[i, j] = Math.Min(deletionCost, Math.Min(insertionCost, substitutionCost));

                    if (i > 1 && j > 1 && a[i - 1] == b[j - 2] && a[i - 2] == b[j - 1])
                    {
                        var transpositionCost = d[i - 2, j - 2] + 1;
                        d[i, j] = Math.Min(d[i, j], transpositionCost);
                    }
                }
            }

            return d[a.Length, b.Length];
        }
    }
}
