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
    private class Node : BinarySearchTree.NodeBase<Node>, AvlTree.INode<Node>
    {
        public int Key { get; set; }
        public int Height { get; set; } = 1;

        public Node(int key)
        {
            this.Key = key;
        }

        public Node UpdateHeight()
        {
            this.Height = 1 + Math.Max(this.Left?.UpdateHeight()?.Height ?? 0, this.Right?.UpdateHeight()?.Height ?? 0);
            return this;
        }

        internal static bool TreeEq(Node? n1, Node? n2) =>
            BinarySearchTreeTests.TreeEquals(n1, n2, (n1, n2) => n1.Key == n2.Key && n1.Height == n2.Height);
    }

    private class AvlSet
    {
        public Node? Root { get; set; }

        public bool Insert(int k)
        {
            var insertion = AvlTree.Insert(
                root: this.Root,
                key: k,
                keySelector: n => n.Key,
                keyComparer: Comparer<int>.Default,
                makeNode: k => new Node(k));
            if (insertion.Existing is not null) return false;
            this.Root = insertion.Root;
            return true;
        }

        public bool Delete(int k)
        {
            var search = BinarySearchTree.Search(
                root: this.Root,
                key: k,
                keySelector: n => n.Key,
                keyComparer: Comparer<int>.Default);
            if (search.Found is null) return false;
            this.Root = AvlTree.Delete(root: this.Root, search.Found);
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

    private static void ValidateTree<TNode>(TNode? root)
        where TNode : class, AvlTree.INode<TNode>
    {
        static int Height(TNode? n) => n is null
            ? 0
            : Math.Max(Height(n.Left), Height(n.Right)) + 1;

        static void ValidateHeight(TNode? n)
        {
            if (n is null) return;
            Assert.Equal(Height(n), n.Height);
            var balance = Height(n.Left) - Height(n.Right);
            Assert.True(-1 <= balance && balance <= 1);
            ValidateHeight(n.Left);
            ValidateHeight(n.Right);
        }

        // Validate as BST
        BinarySearchTreeTests.ValidateTree(root);
        // Check for height
        ValidateHeight(root);
    }

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
        ValidateTree(set.Root);
        foreach (var letter in abc)
        {
            Assert.True(set.Insert(letter));
            ValidateTree(set.Root);
        }
        Assert.True(Node.TreeEq(
            set.Root,
            new Node('b')
            {
                Left = new('a'),
                Right = new('c'),
            }.UpdateHeight()));
    }

    // Insert 15

    [Fact]
    public void Case1Insert15()
    {
        var set = new AvlSet { Root = InsertCase1Root };
        ValidateTree(set.Root);
        Assert.True(set.Insert(15));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new Node(15)
            {
                Left = new(4),
                Right = new(20),
            }.UpdateHeight()));
    }

    [Fact]
    public void Case2Insert15()
    {
        var set = new AvlSet { Root = InsertCase2Root };
        ValidateTree(set.Root);
        Assert.True(set.Insert(15));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
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
            }.UpdateHeight()));
    }

    [Fact]
    public void Case3Insert15()
    {
        var set = new AvlSet { Root = InsertCase3Root };
        ValidateTree(set.Root);
        Assert.True(set.Insert(15));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
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
            }.UpdateHeight()));
    }

    // Insert 8

    [Fact]
    public void Case1Insert8()
    {
        var set = new AvlSet { Root = InsertCase1Root };
        ValidateTree(set.Root);
        Assert.True(set.Insert(8));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new Node(8)
            {
                Left = new(4),
                Right = new(20),
            }.UpdateHeight()));
    }

    [Fact]
    public void Case2Insert8()
    {
        var set = new AvlSet { Root = InsertCase2Root };
        ValidateTree(set.Root);
        Assert.True(set.Insert(8));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
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
            }.UpdateHeight()));
    }

    [Fact]
    public void Case3Insert8()
    {
        var set = new AvlSet { Root = InsertCase3Root };
        ValidateTree(set.Root);
        Assert.True(set.Insert(8));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
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
            }.UpdateHeight()));
    }

    [Fact]
    public void DeleteAFromBcad()
    {
        var set = new AvlSet
        {
            Root = new Node('b')
            {
                Left = new('a'),
                Right = new('c')
                {
                    Right = new('d'),
                },
            }.UpdateHeight()
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete('a'));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new Node('c')
            {
                Left = new('b'),
                Right = new('d'),
            }.UpdateHeight()));
    }

    [Fact]
    public void DeleteDFromCdba()
    {
        var set = new AvlSet
        {
            Root = new Node('c')
            {
                Left = new('b')
                {
                    Left = new('a'),
                },
                Right = new('d'),
            }.UpdateHeight()
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete('d'));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new Node('b')
            {
                Left = new('a'),
                Right = new('c'),
            }.UpdateHeight()));
    }

    [Fact]
    public void DeleteAFromBdac()
    {
        var set = new AvlSet
        {
            Root = new Node('b')
            {
                Left = new('a'),
                Right = new('d')
                {
                    Left = new('c'),
                },
            }.UpdateHeight()
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete('a'));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new Node('c')
            {
                Left = new('b'),
                Right = new('d'),
            }.UpdateHeight()));
    }

    [Fact]
    public void DeleteDFromCadb()
    {
        var set = new AvlSet
        {
            Root = new Node('c')
            {
                Left = new('a')
                {
                    Right = new('b'),
                },
                Right = new('d'),
            }.UpdateHeight()
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete('d'));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new Node('b')
            {
                Left = new('a'),
                Right = new('c'),
            }.UpdateHeight()));
    }

    [Fact]
    public void DeleteAFromCbedfag()
    {
        var set = new AvlSet
        {
            Root = new Node('c')
            {
                Left = new('b')
                {
                    Left = new('a'),
                },
                Right = new('e')
                {
                    Left = new('d'),
                    Right = new('f')
                    {
                        Right = new('g'),
                    },
                },
            }.UpdateHeight()
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete('a'));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new Node('e')
            {
                Left = new('c')
                {
                    Left = new('b'),
                    Right = new('d'),
                },
                Right = new('f')
                {
                    Right = new('g'),
                },
            }.UpdateHeight()));
    }

    [Fact]
    public void DeleteGFromEcfbdga()
    {
        var set = new AvlSet
        {
            Root = new Node('e')
            {
                Left = new('c')
                {
                    Left = new('b')
                    {
                        Left = new('a'),
                    },
                    Right = new('d'),
                },
                Right = new('f')
                {
                    Right = new('g'),
                },
            }.UpdateHeight()
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete('g'));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new Node('c')
            {
                Left = new('b')
                {
                    Left = new('a'),
                },
                Right = new('e')
                {
                    Left = new('d'),
                    Right = new('f'),
                },
            }.UpdateHeight()));
    }

    [Fact]
    public void DeleteTwoRotations()
    {
        var set = new AvlSet
        {
            Root = new Node(5)
            {
                Left = new(2)
                {
                    Left = new(1),
                    Right = new(3)
                    {
                        Right = new(4),
                    },
                },
                Right = new(8)
                {
                    Left = new(7)
                    {
                        Left = new(6),
                    },
                    Right = new(10)
                    {
                        Left = new(9),
                        Right = new(11)
                        {
                            Right = new(12),
                        },
                    }
                },
            }.UpdateHeight()
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete(1));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new Node(8)
            {
                Left = new(5)
                {
                    Left = new(3)
                    {
                        Left = new(2),
                        Right = new(4),
                    },
                    Right = new(7)
                    {
                        Left = new(6),
                    }
                },
                Right = new(10)
                {
                    Left = new(9),
                    Right = new(11)
                    {
                        Right = new(12),
                    },
                },
            }.UpdateHeight()));
    }

    [Fact]
    public void DeleteFuzzed01()
    {
        var set = new AvlSet
        {
            Root = new Node(7)
            {
                Left = new(0),
                Right = new(11)
                {
                    Right = new(18),
                },
            }.UpdateHeight()
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete(0));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new Node(11)
            {
                Left = new(7),
                Right = new(18),
            }.UpdateHeight()));
    }

    [Fact]
    public void DeleteFuzzed02()
    {
        var set = new AvlSet
        {
            Root = new Node(8)
            {
                Left = new(5),
                Right = new(19),
            }.UpdateHeight()
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete(8));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new Node(19)
            {
                Left = new(5),
            }.UpdateHeight()));
    }

    [Fact]
    public void DeleteFuzzed03()
    {
        var set = new AvlSet
        {
            Root = new Node(41)
            {
                Left = new(20)
                {
                    Left = new(5)
                    {
                        Left = new(1),
                    },
                    Right = new(25)
                    {
                        Left = new(23),
                    },
                },
                Right = new(46)
                {
                    Left = new(45),
                    Right = new(57)
                    {
                        Left = new(53),
                        Right = new(58),
                    },
                },
            }.UpdateHeight()
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete(41));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new Node(45)
            {
                Left = new(20)
                {
                    Left = new(5)
                    {
                        Left = new(1),
                    },
                    Right = new(25)
                    {
                        Left = new(23),
                    },
                },
                Right = new(57)
                {
                    Left = new(46)
                    {
                        Right = new(53),
                    },
                    Right = new(58),
                },
            }.UpdateHeight()));
    }
}
