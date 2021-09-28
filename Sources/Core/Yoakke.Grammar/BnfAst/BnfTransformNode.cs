// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.BnfAst
{
    /// <summary>
    /// Represents a transtormation node.
    /// </summary>
    public record BnfTransformNode(IBnfNode Element, object Transformer) : BnfNodeBase
    {
        /// <inheritdoc/>
        public override int Precedence => int.MaxValue - 1;

        /// <inheritdoc/>
        public override IEnumerable<IBnfNode> Traverse()
        {
            yield return this;
            foreach (var node in this.Element.Traverse()) yield return node;
        }

        /// <inheritdoc/>
        protected override IBnfNode ReplaceChildrenByReference(IBnfNode find, IBnfNode replace) =>
            new BnfTransformNode(this.Element.ReplaceByReference(find, replace), this.Transformer);

        /// <inheritdoc/>
        public override string ToString() => $"{this.ChildToString(this.Element)} => {this.Transformer}";
    }
}
