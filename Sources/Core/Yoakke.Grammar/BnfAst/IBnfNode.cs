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
        /// The precedence of the represented operation. Used for printing.
        /// </summary>
        public int Precedence { get; }

        /// <summary>
        /// Traverses all children of this node in a preorder manner.
        /// </summary>
        /// <returns>The traversed sequence of the elements.</returns>
        public IEnumerable<IBnfNode> Traverse();

        /// <summary>
        /// Replaces a node by reference.
        /// </summary>
        /// <param name="find">The node to replace.</param>
        /// <param name="replace">The node to replace with.</param>
        /// <returns>A new node, where <paramref name="find"/> is replaced by <paramref name="replace"/>.</returns>
        public IBnfNode ReplaceByReference(IBnfNode find, IBnfNode replace);
    }
}
