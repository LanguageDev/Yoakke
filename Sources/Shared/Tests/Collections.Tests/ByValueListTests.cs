using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Yoakke.Collections.Tests;

public sealed class ByValueListTests
{
    [InlineData(new int[] { }, new int[] { })]
    [InlineData(new[] { 1 }, new[] { 1 })]
    [InlineData(new[] { 1, 1, 1 }, new[] { 1, 1, 1 })]
    [InlineData(new[] { 1, 2, 3 }, new[] { 1, 2, 3 })]
    [InlineData(new[] { 5, 1, 3, 9, 8 }, new[] { 5, 1, 3, 9, 8 })]
    [Theory]
    public void EqualLists(int[] v1, int[] v2)
    {
        var l1 = new ByValueList<int>(v1);
        var l2 = new ByValueList<int>(v2);

        Assert.Equal(l1, l2);
        Assert.Equal(l2, l1);
        Assert.True(l2 == l1);
        Assert.False(l2 != l1);
        Assert.True(l1.Equals(v1 as IReadOnlyList<int>));
        Assert.True(l1.Equals(v2 as IReadOnlyList<int>));
        Assert.True(l2.Equals(v1 as IReadOnlyList<int>));
        Assert.True(l2.Equals(v2 as IReadOnlyList<int>));
        Assert.Equal(l1.GetHashCode(), l2.GetHashCode());
    }

    [InlineData(new int[] { }, new int[] { 1 })]
    [InlineData(new int[] { 1, 1 }, new int[] { 1, 1, 1 })]
    [InlineData(new[] { 1, 2 }, new[] { 1, 2, 3 })]
    [InlineData(new[] { 5, 1, 3, 9, 8 }, new[] { 5, 1, 3, 9, 8, 10 })]
    [Theory]
    public void NotEqualLists(int[] v1, int[] v2)
    {
        var l1 = new ByValueList<int>(v1);
        var l2 = new ByValueList<int>(v2);

        Assert.NotEqual(l1, l2);
        Assert.NotEqual(l2, l1);
        Assert.False(l2 == l1);
        Assert.True(l2 != l1);
        Assert.True(l1.Equals(v1 as IReadOnlyList<int>));
        Assert.True(l2.Equals(v2 as IReadOnlyList<int>));
        Assert.False(l1.Equals(v2 as IReadOnlyList<int>));
        Assert.False(l2.Equals(v1 as IReadOnlyList<int>));
        Assert.NotEqual(l1.GetHashCode(), l2.GetHashCode());
    }
}