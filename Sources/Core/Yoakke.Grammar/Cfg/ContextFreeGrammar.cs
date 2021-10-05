// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;
using Yoakke.Collections.Values;
using Yoakke.Grammar.Internal;
using Yoakke.Grammar.Lr;

namespace Yoakke.Grammar.Cfg
{
    /// <summary>
    /// Represents some context-free grammar in its purest form, with production rules.
    /// </summary>
    public sealed class ContextFreeGrammar : ICfg
    {
        private class ProductionRuleCollection : IReadOnlyCollection<Production>, ICollection<Production>
        {
            public Dictionary<Nonterminal, ProductionCollection> Rules { get; } = new();

            public int Count => this.Rules.Values.Sum(p => p.Count);

            public bool IsReadOnly => false;

            public event EventHandler<Production>? Added;

            public void RaiseAdded(Production item) => this.Added?.Invoke(this, item);

            public void Add(Production item)
            {
                // Add to productions themselves
                if (!this.Rules.TryGetValue(item.Left, out var list))
                {
                    list = new(this, item.Left);
                    this.Rules.Add(item.Left, list);
                }
                list.Underlying.Add(item);
                this.RaiseAdded(item);
            }

            public bool Remove(Production item)
            {
                if (!this.Rules.TryGetValue(item.Left, out var list)) return false;
                return list.Underlying.Remove(item);
            }

            public void Clear() => this.Rules.Clear();

            public bool Contains(Production item)
            {
                if (!this.Rules.TryGetValue(item.Left, out var list)) return false;
                return list.Underlying.Contains(item);
            }

            public void CopyTo(Production[] array, int arrayIndex)
            {
                foreach (var item in this) array[arrayIndex++] = item;
            }

            public IEnumerator<Production> GetEnumerator() => this.Rules.Values.SelectMany(p => p.Underlying).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        private class ProductionCollection
            : IReadOnlyCollection<IReadOnlyValueList<Symbol>>, ICollection<IReadOnlyValueList<Symbol>>
        {
            public Nonterminal Nonterminal { get; }

            public List<Production> Underlying { get; } = new();

            public int Count => this.Underlying.Count;

            public bool IsReadOnly => false;

            private readonly ProductionRuleCollection ruleCollection;

            public ProductionCollection(ProductionRuleCollection ruleCollection, Nonterminal nonterminal)
            {
                this.Nonterminal = nonterminal;
                this.ruleCollection = ruleCollection;
            }

            public void Add(IReadOnlyValueList<Symbol> item)
            {
                var production = new Production(this.Nonterminal, item);
                this.Underlying.Add(production);
                this.ruleCollection.RaiseAdded(production);
            }

            public bool Remove(IReadOnlyValueList<Symbol> item)
            {
                var enumerator = this.Underlying
                    .Select((p, i) => (Production: p, Index: i))
                    .Where(p => p.Production.Right.Equals(item))
                    .Select(p => p.Index)
                    .GetEnumerator();
                if (!enumerator.MoveNext()) return false;
                this.Underlying.RemoveAt(enumerator.Current);
                return true;
            }

            public void Clear() => this.Underlying.Clear();

            public bool Contains(IReadOnlyValueList<Symbol> item) => this.Underlying.Select(p => p.Right).Contains(item);

            public void CopyTo(IReadOnlyValueList<Symbol>[] array, int arrayIndex)
            {
                foreach (var item in this) array[arrayIndex++] = item;
            }

            public IEnumerator<IReadOnlyValueList<Symbol>> GetEnumerator() => this.Underlying.Select(p => p.Right).GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        }

        /// <inheritdoc/>
        public Nonterminal StartSymbol
        {
            get => this.startSymbol;
            set
            {
                this.startSymbol = value;
                this.nonterminals.Add(value);
            }
        }

        /// <inheritdoc/>
        public ICollection<Terminal> Terminals => this.terminals;

        /// <inheritdoc/>
        IReadOnlyCollection<Terminal> IReadOnlyCfg.Terminals => this.terminals;

        /// <inheritdoc/>
        public ICollection<Nonterminal> Nonterminals => this.nonterminals;

        /// <inheritdoc/>
        IReadOnlyCollection<Nonterminal> IReadOnlyCfg.Nonterminals => this.nonterminals;

        /// <inheritdoc/>
        public ICollection<Production> Productions => this.productionRules;

        /// <inheritdoc/>
        IReadOnlyCollection<Production> IReadOnlyCfg.Productions => this.productionRules;

        /// <inheritdoc/>
        public ICollection<IReadOnlyValueList<Symbol>> this[Nonterminal nonterminal] =>
            this.GetProductionCollection(nonterminal);

        /// <inheritdoc/>
        IReadOnlyCollection<IReadOnlyValueList<Symbol>> IReadOnlyCfg.this[Nonterminal nonterminal] =>
            this.GetProductionCollection(nonterminal);

