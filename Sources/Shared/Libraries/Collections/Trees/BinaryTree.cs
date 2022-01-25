// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Trees;

/// <summary>
/// Utilities to handle tree nodes as a binary tree.
/// </summary>
public static class BinaryTree
{
    /// <summary>
    /// Interface for simple binary tree nodes.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    public interface INode<TNode>
        where TNode : class, INode<TNode>
    {
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
    /// Retrieves the minimum value that can be found in the subtree (meaning the leftmost leaf).
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="node">The root of the subtree to search in.</param>
    /// <returns>The minimum (leftmost) node.</returns>
    public static TNode Min<TNode>(TNode node)
        where TNode : class, INode<TNode>
    {
        for (; node.Left is not null; node = node.Left) ;
        return node;
    }

    /// <summary>
    /// Retrieves the maximum value that can be found in the subtree (meaning the rightmost leaf).
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="node">The root of the subtree to search in.</param>
    /// <returns>The maximum (rightmost) node.</returns>
    public static TNode Max<TNode>(TNode node)
        where TNode : class, INode<TNode>
    {
        for (; node.Right is not null; node = node.Right) ;
        return node;
    }

    /// <summary>
    /// Retrieves the predecessor of a given node (the largest value that is smaller, meaning the rightmost element
    /// in the left subtree) that can be found in the subtree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="node">The node to get the predecessor of.</param>
    /// <returns>The predecessor of the node, if any.</returns>
    public static TNode? Predecessor<TNode>(TNode node)
        where TNode : class, INode<TNode> => node.Left is null
        ? null
        : Max(node.Left);

    /// <summary>
    /// Retrieves the successor of a given node (the smallest value that is larger, meaning the leftmost element
    /// in the right subtree) that can be found in the subtree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="node">The node to get the successor of.</param>
    /// <returns>The successor of the node, if any.</returns>
    public static TNode? Successor<TNode>(TNode node)
        where TNode : class, INode<TNode> => node.Right is null
        ? null
        : Min(node.Right);

    /// <summary>
    /// Finds a node with a given key.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <param name="root">The root of the subtree to search in.</param>
    /// <param name="key">The key to search for.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="comparer">The key comparer.</param>
    /// <returns>The node with the given key, or null, if not found.</returns>
    public static TNode? Find<TNode, TKey>(
        TNode? root,
        TKey key,
        Func<TNode, TKey> keySelector,
        IComparer<TKey> comparer)
        where TNode : class, INode<TNode>
    {
        while (root is not null)
        {
            var rootKey = keySelector(root);
            var cmp = comparer.Compare(key, rootKey);
            if (cmp < 0) root = root.Left;
            else if (cmp > 0) root = root.Right;
            else return root;
        }
        return null;
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
