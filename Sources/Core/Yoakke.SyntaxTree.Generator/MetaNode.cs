// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Yoakke.SyntaxTree.Generator
{
    /// <summary>
    /// A helper to build a hierarchy of syntax tree nodes.
    /// </summary>
    internal class MetaNode
    {
        /// <summary>
        /// The node symbol.
        /// </summary>
        public INamedTypeSymbol NodeClass { get; }

        /// <summary>
        /// The child nodes.
        /// </summary>
        public IList<MetaNode> Children { get; } = new List<MetaNode>();

        /// <summary>
        /// The different overrides for different visitors.
        /// </summary>
        public IDictionary<Visitor, VisitorOverride> VisitorOverrides { get; } = new Dictionary<Visitor, VisitorOverride>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaNode"/> class.
        /// </summary>
        /// <param name="node">The node symbol.</param>
        public MetaNode(INamedTypeSymbol node)
        {
            this.NodeClass = node;
        }
    }
}
