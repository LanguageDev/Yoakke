// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;
using Yoakke.Grammar.Lr;

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
        public IEnumerable<Production> Productions => this.productionRules.Values.SelectMany(x => x);

        /// <summary>
        /// All the terminals in this grammar.
        /// </summary>
        public IEnumerable<Symbol.Terminal> Terminals => this.Productions
            .SelectMany(p => p.Symbols)
            .OfType<Symbol.Terminal>()
            .Distinct();

        /// <summary>
        /// All the nonterminals in this grammar.
        /// </summary>
        public IEnumerable<Symbol.Nonterminal> Nonterminals => this.Productions
            .SelectMany(p => p.Symbols)
            .OfType<Symbol.Nonterminal>()
            .Concat(this.productionRules.Keys.Select(n => new Symbol.Nonterminal(n)))
            .Distinct();

        private readonly Dictionary<string, HashSet<Production>> productionRules = new();

        /// <inheritdoc/>
        public override string ToString() => string.Join("\n", this.Productions);

        /// <summary>
        /// Adds a <see cref="Production"/> to this grammar.
        /// </summary>
        /// <param name="productionRule">The <see cref="Production"/> to add.</param>
        public void AddProduction(Production productionRule)
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
        public IEnumerable<Production> GetProductions(string name) => this.productionRules[name];

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

        /// <summary>
        /// Calculates the <see cref="FollowSet"/> of some nonterminal symbol.
        /// </summary>
        /// <param name="symbol">The nonterminal symbol to calculate the follow-set of.</param>
        /// <returns>The follow-set of <paramref name="symbol"/>.</returns>
        public FollowSet Follow(Symbol.Nonterminal symbol)
        {
            var touched = new HashSet<Symbol>();
            touched.Add(symbol);

            FollowSet FollowImpl(Symbol.Nonterminal symbol)
            {
                var result = new HashSet<Symbol.Terminal>();

                // Place $ in FOLLOW(S), where S is the start symbol and $ is the input right endmarker
                if (symbol.Rule == this.StartSymbol) result.Add(Symbol.EndOfInput);

                foreach (var production in this.Productions)
                {
                    var nonterm = new Symbol.Nonterminal(production.Name);

                    // If there is a production A -> aBb, then everything in FIRST(b), except for ε, is placed in FOLLOW(B)
                    // If there is a production A -> aB, or a production A -> aBb where FIRST(b) contains e (i.e., b -> ε),
                    // then everything in FOLLOW(A) is in FOLLOW(B)
                    // We search for all occurrences of B in the current production
                    var bIndices = production.Symbols
                        .Select((sym, idx) => (Symbol: sym, Index: idx))
                        .Where(p => p.Symbol == symbol)
                        .Select(p => p.Index);
                    foreach (var idx in bIndices)
                    {
                        if (idx < production.Symbols.Count - 1)
                        {
                            // A -> aBb, add FIRST(b) to FOLLOW(B)
                            var first = this.First(production.Symbols.Skip(idx + 1));
                            foreach (var item in first.Terminals) result.Add(item);

                            if (first.HasEmpty && touched.Add(nonterm))
                            {
                                // A -> aBb and FIRST(b) contains ε, add FOLLOW(A) to FOLLOW(B)
                                var follow = FollowImpl(nonterm);
                                foreach (var item in follow.Terminals) result.Add(item);
                            }
                        }
                        else if (touched.Add(nonterm))
                        {
                            // A -> aB, add FOLLOW(A) to FOLLOW(B)
                            var follow = FollowImpl(nonterm);
                            foreach (var item in follow.Terminals) result.Add(item);
                        }
                    }
                }

                return new(symbol, result);
            }

            return FollowImpl(symbol);
        }

        /// <summary>
        /// Calculates the closure of an LR item.
        /// </summary>
        /// <param name="item">The LR item to calculate the closure of.</param>
        /// <returns>The closure of <paramref name="item"/>.</returns>
        public ISet<LrItem> Closure(LrItem item) => this.Closure(new[] { item }.ToHashSet());

        /// <summary>
        /// Calculates the closure of an LR item set.
        /// </summary>
        /// <param name="itemSet">The LR item set to calculate the closure of.</param>
        /// <returns>The closure of <paramref name="itemSet"/>.</returns>
        public ISet<LrItem> Closure(ISet<LrItem> itemSet)
        {
            var result = itemSet.ToHashSet();
            var stk = new Stack<LrItem>();
            foreach (var item in itemSet) stk.Push(item);
            while (stk.TryPop(out var item))
            {
                var afterCursor = item.AfterCursor;
                if (afterCursor is not Symbol.Nonterminal nonterm) continue;
                // It must be a nonterminal
                var prods = this.productionRules[nonterm.Rule];
                foreach (var prod in prods)
                {
                    var prodToAdd = prod.InitialLrItem;
                    if (result.Add(prodToAdd)) stk.Push(prodToAdd);
                }
            }
            return result;
        }
    }
}
