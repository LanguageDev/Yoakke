// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;

namespace Yoakke.Grammar.BnfAst
{
    /// <summary>
    /// A BNF node that merges two nodes in sequence.
    /// </summary>
    public record BnfSeqNode(IBnfNode First, IBnfNode Second) : BnfNodeBase
    {
        /// <inheritdoc/>
        public override IEnumerable<KeyValuePair<int, IBnfNode>> TraverseLeaves(bool reverse, int offset)
        {
            var lastOffset = offset;
            if (reverse)
            {
                foreach (var (nextOffset, node) in this.Second.TraverseLeaves(true, lastOffset))
                {
                    lastOffset = nextOffset;
                    yield return new(nextOffset, node);
                }
                foreach (var (nextOffset, node) in this.First.TraverseLeaves(true, lastOffset))
                {
                    lastOffset = nextOffset;
                    yield return new(nextOffset, node);
                }
            }
            else
            {
                foreach (var (nextOffset, node) in this.First.TraverseLeaves(false, lastOffset))
                {
                    lastOffset = nextOffset;
                    yield return new(nextOffset, node);
                }
                foreach (var (nextOffset, node) in this.Second.TraverseLeaves(false, lastOffset))
                {
                    lastOffset = nextOffset;
                    yield return new(nextOffset, node);
                }
            }
        }

        /// <inheritdoc/>
        protected override IBnfNode ReplaceChildrenByReference(IBnfNode find, IBnfNode replace) =>
            new BnfSeqNode(this.First.ReplaceByReference(find, replace), this.Second.ReplaceByReference(find, replace));
    }
}
