// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Grammar.Cfg;

namespace Yoakke.Grammar.ParseTree
{
    /// <summary>
    /// A parse-tree node created from a production.
    /// </summary>
    public sealed class ProductionParseTreeNode : IParseTreeNode
    {
        /// <inheritdoc/>
        public Symbol Symbol => this.Production.Left;

        /// <inheritdoc/>
        public IReadOnlyCollection<IParseTreeNode> Children { get; }

        /// <summary>
        /// The production resulting in this node.
        /// </summary>
        public Production Production { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductionParseTreeNode"/> class.
        /// </summary>
        /// <param name="production">The production creating this node.</param>
        /// <param name="children">The children of this node.</param>
        public ProductionParseTreeNode(Production production, IReadOnlyCollection<IParseTreeNode> children)
        {
            this.Production = production;
            this.Children = children;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is IParseTreeNode other && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(IParseTreeNode other) =>
               other is ProductionParseTreeNode prod
            && this.Production.Equals(prod.Production)
            && this.Children.SequenceEqual(other.Children);

        /// <inheritdoc/>
        // NOTE: Not exact, simplification
        public override int GetHashCode() => HashCode.Combine(this.Production, this.Children.Count);
    }
}
