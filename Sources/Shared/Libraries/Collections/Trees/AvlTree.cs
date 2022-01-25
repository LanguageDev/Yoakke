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
    /// A structure describing the result of an insertion.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <param name="Root">The root of the tree.</param>
    /// <param name="Inserted">The inserted node, if any.</param>
    /// <param name="Existing">The existing node, if the insertion failed because of a duplicate key.</param>
    public record struct Insertion<TNode>(
        TNode Root,
        TNode? Inserted = null,
        TNode? Existing = null)
        where TNode : class, BinaryTree.INode<TNode>;

    /// <summary>
    /// Performs insertion on an AVL tree.
    /// </summary>
    /// <typeparam name="TNode">The node implementation type.</typeparam>
    /// <typeparam name="TKey">The inserted key type.</typeparam>
    /// <param name="root">The root of the AVL tree to insert into.</param>
    /// <param name="key">The key to insert.</param>
    /// <param name="makeNode">The function to construct a new node.</param>
    /// <param name="keySelector">The key selector.</param>
    /// <param name="keyComparer">The key comparer.</param>
    /// <returns>A structure describing the result of the insertion.</returns>
    public static Insertion<TNode> Insert<TNode, TKey>(
        TNode? root,
        TKey key,
        Func<TKey, TNode> makeNode,
        Func<TNode, TKey> keySelector,
        IComparer<TKey> keyComparer)
        where TNode : class, BalancedTree.IHeight<TNode>
    {
        if (root is null)
        {
            // Insertion into an empty tree is trivial, we just make it as the new root
            root = makeNode(key);
            return new(Root: root, Inserted: root);
        }

        // We compare the root key to the inserted one
        var cmp = keyComparer.Compare(key, keySelector(root));
        TNode inserted;
        if (cmp < 0)
        {
            // Insert into left subtree
            var insertion = Insert(root.Left, key, makeNode, keySelector, keyComparer);
            // Duplicate key, return early
            if (insertion.Existing is not null) return new(Root: root, Existing: insertion.Existing);
            // Store inserted
            inserted = insertion.Inserted!;
            root.Left = insertion.Root;
        }
        else if (cmp > 0)
        {
            // Insert into right subtree
            var insertion = Insert(root.Right, key, makeNode, keySelector, keyComparer);
            // Duplicate key, return early
            if (insertion.Existing is not null) return new(Root: root, Existing: insertion.Existing);
            // Store inserted
            inserted = insertion.Inserted!;
            root.Right = insertion.Root;
        }
        else
        {
            // Duplicate key, disallowed
            return new(Root: root, Existing: root);
        }

        // Update height
        BalancedTree.UpdateHeight(root);

        // Get the balance factor
        var balance = BalancedTree.BalanceFactor(root);

        // Do the 4 balancing cases
        if (balance > 1)
        {
            // Left cases
            cmp = keyComparer.Compare(key, keySelector(root.Left!));
            if (cmp < 0)
            {
                // Left-left case
                root = BalancedTree.RotateRight(root);
            }
            else if (cmp > 0)
            {
                // Left-right case
                root.Left = BalancedTree.RotateLeft(root.Left!);
                root = BalancedTree.RotateRight(root);
            }
        }
        else if (balance < -1)
        {
            // Right cases
            cmp = keyComparer.Compare(key, keySelector(root.Right!));
            if (cmp > 0)
            {
                // Right-right case
                root = BalancedTree.RotateLeft(root);
            }
            else if (cmp < 0)
            {
                // Right-left case
                root.Right = BalancedTree.RotateRight(root.Right!);
                root = BalancedTree.RotateLeft(root);
            }
        }

        // We are done
        return new(Root: root, Inserted: inserted);
    }
}
