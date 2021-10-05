// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Text;
using Yoakke.Collections;
using Yoakke.Grammar.Cfg;

namespace Yoakke.Grammar.Lr
{
    /// <summary>
    /// A single set of LR0 items.
    /// </summary>
    public sealed class Lr0ItemSet : IEquatable<Lr0ItemSet>
    {
        /// <summary>
        /// Calculates the closure of some LR 0 item.
        /// </summary>
        /// <param name="cfg">The grammar associated with the item.</param>
        /// <param name="item">The item to calculate the closure of.</param>
        /// <returns>The LR 0 closure of <paramref name="item"/>.</returns>
        public static Lr0ItemSet Closure(IReadOnlyCfg cfg, Lr0Item item) => Closure(cfg, new[] { item });

        /// <summary>
        /// Calculates the closure of an LR 0 item set.
        /// </summary>
        /// <param name="cfg">The grammar associated with the items.</param>
        /// <param name="set">The item set to calculate the closure of.</param>
        /// <returns>The LR 0 closure of <paramref name="set"/>.</returns>
        public static Lr0ItemSet Closure(IReadOnlyCfg cfg, IReadOnlyCollection<Lr0Item> set)
        {
            var result = set.ToHashSet();
            var stk = new Stack<Lr0Item>();
            foreach (var item in set) stk.Push(item);
            while (stk.TryPop(out var item))
            {
                var afterCursor = item.AfterCursor;
                if (afterCursor is not Nonterminal nonterm) continue;
                // It must be a nonterminal
                var prods = cfg[nonterm];
                foreach (var prod in prods)
                {
                    var prodToAdd = new Production(nonterm, prod);
                    var itemToAdd = new Lr0Item(prodToAdd, 0);
                    if (result.Add(itemToAdd)) stk.Push(itemToAdd);
                }
            }
            return new(cfg, result);
        }

        private readonly IReadOnlyCfg cfg;
        private readonly HashSet<Lr0Item> items;

        private Lr0ItemSet(IReadOnlyCfg cfg, HashSet<Lr0Item> items)
        {
            this.cfg = cfg;
            this.items = items;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Lr0ItemSet other && this.Equals(other);

        /// <inheritdoc/>
        public bool Equals(Lr0ItemSet other) =>
            SetEqualityComparer<Lr0Item>.Default.Equals(this.items, other.items);

        /// <inheritdoc/>
        public override int GetHashCode() => SetEqualityComparer<Lr0Item>.Default.GetHashCode(this.items);
    }
}
