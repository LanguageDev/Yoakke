// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Xunit;

namespace Yoakke.SynKit.Text.Tests;

public class StringMetricsTests
{
    [Theory]
    [InlineData(3, "kitten", "sitting")]
    [InlineData(2, "abcd", "acbd")]
    [InlineData(3, "abc", "def")]
    [InlineData(4, "abcd", "defg")]
    [InlineData(2, "apple", "appel")]
    [InlineData(2, "mug", "gum")]
    [InlineData(3, "mcdonalds", "mcdnoald")]
    [InlineData(5, "table", "desk")]
    [InlineData(3, "abcd", "badc")]
    [InlineData(2, "book", "back")]
    [InlineData(4, "pattern", "parent")]
    [InlineData(4, "abcd", "")]
    [InlineData(0, "abcd", "abcd")]
    [InlineData(4, "abcd", "da")]
    public void LevenshteinDistance(int expextedDistance, string s1, string s2) =>
        Assert.Equal(expextedDistance, Metrics.LevenshteinDistance.Instance.Distance(s1, s2));

    [Theory]
    [InlineData(3, "kitten", "sitting")]
    [InlineData(1, "abcd", "acbd")]
    [InlineData(3, "abc", "def")]
    [InlineData(4, "abcd", "defg")]
    [InlineData(1, "apple", "appel")]
    [InlineData(2, "mug", "gum")]
    [InlineData(2, "mcdonalds", "mcdnoald")]
    [InlineData(5, "table", "desk")]
    [InlineData(2, "abcd", "badc")]
    [InlineData(2, "book", "back")]
    [InlineData(4, "abcd", "")]
    [InlineData(0, "abcd", "abcd")]
    [InlineData(4, "abcd", "da")]
    public void OptimalStringAlignmentDistance(int expextedDistance, string s1, string s2) =>
        Assert.Equal(expextedDistance, Metrics.OptimalStringAlignmentDistance.Instance.Distance(s1, s2));
}