        private readonly ProductionRuleCollection productionRules;
        private readonly ObservableCollection<Terminal> terminals;
        private readonly ObservableCollection<Nonterminal> nonterminals;
        private Nonterminal startSymbol;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextFreeGrammar"/> class.
        /// </summary>
        public ContextFreeGrammar()
        {
            this.startSymbol = new("S");
            this.productionRules = new();
            this.terminals = new(new HashSet<Terminal>());
            this.nonterminals = new(new HashSet<Nonterminal> { this.startSymbol });

            this.productionRules.Added += (_, production) =>
            {
                // Add terminals
                foreach (var t in production.Right.OfType<Terminal>()) this.terminals.Add(t);
                // Add nonterminals
                this.nonterminals.Add(production.Left);
                foreach (var nt in production.Right.OfType<Nonterminal>()) this.nonterminals.Add(nt);
            };

            this.terminals.Removed += (_, terminal) =>
            {
                var productionsToRemove = this.productionRules.Where(p => p.Right.Contains(terminal)).ToList();
                foreach (var p in productionsToRemove) this.productionRules.Remove(p);
            };

            this.nonterminals.Removed += (_, nonterminal) =>
            {
                var productionsToRemove = this.productionRules
                    .Where(p => p.Left.Equals(nonterminal) || p.Right.Contains(nonterminal))
                    .ToList();
                foreach (var p in productionsToRemove) this.productionRules.Remove(p);

                if (this.StartSymbol.Equals(nonterminal)) this.StartSymbol = new("S");
            };

            this.terminals.Cleared += (_, _) =>
            {
                var productionsToRemove = this.productionRules.Where(p => p.Right.OfType<Terminal>().Any()).ToList();
                foreach(var p in productionsToRemove) this.productionRules.Remove(p);
            };

            this.nonterminals.Cleared += (_, _) =>
            {
                this.productionRules.Clear();
                this.StartSymbol = new("S");
            };
        }

        /// <inheritdoc/>
        public override string ToString() => string.Join("\n", this.productionRules);

        /// <inheritdoc/>
        public string ToTex() =>
            $"\\noindent\n{string.Join(" \\\\\n", this.productionRules.Select(r => $"{r.Left} \\rightarrow {string.Join(" ", r.Right)}"))}";

        /// <inheritdoc/>
        public bool DerivesEmpty(Symbol symbol)
        {
            // Terminals can't derive the empty word
            if (symbol is Terminal) return false;

            // TODO: We should cache this later
            return this.CalculateEmptyDerivation()[symbol];
        }

        /// <inheritdoc/>
        public FirstSet First(Symbol symbol)
        {
            // TODO: We should cache this later
            return this.CalculateFirstSets()[symbol];
        }

        /// <inheritdoc/>
        public FirstSet First(IEnumerable<Symbol> symbolSequence)
        {
            var first = new HashSet<Terminal>();
            var derivesEpsilon = true;
            var symbolList = symbolSequence.ToList();

            foreach (var symbol in symbolSequence)
            {
                var firstSym = this.First(symbol);
                foreach (var item in firstSym.Terminals) first.Add(item);
                if (!firstSym.HasEmpty)
                {
                    derivesEpsilon = false;
                    break;
                }
            }

            return new(symbolList, derivesEpsilon, first);
        }

        /// <inheritdoc/>
        public FollowSet Follow(Nonterminal nonterminal)
        {
            // TODO: We should cache this later
            return this.CalculateFollowSets()[nonterminal];
        }

        /// <inheritdoc/>
        public IEnumerable<IEnumerable<Terminal>> GenerateSentences()
        {
            IEnumerable<IEnumerable<Terminal>> GenerateSentencesFrom(IEnumerable<IReadOnlyValueList<Symbol>> symbolSequences)
            {
                // For each sequence we try each substitution
                // If anything results in a sequence of nonterminals, we yield it, otherwise we ass it to the next iteration
                var currentIteration = symbolSequences.ToHashSet();
                while (currentIteration.Count > 0)
                {
                    var nextIteration = new HashSet<IReadOnlyValueList<Symbol>>();
                    foreach (var symbolSequence in currentIteration)
                    {
                        // If this is a complete terminal sequence, yield it
                        if (symbolSequence.All(s => s is Terminal))
                        {
                            yield return symbolSequence.OfType<Terminal>();
                        }
                        else
                        {
                            // Otherwise we need to add all possible substitutions to it
                            for (var i = 0; i < symbolSequence.Count; ++i)
                            {
                                // Skip terminals, they are already substituted
                                if (symbolSequence[i] is not Nonterminal nt) continue;

                                // We need to look at all production rules for this terminal and substitute it
                                var productionRules = this[nt];
                                foreach (var rule in productionRules)
                                {
                                    var symbolSequenceCopy = symbolSequence.ToList();
                                    symbolSequenceCopy.RemoveAt(i);
                                    symbolSequenceCopy.InsertRange(i, rule);
                                    nextIteration.Add(symbolSequenceCopy.ToValue());
                                }
                            }
                        }
                    }
                    currentIteration = nextIteration;
                }
            }

            var initials = this[this.StartSymbol];
            return GenerateSentencesFrom(initials);
        }

