// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Grammar.BnfAst
{
    /// <summary>
    /// A BNF node that merges two alternative nodes.
    /// </summary>
    public record BnfOrNode(IBnfNode First, IBnfNode Second) : BnfNodeBase
    {
        /// <inheritdoc/>
        public override int Precedence => 0;

        /// <inheritdoc/>
        public override IEnumerable<IBnfNode> Traverse()
        {
            yield return this;
            foreach (var node in this.First.Traverse()) yield return node;
            foreach (var node in this.Second.Traverse()) yield return node;
        }

        /// <inheritdoc/>
        protected override IBnfNode ReplaceChildrenByReference(IBnfNode find, IBnfNode replace) =>
            new BnfOrNode(this.First.ReplaceByReference(find, replace), this.Second.ReplaceByReference(find, replace));

        /// <inheritdoc/>
        public override string ToString() => $"{this.ChildToString(this.First)} | {this.ChildToString(this.Second)}";
    }
}
