// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Yoakke.Collections;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Internal;
using Yoakke.Grammar.Lr.Clr;
using Yoakke.Grammar.Lr.Lr0;

namespace Yoakke.Grammar.Lr.Lalr
{
    /// <summary>
    /// A LALR parsing table.
    /// </summary>
    public sealed class LalrParsingTable : ILrParsingTable<LalrItem>
    {
        private class Lr0Comparer : IEqualityComparer<LalrItem>
        {
            public static Lr0Comparer Instance { get; } = new();

            private Lr0Comparer()
            {
            }

            public bool Equals(LalrItem x, LalrItem y) =>
                   x.Production.Equals(y.Production)
                && x.Cursor == y.Cursor;

            public int GetHashCode(LalrItem obj) => HashCode.Combine(obj.Production, obj.Cursor);
        }

        /// <inheritdoc/>
        public IReadOnlyCfg Grammar { get; }

        /// <inheritdoc/>
        public LrStateAllocator<LalrItem> StateAllocator { get; } = new();

        /// <inheritdoc/>
        public LrActionTable Action { get; } = new();

        /// <inheritdoc/>
        public LrGotoTable Goto { get; } = new();

        /// <inheritdoc/>
        public bool HasConflicts => TrivialImpl.HasConflicts(this);

        /// <summary>
        /// Initializes a new instance of the <see cref="LalrParsingTable"/> class.
        /// </summary>
        /// <param name="grammar">The grammar for the table.</param>
        public LalrParsingTable(IReadOnlyCfg grammar)
        {
            this.Grammar = grammar;
        }

        /// <inheritdoc/>
        public string ToDotDfa() => LrTablePrinter.ToDotDfa(this);

        /// <inheritdoc/>
        public string ToHtmlTable() => LrTablePrinter.ToHtmlTable(this);

        /// <inheritdoc/>
        public ISet<LalrItem> Closure(LalrItem item) => this.Closure(new[] { item });

        /// <inheritdoc/>
        public ISet<LalrItem> Closure(IEnumerable<LalrItem> set) =>
            TrivialImpl.Closure(this.Grammar, set, (item, prod) => new[] { new LalrItem(prod, 0, new HashSet<Terminal>()) });

        /// <inheritdoc/>
        public void Build()
        {
            TrivialImpl.Build(
              this,
              prod => new(prod, 0, new HashSet<Terminal>()),
              item => item.Next,
              // Filter for kernel items
              set => set.Where(this.IsKernel).ToHashSet(),
              // NOTE: We skip the reductions for now, we need to determine the lookahead sets
              (state, finalItem) => { });

            // Now we calculate the lookaheads, which is an iterative process
            this.CalculateLookaheads();

            // Now we can assign the final items properly
            foreach (var (state, itemSet) in this.StateAllocator.States.Select(s => (s, this.StateAllocator[s])))
            {
                // Final items
                var finalItems = itemSet.Where(prod => prod.IsFinal);
                foreach (var finalItem in finalItems)
                {
                    if (finalItem.Production.Left.Equals(this.Grammar.StartSymbol))
                    {
                        this.Action[state, Terminal.EndOfInput].Add(Accept.Instance);
                    }
                    else
                    {
                        var reduction = new Reduce(finalItem.Production);
                        foreach (var lookahead in finalItem.Lookaheads) this.Action[state, lookahead].Add(reduction);
                    }
                }
            }
        }

        /// <inheritdoc/>
        public bool IsKernel(LalrItem item) => TrivialImpl.IsKernel(this, item);

