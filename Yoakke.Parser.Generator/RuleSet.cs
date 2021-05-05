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
    }
}
