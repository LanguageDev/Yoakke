// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

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
    
    [Fact]
    public void EnumerableStream1()
    {
        var es = new EnumerableStream<int>(new [] { 1, 2, 3 });
        var s = es.ToBuffered();
        
        Assert.False(s.IsEnd);
        Assert.True(s.TryPeek(out var t0));
        Assert.Equal(1, t0);
        s.Consume(1); Assert.False(s.IsEnd);
        Assert.True(s.TryPeek(out var t1));
        Assert.Equal(2, t1);
        s.Consume(1); Assert.False(s.IsEnd);
        Assert.True(s.TryPeek(out var t2));
        Assert.Equal(3, t2);
        
        s.Consume(1);
        Assert.True(es.IsEnd);
        Assert.True(s.IsEnd);
    }
}