        private Dictionary<Symbol, bool> CalculateEmptyDerivation()
        {
            var result = new Dictionary<Symbol, bool>();

            // Terminals don't derive it
            foreach (var term in this.Terminals) result.Add(term, false);

            // Any production rule that has an empty production derives it
            foreach (var nonterm in this.Nonterminals)
            {
                if (this[nonterm].Any(p => p.Count == 0)) result.Add(nonterm, true);
            }

            // For nonterminals we loop as long as there is a change
            while (true)
            {
                var changed = false;

                // We loop through the remaining nonterminals
                foreach (var nonterm in this.Nonterminals.Except(result.Keys).OfType<Nonterminal>())
                {
                    // If any of the productions happen to all derive empty already, this one also does
                    foreach (var prod in this[nonterm])
                    {
                        if (prod.All(sym => result.TryGetValue(sym, out var derives) && derives))
                        {
                            // All symbols in this production derive the empty word, so this nonterminal does so
                            result.Add(nonterm, true);
                            changed = true;
                            break;
                        }
                    }
                }

                if (!changed) break;
            }

            // The remaining nonterminals simply don't derive it
            foreach (var nonterm in this.Nonterminals.Except(result.Keys).OfType<Nonterminal>()) result.Add(nonterm, false);

            return result;
        }

        private Dictionary<Symbol, FirstSet> CalculateFirstSets()
        {
            var result = new Dictionary<Symbol, (HashSet<Terminal> Terminals, bool Empty)>();

            // For all terminals X, FIRST(X) = { X }
            foreach (var t in this.Terminals) result[t] = (new() { t }, false);

            // For all nonterminals we simply initialize with an empty set
            foreach (var nt in this.Nonterminals) result[nt] = (new(), false);

            // While there is change, we refine the sets
            while (true)
            {
                var change = false;

                foreach (var (left, right) in this.Productions)
                {
                    var pair = result[left];
                    var producesEpsilon = true;
                    for (var i = 0; i < right.Count; ++i)
                    {
                        var firstB = result[right[i]];
                        foreach (var item in firstB.Terminals) change = pair.Terminals.Add(item) || change;

                        if (!firstB.Empty)
                        {
                            producesEpsilon = false;
                            break;
                        }
                    }

                    if (producesEpsilon && !pair.Empty)
                    {
                        pair.Empty = true;
                        change = true;
                    }

                    result[left] = pair;
                }

                if (!change) break;
            }

            return result.ToDictionary(kv => kv.Key, kv => new FirstSet(kv.Key, kv.Value.Empty, kv.Value.Terminals));
        }

        private Dictionary<Nonterminal, FollowSet> CalculateFollowSets()
        {
            var result = new Dictionary<Nonterminal, HashSet<Terminal>>();

            // Initialize with an empty set
            foreach (var nt in this.Nonterminals) result.Add(nt, new());

            // Add $ to FOLLOW(S)
            result[this.StartSymbol].Add(Symbol.EndOfInput);

            // We loop while we can refine the sets
            while (true)
            {
                var change = false;

                // Go through each production
                foreach (var production in this.Productions)
                {
                    var productionSet = result[production.Left];

                    for (var i = 0; i < production.Right.Count; ++i)
                    {
                        // We only care about nonterminals
                        if (production.Right[i] is not Nonterminal nt) continue;

                        // Anything in FIRST(remaining) will be in FOLLOW(nt)
                        var ntSet = result[nt];
                        var remaining = this.First(production.Right.Skip(i + 1));
                        foreach (var item in remaining.Terminals)
                        {
                            change = ntSet.Add(item) || change;
                        }

                        // If FIRST(remaining) produced the empty word,
                        // we add everything in FOLLOW(production.Left) to FOLLOW(nt)
                        if (remaining.HasEmpty)
                        {
                            foreach (var item in productionSet) change = ntSet.Add(item) || change;
                        }
                    }
                }

                if (!change) break;
            }

            return result.ToDictionary(kv => kv.Key, kv => new FollowSet(kv.Key, kv.Value));
        }

        private ProductionCollection GetProductionCollection(Nonterminal nonterminal)
        {
            if (!this.productionRules.Rules.TryGetValue(nonterminal, out var collection))
            {
                collection = new(this.productionRules, nonterminal);
                this.productionRules.Rules.Add(nonterminal, collection);
            }
            return collection;
        }
    }
}
