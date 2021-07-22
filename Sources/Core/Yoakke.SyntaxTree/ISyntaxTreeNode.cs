// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.SyntaxTree
{
    /// <summary>
    /// Interface for all syntax tree nodes.
    /// </summary>
    public interface ISyntaxTreeNode
    {
        /// <summary>
        /// The number of children this node has.
        /// </summary>
        public int ChildCount { get; }

        /// <summary>
        /// Gets a child of this <see cref="ISyntaxTreeNode"/>.
        /// </summary>
        /// <param name="index">The index of the child to get. Must be between 0 (inclusive) and
        /// <see cref="ChildCount"/> (exclusive).</param>
        /// <returns>The child with the given <paramref name="index"/> index.</returns>
        public object GetChild(int index);
    }
}
