// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.BnfAst
{
    /// <summary>
    /// Represents a single BNF node in the BNF rule AST.
    /// </summary>
    public interface IBnfNode
    {
        /// <summary>
        /// True, if this is a leaf node.
        /// </summary>
        public bool IsLeaf { get; }

        /// <summary>
        /// Traverses all leaf children of this node.
        /// </summary>
        /// <param name="reverse">True, if the traversal should be in reverse order.</param>
        /// <param name="offset">The offset to start from.</param>
        /// <returns>The sequence of the elements and their match position to be traversed.</returns>
        public IEnumerable<KeyValuePair<int, IBnfNode>> TraverseLeaves(bool reverse, int offset);

        /// <summary>
        /// Replaces a node by reference.
        /// </summary>
        /// <param name="find">The node to replace.</param>
        /// <param name="replace">The node to replace with.</param>
        /// <returns>A new node, where <paramref name="find"/> is replaced by <paramref name="replace"/>.</returns>
        public IBnfNode ReplaceByReference(IBnfNode find, IBnfNode replace);
    }
}
