// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yoakke.Streams;

namespace Yoakke.SynKit.Parser.Tests;

public class CombinatorTests
{
    [Fact]
    public void ItemParser()
    {
        var stream = new MemoryStream<char>("abc".AsMemory());
        var p = Combinator.Item<char>();

        var r1 = p.Parse(stream);
        var r2 = p.Parse(stream);
        var r3 = p.Parse(stream);
        var r4 = p.Parse(stream);

        Assert.True(r1.IsOk);
        Assert.True(r2.IsOk);
        Assert.True(r3.IsOk);
        Assert.False(r4.IsOk);
        Assert.Equal('a', r1.Ok.Value);
        Assert.Equal('b', r2.Ok.Value);
        Assert.Equal('c', r3.Ok.Value);
    }

    [Fact]
    public void ExactItemParser()
    {
        var stream = new MemoryStream<char>("abc".AsMemory());
        var p1 = Combinator.Item('a');
        var p2 = Combinator.Item('b');
        var p3 = Combinator.Item('d');
        var p4 = Combinator.Item('c');

        var r1 = p1.Parse(stream);
        var r2 = p2.Parse(stream);
        var r3 = p3.Parse(stream);
        var r4 = p4.Parse(stream);

        Assert.True(r1.IsOk);
        Assert.True(r2.IsOk);
        Assert.False(r3.IsOk);
        Assert.True(r4.IsOk);
        Assert.Equal('a', r1.Ok.Value);
        Assert.Equal('b', r2.Ok.Value);
        Assert.Equal('c', r4.Ok.Value);
    }

    [Fact]
    public void SequenceParser()
    {
        var stream = new MemoryStream<char>("abc".AsMemory());

        var p1 = Combinator.Seq(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('d'));
        var p2 = Combinator.Seq(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'));

        var r1 = p1.Parse(stream);
        var r2 = p2.Parse(stream);

        Assert.False(r1.IsOk);
        Assert.True(r2.IsOk);
        Assert.Equal(('a', 'b', 'c'), r2.Ok.Value);
    }

    [Fact]
    public void AlternativeParser()
    {
        var stream = new MemoryStream<char>("a".AsMemory());

        var p1 = Combinator.Alt(Combinator.Char('x'), Combinator.Char('y'), Combinator.Char('z'));
        var p2 = Combinator.Alt(Combinator.Char('a'), Combinator.Char('b'), Combinator.Char('c'));

        var r1 = p1.Parse(stream);
        var r2 = p2.Parse(stream);

        Assert.False(r1.IsOk);
        Assert.True(r2.IsOk);
        Assert.Equal('a', r2.Ok.Value);
    }

    [Fact]
    public void Repeat0Parser()
    {
        var emptyStream = new MemoryStream<char>(string.Empty.AsMemory());
        var nonEmptyStream = new MemoryStream<char>("aaa".AsMemory());

        var p = Combinator.Rep0(Combinator.Char('a'));

        var r1 = p.Parse(emptyStream);
        var r2 = p.Parse(nonEmptyStream);

        Assert.True(r1.IsOk);
        Assert.True(r2.IsOk);
        Assert.Equal(0, r1.Ok.Value.Count);
        Assert.Equal(3, r2.Ok.Value.Count);
        Assert.Equal('a', r2.Ok.Value[0]);
        Assert.Equal('a', r2.Ok.Value[1]);
        Assert.Equal('a', r2.Ok.Value[2]);
    }

    [Fact]
    public void Repeat1Parser()
    {
        var emptyStream = new MemoryStream<char>(string.Empty.AsMemory());
        var nonEmptyStream = new MemoryStream<char>("aaa".AsMemory());

        var p = Combinator.Rep1(Combinator.Char('a'));

        var r1 = p.Parse(emptyStream);
        var r2 = p.Parse(nonEmptyStream);

        Assert.False(r1.IsOk);
        Assert.True(r2.IsOk);
        Assert.Equal(3, r2.Ok.Value.Count);
        Assert.Equal('a', r2.Ok.Value[0]);
        Assert.Equal('a', r2.Ok.Value[1]);
        Assert.Equal('a', r2.Ok.Value[2]);
    }

    [Fact]
    public void OptParser()
    {
        var stream1 = new MemoryStream<char>("a".AsMemory());
        var stream2 = new MemoryStream<char>("b".AsMemory());

        var p = Combinator.Opt(Combinator.Char('a'));

        var r1 = p.Parse(stream1);
        var r2 = p.Parse(stream2);

        Assert.True(r1.IsOk);
        Assert.True(r2.IsOk);
        Assert.Equal('a', r1.Ok.Value);
        Assert.Equal(default, r2.Ok.Value);
    }

    [Fact]
    public void TransformParser()
    {
        var stream1 = new MemoryStream<char>("a".AsMemory());
        var stream2 = new MemoryStream<char>("b".AsMemory());

        var p = Combinator.Transform(Combinator.Char('a'), _ => 'b');

        var r1 = p.Parse(stream1);
        var r2 = p.Parse(stream2);

        Assert.True(r1.IsOk);
        Assert.False(r2.IsOk);
        Assert.Equal('b', r1.Ok.Value);
    }
}
