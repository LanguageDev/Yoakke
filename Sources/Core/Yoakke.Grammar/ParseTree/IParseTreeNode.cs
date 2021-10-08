// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Grammar.Cfg;

namespace Yoakke.Grammar.ParseTree
{
    /// <summary>
    /// Represents a single node in a parse-tree.
    /// </summary>
    public interface IParseTreeNode
    {
        /// <summary>
        /// The symbol that produced this node.
        /// </summary>
        public Symbol Symbol { get; }

        /// <summary>
        /// The child nodes of this one.
        /// </summary>
        public IReadOnlyCollection<IParseTreeNode> Children { get; }
    }
}
