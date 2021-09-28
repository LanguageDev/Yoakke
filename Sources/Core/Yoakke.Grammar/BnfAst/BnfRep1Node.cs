// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.BnfAst
{
    /// <summary>
    /// Represents a 1-or-more repetition BNF node.
    /// </summary>
    public record BnfRep1Node(IBnfNode Element) : BnfNodeBase
    {
        /// <inheritdoc/>
        public override int Precedence => 2;

        /// <inheritdoc/>
        public override IEnumerable<IBnfNode> Traverse()
        {
            yield return this.Element;
            foreach (var node in this.Element.Traverse()) yield return node;
        }

        /// <inheritdoc/>
        protected override IBnfNode ReplaceChildrenByReference(IBnfNode find, IBnfNode replace) =>
            new BnfRep1Node(this.Element.ReplaceByReference(find, replace));

        /// <inheritdoc/>
        public override string ToString() => $"{this.ChildToString(this.Element)}+";
    }
}
