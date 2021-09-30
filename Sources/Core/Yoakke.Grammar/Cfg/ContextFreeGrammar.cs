// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Grammar.Cfg
{
    /// <summary>
    /// Represents some context-free grammar in its purest form, with production rules.
    /// </summary>
    public sealed class ContextFreeGrammar
    {
        /// <summary>
        /// The start symbol.
        /// </summary>
        public string? StartSymbol { get; set; }

        /// <summary>
        /// All production rules in this grammar.
        /// </summary>
        public IEnumerable<ProductionRule> Productions => this.productionRules.Values.SelectMany(x => x);

        /// <summary>
        /// All the terminals in this grammar.
        /// </summary>
        public IEnumerable<Symbol.Terminal> Terminals => this.Productions
            .OfType<Symbol.Terminal>()
            .Distinct();

        /// <summary>
        /// All the nonterminals in this grammar.
        /// </summary>
        public IEnumerable<Symbol.Nonterminal> Nonterminals => this.Productions
            .OfType<Symbol.Nonterminal>()
            .Concat(this.productionRules.Keys.Select(n => new Symbol.Nonterminal(n)))
            .Distinct();

        private readonly Dictionary<string, List<ProductionRule>> productionRules = new();

        /// <inheritdoc/>
        public override string ToString() => string.Join("\n", this.Productions);

        /// <summary>
        /// Adds a <see cref="ProductionRule"/> to this grammar.
        /// </summary>
        /// <param name="productionRule">The <see cref="ProductionRule"/> to add.</param>
        public void AddProduction(ProductionRule productionRule)
        {
            if (!this.productionRules.TryGetValue(productionRule.Name, out var existingList))
            {
                existingList = new();
                this.productionRules.Add(productionRule.Name, existingList);
            }
            existingList.Add(productionRule);
        }

        /// <summary>
        /// Retrieves the productions associated to a name.
        /// </summary>
        /// <param name="name">The name of the rule to get the productions for.</param>
        /// <returns>The productions belonging to the role with name <paramref name="name"/>.</returns>
        public IEnumerable<ProductionRule> GetProductions(string name) => this.productionRules.TryGetValue(name, out var rules)
            ? rules
            : Enumerable.Empty<ProductionRule>();
    }
}