        private void CalculateLookaheads()
        {
            // We determine the lookahead relations
            // We determine 2 things:
            //  - Spontaneous lookahead generation: In this case we add the item immediately to the relevant item in the set
            //  - Propagation: We make passes with these relations and add the propagated lookaheads until there is no change
            // For this we just need to store which items propagate into the currently observed item (also spontaneous
            // generations for the first pass)
            var tupleComparer = new TupleEqualityComparer<int, LalrItem>(EqualityComparer<int>.Default, Lr0Comparer.Instance);
            var generatesFrom = new Dictionary<(int State, LalrItem Item), HashSet<Terminal>>(tupleComparer);
            var propagatesFrom = new Dictionary<(int State, LalrItem Item), HashSet<(int State, LalrItem Item)>>(tupleComparer);

            // $ generates from the initial item
            var initialProduction = new Production(this.Grammar.StartSymbol, this.Grammar[this.Grammar.StartSymbol].First());
            var initialItem = new LalrItem(initialProduction, 0, new HashSet<Terminal>());
            generatesFrom[(0, initialItem)] = new() { Terminal.EndOfInput };

            // The closure should act as an LR(1) closure grouped by the lookaheads into LALR items
            ISet<LalrItem> LookaheadClosure(LalrItem item) => TrivialImpl.Closure(
                this.Grammar,
                new[] { new ClrItem(item.Production, item.Cursor, Terminal.NotInGrammar) },
                this.GetClrClosureItems)
                .GroupBy(item => new Lr0Item(item.Production, item.Cursor))
                .Select(g => new LalrItem(g.Key.Production, g.Key.Cursor, g.Select(i => i.Lookahead).ToHashSet()))
                .ToHashSet();

            void DetermineLookaheads(int fromState, ISet<LalrItem> kernelItems)
            {
                foreach (var kernelItem in kernelItems)
                {
                    var kernelClosure = LookaheadClosure(kernelItem);
                    foreach (var closureItem in kernelClosure)
                    {
                        if (closureItem.IsFinal) continue;

                        var toState = closureItem.AfterCursor is Terminal t
                            ? this.Action[fromState, t].OfType<Shift>().First().State
                            : this.Goto[fromState, (Nonterminal)closureItem.AfterCursor!]!.Value;
                        var toItem = closureItem.Next;

                        foreach (var lookahead in closureItem.Lookaheads)
                        {
                            if (lookahead.Equals(Terminal.NotInGrammar))
                            {
                                // Propagation
                                if (!propagatesFrom!.TryGetValue((toState, toItem), out var fromSet))
                                {
                                    fromSet = new(tupleComparer);
                                    propagatesFrom.Add((toState, toItem), fromSet);
                                }
                                fromSet.Add((fromState, closureItem));
                            }
                            else
                            {
                                // Spontaneous generation
                                if (!generatesFrom!.TryGetValue((toState, toItem), out var terminalSet))
                                {
                                    terminalSet = new();
                                    generatesFrom.Add((toState, toItem), terminalSet);
                                }
                                terminalSet.Add(lookahead);
                            }
                        }
                    }
                }
            }

            // We run the lookahead determination for each item in the item set
            foreach (var fromState in this.StateAllocator.States) DetermineLookaheads(fromState, this.StateAllocator[fromState]);

            // First we do an initial pass, where we simply write the spontaneous terminals into the lookaheads
            foreach (var state in this.StateAllocator.States)
            {
                foreach (var item in this.StateAllocator[state])
                {
                    if (!generatesFrom.TryGetValue((state, item), out var terminals)) continue;
                    foreach (var term in terminals) item.Lookaheads.Add(term);
                }
            }

            // Now we propagate as long as there is a change
            while (true)
            {
                var change = false;

                foreach (var state in this.StateAllocator.States)
                {
                    foreach (var item in this.StateAllocator[state])
                    {
                        if (!propagatesFrom.TryGetValue((state, item), out var propagationSources)) continue;
                        foreach (var (fromState, fromItem) in propagationSources)
                        {
                            var lookaheads = this.StateAllocator[fromState]
                                .Where(i => Lr0Comparer.Instance.Equals(i, fromItem))
                                .First()
                                .Lookaheads;
                            foreach (var lookahead in lookaheads) change = item.Lookaheads.Add(lookahead) || change;
                        }
                    }
                }

                if (!change) break;
            }
        }

        private IEnumerable<ClrItem> GetClrClosureItems(ClrItem item, Production prod)
        {
            // Construct the sequence consisting of everything after the nonterminal plus the lookahead
            var after = item.Production.Right.Skip(item.Cursor + 1).Append(item.Lookahead);
            // Compute the first-set
            var firstSet = this.Grammar.First(after);
            // Yield returns
            foreach (var term in firstSet.Terminals) yield return new(prod, 0, term);
        }
    }
}
