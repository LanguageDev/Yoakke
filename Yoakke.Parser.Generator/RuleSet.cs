using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
