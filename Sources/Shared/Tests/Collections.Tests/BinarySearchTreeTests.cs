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

public class BinarySearchTreeTests
{
    internal static void ValidateTree<TNode>(TNode? root)
        where TNode : class, BinarySearchTree.INode<TNode>
    {
        static void ValidateSubtree(TNode? parent, TNode node)
        {
            Assert.True(ReferenceEquals(parent, node.Parent));
            if (node.Left is not null) ValidateSubtree(node, node.Left);
            if (node.Right is not null) ValidateSubtree(node, node.Right);
        }

        if (root is null) return;
        ValidateSubtree(null, root);
    }

    internal static bool TreeEquals<TNode>(TNode? root1, TNode? root2, Func<TNode, TNode, bool> equals)
        where TNode : class, BinarySearchTree.INode<TNode>
    {
        if (ReferenceEquals(root1, root2)) return true;
        if (root1 is null || root2 is null) return false;
        if (!equals(root1, root2)) return false;
        if (root1.Left is null != root2.Left is null
         || root1.Right is null != root2.Right is null) return false;
        if (root1.Left is not null && !TreeEquals(root1.Left, root2.Left, equals)) return false;
        if (root1.Right is not null && !TreeEquals(root1.Right, root2.Right, equals)) return false;
        return true;
    }

    private class Node : BinarySearchTree.NodeBase<Node>
    {
        public int Key { get; }

        public Node(int key)
        {
            this.Key = key;
        }

        internal static bool TreeEq(Node? n1, Node? n2) =>
            TreeEquals(n1, n2, (n1, n2) => n1.Key == n2.Key);
    }

    private class BstSet
    {
        public Node? Root { get; set; }

        public bool Insert(int x)
        {
            var insertion = BinarySearchTree.Insert(
                root: this.Root,
                key: x,
                keySelector: n => n.Key,
                keyComparer: Comparer<int>.Default,
                makeNode: x => new(x));
            this.Root = insertion.Root;
            return insertion.Existing is null;
        }

        public bool Delete(int x)
        {
            var search = BinarySearchTree.Search(
                root: this.Root,
                key: x,
                keySelector: n => n.Key,
                keyComparer: Comparer<int>.Default);
            if (search.Found is null) return false;
            this.Root = BinarySearchTree.Delete(
                root: this.Root,
                node: search.Found);
            return true;
        }
    }

    [Fact]
    public void Insert123()
    {
        var set = new BstSet();
        ValidateTree(set.Root);
        Assert.True(set.Insert(1));
        ValidateTree(set.Root);
        Assert.True(set.Insert(2));
        ValidateTree(set.Root);
        Assert.True(set.Insert(3));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new(1)
            {
                Right = new(2)
                {
                    Right = new(3),
                }
            }));
    }

    [Fact]
    public void Insert321()
    {
        var set = new BstSet();
        ValidateTree(set.Root);
        Assert.True(set.Insert(3));
        ValidateTree(set.Root);
        Assert.True(set.Insert(2));
        ValidateTree(set.Root);
        Assert.True(set.Insert(1));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new(3)
            {
                Left = new(2)
                {
                    Left = new(1),
                }
            }));
    }

    [Theory]
    [InlineData(2, 3, 1)]
    [InlineData(2, 1, 3)]
    public void Insert231or213(int a, int b, int c)
    {
        var set = new BstSet();
        ValidateTree(set.Root);
        Assert.True(set.Insert(a));
        ValidateTree(set.Root);
        Assert.True(set.Insert(b));
        ValidateTree(set.Root);
        Assert.True(set.Insert(c));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new(2)
            {
                Left = new(1),
                Right = new(3),
            }));
    }

    [Fact]
    public void RepeatedInsert()
    {
        var set = new BstSet();
        ValidateTree(set.Root);
        Assert.True(set.Insert(1));
        ValidateTree(set.Root);
        Assert.False(set.Insert(1));
        ValidateTree(set.Root);
        Assert.True(set.Insert(2));
        ValidateTree(set.Root);
        Assert.False(set.Insert(2));
        ValidateTree(set.Root);
    }

    [Fact]
    public void InsertMany()
    {
        var set = new BstSet();
        foreach (var n in new[] { 14, 23, 29, 8, 25, 22, 1, 6, 24, 28 })
        {
            ValidateTree(set.Root);
            Assert.True(set.Insert(n));
        }
        Assert.True(Node.TreeEq(
            set.Root,
            new(14)
            {
                Left = new(8)
                {
                    Left = new(1)
                    {
                        Right = new(6),
                    },
                },
                Right = new(23)
                {
                    Left = new(22),
                    Right = new(29)
                    {
                        Left = new(25)
                        {
                            Left = new(24),
                            Right = new(28),
                        },
                    },
                },
            }));
    }

    [Fact]
    public void DeleteRootSingleRightChild()
    {
        var set = new BstSet
        {
            Root = new(1)
            {
                Right = new(3)
                {
                    Left = new(2),
                    Right = new(4),
                },
            }
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete(1));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new(3)
            {
                Left = new(2),
                Right = new(4),
            }));
    }

    [Fact]
    public void DeleteRootSingleLeftChild()
    {
        var set = new BstSet
        {
            Root = new(5)
            {
                Left = new(3)
                {
                    Left = new(2),
                    Right = new(4),
                },
            }
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete(5));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new(3)
            {
                Left = new(2),
                Right = new(4),
            }));
    }

    [Fact]
    public void DeleteRootRightChildWithNoLeftChild()
    {
        var set = new BstSet
        {
            Root = new(5)
            {
                Left = new(3),
                Right = new(8)
                {
                    Right = new(9),
                },
            }
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete(5));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new(8)
            {
                Left = new(3),
                Right = new(9),
            }));
    }

    [Fact]
    public void DeleteRootRightChildWithLeftChild()
    {
        var set = new BstSet
        {
            Root = new(5)
            {
                Left = new(3),
                Right = new(8)
                {
                    Left = new(6)
                    {
                        Right = new(7),
                    },
                },
            }
        };
        ValidateTree(set.Root);
        Assert.True(set.Delete(5));
        ValidateTree(set.Root);
        Assert.True(Node.TreeEq(
            set.Root,
            new(6)
            {
                Left = new(3),
                Right = new(8)
                {
                    Left = new(7),
                },
            }));
    }

    [Fact]
    public void RotateRight()
    {
        var root = new Node('Q')
        {
            Left = new('P')
            {
                Left = new('A'),
                Right = new('B'),
            },
            Right = new('C'),
        };
        ValidateTree(root);
        root = BinarySearchTree.RotateRight(root);
        ValidateTree(root);
        Assert.True(Node.TreeEq(
            root,
            new('P')
            {
                Left = new('A'),
                Right = new('Q')
                {
                    Left = new('B'),
                    Right = new('C'),
                },
            }));
    }

    [Fact]
    public void RotateLeft()
    {
        var root = new Node('P')
        {
            Left = new('A'),
            Right = new('Q')
            {
                Left = new('B'),
                Right = new('C'),
            },
        };
        ValidateTree(root);
        root = BinarySearchTree.RotateLeft(root);
        ValidateTree(root);
        Assert.True(Node.TreeEq(
            root,
            new('Q')
            {
                Left = new('P')
                {
                    Left = new('A'),
                    Right = new('B'),
                },
                Right = new('C'),
            }));
    }
}
