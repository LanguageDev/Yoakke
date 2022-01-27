// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections.Trees;

namespace Yoakke.Collections.Fuzzer;

internal abstract class TreeNodeBase<TNode> : BinarySearchTree.NodeBase<TNode>
    where TNode : BinarySearchTree.NodeBase<TNode>
{
    public int Key { get; }

    public TreeNodeBase(int key)
    {
        this.Key = key;
    }
}

internal abstract class TreeSetBase<TNode> : ITreeSet
    where TNode : TreeNodeBase<TNode>
{
    public int Count { get; protected set; }

    public TNode? Root { get; set; }

    public abstract bool Delete(int k);
    public abstract bool Insert(int k);

    public virtual void Validate(IEnumerable<int> expected)
    {
        this.ValidateAdjacency();
        this.ValidateContents(expected);
    }

    public string ToTestCaseString()
    {
        var builder = new StringBuilder();

        void Impl(TNode? node)
        {
            if (node is null) return;
            builder.Append("new(").Append(node.Key).Append(')');
            if (node.Left is not null || node.Right is not null)
            {
                builder.Append(" { ");
                if (node.Left is not null)
                {
                    builder.Append("Left = ");
                    Impl(node.Left);
                    builder.Append(", ");
                }
                if (node.Right is not null)
                {
                    builder.Append("Right = ");
                    Impl(node.Right);
                    builder.Append(", ");
                }
                builder.Append('}');
            }
        }

        Impl(this.Root);
        return builder.ToString();
    }

    protected void ValidateAdjacency()
    {
        static void Impl(TNode node)
        {
            if (node is null) return;
            if (node.Left is not null)
            {
                if (!ReferenceEquals(node.Left.Parent, node)) throw new ValidationException("Adjacency error: The left node's parent is not the node");
                Impl(node.Left);
            }
            if (node.Right is not null)
            {
                if (!ReferenceEquals(node.Right.Parent, node)) throw new ValidationException("Adjacency error: The right node's parent is not the node");
                Impl(node.Right);
            }
        }

        if (this.Root is null) return;
        if (this.Root.Parent is not null) throw new ValidationException("Adjacency error: The parent of root is not null");
        Impl(this.Root);
    }

    protected void ValidateContents(IEnumerable<int> expected)
    {
        var remaining = expected.ToHashSet();

        void Impl(TNode? node)
        {
            if (node is null) return;
            Impl(node.Left);
            Impl(node.Right);
            if (!remaining!.Remove(node.Key)) throw new ValidationException($"Content error: The element {node.Key} was not expected to be present in the tree");
        }

        Impl(this.Root);
        if (remaining.Count > 0) throw new ValidationException($"Content error: The elements [{string.Join(", ", remaining)}] were not found in the tree, but were expected");
    }
}
