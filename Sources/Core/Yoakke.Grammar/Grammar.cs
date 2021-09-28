using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar
{
    /// <summary>
    /// Represents a set of grammar rules.
    /// </summary>
    public class Grammar
    {
        /// <summary>
        /// The rules in the grammar.
        /// </summary>
        public IReadOnlyDictionary<string, GrammarRule> Rules => this.rules;

        private readonly Dictionary<string, GrammarRule> rules = new();
    }
}
