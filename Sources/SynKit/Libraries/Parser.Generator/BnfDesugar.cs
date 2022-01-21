// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Yoakke.SynKit.Parser.Generator.Ast;
using Yoakke.SynKit.Parser.Generator.Model;

namespace Yoakke.SynKit.Parser.Generator;

/// <summary>
/// Utilities for various external desugaring steps for <see cref="Rule"/>s.
/// </summary>
internal static class BnfDesugar
{
    /// <summary>
    /// Eliminates left-recursion in a set of <see cref="Rule"/>s.
    /// </summary>
    /// <param name="rules">The left-recursion eliminated <see cref="Rule"/>.</param>
    public static void EliminateLeftRecursion(IDictionary<string, Rule> rules)
    {
        void DesugarRuleSet()
        {
            foreach (var rule in rules.Values) rule.Ast = rule.Ast.Desugar();
        }

        // Desugar the rules
        DesugarRuleSet();

        // First we need to find the indirect cycles
        var indirectCycles = FindLeftRecursionCycles(rules)
            .Where(cycle => cycle.Count > 1)
            .ToList();

        // Figure out the least amount of rules we need to transform to remove all cycles
        var rulesToEliminate = LeastAmountOfRulesToEliminateCycles(indirectCycles);

        // Eliminate the indirect cycles
        foreach (var ruleToEliminate in rulesToEliminate)
        {
            // We find all cycles that need to be eliminated
            foreach (var cycle in indirectCycles.Where(cycle => cycle.Contains(ruleToEliminate)))
            {
                EliminateIndirectLeftRecursionCycle(ruleToEliminate, cycle);
            }
        }

        // Desugar again for simplicity
        DesugarRuleSet();

        // All remaining cycles have to be direct left-recursion
        var directCycles = FindLeftRecursionCycles(rules);
        if (!directCycles.All(cycle => cycle.Count == 1)) throw new InvalidOperationException("Indirect cycle elimination failed: indirect cycles remained.");
        // Eliminate direct left-recursion
        var directLrRules = directCycles.Select(c => c[0]);
        foreach (var rule in directLrRules) EliminateDirectLeftRecursion(rule);

        // Final desugaring step
        DesugarRuleSet();
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

    /* Left-recursion */

    private static List<List<Rule>> FindLeftRecursionCycles(IDictionary<string, Rule> rules)
    {
        var result = new List<List<Rule>>();
        var touched = new HashSet<Rule>();

        IEnumerable<Rule> GetFirstCalls(BnfAst ast) => ast
            .GetFirstCalls()
            .SelectMany(call => rules.TryGetValue(call.Name, out var rule) ? new[] { rule } : Enumerable.Empty<Rule>());

        void Cycle(List<Rule> ruleStack, Rule current)
        {
            var index = ruleStack.IndexOf(current);
            if (index != -1)
            {
                // Found a cycle, only store it, if it's an indirect one (<=> the stack has more than 1 element)
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

    private static List<Rule> LeastAmountOfRulesToEliminateCycles(List<List<Rule>> cycles)
    {
        // We just count which rule occurs in the most cycles
        var mostOccurrences = new Dictionary<Rule, int>();
        foreach (var cycle in cycles)
        {
            foreach (var rule in cycle)
            {
                if (!mostOccurrences.TryGetValue(rule, out var count)) count = 0;
                mostOccurrences[rule] = count + 1;
            }
        }

        // Start eliminating cycles
        var usedRules = new List<Rule>();
        var remainingCycles = cycles.ToList();
        var mostOccurredByDesc = mostOccurrences
            .OrderByDescending(kv => kv.Value)
            .Select(kv => kv.Key)
            .ToList();
        foreach (var rule in mostOccurredByDesc)
        {
            if (remainingCycles.Count == 0) break;

            var used = false;
            for (var i = 0; i < remainingCycles.Count;)
            {
                if (remainingCycles[i].Contains(rule))
                {
                    remainingCycles.RemoveAt(i);
                    used = true;
                }
                else
                {
                    ++i;
                }
            }

            if (used) usedRules.Add(rule);
        }

        return usedRules;
    }

    /* Indirect left-recursion */

    private static void EliminateIndirectLeftRecursionCycle(Rule start, List<Rule> cycle)
    {
        var offset = cycle.IndexOf(start);
        var rule = cycle[offset];
        // Transform the ith rule in the cycle into a direct left-recursive one
        // All the subsequent rules will be substituted one by one
        for (var j = 1; j < cycle.Count; ++j)
        {
            // Get the rule we are substituting
            var index = (offset + j) % cycle.Count;
            var ruleToSubstitute = cycle[index];
            var callsToRule = rule.Ast
                .GetFirstCalls()
                .Where(n => n.Name == ruleToSubstitute.Name);
            foreach (var callToRule in callsToRule)
            {
                rule.Ast = rule.Ast.SubstituteByReference(callToRule, ruleToSubstitute.Ast);
            }
        }
    }

    /* Direct left-recursion */

    /*
     We need to find alternatives that are left-recursive.
     For example:
     A -> A + X => { ... }
        | A - Y => { ... }
        | C
        | D
     In this case we need to collect the first 2 alternatives.
     Then we need to make it into:
     A -> FoldLeft((C | D)(X | Y)*)
     */

    private static void EliminateDirectLeftRecursion(Rule rule)
    {
        // Check if this is even an alternation case
        // If not, we just skip it, it's probably not recursive or we can't eliminate it
        if (rule.Ast is not BnfAst.Alt alt) return;
        var alphas = new List<BnfAst>();
        var betas = new List<BnfAst>();
        foreach (var alternative in alt.Elements)
        {
            var calls = alternative.GetFirstCalls()
                .Where(call => call.Name == rule.Name)
                .ToList();
            if (calls.Count == 0)
            {
                // Non-left-recursive
                betas.Add(alternative);
            }
            else
            {
                // Is left-recursive
                var alpha = alternative;
                var placeholder = new BnfAst.Placeholder(alpha);
                foreach (var call in calls) alpha = alpha.SubstituteByReference(call, placeholder);
                alphas.Add(alpha);
            }
        }

        // Sanity check
        if (alphas.Count == 0 || betas.Count == 0) throw new InvalidOperationException($"Cannot eliminate direct left-recursion! ({rule.Name})");
        // Construct the fold
        rule.Ast = new BnfAst.FoldLeft(new BnfAst.Alt(betas), new BnfAst.Alt(alphas));
    }
}
