using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Yoakke.Collections.Tests;

public sealed class ByValueSetTests
{
    [InlineData(new int[] { }, new int[] { })]
    [InlineData(new[] { 1 }, new[] { 1 })]
    [InlineData(new[] { 1, 2, 3 }, new[] { 1, 2, 3 })]
    [InlineData(new[] { 1, 2, 3 }, new[] { 3, 2, 1 })]
    [InlineData(new[] { 5, 1, 3, 9, 8 }, new[] { 8, 9, 3, 1, 5 })]
    [InlineData(new[] { 5, 1, 3, 9, 8 }, new[] { 9, 1, 8, 5, 3 })]
    [Theory]
    public void EqualSets(int[] v1, int[] v2)
    {
        var l1 = new ByValueSet<int>(v1, EqualityComparer<int>.Default);
        var l2 = new ByValueSet<int>(v2, EqualityComparer<int>.Default);

        Assert.Equal(l1, l2);
        Assert.Equal(l2, l1);
        Assert.True(l2 == l1);
        Assert.False(l2 != l1);
        Assert.Equal(l1.GetHashCode(), l2.GetHashCode());
    }

    [InlineData(new int[] { }, new int[] { 1 })]
    [InlineData(new[] { 1, 2 }, new[] { 1, 2, 3 })]
    [InlineData(new[] { 5, 1, 3, 9, 8 }, new[] { 5, 1, 3, 9, 8, 10 })]
    [Theory]
    public void NotEqualSets(int[] v1, int[] v2)
    {
        var l1 = new ByValueSet<int>(v1, EqualityComparer<int>.Default);
        var l2 = new ByValueSet<int>(v2, EqualityComparer<int>.Default);

        Assert.NotEqual(l1, l2);
        Assert.NotEqual(l2, l1);
        Assert.False(l2 == l1);
        Assert.True(l2 != l1);
        Assert.NotEqual(l1.GetHashCode(), l2.GetHashCode());
    }
}
