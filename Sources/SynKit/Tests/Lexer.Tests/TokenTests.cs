// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Xunit;

namespace Yoakke.SynKit.Lexer.Tests;

public class TokenTests : TestBase<int>
{
    [Fact]
    public void Equality()
    {
        var t1 = Token("hello", 3, Range((3, 4), 5));
        var t2 = Token("hello", 3, Range((3, 4), 5));
        Assert.Equal(t1, t2);
        Assert.Equal(t1.GetHashCode(), t2.GetHashCode());
    }

    [Fact]
    public void InequalityContent()
    {
        var t1 = Token("hello", 3, Range((3, 4), 5));
        var t2 = Token("bye", 3, Range((3, 4), 5));
        Assert.NotEqual(t1, t2);
        Assert.NotEqual(t1.GetHashCode(), t2.GetHashCode());
    }

    [Fact]
    public void InequalityKind()
    {
        var t1 = Token("hello", 3, Range((3, 4), 5));
        var t2 = Token("hello", 4, Range((3, 4), 5));
        Assert.NotEqual(t1, t2);
        Assert.NotEqual(t1.GetHashCode(), t2.GetHashCode());
    }

    [Fact]
    public void InequalityPosition()
    {
        var t1 = Token("hello", 3, Range((3, 4), 5));
        var t2 = Token("hello", 4, Range((4, 4), 5));
        Assert.NotEqual(t1, t2);
        Assert.NotEqual(t1.GetHashCode(), t2.GetHashCode());
    }

    [Fact]
    public void InequalityLength()
    {
        var t1 = Token("hello", 3, Range((3, 4), 5));
        var t2 = Token("hello", 4, Range((3, 4), 6));
        Assert.NotEqual(t1, t2);
        Assert.NotEqual(t1.GetHashCode(), t2.GetHashCode());
    }

    [Fact]
    public void NoLocation()
    {
        var t1 = Token("hello", new(), 3, Range((3, 4), 5));
        var t2 = Token("hello", new(), 3, Range((3, 4), 5));
        Assert.Equal(t1, t2);
        Assert.Equal(t1.GetHashCode(), t2.GetHashCode());
    }
}
