// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Trees;

/// <summary>
/// Utilities to handle tree nodes as a binary search tree.
/// </summary>
public static class BinarySearchTree
{
    /// <summary>
    /// An enumeration describing the two children of a node.
    /// </summary>
    public enum Child
    {
        /// <summary>
        /// The left child.
        /// </summary>
        Left,

        /// <summary>
        /// The right child.
        /// </summary>
        Right,
    }

    /// <summary>
    /// Interface for simple binary tree nodes.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    public interface INode<TNode>
        where TNode : class, INode<TNode>
    {
        /// <summary>
        /// The parent of this node.
        /// </summary>
        public TNode? Parent { get; set; }

        /// <summary>
        /// The left child of this node.
        /// </summary>
        public TNode? Left { get; set; }

        /// <summary>
        /// The right child of this node.
        /// </summary>
        public TNode? Right { get; set; }
    }

    /// <summary>
    /// A simple base class for nodes that implements the left-right-parent update logic.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    public abstract class NodeBase<TNode> : INode<TNode>
        where TNode : NodeBase<TNode>
    {
        /// <inheritdoc/>
        public TNode? Parent { get; set; }

        /// <inheritdoc/>
        public TNode? Left
        {
            get => this.left;
            set
            {
                this.left = value;
                if (value is not null) value.Parent = (TNode)this;
            }
        }

        /// <inheritdoc/>
        public TNode? Right
        {
            get => this.right;
            set
            {
                this.right = value;
                if (value is not null) value.Parent = (TNode)this;
            }
        }

        private TNode? left;
        private TNode? right;
    }

    /// <summary>
    /// Represents the result of a tree-search.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="Found">The exact match found.</param>
    /// <param name="Hint">The hint for insertion, if an exact match is not found.</param>
    public record struct SearchResult<TNode>(TNode? Found, (TNode Node, Child Child)? Hint)
        where TNode : class, INode<TNode>;

    /// <summary>
    /// Represents the result of an insertion.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="Root">The root of the tree.</param>
    /// <param name="Inserted">The inserted node, if any.</param>
    /// <param name="Existing">The existing node that blocked the insertion, if any.</param>
    public record struct InsertResult<TNode>(TNode Root, TNode? Inserted, TNode? Existing)
        where TNode : class, INode<TNode>;

    /// <summary>
    /// Retrieves the minimum (leftmost leaf) of a given subtree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="root">The root of the subtree.</param>
    /// <returns>The minimum of the subtree with root <paramref name="root"/>.</returns>
    public static TNode Minimum<TNode>(TNode root)
        where TNode : class, INode<TNode>
    {
        for (; root.Left is not null; root = root.Left) ;
        return root;
    }

    /// <summary>
    /// Retrieves the maximum (rightmost leaf) of a given subtree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="root">The root of the subtree.</param>
    /// <returns>The maximum of the subtree with root <paramref name="root"/>.</returns>
    public static TNode Maximum<TNode>(TNode root)
        where TNode : class, INode<TNode>
    {
        for (; root.Right is not null; root = root.Right) ;
        return root;
    }

    /// <summary>
    /// Retrieves the in-order predecessor of a given node.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="node">The node to get the predecessor of.</param>
    /// <returns>The predecessor of <paramref name="node"/>.</returns>
    public static TNode? Predecessor<TNode>(TNode node)
        where TNode : class, INode<TNode>
    {
        if (node.Left is not null) return Maximum(node.Left);
        var y = node.Parent;
        while (y is not null && ReferenceEquals(node, y.Left))
        {
            node = y;
            y = y.Parent;
        }
        return y;
    }

    /// <summary>
    /// Retrieves the in-order successor of a given node.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="node">The node to get the successor of.</param>
    /// <returns>The successor of <paramref name="node"/>.</returns>
    public static TNode? Successor<TNode>(TNode node)
        where TNode : class, INode<TNode>
    {
        if (node.Right is not null) return Minimum(node.Right);
        var y = node.Parent;
        while (y is not null && ReferenceEquals(node, y.Right))
        {
            node = y;
            y = y.Parent;
        }
        return y;
    }

