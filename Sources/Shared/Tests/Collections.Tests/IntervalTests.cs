// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Xunit;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Tests;

public sealed class IntervalTests
{
    [InlineData("(-oo; +oo)", 0)]
    [InlineData("(-oo; +oo)", -10)]
    [InlineData("(-oo; +oo)", 10)]
    [InlineData("[0; 10]", 0)]
    [InlineData("[0; 10]", 10)]
    [InlineData("[0; 10)", 0)]
    [InlineData("[0; 10)", 9)]
    [InlineData("(0; 10]", 1)]
    [InlineData("(0; 10]", 10)]
    [InlineData("(0; 10)", 1)]
    [InlineData("(0; 10)", 9)]
    [InlineData("[0; 0]", 0)]
    [Theory]
    public void Contains(string intervalText, int value)
    {
        var interval = ParseInterval(intervalText);
        Assert.True(interval.Contains(value));
    }

    [InlineData("(0; 0)", 0)]
    [InlineData("(0; 0)", -10)]
    [InlineData("(0; 0)", 10)]
    [InlineData("[0; 10]", -1)]
    [InlineData("[0; 10]", 11)]
    [InlineData("[0; 10)", -1)]
    [InlineData("[0; 10)", 10)]
    [InlineData("(0; 10]", 0)]
    [InlineData("(0; 10]", 11)]
    [InlineData("(0; 10)", 0)]
    [InlineData("(0; 10)", 10)]
    [InlineData("[0; 0]", -1)]
    [InlineData("[0; 0]", 1)]
    [Theory]
    public void DoesNotContain(string intervalText, int value)
    {
        var interval = ParseInterval(intervalText);
        Assert.False(interval.Contains(value));
    }

    internal static Interval<int> ParseInterval(string text) => ParseInterval(text, int.Parse);

    internal static Interval<T> ParseInterval<T>(string text, Func<string, T> parser)
    {
        var parts = text.Split("; ");
        var loStr = parts[0].Trim();
        var hiStr = parts[1].Trim();

        var lo = loStr == "(-oo" ? LowerEndpoint.Unbounded<T>()
               : loStr[0] == '[' ? LowerEndpoint.Inclusive(parser(loStr[1..]))
               : LowerEndpoint.Exclusive(parser(loStr[1..]));
        var hi = hiStr == "+oo)" ? UpperEndpoint.Unbounded<T>()
               : hiStr[^1] == ']' ? UpperEndpoint.Inclusive(parser(hiStr[..^1]))
               : UpperEndpoint.Exclusive(parser(hiStr[..^1]));
        return new(lo, hi);
    }
}
