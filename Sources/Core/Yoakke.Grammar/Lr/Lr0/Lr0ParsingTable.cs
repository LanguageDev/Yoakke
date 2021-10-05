// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;
using Yoakke.Grammar.Cfg;

namespace Yoakke.Grammar.Lr.Lr0
{
    /// <summary>
    /// An LR(0) parsing table.
    /// </summary>
    public class Lr0ParsingTable : ILrParsingTable<Lr0Item>
    {
        /// <inheritdoc/>
        public IReadOnlyCfg Grammar { get; }

        /// <inheritdoc/>
        public int StateCount => this.StateAllocator.StateCount;

        /// <inheritdoc/>
        public LrStateAllocator<Lr0Item> StateAllocator { get; } = new();

        /// <inheritdoc/>
        public LrActionTable Action { get; } = new();

        /// <inheritdoc/>
        public LrGotoTable Goto { get; } = new();

        /// <inheritdoc/>
        public bool HasConflicts => Enumerable.Range(0, this.StateCount)
            .Any(state => this.Grammar.Terminals.Any(term => this.Action[state, term].Count > 1));

        /// <summary>
        /// Initializes a new instance of the <see cref="Lr0ParsingTable"/> class.
        /// </summary>
        /// <param name="grammar">The grammar for the table.</param>
        public Lr0ParsingTable(IReadOnlyCfg grammar)
        {
            this.Grammar = grammar;
        }

        /// <inheritdoc/>
        public string ToDfaTex() => throw new NotImplementedException();

        /// <inheritdoc/>
        public string ToTableTex()
        {
            var sb = new StringBuilder();

            // Start of table
            sb.Append(@"\begin{tabular}{|c|");
            for (var i = 0; i < this.Grammar.Terminals.Count; ++i) sb.Append("|c");
            sb.Append('|');
            for (var i = 0; i < this.Grammar.Nonterminals.Count; ++i) sb.Append("|c");
            sb.Append('|');
            sb.AppendLine("}");

            // Header with Action and Goto
            sb
                .Append("  ")
                .Append(@$"\multicolumn{{1}}{{c}}{{}} & ")
                .Append($@"\multicolumn{{{this.Grammar.Terminals.Count}}}{{c}}{{Action}} &")
                .AppendLine($@"\multicolumn{{{this.Grammar.Nonterminals.Count}}}{{c}}{{Goto}} \\");
            sb.AppendLine(@"  \hline");

            // Header with state, terminals and symbols
            sb.Append("  State");
            foreach (var t in this.Grammar.Terminals) sb.Append($" & {t}");
            foreach (var t in this.Grammar.Nonterminals) sb.Append($" & {t}");
            sb
                .AppendLine(@" \\")
                .AppendLine(@"  \hline");

            for (var i = 0; i < this.StateCount; ++i)
            {
                sb.AppendLine(@"  \hline");
                sb.Append($"  {i}");
                // Action
                foreach (var t in this.Grammar.Terminals)
                {
                    sb.Append(" & ");
                    var actions = this.Action[i, t];
                    if (actions.Count == 1)
                    {
                        sb.Append(actions.First());
                    }
                    else
                    {
                        sb.Append(@"\begin{tabular}{c} ");
                        sb.Append(string.Join(@" \\ ", actions));
                        sb.Append(@" \end{tabular}");
                    }
                }
                // Goto
                foreach (var t in this.Grammar.Nonterminals)
                {
                    sb.Append(" & ");
                    var to = this.Goto[i, t];
                    if (to is not null) sb.Append(to.Value);
                }
                sb.AppendLine(@" \\");
            }

            // End of table
            sb.AppendLine(@"  \hline");
            sb.Append(@"\end{tabular}");

            // Some escapes
            sb
                .Replace("$", @"\$")
                .Replace("->", @"$\rightarrow$");

            return sb.ToString();
        }

        /// <inheritdoc/>
        public ISet<Lr0Item> Closure(Lr0Item item) => this.Closure(new[] { item });

        /// <inheritdoc/>
        public ISet<Lr0Item> Closure(IEnumerable<Lr0Item> set)
        {
            var result = set.ToHashSet();
            var stk = new Stack<Lr0Item>();
            foreach (var item in set) stk.Push(item);
            while (stk.TryPop(out var item))
            {
                var afterCursor = item.AfterCursor;
                if (afterCursor is not Nonterminal nonterm) continue;
                // It must be a nonterminal
                var prods = this.Grammar[nonterm];
                foreach (var prod in prods)
                {
                    var prodToAdd = new Production(nonterm, prod);
                    var itemToAdd = new Lr0Item(prodToAdd, 0);
                    if (result.Add(itemToAdd)) stk.Push(itemToAdd);
                }
            }
            return result;
        }

        /// <inheritdoc/>
        public void Build()
        {
            var startProductions = this.Grammar[this.Grammar.StartSymbol];
            if (startProductions.Count != 1) throw new InvalidOperationException("The grammar must have an augmented, single start symbol!");

            // Construct the I0 set
            var startProduction = new Production(this.Grammar.StartSymbol, startProductions.First());
            var i0 = this.Closure(new Lr0Item(startProduction, 0));
            var stk = new Stack<(ISet<Lr0Item> ItemSet, int State)>();
            this.StateAllocator.Allocate(i0, out var state0);
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
                    var nextSet = this.Closure(group.Select(prod => prod.Next));
                    if (this.StateAllocator.Allocate(nextSet, out var nextState)) stk.Push((nextSet, nextState));
                    this.Action[state, term].Add(new Shift(nextState));
                }

                // Nonterminal advance
                var itemsWithNonterminals = itemSet
                    .Where(prod => prod.AfterCursor is Nonterminal)
                    .GroupBy(prod => prod.AfterCursor);
                foreach (var group in itemsWithNonterminals)
                {
                    var nonterm = (Nonterminal)group.Key!;
                    var nextSet = this.Closure(group.Select(prod => prod.Next));
                    if (this.StateAllocator.Allocate(nextSet, out var nextState)) stk.Push((nextSet, nextState));
                    this.Goto[state, nonterm] = nextState;
                }

                // Final items
                var finalItems = itemSet.Where(prod => prod.IsFinal);
                foreach (var item in finalItems) this.BuildFinalItem(state, item);
            }
        }

        /// <summary>
        /// Handles the build of a final item.
        /// </summary>
        /// <param name="state">The current state.</param>
        /// <param name="finalItem">The final item.</param>
        protected virtual void BuildFinalItem(int state, Lr0Item finalItem)
        {
            if (finalItem.Production.Left.Equals(this.Grammar.StartSymbol))
            {
                this.Action[state, Symbol.EndOfInput].Add(Accept.Instance);
            }
            else
            {
                var reduction = new Reduce(finalItem.Production);
                foreach (var term in this.Grammar.Terminals) this.Action[state, term].Add(reduction);
            }
        }
    }
}
