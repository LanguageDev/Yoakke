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
    }

    private class AvlSet
    {
        public Node? Root { get; set; }

        public bool Insert(int k)
        {
            var insertion = AvlTree.Insert(
                root: this.Root,
                key: k,
                makeNode: k => new Node() { Key = k },
                keySelector: n => n.Key,
                keyComparer: Comparer<int>.Default);
            if (insertion.Existing is not null) return false;
            this.Root = insertion.Root;
            return true;
        }
    }

    [Fact]
    public void InsertAbcIs1LRotation()
    {
        var set = new AvlSet();
        set.Insert('a');
        set.Insert('b');
        set.Insert('c');

        Assert.Equal(
            set.Root,
            new Node
            {
                Key = 'b',
                Height = 2,
                Left = new()
                {
                    Key = 'a',
                    Height = 1,
                },
                Right = new()
                {
                    Key = 'c',
                    Height = 1,
                },
            });
    }
}
