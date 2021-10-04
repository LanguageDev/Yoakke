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
        public bool DerivesEmpty(Symbol symbol) => throw new NotImplementedException();

        /// <inheritdoc/>
        public FirstSet First(Symbol symbol) => throw new NotImplementedException();

        /// <inheritdoc/>
        public FirstSet First(IEnumerable<Symbol> symbolSequence) => throw new NotImplementedException();

        /// <inheritdoc/>
        public FollowSet Follow(Symbol symbol) => throw new NotImplementedException();

        /// <inheritdoc/>
        public IEnumerable<IEnumerable<object>> GenerateSentences() => throw new NotImplementedException();

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
