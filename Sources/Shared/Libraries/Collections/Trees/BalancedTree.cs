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
}
