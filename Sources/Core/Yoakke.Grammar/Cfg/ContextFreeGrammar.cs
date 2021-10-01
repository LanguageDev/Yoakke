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

        private readonly Dictionary<string, HashSet<ProductionRule>> productionRules = new();

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
        /// Retrieves the productions associated to a rule name.
        /// </summary>
        /// <param name="name">The name of the rule to get the productions for.</param>
        /// <returns>The productions belonging to the role with name <paramref name="name"/>.</returns>
        public IEnumerable<ProductionRule> GetProductions(string name) => this.productionRules[name];

        /// <summary>
        /// Checks, if a given symbol derives the empty word (epsilon).
        /// </summary>
        /// <param name="symbol">The symbol to check.</param>
        /// <returns>True, if <paramref name="symbol"/> derives the empty word.</returns>
        public bool DerivesEpsilon(Symbol symbol)
        {
            var touched = new HashSet<Symbol>();
            touched.Add(symbol);

            bool DerivesEpsilonImpl(Symbol symbol)
            {
                // A terminal can't derive the empty word
                if (symbol is Symbol.Terminal) return false;

                var productions = this.GetProductions(((Symbol.Nonterminal)symbol).Rule);

                // If any production derives the empty word, we also do
                if (productions.Any(p => p.Symbols.Count == 0)) return true;

                // We need to check, if any productions consist of nonterminals that all derive the empty word
                foreach (var production in productions)
                {
                    // Terminals can't derive to the empty word
                    if (production.Symbols.OfType<Symbol.Terminal>().Any()) continue;

                    // It only consists of nonterminals
                    var anyNew = false;
                    foreach (var s in production.Symbols)
                    {
                        if (!touched.Add(s)) continue;
                        // A nonterminal we haven't seen before
                        anyNew = true;
                        // If it doesn't derive epsilon, this production can't derive it
                        if (!DerivesEpsilonImpl(s)) goto retry;
                    }

                    // We do derive epsilon!
                    if (anyNew) return true;

                    retry: ;
                }

                // Haven't found anything deriving epsilon
                return false;
            }

            return DerivesEpsilonImpl(symbol);
        }

        /// <summary>
        /// Calculates the <see cref="FirstSet"/> of some symbol.
        /// </summary>
        /// <param name="symbol">The symbol to calculate the first-set of.</param>
        /// <returns>The first-set of <paramref name="symbol"/>.</returns>
        public FirstSet First(Symbol symbol)
        {
            var touched = new HashSet<Symbol>();
            touched.Add(symbol);

            FirstSet FirstImpl(Symbol symbol)
            {
                // If X is terminal, then FIRST(X) is { X }
                if (symbol is Symbol.Terminal term) return new(symbol, false, new[] { term });

                var productions = this.productionRules[((Symbol.Nonterminal)symbol).Rule];
                var result = new HashSet<Symbol.Terminal>();
                var hasEpsilon = false;

                // If X -> ε is a prodiction, then add ε to FIRST(X)
                if (productions.Any(p => p.Symbols.Count == 0)) hasEpsilon = true;

                foreach (var production in productions)
                {
                    // If X is a nonterminal and X -> Y1 Y2 ... Yk is a production then place 'a' in FIRST(X) if for some i,
                    // 'a' is in FIRST(Yi), and Y1 ... Y(i - 1) => ε
                    var allEpsilon = true;
                    foreach (var sym in production.Symbols)
                    {
                        if (touched.Add(sym))
                        {
                            // It is a new symbol, we can ask its FIRST
                            var first = FirstImpl(sym);
                            foreach (var item in first.Terminals) result.Add(item);
                            // If it contains ε, we can continue
                            if (first.HasEmpty) continue;
                            // Otherwise we are done with this production
                            allEpsilon = false;
                            break;
                        }
                        else if (!this.DerivesEpsilon(sym))
                        {
                            // We have already computed it, but we need to stop if it does not derive ε
                            allEpsilon = false;
                            break;
                        }
                    }
                    hasEpsilon = hasEpsilon || allEpsilon;
                }

                return new(symbol, hasEpsilon, result);
            }

            return FirstImpl(symbol);
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
