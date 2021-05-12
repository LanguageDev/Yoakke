using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Yoakke.Parser.Generator.Ast;

namespace Yoakke.Parser.Generator
{
    internal class RuleSet
    {
        /// <summary>
        /// The rule name (left of grammar) mapping to the different possible rules (basically alternatives).
        /// </summary>
        public IDictionary<string, Rule> Rules { get; private set; } = new Dictionary<string, Rule>();

        private Dictionary<string, IList<PrecedenceEntry>> precedences = new();

        public bool TryGetRule(string name, out Rule rule) => Rules.TryGetValue(name, out rule);
        public Rule GetRule(string name) => Rules[name];

        public void Add(Rule rule)
        {
            if (Rules.TryGetValue(rule.Name, out var existingRule))
            {
                // Combine them with an alternation
                existingRule.Ast = new BnfAst.Alt(existingRule.Ast, rule.Ast);
            }
            else
            {
                Rules.Add(rule.Name, rule);
            }
        }

        public void AddPrecedence(string ruleName, IList<(bool Left, HashSet<object> Operators)> precs, IMethodSymbol method)
        {
            if (!precedences.TryGetValue(ruleName, out var precList))
            {
                precList = new List<PrecedenceEntry>();
                precedences.Add(ruleName, precList);
            }
            foreach (var (l, p) in precs) precList.Add(new PrecedenceEntry(l, p, method));
        }

        public void Desugar()
        {
            // Generate precedence rules
            foreach (var kv in precedences)
            {
                if (!Rules.TryGetValue(kv.Key, out var rule)) continue;
                Rules.Remove(kv.Key);
                var newRules = BnfDesugar.GeneratePrecedenceParser(rule, kv.Value);
                foreach (var r in newRules) Add(r);
            }
            // Desugar AST nodes
            foreach (var r in Rules.Values) r.Ast = r.Ast.Desugar();
            // Eliminate left-recursion
            Rules = Rules.Values
                .Select(BnfDesugar.EliminateLeftRecursion)
                .ToDictionary(r => r.Name);
        }
    }
}
