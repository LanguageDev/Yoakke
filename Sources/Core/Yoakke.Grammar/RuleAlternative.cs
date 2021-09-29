// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;
using Yoakke.Grammar.BnfAst;

namespace Yoakke.Grammar
{
    /// <summary>
    /// A single alternative of a rule.
    /// </summary>
    public record RuleAlternative(IBnfNode Ast)
    {
        /// <inheritdoc/>
        public override string ToString() => this.Ast.ToString();

        /// <summary>
        /// Splits all or nodes into separate alternatives.
        /// </summary>
        /// <returns>The resulting sequence of alternatives, that contain no or nodes.</returns>
        public IEnumerable<RuleAlternative> SplitOrNodes()
        {
            var stk = new Stack<IBnfNode>();
            stk.Push(this.Ast);
            while (stk.TryPop(out var node))
            {
                var alt = node.Traverse().OfType<BnfOrNode>().FirstOrDefault();
                if (alt is null)
                {
                    yield return new(node);
                    continue;
                }

                var firstAlt = node.ReplaceByReference(alt, alt.First);
                var secondAlt = node.ReplaceByReference(alt, alt.Second);
                stk.Push(firstAlt);
                stk.Push(secondAlt);
            }
        }
    }
}
