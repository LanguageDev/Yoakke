// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Xunit;

namespace Yoakke.SynKit.Text.Tests;

public class PositionTests
{
    public static IReadOnlyList<object[]> EqualPositionsData { get; } = new object[][]
    {
            new object[] { new Position(0, 0), new Position(0, 0) },
            new object[] { new Position(0, 3), new Position(0, 3) },
            new object[] { new Position(3, 3), new Position(3, 3) },
    };

    public static IReadOnlyList<object[]> InequalPositionsData { get; } = new object[][]
    {
            new object[] { new Position(0, 0), new Position(0, 1) },
            new object[] { new Position(0, 0), new Position(1, 0) },
            new object[] { new Position(0, 3), new Position(1, 3) },
            new object[] { new Position(3, 3), new Position(4, 3) },
            new object[] { new Position(3, 3), new Position(4, 5) },
            new object[] { new Position(3, 3), new Position(2, 1) },
    };

    [Theory]
    [MemberData(nameof(EqualPositionsData))]
    public void EqualPositions(Position p1, Position p2)
    {
        Assert.Equal(p1, p2);
        Assert.Equal(p1, (object)p2);
        Assert.True(p1 == p2);
        Assert.False(p1 != p2);
        Assert.True(p1 >= p2);
        Assert.True(p1 <= p2);
        Assert.False(p1 > p2);
        Assert.False(p1 < p2);
        Assert.Equal(p1.GetHashCode(), p2.GetHashCode());
    }

    [Theory]
    [MemberData(nameof(InequalPositionsData))]
    public void NotEqualPositions(Position p1, Position p2)
    {
        Assert.NotEqual(p1, p2);
        Assert.NotEqual(p1, (object)p2);
        Assert.False(p1 == p2);
        Assert.True(p1 != p2);
        Assert.True(p1 > p2 || p1 < p2);
        Assert.True(p1 >= p2 || p1 <= p2);
        Assert.True((p1 >= p2) != (p1 <= p2));
    }
}
