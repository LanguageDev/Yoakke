// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.BnfAst
{
    /// <summary>
    /// Represents a left-fold node.
    /// </summary>
    public record BnfFoldlNode(IBnfNode First, IBnfNode Second, object Transformer) : BnfNodeBase
    {
        /// <inheritdoc/>
        public override int Precedence => int.MaxValue - 1;

        /// <inheritdoc/>
        public override IEnumerable<IBnfNode> Traverse()
        {
            yield return this;
            foreach (var node in this.First.Traverse()) yield return node;
            foreach (var node in this.Second.Traverse()) yield return node;
        }

        /// <inheritdoc/>
        protected override IBnfNode ReplaceChildrenByReference(IBnfNode find, IBnfNode replace) => new BnfFoldlNode(
            this.First.ReplaceByReference(find, replace),
            this.Second.ReplaceByReference(find, replace),
            this.Transformer);

        /// <inheritdoc/>
        public override string ToString() => $"foldl({this.First}, {this.Second}, {this.Transformer})";
    }
}
