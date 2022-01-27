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

internal class AvlTreeNode : TreeNodeBase<AvlTreeNode>, AvlTree.INode<AvlTreeNode>
{
    public int Height { get; set; } = 1;

    public AvlTreeNode(int key)
        : base(key)
    {
    }
}

internal class AvlTreeSet : TreeSetBase<AvlTreeNode>
{
    public override bool Insert(int k)
    {
        var insertion = AvlTree.Insert(
            root: this.root,
            key: k,
            keySelector: n => n.Key,
            keyComparer: Comparer<int>.Default,
            makeNode: k => new(k));
        if (insertion.Existing is not null) return false;
        this.root = insertion.Root;
        ++this.Count;
        return true;
    }

    public override bool Delete(int k)
    {
        var search = BinarySearchTree.Search(
            root: this.root,
            key: k,
            keySelector: n => n.Key,
            keyComparer: Comparer<int>.Default);
        if (search.Found is null) return false;
        this.root = AvlTree.Delete(root: this.root, search.Found);
        --this.Count;
        return true;
    }

    public override void Validate(IEnumerable<int> expected)
    {
        base.Validate(expected);
        this.ValidateBalanceAndHeight();
    }

    private void ValidateBalanceAndHeight()
    {
        static int Impl(AvlTreeNode? node)
        {
            if (node is null) return 0;

            var leftExpHeight = Impl(node.Left);
            var leftActHeight = node.Left?.Height ?? 0;
            if (leftExpHeight != leftActHeight) throw new ValidationException($"Height error: The left node's height ({leftActHeight}) does not match the expected ({leftExpHeight})");

            var rightExpHeight = Impl(node.Right);
            var rightActHeight = node.Right?.Height ?? 0;
            if (rightExpHeight != rightActHeight) throw new ValidationException($"Height error: The right node's height ({rightActHeight}) does not match the expected ({rightExpHeight})");

            var balance = leftActHeight - rightActHeight;
            if (!(-1 <= balance && balance <= 1)) throw new ValidationException($"Balance error: The node is unbalanced (balance factor {balance})");

            return 1 + Math.Max(leftActHeight, rightActHeight);
        }

        var rootExpHeight = Impl(this.root);
        var rootActHeight = this.root?.Height ?? 0;
        if (rootExpHeight != rootActHeight) throw new ValidationException($"Height error: The root's height ({rootActHeight}) does not match the expected ({rootExpHeight})");
    }
}
