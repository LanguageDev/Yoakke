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

public sealed class EndpointTests
{
    public static IEnumerable<object[]> LessThanLowerEndpointData { get; } = new object[][]
    {
        new object[] { LowerEndpoint.Unbounded<int>(), LowerEndpoint.Inclusive(0) },
        new object[] { LowerEndpoint.Unbounded<int>(), LowerEndpoint.Exclusive(0) },
        new object[] { LowerEndpoint.Unbounded<int>(), LowerEndpoint.Inclusive(-100) },
        new object[] { LowerEndpoint.Unbounded<int>(), LowerEndpoint.Exclusive(-100) },
        new object[] { LowerEndpoint.Inclusive(0), LowerEndpoint.Exclusive(1) },
        new object[] { LowerEndpoint.Inclusive(0), LowerEndpoint.Exclusive(0) },
        new object[] { LowerEndpoint.Inclusive(0), LowerEndpoint.Inclusive(1) },
        new object[] { LowerEndpoint.Exclusive(0), LowerEndpoint.Exclusive(1) },
        new object[] { LowerEndpoint.Exclusive(0), LowerEndpoint.Inclusive(1) },
        new object[] { LowerEndpoint.Inclusive(3), LowerEndpoint.Exclusive(7) },
        new object[] { LowerEndpoint.Inclusive(7), LowerEndpoint.Exclusive(7) },
        new object[] { LowerEndpoint.Inclusive(3), LowerEndpoint.Inclusive(7) },
        new object[] { LowerEndpoint.Exclusive(3), LowerEndpoint.Exclusive(7) },
        new object[] { LowerEndpoint.Exclusive(3), LowerEndpoint.Inclusive(7) },
    };

    [MemberData(nameof(LessThanLowerEndpointData))]
    [Theory]
    public void LessThanLowerEndpoint(LowerEndpoint<int> x, LowerEndpoint<int> y)
    {
        Assert.True(x.CompareTo(y) < 0);
        Assert.True(x.CompareTo(y) <= 0);
        Assert.True(x < y);
        Assert.False(x > y);
        Assert.True(x <= y);
        Assert.False(x >= y);
        Assert.True(EndpointComparer<int>.Default.Compare(x, y) < 0);
        Assert.True(EndpointComparer<int>.Default.Compare(x, y) <= 0);

        Assert.True(y.CompareTo(x) > 0);
        Assert.True(y.CompareTo(x) >= 0);
        Assert.True(y > x);
        Assert.True(y >= x);
        Assert.True(EndpointComparer<int>.Default.Compare(y, x) > 0);
        Assert.True(EndpointComparer<int>.Default.Compare(y, x) >= 0);

        Assert.NotEqual(x, y);
        Assert.False(x.Equals(y));
        Assert.False(x == y);
        Assert.True(x != y);
    }

    public static IEnumerable<object[]> LessThanUpperEndpointData { get; } = new object[][]
    {
        new object[] { UpperEndpoint.Inclusive(0), UpperEndpoint.Unbounded<int>() },
        new object[] { UpperEndpoint.Exclusive(0), UpperEndpoint.Unbounded<int>() },
        new object[] { UpperEndpoint.Inclusive(100), UpperEndpoint.Unbounded<int>() },
        new object[] { UpperEndpoint.Exclusive(100), UpperEndpoint.Unbounded<int>() },
        new object[] { UpperEndpoint.Inclusive(0), UpperEndpoint.Exclusive(1) },
        new object[] { UpperEndpoint.Exclusive(0), UpperEndpoint.Inclusive(0) },
        new object[] { UpperEndpoint.Inclusive(0), UpperEndpoint.Inclusive(1) },
        new object[] { UpperEndpoint.Exclusive(0), UpperEndpoint.Exclusive(1) },
        new object[] { UpperEndpoint.Exclusive(0), UpperEndpoint.Inclusive(1) },
        new object[] { UpperEndpoint.Inclusive(3), UpperEndpoint.Exclusive(7) },
        new object[] { UpperEndpoint.Exclusive(7), UpperEndpoint.Inclusive(7) },
        new object[] { UpperEndpoint.Inclusive(3), UpperEndpoint.Inclusive(7) },
        new object[] { UpperEndpoint.Exclusive(3), UpperEndpoint.Exclusive(7) },
        new object[] { UpperEndpoint.Exclusive(3), UpperEndpoint.Inclusive(7) },
    };

    [MemberData(nameof(LessThanUpperEndpointData))]
    [Theory]
    public void LessThanUpperEndpoint(UpperEndpoint<int> x, UpperEndpoint<int> y)
    {
        Assert.True(x.CompareTo(y) < 0);
        Assert.True(x.CompareTo(y) <= 0);
        Assert.True(x < y);
        Assert.False(x > y);
        Assert.True(x <= y);
        Assert.False(x >= y);
        Assert.True(EndpointComparer<int>.Default.Compare(x, y) < 0);
        Assert.True(EndpointComparer<int>.Default.Compare(x, y) <= 0);

        Assert.True(y.CompareTo(x) > 0);
        Assert.True(y.CompareTo(x) >= 0);
        Assert.True(y > x);
        Assert.True(y >= x);
        Assert.True(EndpointComparer<int>.Default.Compare(y, x) > 0);
        Assert.True(EndpointComparer<int>.Default.Compare(y, x) >= 0);

        Assert.NotEqual(x, y);
        Assert.False(x.Equals(y));
        Assert.False(x == y);
        Assert.True(x != y);
    }

    public static IEnumerable<object[]> EqualEndpointsData { get; } = new object[][]
    {
        new object[] { LowerEndpoint.Unbounded<int>(), LowerEndpoint.Unbounded<int>() },
        new object[] { UpperEndpoint.Unbounded<int>(), UpperEndpoint.Unbounded<int>() },
        new object[] { LowerEndpoint.Exclusive(0), LowerEndpoint.Exclusive(0) },
        new object[] { UpperEndpoint.Exclusive(0), UpperEndpoint.Exclusive(0) },
        new object[] { LowerEndpoint.Inclusive(0), LowerEndpoint.Inclusive(0) },
        new object[] { UpperEndpoint.Inclusive(0), UpperEndpoint.Inclusive(0) },
        new object[] { LowerEndpoint.Exclusive(7), LowerEndpoint.Exclusive(7) },
        new object[] { UpperEndpoint.Exclusive(7), UpperEndpoint.Exclusive(7) },
        new object[] { LowerEndpoint.Inclusive(7), LowerEndpoint.Inclusive(7) },
        new object[] { UpperEndpoint.Inclusive(7), UpperEndpoint.Inclusive(7) },
    };

    [MemberData(nameof(EqualEndpointsData))]
    [Theory]
    public void EqualEndpoints(IComparable x, IComparable y)
    {
        Assert.True(x.CompareTo(y) == 0);
        Assert.Equal(x, y);
        Assert.True(x.Equals(y));
        Assert.Equal(x.GetHashCode(), y.GetHashCode());
    }
}
