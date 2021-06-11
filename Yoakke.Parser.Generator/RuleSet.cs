using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Parser.Generator.Ast;

namespace Yoakke.Parser.Generator
{
    /// <summary>
    /// Represents a set of BNF <see cref="Rule"/> definitions.
    /// </summary>
    internal class RuleSet
    {
        /// <summary>
        /// The rule name (left of grammar) mapping to the different possible rules (basically alternatives).
        /// </summary>
        public IDictionary<string, Rule> Rules { get; private set; } = new Dictionary<string, Rule>();

        private readonly Dictionary<string, IList<PrecedenceEntry>> precedences = new();

        public bool TryGetRule(string name, out Rule rule) => this.Rules.TryGetValue(name, out rule);
        public Rule GetRule(string name) => this.Rules[name];

        public void Add(Rule rule)
        {
            if (this.Rules.TryGetValue(rule.Name, out var existingRule))
            {
                // Combine them with an alternation
                existingRule.Ast = new BnfAst.Alt(existingRule.Ast, rule.Ast);
            }
            else
            {
                this.Rules.Add(rule.Name, rule);
            }
        }

        public void AddPrecedence(string ruleName, IList<(bool Left, HashSet<object> Operators)> precs, IMethodSymbol method)
        {
            if (!this.precedences.TryGetValue(ruleName, out var precList))
            {
                precList = new List<PrecedenceEntry>();
                this.precedences.Add(ruleName, precList);
            }
            foreach (var (l, p) in precs) precList.Add(new PrecedenceEntry(l, p, method));
        }

        public void Desugar()
        {
            // Generate precedence rules
            foreach (var kv in this.precedences)
            {
                if (!this.Rules.TryGetValue(kv.Key, out var rule)) continue;
                this.Rules.Remove(kv.Key);
                var newRules = BnfDesugar.GeneratePrecedenceParser(rule, kv.Value.Reverse().ToList());
                foreach (var r in newRules) this.Add(r);
            }
            // Desugar AST nodes
            foreach (var r in this.Rules.Values) r.Ast = r.Ast.Desugar();
            // Eliminate left-recursion
            this.Rules = this.Rules.Values
                .Select(BnfDesugar.EliminateLeftRecursion)
                .ToDictionary(r => r.Name);
        }
    }
}
