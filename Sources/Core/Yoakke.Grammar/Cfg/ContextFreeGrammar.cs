// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
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

        /// <summary>
        /// Checks, if a production can end in an empty word (epsilon).
        /// </summary>
        /// <param name="name">The rule to check.</param>
        /// <returns>True, if the rule with <paramref name="name"/> can result in an emoty word (epsilon).</returns>
        public bool HasEpsilonProduction(string name)
        {
            if (!this.productionRules.TryGetValue(name, out var productions)) return false;
            return productions.Any(p => p.Symbols.Count == 0);
        }

        /// <summary>
        /// Calculates the <see cref="FirstSet"/> of some symbol.
        /// </summary>
        /// <param name="symbol">The symbol to calculate the first-set of.</param>
        /// <returns>The first-set of <paramref name="symbol"/>.</returns>
        public FirstSet First(Symbol symbol)
        {
            var result = new HashSet<Symbol.Terminal>();
            var hasEpsilon = false;

            if (symbol is Symbol.Nonterminal nonterm
             && this.HasEpsilonProduction(nonterm.Rule)) hasEpsilon = true;

            var touched = new HashSet<Symbol>();
            var stk = new Stack<Symbol>();

            touched.Add(symbol);
            stk.Push(symbol);

            while (stk.TryPop(out var top))
            {
                if (top is Symbol.Terminal term)
                {
                    result.Add(term);
                }
                else
                {
                    nonterm = (Symbol.Nonterminal)top;
                    foreach (var prod in this.GetProductions(nonterm.Rule))
                    {
                        var allEpsilon = true;
                        foreach (var sym in prod.Symbols)
                        {
                            if (sym is Symbol.Terminal t)
                            {
                                allEpsilon = false;
                                result.Add(t);
                                break;
                            }
                            var nt = (Symbol.Nonterminal)sym;
                            if (touched.Add(nt)) stk.Push(nt);
                            if (this.HasEpsilonProduction(nt.Rule)) continue;
                            allEpsilon = false;
                            break;
                        }
                        hasEpsilon = hasEpsilon || allEpsilon;
                    }
                }
            }

            return new(symbol, hasEpsilon, result);
        }

        /// <summary>
        /// Calculates the <see cref="FirstSet"/> of a sequence of symbols.
        /// </summary>
        /// <param name="symbols">The sequence of symbols to calculate the first-set of.</param>
        /// <returns>The first-set of <paramref name="symbols"/>.</returns>
        public FirstSet First(IEnumerable<Symbol> symbols)
        {
            var result = new HashSet<Symbol.Terminal>();
            var hasEpsilon = true;

            foreach (var sym in symbols)
            {
                if (sym is Symbol.Terminal t)
                {
                    hasEpsilon = false;
                    result.Add(t);
                    break;
                }

                var first = this.First(sym);
                foreach (var item in first.Terminals) result.Add(item);
                if (!first.HasEmpty)
                {
                    hasEpsilon = false;
                    break;
                }
            }

            return new(symbols.ToList(), hasEpsilon, result);
        }
    }
}
