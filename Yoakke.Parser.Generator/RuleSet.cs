using System;
using System.Collections.Generic;
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

        public void Desugar()
        {
            Rules = Rules.Values
                .Select(BnfDesugar.EliminateLeftRecursion)
                .ToDictionary(r => r.Name);
        }
    }
}
