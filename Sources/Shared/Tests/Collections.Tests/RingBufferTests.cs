// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Yoakke.Collections.Tests;

public class RingBufferTests
{
    [Fact]
    public void EmptyBuffer()
    {
        var rb = new RingBuffer<int>();

        Assert.Empty(rb);
        Assert.Equal(0, rb.Head);
        Assert.Equal(0, rb.Tail);
    }

    [Fact]
    public void IndexEmptyBuffer()
    {
        var rb = new RingBuffer<int>();

        Assert.Throws<ArgumentOutOfRangeException>(() => rb[0]);
        Assert.Throws<ArgumentOutOfRangeException>(() => rb[0] = 0);
    }

    [Fact]
    public void AddingElementToBack()
    {
        var rb = new RingBuffer<int>();

        rb.AddBack(1);

        Assert.Single(rb);
        Assert.Equal(0, rb.Head);
        Assert.Equal(1, rb.Tail);
        Assert.Equal(1, rb[0]);
    }

    [Fact]
    public void AddingElementToFront()
    {
        var rb = new RingBuffer<int>();

        rb.AddFront(1);

        Assert.Single(rb);
        Assert.Equal(rb.Capacity - 1, rb.Head);
        Assert.Equal(0, rb.Tail);
        Assert.Equal(1, rb[0]);
    }

    [Fact]
    public void AddingElementToFrontAndBack()
    {
        var rb = new RingBuffer<int>();

        rb.AddFront(2);
        rb.AddBack(1);

        Assert.Equal(2, rb.Count);
        Assert.Equal(rb.Capacity - 1, rb.Head);
        Assert.Equal(1, rb.Tail);
        Assert.Equal(2, rb[0]);
        Assert.Equal(1, rb[1]);
    }

    [Fact]
    public void InsteringElementIntoEmpty()
    {
        var rb = new RingBuffer<int>();

        rb.Insert(0, 1);

        Assert.Single(rb);
        Assert.Equal(1, rb[0]);
    }

    [Fact]
    public void RemovingElementFromBack()
    {
        var rb = new RingBuffer<int> { 1, 2 };

        var removed = rb.RemoveBack();

        Assert.Single(rb);
        Assert.Equal(0, rb.Head);
        Assert.Equal(1, rb.Tail);
        Assert.Equal(1, rb[0]);
        Assert.Equal(2, removed);
    }

    [Fact]
    public void RemovingElementFromFront()
    {
        var rb = new RingBuffer<int> { 1, 2 };

        var removed = rb.RemoveFront();

        Assert.Single(rb);
        Assert.Equal(1, rb.Head);
        Assert.Equal(2, rb.Tail);
        Assert.Equal(2, rb[0]);
        Assert.Equal(1, removed);
    }

    [Fact]
    public void RemovingElementFromBackEmpty()
    {
        var rb = new RingBuffer<int>();

        Assert.Throws<InvalidOperationException>(() => rb.RemoveBack());
    }

    [Fact]
    public void RemovingElementFromFrontEmpty()
    {
        var rb = new RingBuffer<int>();

        Assert.Throws<InvalidOperationException>(() => rb.RemoveFront());
    }

    [Fact]
    public void ChangingContentsWithIndexer()
    {
        var rb = new RingBuffer<int> { 1, 2, 3 };

        rb[2] = 4;

        Assert.Equal(3, rb.Count);
        Assert.Equal(0, rb.Head);
        Assert.Equal(3, rb.Tail);
        Assert.Equal(1, rb[0]);
        Assert.Equal(2, rb[1]);
        Assert.Equal(4, rb[2]);
    }

    [Fact]
    public void InsertingElementInTheMiddle()
    {
        var rb = new RingBuffer<int> { 1, 2, 3, 4, 5, 6 };

        rb.Insert(2, 8);

        Assert.Equal(7, rb.Count);
        Assert.Equal(1, rb[0]);
        Assert.Equal(2, rb[1]);
        Assert.Equal(8, rb[2]);
        Assert.Equal(3, rb[3]);
        Assert.Equal(4, rb[4]);
        Assert.Equal(5, rb[5]);
        Assert.Equal(6, rb[6]);
    }

    [Fact]
    public void InsertingElementInTheMiddleWhileSplitFirst()
    {
        var rb = new RingBuffer<int>(5) { 1, 2, 3, 4 };

        rb.RemoveFront();
        rb.RemoveFront();
        rb.AddBack(5);
        rb.AddBack(6);

        rb.Insert(1, 7);

        Assert.Equal(5, rb.Count);
        Assert.Equal(3, rb[0]);
        Assert.Equal(7, rb[1]);
        Assert.Equal(4, rb[2]);
        Assert.Equal(5, rb[3]);
        Assert.Equal(6, rb[4]);
    }

    [Fact]
    public void InsertingElementInTheMiddleWhileSplitSecond()
    {
        var rb = new RingBuffer<int>(5) { 1, 2, 3, 4 };

        rb.RemoveFront();
        rb.RemoveFront();
        rb.AddBack(5);
        rb.AddBack(6);

        rb.Insert(3, 7);

        Assert.Equal(5, rb.Count);
        Assert.Equal(3, rb[0]);
        Assert.Equal(4, rb[1]);
        Assert.Equal(5, rb[2]);
        Assert.Equal(7, rb[3]);
        Assert.Equal(6, rb[4]);
    }

    [Fact]
    public void RemovingElementInTheMiddle()
    {
        var rb = new RingBuffer<int> { 1, 2, 3, 4, 5, 6 };

        rb.RemoveAt(2);

        Assert.Equal(5, rb.Count);
        Assert.Equal(1, rb[0]);
        Assert.Equal(2, rb[1]);
        Assert.Equal(4, rb[2]);
        Assert.Equal(5, rb[3]);
        Assert.Equal(6, rb[4]);
    }

    [Fact]
    public void RemovingElementInTheMiddleWhileSplitFirst()
    {
        var rb = new RingBuffer<int>(4) { 1, 2, 3, 4 };

        rb.RemoveFront();
        rb.RemoveFront();
        rb.AddBack(5);
        rb.AddBack(6);

        rb.RemoveAt(1);

        Assert.Equal(3, rb.Count);
        Assert.Equal(3, rb[0]);
        Assert.Equal(5, rb[1]);
        Assert.Equal(6, rb[2]);
    }

    [Fact]
    public void RemovingElementInTheMiddleWhileSplitSecond()
    {
        var rb = new RingBuffer<int>(4) { 1, 2, 3, 4 };

        rb.RemoveFront();
        rb.RemoveFront();
        rb.AddBack(5);
        rb.AddBack(6);

        rb.RemoveAt(2);

        Assert.Equal(3, rb.Count);
        Assert.Equal(3, rb[0]);
        Assert.Equal(4, rb[1]);
        Assert.Equal(6, rb[2]);
    }
}
