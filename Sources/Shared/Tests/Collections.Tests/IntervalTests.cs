// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yoakke.Collections.Intervals;

namespace Yoakke.Collections.Tests;

public sealed class IntervalTests
{
    public static IEnumerable<object[]> ContainsData { get; } = new object[][]
    {
        new object[] { Interval.Full<int>(), 0 },
        new object[] { Interval.Full<int>(), -10 },
        new object[] { Interval.Full<int>(), 10 },
        new object[] { Interval.Of(LowerEndpoint.Inclusive(0), UpperEndpoint.Inclusive(10)), 0 },
        new object[] { Interval.Of(LowerEndpoint.Inclusive(0), UpperEndpoint.Inclusive(10)), 10 },
        new object[] { Interval.Of(LowerEndpoint.Inclusive(0), UpperEndpoint.Exclusive(10)), 0 },
        new object[] { Interval.Of(LowerEndpoint.Inclusive(0), UpperEndpoint.Exclusive(10)), 9 },
        new object[] { Interval.Of(LowerEndpoint.Exclusive(0), UpperEndpoint.Inclusive(10)), 1 },
        new object[] { Interval.Of(LowerEndpoint.Exclusive(0), UpperEndpoint.Inclusive(10)), 10 },
        new object[] { Interval.Of(LowerEndpoint.Exclusive(0), UpperEndpoint.Exclusive(10)), 1 },
        new object[] { Interval.Of(LowerEndpoint.Exclusive(0), UpperEndpoint.Exclusive(10)), 9 },
        new object[] { Interval.Singleton(0), 0 },
    };

    [MemberData(nameof(ContainsData))]
    [Theory]
    public void Contains(Interval<int> interval, int value)
    {
        Assert.True(interval.Contains(value));
    }

    public static IEnumerable<object[]> DoesNotContainData { get; } = new object[][]
    {
        new object[] { Interval.Empty<int>(), 0 },
        new object[] { Interval.Empty<int>(), -10 },
        new object[] { Interval.Empty<int>(), 10 },
        new object[] { Interval.Of(LowerEndpoint.Inclusive(0), UpperEndpoint.Inclusive(10)), -1 },
        new object[] { Interval.Of(LowerEndpoint.Inclusive(0), UpperEndpoint.Inclusive(10)), 11 },
        new object[] { Interval.Of(LowerEndpoint.Inclusive(0), UpperEndpoint.Exclusive(10)), -1 },
        new object[] { Interval.Of(LowerEndpoint.Inclusive(0), UpperEndpoint.Exclusive(10)), 10 },
        new object[] { Interval.Of(LowerEndpoint.Exclusive(0), UpperEndpoint.Inclusive(10)), 0 },
        new object[] { Interval.Of(LowerEndpoint.Exclusive(0), UpperEndpoint.Inclusive(10)), 11 },
        new object[] { Interval.Of(LowerEndpoint.Exclusive(0), UpperEndpoint.Exclusive(10)), 0 },
        new object[] { Interval.Of(LowerEndpoint.Exclusive(0), UpperEndpoint.Exclusive(10)), 10 },
        new object[] { Interval.Singleton(0), -1 },
        new object[] { Interval.Singleton(0), 1 },
    };

    [MemberData(nameof(DoesNotContainData))]
    [Theory]
    public void DoesNotContain(Interval<int> interval, int value)
    {
        Assert.False(interval.Contains(value));
    }
}
