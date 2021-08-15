// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Yoakke.Parser.Generator.Ast;

namespace Yoakke.Parser.Generator
{
    /// <summary>
    /// Utilities for various external desugaring steps for <see cref="Rule"/>s.
    /// </summary>
    internal static class BnfDesugar
    {
        /// <summary>
        /// Eliminates indirect left-recursion in a set of <see cref="Rule"/>s.
        /// </summary>
        /// <param name="rules">The left-recursion eliminated <see cref="Rule"/>.</param>
        public static void EliminateIndirectLeftRecursion(IDictionary<string, Rule> rules)
        {
            // TODO
        }

        /// <summary>
        /// Eliminates direct left-recursion in a <see cref="Rule"/>.
        /// </summary>
        /// <param name="rule">The <see cref="Rule"/> to eliminate direct left-recursion in.</param>
        /// <returns>The left-recursion eliminated <see cref="Rule"/>.</returns>
        public static Rule EliminateDirectLeftRecursion(Rule rule)
        {
            // TODO: We should probably do something a bit more sophisticated
            // Like what about potential grouping?
            // A good starting point could be what we do at GetFirstCall
            if (rule.Ast is BnfAst.Alt alt)
            {
                var newAlt = EliminateDirectLeftRecursionInAlternation(rule, alt);
                return new Rule(rule.Name, newAlt, rule.PublicApi) { VisualName = rule.VisualName };
            }
            return rule;
        }

        /// <summary>
        /// Generates a precedence-parser for a <see cref="Rule"/>.
        /// </summary>
        /// <param name="rule">The <see cref="Rule"/> that represents the operants for a precedence-parser.</param>
        /// <param name="precedenceTable">The precedence table, going from the highest-precedence entry to the lowest one.</param>
        /// <returns>The new set of <see cref="Rule"/>s that correctly parse with precedence.</returns>
        public static IList<Rule> GeneratePrecedenceParser(Rule rule, IList<PrecedenceEntry> precedenceTable)
        {
            // The resulting precedence rules should look like
            // RULE_N : (RULE_N OP RULE_(N+1)) {TR} | RULE_(N+1) for left-associative
            // RULE_N : (RULE_(N+1) OP RULE_N) {TR} | RULE_(N+1) for right-associative
            // And simply the passed-in rule as atomic
            var result = new List<Rule>();
            var atom = new Rule($"{rule.Name}_atomic", rule.Ast, false);
            result.Add(atom);
            for (var i = 0; i < precedenceTable.Count; ++i)
            {
                var prec = precedenceTable[i];
                var currentCall = new BnfAst.Call(i == 0 ? rule.Name : $"{rule.Name}_level{i}");
                var nextCall = new BnfAst.Call(i == precedenceTable.Count - 1 ? atom.Name : $"{rule.Name}_level{i + 1}");

                BnfAst? toAdd = null;
                foreach (var op in prec.Operators)
                {
                    var opNode = new BnfAst.Literal(op);
                    var seq = prec.Left
                        ? new BnfAst[] { currentCall, opNode, nextCall }
                        : new BnfAst[] { nextCall, opNode, currentCall };
                    var alt = new BnfAst.Transform(new BnfAst.Seq(seq), prec.Method);
                    if (toAdd == null) toAdd = alt;
                    else toAdd = new BnfAst.Alt(toAdd, alt);
                }
                // Default is always stepping a level down
                if (toAdd is null) toAdd = nextCall;
                else toAdd = new BnfAst.Alt(toAdd, nextCall);

                result.Add(new Rule(currentCall.Name, toAdd, i == 0) { VisualName = rule.VisualName });
            }
            return result;
        }

        // Indirect left-recursion /////////////////////////////////////////////

        private static void EliminateIndirectLeftRecursionCycle(IDictionary<string, Rule> rules, List<Rule> cycle)
        {

        }

        private static List<List<Rule>> FindIndirectLeftRecursionCycles(IDictionary<string, Rule> rules)
        {
            var result = new List<List<Rule>>();
            var touched = new HashSet<Rule>();

            IEnumerable<Rule> GetFirstCalls(BnfAst ast) => ast switch
            {
                BnfAst.Alt alt => alt.Elements.SelectMany(GetFirstCalls),
                BnfAst.Seq seq => seq.Elements.Count > 0
                    ? GetFirstCalls(seq.Elements[0])
                    : Enumerable.Empty<Rule>(),
                BnfAst.Call call => rules.TryGetValue(call.Name, out var rule)
                    ? new[] { rule }
                    : Enumerable.Empty<Rule>(),
                BnfAst.Group grp => GetFirstCalls(grp.Subexpr),
                BnfAst.Transform trafo => GetFirstCalls(trafo.Subexpr),
                _ => Enumerable.Empty<Rule>(),
            };

            void Cycle(List<Rule> ruleStack, Rule current)
            {
                var index = ruleStack.IndexOf(current);
                if (index != -1)
                {
                    // TODO: If we remove this limit, we can just detect direct LR too, maybe useful to clean up that one
                    // Found a cycle, only store it, if it's an indirect one (<=> the stack has more than 1 element)
                    if (ruleStack.Count == 1) return;
                    result.Add(ruleStack.GetRange(index, ruleStack.Count - index));
                    return;
                }
                // If we already touched the rule, skip it from further processing
                if (!touched.Add(current)) return;
                // Otherwise we push on, go to each called subrule
                ruleStack.Add(current);
                // Get all the leftmost rules this one gets expanded into
                var nextRules = GetFirstCalls(current.Ast);
                // Try for cycles
                foreach (var next in nextRules) Cycle(ruleStack, next);
                ruleStack.RemoveAt(ruleStack.Count - 1);
            }

            foreach (var rule in rules.Values) Cycle(new(), rule);
            return result;
        }

        // Direct left-recursion ///////////////////////////////////////////////

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

        private static BnfAst EliminateDirectLeftRecursionInAlternation(Rule rule, BnfAst.Alt alt)
        {
            var alphas = new List<(BnfAst Node, IMethodSymbol Method)>();
            var betas = new List<BnfAst>();
            foreach (var child in alt.Elements)
            {
                // If the inside has no transformation, we don't care
                if (child is not BnfAst.Transform transform)
                {
                    betas.Add(child);
                    continue;
                }
                // We found a left-recursive sequence inside an alternative, add it as alpha
                if (transform.Subexpr is BnfAst.Seq seq && TrySplitDirectLeftRecursion(rule, seq, out var alpha))
                {
                    alphas.Add((alpha!, transform.Method));
                }
                else
                {
                    betas.Add(child);
                }
            }
            if (alphas.Count == 0 || betas.Count == 0) return alt;
            // We have left-recursion
            var betaNode = betas.Count == 1 ? betas[0] : new BnfAst.Alt(betas);
            return new BnfAst.FoldLeft(betaNode, alphas);
        }

        private static bool TrySplitDirectLeftRecursion(Rule rule, BnfAst.Seq seq, [MaybeNullWhen(false)] out BnfAst? alpha)
        {
            if (seq.Elements[0] is BnfAst.Call call && call.Name == rule.Name)
            {
                // Is left recursive, split out the left-recursive part, make the rest be in alpha
                if (seq.Elements.Count == 2) alpha = seq.Elements[1];
                else alpha = new BnfAst.Seq(seq.Elements.Skip(1));
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
