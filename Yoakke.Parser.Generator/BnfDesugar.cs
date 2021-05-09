using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Yoakke.Parser.Generator.Ast;

namespace Yoakke.Parser.Generator
{
    internal static class BnfDesugar
    {
        public static Rule EliminateLeftRecursion(Rule rule)
        {
            if (rule.Ast is BnfAst.Alt alt)
            {
                var newAlt = EliminateLeftRecursionInAlternation(rule, alt);
                return new Rule(rule.Name, newAlt);
            }
            return rule;
        }

        /*
         We need to find alternatives that are left recursive
         The problem is that they might be inside transformations
         For example:
         A -> A + X => { ... }
            | A - Y => { ... }
            | C
            | D
         In this case we need to collect the first 2 alternatives (assuming same transformation)
         And we need to make it into:
         A -> FoldLeft((C | D)(X | Y)*)
         */

        private static BnfAst EliminateLeftRecursionInAlternation(Rule rule, BnfAst.Alt alt)
        {
            var alphas = new List<BnfAst>();
            var betas = new List<BnfAst>();
            IMethodSymbol fold = null;
            foreach (var child in alt.Elements)
            {
                // If the inside has no transformation, we don't care
                if (child is not BnfAst.Transform transform)
                {
                    betas.Add(child);
                    continue;
                }
                // We found a left-recursive sequence inside an alternative, add it as alpha
                if (transform.Subexpr is BnfAst.Seq seq && TrySplit(rule, seq, out var alpha))
                {
                    if (fold == null)
                    {
                        fold = transform.Method;
                    }
                    else if (!SymbolEqualityComparer.Default.Equals(fold, transform.Method))
                    {
                        throw new InvalidOperationException("Incompatible fold functions");
                    }
                    alphas.Add(alpha);
                }
                else betas.Add(child);
            }
            if (alphas.Count == 0) return alt;
            // We have left-recursion
            return new BnfAst.FoldLeft(new BnfAst.Alt(betas), new BnfAst.Alt(alphas), fold);
        }

        private static bool TrySplit(Rule rule, BnfAst.Seq seq, out BnfAst alpha)
        {
            if (seq.Elements[0] is BnfAst.Call call && call.Name == rule.Name)
            {
                // Is left recursive, split out the left-recursive part, make the rest be in alphy
                alpha = seq.Elements[1];
                for (int i = 2; i < seq.Elements.Count; ++i) alpha = new BnfAst.Seq(alpha, seq.Elements[i]);
                return true;
            }
            else
            {
                alpha = null;
                return false;
            }
        }
    }
}
