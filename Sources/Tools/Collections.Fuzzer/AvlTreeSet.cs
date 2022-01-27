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

internal class AvlTreeSet : ITreeSet
{
    private class Node : BinarySearchTree.NodeBase<Node>, AvlTree.INode<Node>
    {
        public int Key { get; set; }
        public int Height { get; set; } = 1;

        public Node(int key)
        {
            this.Key = key;
        }
    }

    public int Count { get; private set; }

    private Node? root;

    public bool Insert(int k)
    {
        var insertion = AvlTree.Insert(
            root: this.root,
            key: k,
            keySelector: n => n.Key,
            keyComparer: Comparer<int>.Default,
            makeNode: k => new Node(k));
        if (insertion.Existing is not null) return false;
        this.root = insertion.Root;
        ++this.Count;
        return true;
    }

    public bool Delete(int k)
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

    public string ToTestCaseString()
    {
        static void Impl(StringBuilder builder, Node? node)
        {
            if (node is null) return;
            builder.Append("new(").Append(node.Key).Append(')');
            if (node.Left is not null || node.Right is not null)
            {
                builder.Append(" { ");
                if (node.Left is not null)
                {
                    builder.Append("Left = ");
                    Impl(builder, node.Left);
                    builder.Append(", ");
                }
                if (node.Right is not null)
                {
                    builder.Append("Right = ");
                    Impl(builder, node.Right);
                    builder.Append(", ");
                }
                builder.Append('}');
            }
        }

        var result = new StringBuilder();
        Impl(result, this.root);
        return result.ToString();
    }

    public bool IsValid(IEnumerable<int> expectedElements)
    {
        static (bool Valid, int Height) HeightCheck(Node? node)
        {
            if (node is null) return (true, 0);
            var (leftValid, leftHeight) = HeightCheck(node.Left);
            var (rightValid, rightHeight) = HeightCheck(node.Right);
            var balance = leftHeight - rightHeight;
            return (
                leftValid && rightValid && -1 <= balance && balance <= 1,
                Math.Max(leftHeight, rightHeight) + 1);
        }

        bool AncestoryCheck(Node? node)
        {
            if (ReferenceEquals(node, this.root) && this.root!.Parent is not null) return false;
            if (node is null) return true;
            if (!AncestoryCheck(node.Left)) return false;
            if (!AncestoryCheck(node.Right)) return false;
            if (node.Left is not null && !ReferenceEquals(node.Left.Parent, node)) return false;
            if (node.Right is not null && !ReferenceEquals(node.Right.Parent, node)) return false;
            return true;
        }

        var expected = expectedElements.ToHashSet();

        bool ContentCheck(Node? node)
        {
            if (node is null) return true;
            ContentCheck(node.Left);
            ContentCheck(node.Right);
            if (!expected!.Remove(node.Key)) return false;
            if (ReferenceEquals(node, this.root)) return expected.Count == 0;
            return true;
        }

        var (heightValid, _) = HeightCheck(this.root);
        var ancestoryValid = AncestoryCheck(this.root);
        var contentValid = ContentCheck(this.root);
        return heightValid && ancestoryValid && contentValid;
    }
}
