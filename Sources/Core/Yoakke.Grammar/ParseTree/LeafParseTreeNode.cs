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
    /// A leaf node of the parse-tree, representing a single terminal.
    /// </summary>
    public sealed class LeafParseTreeNode : IParseTreeNode
    {
        /// <inheritdoc/>
        public Symbol Symbol { get; }

        /// <inheritdoc/>
        public IReadOnlyCollection<IParseTreeNode> Children => Array.Empty<IParseTreeNode>();

        /// <summary>
        /// The terminal read from the input.
        /// </summary>
        public object Terminal { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LeafParseTreeNode"/> class.
        /// </summary>
        /// <param name="symbol">The terminal symbol that matched the input.</param>
        /// <param name="terminal">The actual terminal object read.</param>
        public LeafParseTreeNode(Terminal symbol, object terminal)
        {
            this.Symbol = symbol;
            this.Terminal = terminal;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is IParseTreeNode other && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(IParseTreeNode other) =>
               other is LeafParseTreeNode leaf
            && this.Symbol.Equals(leaf.Symbol)
            && this.Terminal.Equals(leaf.Terminal);

        /// <inheritdoc/>
        public override int GetHashCode() => HashCode.Combine(this.Symbol, this.Terminal);
    }
}
