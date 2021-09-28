// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.BnfAst
{
    /// <summary>
    /// Base for common BNF nodes.
    /// </summary>
    public abstract record BnfNodeBase : IBnfNode
    {
        /// <inheritdoc/>
        public virtual bool IsLeaf => false;

        /// <inheritdoc/>
        public abstract IEnumerable<KeyValuePair<int, IBnfNode>> TraverseLeaves(bool reverse, int offset);

        /// <inheritdoc/>
        public IBnfNode ReplaceByReference(IBnfNode find, IBnfNode replace) => ReferenceEquals(this, find)
            ? replace
            : this.ReplaceChildrenByReference(find, replace);

        /// <summary>
        /// Constructs a new instance with children replaced using <see cref="ReplaceByReference(IBnfNode, IBnfNode)"/>.
        /// </summary>
        /// <param name="find">The reference to find.</param>
        /// <param name="replace">The reference to replace with.</param>
        /// <returns>A new instance, where <paramref name="find"/> is rewplaced with <paramref name="replace"/>.</returns>
        protected abstract IBnfNode ReplaceChildrenByReference(IBnfNode find, IBnfNode replace);
    }
}