    /// <summary>
    /// Searches for a node with a given key.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <param name="root">The root of the subtree to search in.</param>
    /// <param name="key">The key to search for.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="keyComparer">The key comparer.</param>
    /// <returns>The results of the search as a <see cref="SearchResult{TNode}"/>.</returns>
    public static SearchResult<TNode> Search<TNode, TKey>(
        TNode? root,
        TKey key,
        Func<TNode, TKey> keySelector,
        IComparer<TKey> keyComparer)
        where TNode : class, INode<TNode>
    {
        (TNode Node, Child Child)? hint = null;
        while (root is not null)
        {
            var rootKey = keySelector(root);
            var cmp = keyComparer.Compare(key, rootKey);
            if (cmp < 0)
            {
                hint = (root, Child.Left);
                root = root.Left;
            }
            else if (cmp > 0)
            {
                hint = (root, Child.Right);
                root = root.Right;
            }
            else
            {
                return new(Found: root, Hint: null);
            }
        }
        return new(Found: null, Hint: hint);
    }

    /// <summary>
    /// Inserts a new node into the binary tree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <param name="root">The root of the subtree to insert into.</param>
    /// <param name="key">The key to insert with.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="keyComparer">The key comparer.</param>
    /// <param name="makeNode">The node factory.</param>
    /// <returns>The results of the insertion as an <see cref="InsertResult{TNode}"/>.</returns>
    public static InsertResult<TNode> Insert<TNode, TKey>(
        TNode? root,
        TKey key,
        Func<TNode, TKey> keySelector,
        IComparer<TKey> keyComparer,
        Func<TKey, TNode> makeNode)
        where TNode : class, INode<TNode>
    {
        // Try a search
        var (found, hint) = Search(
            root: root,
            key: key,
            keySelector: keySelector,
            keyComparer: keyComparer);
        // If found, we don't do an insertion
        if (found is not null) return new(Root: root!, Inserted: null, Existing: found);
        // If there's a hint, use it
        if (hint is not null)
        {
            var h = hint.Value;
            var newNode = makeNode(key);
            if (h.Child == Child.Left) h.Node.Left = newNode;
            else h.Node.Right = newNode;
            return new(Root: root!, Inserted: newNode, Existing: null);
        }
        // Otherwise, this has to be a new root
        var newRoot = makeNode(key);
        return new(Root: newRoot, Inserted: newRoot, Existing: null);
    }

    /// <summary>
    /// Deletes a node from the binary tree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="root">The root of the tree.</param>
    /// <param name="node">The node to delete.</param>
    /// <returns>The new root of the tree.</returns>
    public static TNode? Delete<TNode>(TNode? root, TNode node)
        where TNode : class, INode<TNode>
    {
        void Shift(TNode u, TNode? v)
        {
            if (u.Parent is null)
            {
                root = v;
                if (v is not null) v.Parent = null;
            }
            else if (ReferenceEquals(u, u.Parent.Left))
            {
                u.Parent.Left = v;
            }
            else
            {
                u.Parent.Right = v;
            }
        }

        if (node.Left is null)
        {
            // 0 or 1 child
            Shift(node, node.Right);
        }
        else if (node.Right is null)
        {
            // 0 or 1 child
            Shift(node, node.Left);
        }
        else
        {
            // 2 children
            var y = Successor(node);
            if (!ReferenceEquals(y.Parent, node))
            {
                Shift(y, y.Right);
                y.Right = node.Right;
            }
            Shift(node, y);
            y.Left = node.Left;
        }
        return root;
    }

    /// <summary>
    /// Rotates the subtree left.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="root">The root of the subtree to rotate.</param>
    /// <returns>The new root of the subtree.</returns>
    public static TNode RotateLeft<TNode>(TNode root)
        where TNode : class, INode<TNode>
    {
        var y = root.Right ?? throw new InvalidOperationException("The right child can not be null");
        var t2 = y.Left;
        y.Left = root;
        root.Right = t2;
        return y;
    }

    /// <summary>
    /// Rotates the subtree right.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="root">The root of the subtree to rotate.</param>
    /// <returns>The new root of the subtree.</returns>
    public static TNode RotateRight<TNode>(TNode root)
        where TNode : class, INode<TNode>
    {
        var x = root.Left ?? throw new InvalidOperationException("The left child can not be null");
        var t2 = x.Right;
        x.Right = root;
        root.Left = t2;
        return x;
    }
}
