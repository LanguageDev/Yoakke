using System;
using System.Collections.Generic;
using System.Linq;
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
        public IReadOnlyDictionary<string, Rule> Rules => this.rules;

        private readonly Dictionary<string, Rule> rules = new();

        /// <inheritdoc/>
        public override string ToString() => string.Join("\n", this.rules.Values);

        /// <summary>
        /// Adds a <see cref="Rule"/> to this grammar. If a <see cref="Rule"/> with the same name is already present,
        /// the alternatives will be added.
        /// </summary>
        /// <param name="rule">The <see cref="Rule"/> to add.</param>
        public void Add(Rule rule)
        {
            if (this.rules.TryGetValue(rule.Name, out var existing))
            {
                var allAlternatives = rule.Alternatives.Concat(existing.Alternatives).ToList();
                rule = new(rule.Name, allAlternatives);
            }
            this.rules[rule.Name] = rule;
        }

        /// <summary>
        /// Adds a <see cref="RuleAlternative"/> to this grammar. If a <see cref="Rule"/> with <paramref name="name"/> is
        /// not present yet, it is created.
        /// </summary>
        /// <param name="name">The name of the <see cref="Rule"/> the alternative belongs to.</param>
        /// <param name="alternative">The rule alternative to add.</param>
        public void Add(string name, RuleAlternative alternative) => this.Add(new Rule(name, new[] { alternative }));

        /// <summary>
        /// Splits all or nodes into separate rule alternatives.
        /// </summary>
        public void SplitOrAlternatives()
        {
            var keys = this.rules.Keys.ToList();
            foreach (var key in keys) this.rules[key] = this.rules[key].SplitOrAlternatives();
        }
    }
}
