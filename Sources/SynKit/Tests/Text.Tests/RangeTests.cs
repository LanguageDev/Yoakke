// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Xunit;

namespace Yoakke.SynKit.Text.Tests;

public class RangeTests
{
    public static IReadOnlyList<object[]> InvalidPositionPairsData { get; } = new object[][]
    {
            new object[] { new Position(0, 1), new Position(0, 0) },
            new object[] { new Position(0, 3), new Position(0, 0) },
            new object[] { new Position(0, 3), new Position(0, 2) },
            new object[] { new Position(4, 3), new Position(3, 3) },
            new object[] { new Position(4, 3), new Position(3, 5) },
            new object[] { new Position(4, 3), new Position(4, 1) },
    };

    public static IReadOnlyList<object[]> EqualRangesData { get; } = new object[][]
    {
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(0, 0), new Position(0, 3)) },
            new object[] { new Range(new Position(0, 2), 3), new Range(new Position(0, 2), new Position(0, 5)) },
            new object[] { new Range(new Position(0, 2), 0), new Range(new Position(0, 2), new Position(0, 2)) },
            new object[] { new Range(new Position(1, 3), 5), new Range(new Position(1, 3), new Position(1, 8)) },
    };

    public static IReadOnlyList<object[]> UnequalRangesData { get; } = new object[][]
    {
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(0, 0), new Position(0, 4)) },
            new object[] { new Range(new Position(0, 1), 3), new Range(new Position(0, 2), new Position(0, 5)) },
            new object[] { new Range(new Position(0, 2), 0), new Range(new Position(1, 2), new Position(2, 2)) },
            new object[] { new Range(new Position(1, 4), 4), new Range(new Position(1, 3), new Position(1, 8)) },
    };

    public static IReadOnlyList<object[]> ContainedPointsData { get; } = new object[][]
    {
            new object[] { new Range(new Position(0, 0), 3), new Position(0, 0) },
            new object[] { new Range(new Position(0, 0), 3), new Position(0, 1) },
            new object[] { new Range(new Position(0, 0), 3), new Position(0, 2) },

            new object[] { new Range(new Position(1, 3), 5), new Position(1, 3) },
            new object[] { new Range(new Position(1, 3), 5), new Position(1, 5) },
            new object[] { new Range(new Position(1, 3), 5), new Position(1, 7) },
    };

    public static IReadOnlyList<object[]> NotContainedPointsData { get; } = new object[][]
    {
            new object[] { new Range(new Position(0, 0), 3), new Position(0, 3) },
            new object[] { new Range(new Position(0, 0), 3), new Position(0, 4) },
            new object[] { new Range(new Position(0, 0), 3), new Position(1, 2) },
            new object[] { new Range(new Position(0, 0), 3), new Position(1, 0) },

            new object[] { new Range(new Position(1, 3), 5), new Position(0, 3) },
            new object[] { new Range(new Position(1, 3), 5), new Position(0, 5) },
            new object[] { new Range(new Position(1, 3), 5), new Position(0, 7) },
            new object[] { new Range(new Position(1, 3), 5), new Position(1, 2) },
            new object[] { new Range(new Position(1, 3), 5), new Position(1, 8) },
            new object[] { new Range(new Position(1, 3), 5), new Position(1, 0) },
            new object[] { new Range(new Position(1, 3), 5), new Position(1, 10) },
    };

    public static IReadOnlyList<object[]> IntersectingRangesData { get; } = new object[][]
    {
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(0, 0), new Position(0, 3)) },
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(0, 1), new Position(0, 1)) },
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(0, 0), new Position(0, 1)) },
            new object[] { new Range(new Position(0, 0), 5), new Range(new Position(0, 1), new Position(0, 2)) },
            new object[] { new Range(new Position(0, 0), 5), new Range(new Position(0, 0), new Position(0, 1)) },
            new object[] { new Range(new Position(0, 0), 5), new Range(new Position(0, 4), new Position(0, 5)) },
            new object[] { new Range(new Position(0, 0), 5), new Range(new Position(0, 4), new Position(0, 8)) },
            new object[] { new Range(new Position(0, 0), 5), new Range(new Position(0, 2), new Position(0, 8)) },
    };

    public static IReadOnlyList<object[]> NotIntersectingRangesData { get; } = new object[][]
    {
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(0, 0), new Position(0, 0)) },
            new object[] { new Range(new Position(0, 2), 3), new Range(new Position(0, 1), new Position(0, 2)) },
            new object[] { new Range(new Position(0, 2), 3), new Range(new Position(0, 5), new Position(0, 7)) },
            new object[] { new Range(new Position(0, 0), 3), new Range(new Position(1, 0), new Position(1, 3)) },
    };

    [Theory]
    [MemberData(nameof(InvalidPositionPairsData))]
    public void InvalidConstruction(Position start, Position end) =>
        Assert.Throws<ArgumentException>(() => new Range(start, end));

    [Theory]
    [MemberData(nameof(EqualRangesData))]
    public void EqualRanges(Range r1, Range r2)
    {
        Assert.Equal(r1, r2);
        Assert.Equal(r1, (object)r2);
        Assert.True(r1 == r2);
        Assert.False(r1 != r2);
        Assert.Equal(r1.GetHashCode(), r2.GetHashCode());
    }

    [Theory]
    [MemberData(nameof(UnequalRangesData))]
    public void NotEqualRanges(Range r1, Range r2)
    {
        Assert.NotEqual(r1, r2);
        Assert.NotEqual(r1, (object)r2);
        Assert.True(r1 != r2);
        Assert.False(r1 == r2);
    }

    [Theory]
    [MemberData(nameof(ContainedPointsData))]
    public void Contains(Range r, Position p) => Assert.True(r.Contains(p));

    [Theory]
    [MemberData(nameof(NotContainedPointsData))]
    public void NotContains(Range r, Position p) => Assert.False(r.Contains(p));

    [Theory]
    [MemberData(nameof(IntersectingRangesData))]
    public void Intersects(Range r1, Range r2)
    {
        Assert.True(r1.Intersects(r2));
        Assert.True(r2.Intersects(r1));
    }

    [Theory]
    [MemberData(nameof(NotIntersectingRangesData))]
    public void NotIntersects(Range r1, Range r2)
    {
        Assert.False(r1.Intersects(r2));
        Assert.False(r2.Intersects(r1));
    }
}
