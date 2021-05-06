using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Parser.Generator.Ast;

namespace Yoakke.Parser.Generator
{
    internal class RuleSet
    {
        /// <summary>
        /// The rule name (left of grammar) mapping to the different possible rules (basically alternatives).
        /// </summary>
        public readonly IDictionary<string, IList<Rule>> Rules = new Dictionary<string, IList<Rule>>();

        public void Add(Rule rule)
        {
            if (!Rules.TryGetValue(rule.Name, out var ruleList))
            {
                ruleList = new List<Rule>();
                Rules.Add(rule.Name, ruleList);
            }
            ruleList.Add(rule);
        }

        /// <summary>
        /// Normalizes the top-level alternatives until they are either sequences or atoms
        /// </summary>
        public void Normalize()
        {
            foreach (var rules in Rules.Values) Normalize(rules);
        }

        private void Normalize(IList<Rule> rules)
        {
            for (int i = 0; i < rules.Count;)
            {
                var rule = rules[i];
                var ast = rules[i].Ast;
                if (ast is BnfAst.Alt alt)
                {
                    // The top-level node is an alternative, split it up
                    var first = new Rule(rule.Name, alt.First, rule.Method);
                    var second = new Rule(rule.Name, alt.Second, rule.Method);
                    rules[i] = first;
                    rules.Insert(i + 1, second);
                }
                else
                {
                    ++i;
                }
            }
        }
    }
}
