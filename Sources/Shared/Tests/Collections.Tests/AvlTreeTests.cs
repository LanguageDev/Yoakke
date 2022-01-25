// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yoakke.Collections.Trees;

namespace Yoakke.Collections.Tests;

public class AvlTreeTests
{
    private class Node : BalancedTree.IHeight<Node>, IEquatable<Node>
    {
        public int Key { get; set; }
        public int Height { get; set; } = 1;
        public Node? Left { get; set; }
        public Node? Right { get; set; }

        public Node(int key)
        {
            this.Key = key;
        }

        public bool Equals(Node? other)
        {
            if (other is null) return false;
            if (this.Key != other.Key
             || this.Height != other.Height) return false;
            if (this.Left is null != other.Left is null
             || this.Right is null != other.Right is null) return false;
            if (this.Left is not null && !this.Left.Equals(other.Left)) return false;
            if (this.Right is not null && !this.Right.Equals(other.Right)) return false;
            return true;
        }

        public Node UpdateHeight()
        {
            this.Height = 1 + Math.Max(this.Left?.UpdateHeight()?.Height ?? 0, this.Right?.UpdateHeight()?.Height ?? 0);
            return this;
        }
    }

    private class AvlSet
    {
        public Node? Root { get; set; }

        public bool Insert(int k)
        {
            var insertion = AvlTree.Insert(
                root: this.Root,
                key: k,
                makeNode: k => new Node(k),
                keySelector: n => n.Key,
                keyComparer: Comparer<int>.Default);
            if (insertion.Existing is not null) return false;
            this.Root = insertion.Root;
            return true;
        }
    }

    private static Node InsertCase1Root => new Node(20)
    {
        Left = new(4),
    }.UpdateHeight();

    private static Node InsertCase2Root => new Node(20)
    {
        Left = new(4)
        {
            Left = new(3),
            Right = new(9),
        },
        Right = new(26),
    }.UpdateHeight();

    private static Node InsertCase3Root => new Node(20)
    {
        Left = new(4)
        {
            Left = new(3)
            {
                Left = new(2),
            },
            Right = new(9)
            {
                Left = new(7),
                Right = new(11),
            },
        },
        Right = new(26)
        {
            Left = new(21),
            Right = new(30),
        },
    }.UpdateHeight();

    [Theory]
    [InlineData("abc")]
    [InlineData("cba")]
    [InlineData("acb")]
    [InlineData("cab")]
    [InlineData("bac")]
    [InlineData("bca")]
    public void InsertAbcInAllOrders(string abc)
    {
        var set = new AvlSet();
        foreach (var letter in abc) Assert.True(set.Insert(letter));
        Assert.Equal(
            set.Root,
            new Node('b')
            {
                Left = new('a'),
                Right = new('c'),
            }.UpdateHeight());
    }

    // Insert 15

    [Fact]
    public void Case1Insert15()
    {
        var set = new AvlSet { Root = InsertCase1Root };
        Assert.True(set.Insert(15));
        Assert.Equal(
            set.Root,
            new Node(15)
            {
                Left = new(4),
                Right = new(20),
            }.UpdateHeight());
    }

    [Fact]
    public void Case2Insert15()
    {
        var set = new AvlSet { Root = InsertCase2Root };
        Assert.True(set.Insert(15));
        Assert.Equal(
            set.Root,
            new Node(9)
            {
                Left = new(4)
                {
                    Left = new(3),
                },
                Right = new(20)
                {
                    Left = new(15),
                    Right = new(26),
                },
            }.UpdateHeight());
    }

    [Fact]
    public void Case3Insert15()
    {
        var set = new AvlSet { Root = InsertCase3Root };
        Assert.True(set.Insert(15));
        Assert.Equal(
            set.Root,
            new Node(9)
            {
                Left = new(4)
                {
                    Left = new(3)
                    {
                        Left = new(2),
                    },
                    Right = new(7),
                },
                Right = new(20)
                {
                    Left = new(11)
                    {
                        Right = new(15),
                    },
                    Right = new(26)
                    {
                        Left = new(21),
                        Right = new(30),
                    },
                },
            }.UpdateHeight());
    }

    // Insert 8

    [Fact]
    public void Case1Insert8()
    {
        var set = new AvlSet { Root = InsertCase1Root };
        Assert.True(set.Insert(8));
        Assert.Equal(
            set.Root,
            new Node(8)
            {
                Left = new(4),
                Right = new(20),
            }.UpdateHeight());
    }

    [Fact]
    public void Case2Insert8()
    {
        var set = new AvlSet { Root = InsertCase2Root };
        Assert.True(set.Insert(8));
        Assert.Equal(
            set.Root,
            new Node(9)
            {
                Left = new(4)
                {
                    Left = new(3),
                    Right = new(8),
                },
                Right = new(20)
                {
                    Right = new(26),
                },
            }.UpdateHeight());
    }

    [Fact]
    public void Case3Insert8()
    {
        var set = new AvlSet { Root = InsertCase3Root };
        Assert.True(set.Insert(8));
        Assert.Equal(
            set.Root,
            new Node(9)
            {
                Left = new(4)
                {
                    Left = new(3)
                    {
                        Left = new(2),
                    },
                    Right = new(7)
                    {
                        Right = new(8),
                    },
                },
                Right = new(20)
                {
                    Left = new(11),
                    Right = new(26)
                    {
                        Left = new(21),
                        Right = new(30),
                    },
                },
            }.UpdateHeight());
    }
}
