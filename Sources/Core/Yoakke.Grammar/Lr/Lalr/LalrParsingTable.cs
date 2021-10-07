// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Internal;
using Yoakke.Grammar.Lr.Lr0;

namespace Yoakke.Grammar.Lr.Lalr
{
    /// <summary>
    /// A LALR parsing table.
    /// </summary>
    public sealed class LalrParsingTable : ILrParsingTable<LalrItem>
    {
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
            TrivialImpl.Closure(this, set, (item, prod) => new[] { new LalrItem(prod, 0, new HashSet<Terminal>()) });

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
            // First, the lookahead of the initial item always has the '$'
            var startProduction = new Production(this.Grammar.StartSymbol, this.Grammar[this.Grammar.StartSymbol].First());
            var initialSet = this
                .Closure(new LalrItem(startProduction, 0, new HashSet<Terminal>()))
                .Where(this.IsKernel)
                .ToHashSet();
            var initialState = this.StateAllocator[initialSet];
            foreach (var item in this.StateAllocator[initialState]) item.Lookaheads.Add(Terminal.EndOfInput);

            // Now we determine the lookahead relations
            // We determine 2 things:
            //  - Spontaneous lookahead generation: In this case we add the item immediately to the relevant item in the set
            //  - Propagation: We make passes with these relations and add the propagated lookaheads until there is no change
            // For this we just need to store which items propagate into the currently observed item (also spontaneous
            // generations for the first pass)
            var generatesFrom = new Dictionary<LalrItem, HashSet<Terminal>>();
            var propagatesFrom = new Dictionary<LalrItem, HashSet<LalrItem>>();

            ISet<LalrItem> LookaheadClosure(LalrItem item) => TrivialImpl.Closure(
                this,
                new[] { new LalrItem(item.Production, item.Cursor, new HashSet<Terminal> { Terminal.NotInGrammar }) },
                (item, prod) => item.Lookaheads.Select(lookahead =>
                {
                    // Construct the sequence consisting of everything after the nonterminal plus the lookahead
                    var after = item.Production.Right.Skip(item.Cursor + 1).Append(lookahead);
                    // Compute the first-set
                    var firstSet = this.Grammar.First(after);
                    // Merge results
                    return new LalrItem(prod, 0, firstSet.Terminals.ToHashSet());
                })).ToHashSet();

            void DetermineLookaheads(ISet<LalrItem> kernelItems)
            {
                foreach (var kernelItem in kernelItems)
                {
                    var kernelClosure = LookaheadClosure(kernelItem);
                    foreach (var closureItem in kernelClosure)
                    {
                        foreach (var lookahead in closureItem.Lookaheads)
                        {
                            var to = closureItem.Next;
                            if (lookahead.Equals(Terminal.NotInGrammar))
                            {
                                // Propagation
                                if (!propagatesFrom!.TryGetValue(to, out var fromSet))
                                {
                                    fromSet = new();
                                    propagatesFrom.Add(to, fromSet);
                                }
                                fromSet.Add(kernelItem);
                            }
                            else
                            {
                                // Spontaneous generation
                                if (!generatesFrom!.TryGetValue(to, out var terminalSet))
                                {
                                    terminalSet = new();
                                    generatesFrom.Add(to, terminalSet);
                                }
                                terminalSet.Add(lookahead);
                            }
                        }
                    }
                }
            }

            // We run the lookahead determination for each item in the item set
            foreach (var itemSet in this.StateAllocator.ItemSets) DetermineLookaheads(itemSet);

            // First we do an initial pass, where we simply write the spontaneous terminals into the lookaheads
            foreach (var item in this.StateAllocator.ItemSets.SelectMany(i => i))
            {
                if (!generatesFrom.TryGetValue(item, out var terminals)) continue;
                foreach (var term in terminals) item.Lookaheads.Add(term);
            }

            // Now we propagate as long as there is a change
            while (true)
            {
                var change = false;

                foreach (var item in this.StateAllocator.ItemSets.SelectMany(i => i))
                {
                    if (!propagatesFrom.TryGetValue(item, out var propagationSources)) continue;
                    foreach (var fromItem in propagationSources)
                    {
                        foreach (var lookahead in fromItem.Lookaheads) change = item.Lookaheads.Add(lookahead) || change;
                    }
                }

                if (!change) break;
            }
        }
    }
}
