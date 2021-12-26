using System;
using Xunit;

namespace Yoakke.Streams.Tests;

public class StreamsTests
{
    [Fact]
    public void MemoryStreamSequence()
    {
        var ms = new MemoryStream<int>(new int[] { 1, 2, 3 }.AsMemory());

        Assert.False(ms.IsEnd);
        Assert.Equal(1, ms.Consume());
        Assert.False(ms.IsEnd);
        Assert.Equal(2, ms.Consume());
        Assert.False(ms.IsEnd);
        Assert.Equal(3, ms.Consume());
        Assert.True(ms.IsEnd);
    }

    [Fact]
    public void FilterStream()
    {
        var ms = new MemoryStream<int>(new int[] { 1, 2, 3, 4, 5, 6 }.AsMemory()).Filter(n => n % 2 == 1);

        Assert.False(ms.IsEnd);
        Assert.Equal(1, ms.Consume());
        Assert.False(ms.IsEnd);
        Assert.Equal(3, ms.Consume());
        Assert.False(ms.IsEnd);
        Assert.Equal(5, ms.Consume());
        Assert.True(ms.IsEnd);
    }

    [Fact]
    public void BufferedStream()
    {
        var ms = new MemoryStream<int>(new int[] { 1, 2, 3, 4, 5, 6 }.AsMemory()).Filter(n => n % 2 == 1).ToBuffered();

        Assert.False(ms.IsEnd);
        Assert.True(ms.TryPeek(out var t0));
        Assert.Equal(1, t0);
        Assert.True(ms.TryLookAhead(1, out var t1));
        Assert.Equal(3, t1);
        Assert.False(ms.IsEnd);
        Assert.Equal(1, ms.Consume());
        Assert.False(ms.IsEnd);
        Assert.Equal(3, ms.Consume());
        Assert.False(ms.IsEnd);
        Assert.Equal(5, ms.Consume());
        Assert.True(ms.IsEnd);
    }
}
