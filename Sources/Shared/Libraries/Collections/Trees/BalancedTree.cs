// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Collections.Trees;

/// <summary>
/// Utilities to handle tree nodes as a balanced binary tree.
/// </summary>
public static class BalancedTree
{
    /// <summary>
    /// Interface for balanced binary tree nodes with a height.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    public interface IHeight<TNode> : BinaryTree.INode<TNode>
        where TNode : class, BinaryTree.INode<TNode>, IHeight<TNode>
    {
        /// <summary>
        /// The height of the subtree.
        /// </summary>
        public int Height { get; set; }
    }

    /// <summary>
    /// Retrieves the balance of a given tree node.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="node">The node to retrieve the balance of.</param>
    /// <returns>The balance of <paramref name="node"/>, which is the difference of the left and right
    /// subtree height.</returns>
    public static int BalanceFactor<TNode>(TNode node)
        where TNode : class, IHeight<TNode> =>
        (node.Left?.Height ?? 0) - (node.Right?.Height ?? 0);

    /// <summary>
    /// Updates the height of a node based on the descendants.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="node">The node to update the height of.</param>
    public static void UpdateHeight<TNode>(TNode node)
        where TNode : class, IHeight<TNode> =>
        node.Height = Math.Max(node.Left?.Height ?? 0, node.Right?.Height ?? 0) + 1;

    /// <summary>
    /// Rotates the subtree left and updates the height.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="root">The root of the subtree to rotate.</param>
    /// <returns>The new root of the subtree.</returns>
    public static TNode RotateLeft<TNode>(TNode root)
        where TNode : class, IHeight<TNode>
    {
        root = BinaryTree.RotateLeft(root);
        UpdateHeight(root.Left!);
        UpdateHeight(root);
        return root;
    }

    /// <summary>
    /// Rotates the subtree right and updates the height.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="root">The root of the subtree to rotate.</param>
    /// <returns>The new root of the subtree.</returns>
    public static TNode RotateRight<TNode>(TNode root)
        where TNode : class, IHeight<TNode>
    {
        root = BinaryTree.RotateRight(root);
        UpdateHeight(root.Right!);
        UpdateHeight(root);
        return root;
    }
}
