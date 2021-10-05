// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Lr;

namespace Yoakke.Grammar.Internal
{
    /// <summary>
    /// Implementation helpers for the library, mainly for the parsing tables.
    /// </summary>
    internal static class TrivialImpl
    {
        /// <summary>
        /// Checks if a table has any conflicts.
        /// </summary>
        /// <typeparam name="TItem">The LR item type.</typeparam>
        /// <param name="table">The table to check.</param>
        /// <returns>True, if <paramref name="table"/> contains conflicts.</returns>
        public static bool HasConflicts<TItem>(ILrParsingTable<TItem> table)
            where TItem : ILrItem => Enumerable.Range(0, table.StateCount)
            .Any(state => table.Grammar.Terminals.Any(term => table.Action[state, term].Count > 1));

        /// <summary>
        /// Calculates the closure for an item set.
        /// </summary>
        /// <typeparam name="TItem">The LR item type.</typeparam>
        /// <param name="table">The table to use.</param>
        /// <param name="set">The set of items to generate the closure for.</param>
        /// <param name="getItems">The function that returns all items that belong in the closure, given an existing
        /// item and a production that expands from the given item.</param>
        /// <returns>The closure of <paramref name="set"/>.</returns>
        public static ISet<TItem> Closure<TItem>(
            ILrParsingTable<TItem> table,
            IEnumerable<TItem> set,
            Func<TItem, Production, IEnumerable<TItem>> getItems)
            where TItem : ILrItem
        {
            var result = set.ToHashSet();
            var stk = new Stack<TItem>();
            foreach (var item in set) stk.Push(item);
            while (stk.TryPop(out var item))
            {
                var afterCursor = item.AfterCursor;
                if (afterCursor is not Nonterminal nonterm) continue;
                // It must be a nonterminal
                var prods = table.Grammar[nonterm];
                foreach (var prod in prods)
                {
                    var prodToAdd = new Production(nonterm, prod);
                    var itemsToAdd = getItems(item, prodToAdd);
                    foreach (var itemToAdd in itemsToAdd)
                    {
                        if (result.Add(itemToAdd)) stk.Push(itemToAdd);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Implements the build of a parsing table.
        /// </summary>
        /// <typeparam name="TItem">The LR item type.</typeparam>
        /// <param name="table">The table to build.</param>
        /// <param name="makeFirstItem">A function to build the initial item from a production.</param>
        /// <param name="makeNextItem">A function to advance the cursor of an item.</param>
        /// <param name="handleFinalItem">A function to handle final items.</param>
        public static void Build<TItem>(
            ILrParsingTable<TItem> table,
            Func<Production, TItem> makeFirstItem,
            Func<TItem, TItem> makeNextItem,
            Action<int, TItem> handleFinalItem)
            where TItem : ILrItem
        {
            var startProductions = table.Grammar[table.Grammar.StartSymbol];
            if (startProductions.Count != 1) throw new InvalidOperationException("The grammar must have an augmented, single start symbol!");

            // Construct the I0 set
            var startProduction = new Production(table.Grammar.StartSymbol, startProductions.First());
            var i0 = table.Closure(makeFirstItem(startProduction));
            var stk = new Stack<(ISet<TItem> ItemSet, int State)>();
            table.StateAllocator.Allocate(i0, out var state0);
            stk.Push((i0, state0));

            while (stk.TryPop(out var itemSetPair))
            {
                var itemSet = itemSetPair.ItemSet;
                var state = itemSetPair.State;

                // Terminal advance
                var itemsWithTerminals = itemSet
                    .Where(prod => prod.AfterCursor is Terminal)
                    .GroupBy(prod => prod.AfterCursor);
                foreach (var group in itemsWithTerminals)
                {
                    var term = (Terminal)group.Key!;
                    var nextSet = table.Closure(group.Select(makeNextItem));
                    if (table.StateAllocator.Allocate(nextSet, out var nextState)) stk.Push((nextSet, nextState));
                    table.Action[state, term].Add(new Shift(nextState));
                }

                // Nonterminal advance
                var itemsWithNonterminals = itemSet
                    .Where(prod => prod.AfterCursor is Nonterminal)
                    .GroupBy(prod => prod.AfterCursor);
                foreach (var group in itemsWithNonterminals)
                {
                    var nonterm = (Nonterminal)group.Key!;
                    var nextSet = table.Closure(group.Select(makeNextItem));
                    if (table.StateAllocator.Allocate(nextSet, out var nextState)) stk.Push((nextSet, nextState));
                    table.Goto[state, nonterm] = nextState;
                }

                // Final items
                var finalItems = itemSet.Where(prod => prod.IsFinal);
                foreach (var item in finalItems) handleFinalItem(state, item);
            }
        }
    }
}
