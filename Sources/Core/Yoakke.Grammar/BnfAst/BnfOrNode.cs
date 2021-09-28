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
        public override IEnumerable<KeyValuePair<int, IBnfNode>> TraverseLeaves(bool reverse, int offset) => this.First
            .TraverseLeaves(reverse, offset)
            .Concat(this.Second.TraverseLeaves(reverse, offset));

        /// <inheritdoc/>
        protected override IBnfNode ReplaceChildrenByReference(IBnfNode find, IBnfNode replace) =>
            new BnfOrNode(this.First.ReplaceByReference(find, replace), this.Second.ReplaceByReference(find, replace));
    }
}
