// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Xunit;
using Yoakke.Collections.Values;

namespace Yoakke.Collections.Tests;

public class ValueListTests
{
    [Fact]
    public void Equal()
    {
        var list1 = new List<int> { 1, 2, 3 }.ToValue();
        var list2 = new List<int> { 1, 2, 3 }.ToValue();

        Assert.Equal(list1, list2);
        Assert.False(ReferenceEquals(list1, list2));
        Assert.Equal(list1.GetHashCode(), list2.GetHashCode());
    }

    [Fact]
    public void DifferentCount()
    {
        var list1 = new List<int> { 1, 2, 3 }.ToValue();
        var list2 = new List<int> { 1, 2, 3, 4 }.ToValue();

        Assert.NotEqual(list1, list2);
        Assert.False(ReferenceEquals(list1, list2));
    }

    [Fact]
    public void DifferentValue()
    {
        var list1 = new List<int> { 1, 2, 3 }.ToValue();
        var list2 = new List<int> { 1, 2, 4 }.ToValue();

        Assert.NotEqual(list1, list2);
        Assert.False(ReferenceEquals(list1, list2));
    }
}
