// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Trees;

/// <summary>
/// Utilities to handle tree nodes as an AVL tree.
/// </summary>
public static class AvlTree
{
    /// <summary>
    /// Interface for AVL tree nodes.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    public interface INode<TNode> : BinarySearchTree.INode<TNode>
        where TNode : class, INode<TNode>
    {
        /// <summary>
        /// The height of the subtree.
        /// </summary>
        public int Height { get; set; }
    }

    /// <summary>
    /// Represents the results of rebalancing an AVL tree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="Root">The new root of the subtree that was rebalanced.</param>
    /// <param name="Rebalanced">True, if rebalancing did happen, false otherwise.</param>
    public record struct RebalanceResult<TNode>(TNode Root, bool Rebalanced)
        where TNode : class, INode<TNode>;

    /// <summary>
    /// A structure describing the result of an insertion.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="Root">The root of the tree.</param>
    /// <param name="Inserted">The inserted node, if any.</param>
    /// <param name="Existing">The existing node, if the insertion failed because of a duplicate key.</param>
    public record struct InsertResult<TNode>(
        TNode Root,
        TNode? Inserted = null,
        TNode? Existing = null)
        where TNode : class, INode<TNode>;

    /// <summary>
    /// Updates the height of a node based on its children.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="node">The node to update the height of.</param>
    public static void UpdateHeight<TNode>(TNode node)
        where TNode : class, INode<TNode> =>
        node.Height = Math.Max(node.Left?.Height ?? 0, node.Right?.Height ?? 0) + 1;

    /// <summary>
    /// Retrieves the balance factor of a node.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="node">The node to get the balance factor of.</param>
    /// <returns>The balance factor of <paramref name="node"/>, which is the height of its right child subtracted from
    /// the height of its left child.</returns>
    public static int BalanceFactor<TNode>(TNode node)
        where TNode : class, INode<TNode> =>
        (node.Left?.Height ?? 0) - (node.Right?.Height ?? 0);

    /// <summary>
    /// Rotates the subtree left and updates the heights.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="root">The root of the subtree to rotate.</param>
    /// <returns>The new root of the subtree.</returns>
    public static TNode RotateLeft<TNode>(TNode root)
        where TNode : class, INode<TNode>
    {
        root = BinarySearchTree.RotateLeft(root);
        UpdateHeight(root.Left!);
        UpdateHeight(root);
        return root;
    }

    /// <summary>
    /// Rotates the subtree right and updates the heights.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="root">The root of the subtree to rotate.</param>
    /// <returns>The new root of the subtree.</returns>
    public static TNode RotateRight<TNode>(TNode root)
        where TNode : class, INode<TNode>
    {
        root = BinarySearchTree.RotateRight(root);
        UpdateHeight(root.Right!);
        UpdateHeight(root);
        return root;
    }

    /// <summary>
    /// Performs a rebalancing step on an AVL node.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="root">The node to perform the rebalancing on.</param>
    /// <returns>The </returns>
    public static RebalanceResult<TNode> Rebalance<TNode>(TNode root)
        where TNode : class, INode<TNode>
    {
        var bf = BalanceFactor(root);
        if (bf < -1)
        {
            // Right-left
            if (BalanceFactor(root.Right!) > 0) RotateRight(root.Right!);
            // Right-right
            root = RotateLeft(root);
            return new(Root: root, Rebalanced: true);
        }
        else if (bf > 1)
        {
            // Left-right
            if (BalanceFactor(root.Left!) < 0) RotateLeft(root.Left!);
            // Left-left
            root = RotateRight(root);
            return new(Root: root, Rebalanced: true);
        }
        return new(Root: root, Rebalanced: false);
    }

    /// <summary>
    /// Performs insertion on an AVL tree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TKey">The inserted key type.</typeparam>
    /// <param name="root">The root of the AVL tree to insert into.</param>
    /// <param name="key">The key to insert.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="keyComparer">The key comparer.</param>
    /// <param name="makeNode">The function to construct a new node.</param>
    /// <returns>A structure describing the result of the insertion.</returns>
    public static InsertResult<TNode> Insert<TNode, TKey>(
        TNode? root,
        TKey key,
        Func<TNode, TKey> keySelector,
        IComparer<TKey> keyComparer,
        Func<TKey, TNode> makeNode)
        where TNode : class, INode<TNode>
    {
        // First we use the trivial BST insertion
        var insertion = BinarySearchTree.Insert(
            root: root,
            key: key,
            keySelector: keySelector,
            keyComparer: keyComparer,
            makeNode: makeNode);

        // If nothing is inserted, return here
        if (insertion.Inserted is null) return new(Root: root!, Existing: insertion.Existing);

        // There was a node inserted
        root = insertion.Root;

        // We start walking up the tree, updating the heights
        // If we find a node that needs rebalancing, we rebalance it, then stop
        for (var n = insertion.Inserted; n is not null; n = n.Parent)
        {
            UpdateHeight(n);
            var rebalance = Rebalance(n);
            if (ReferenceEquals(n, root)) root = rebalance.Root;
            if (rebalance.Rebalanced) break;
        }

        // We are done
        return new(Root: root, Inserted: insertion.Inserted);
    }

    /// <summary>
    /// Deletes a node from the AVL tree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="root">The root of the tree.</param>
    /// <param name="node">The node to delete.</param>
    /// <returns>The new root of the tree.</returns>
    public static TNode? Delete<TNode>(TNode? root, TNode node)
        where TNode : class, INode<TNode>
    {
        // NOTE: Mostly copied from BST.Delete
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
            // Update upwards
            for (var n = node.Parent; n is not null; n = n.Parent)
            {
                UpdateHeight(n);
                var rebalance = Rebalance(n);
                if (ReferenceEquals(n, root)) root = rebalance.Root;
            }
        }
        else if (node.Right is null)
        {
            // TODO
            throw new NotImplementedException();
            // 0 or 1 child
            Shift(node, node.Left);
        }
        else
        {
            // TODO
            throw new NotImplementedException();
            // 2 children
            var y = BinarySearchTree.Successor(node);
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
}
