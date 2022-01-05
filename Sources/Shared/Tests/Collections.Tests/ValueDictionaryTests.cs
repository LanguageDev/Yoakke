// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Xunit;
using Yoakke.Collections.Values;

namespace Yoakke.Collections.Tests;

public class ValueDictionaryTests
{
    [Fact]
    public void Equal()
    {
        var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
        var dict2 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();

        Assert.Equal(dict1, dict2);
        Assert.False(ReferenceEquals(dict1, dict2));
        Assert.Equal(dict1.GetHashCode(), dict2.GetHashCode());
    }

    [Fact]
    public void EqualDifferentOrder()
    {
        var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
        var dict2 = new Dictionary<string, int> { { "b", 2 }, { "c", 3 }, { "a", 1 } }.ToValue();

        Assert.Equal(dict1, dict2);
        Assert.False(ReferenceEquals(dict1, dict2));
        Assert.Equal(dict1.GetHashCode(), dict2.GetHashCode());
    }

    [Fact]
    public void DifferentCount()
    {
        var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
        var dict2 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 }, { "d", 4 } }.ToValue();

        Assert.NotEqual(dict1, dict2);
        Assert.False(ReferenceEquals(dict1, dict2));
    }

    [Fact]
    public void DifferentKey()
    {
        var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
        var dict2 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "d", 3 } }.ToValue();

        Assert.NotEqual(dict1, dict2);
        Assert.False(ReferenceEquals(dict1, dict2));
    }

    [Fact]
    public void DifferentValue()
    {
        var dict1 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 3 } }.ToValue();
        var dict2 = new Dictionary<string, int> { { "a", 1 }, { "b", 2 }, { "c", 4 } }.ToValue();

        Assert.NotEqual(dict1, dict2);
        Assert.False(ReferenceEquals(dict1, dict2));
    }
}
