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

namespace Yoakke.Grammar.Lr.Clr
{
    /// <summary>
    /// A canonical LR (aka. LR(1)) parsing table.
    /// </summary>
    public class ClrParsingTable : ILrParsingTable<ClrItem>
    {
        /// <inheritdoc/>
        public IReadOnlyCfg Grammar { get; }

        /// <inheritdoc/>
        public int StateCount => this.StateAllocator.StateCount;

        /// <inheritdoc/>
        public LrStateAllocator<ClrItem> StateAllocator { get; } = new();

        /// <inheritdoc/>
        public LrActionTable Action { get; } = new();

        /// <inheritdoc/>
        public LrGotoTable Goto { get; } = new();

        /// <inheritdoc/>
        public bool HasConflicts => TrivialImpl.HasConflicts(this);

        /// <summary>
        /// Initializes a new instance of the <see cref="ClrParsingTable"/> class.
        /// </summary>
        /// <param name="grammar">The grammar for the table.</param>
        public ClrParsingTable(IReadOnlyCfg grammar)
        {
            this.Grammar = grammar;
        }

        /// <inheritdoc/>
        public string ToDotDfa() => LrTablePrinter.ToDotDfa(this);

        /// <inheritdoc/>
        public string ToHtmlTable() => LrTablePrinter.ToHtmlTable(this);

        /// <inheritdoc/>
        public ISet<ClrItem> Closure(ClrItem item) => this.Closure(new[] { item });

        /// <inheritdoc/>
        public ISet<ClrItem> Closure(IEnumerable<ClrItem> set) => TrivialImpl.Closure(
            this,
            set,
            this.GetClrClosureItems);

        /// <inheritdoc/>
        public void Build() => TrivialImpl.Build(
            this,
            prod => new(prod, 0, Terminal.EndOfInput),
            item => item.Next,
            (state, finalItem) =>
            {
                if (finalItem.Production.Left.Equals(this.Grammar.StartSymbol))
                {
                    this.Action[state, Terminal.EndOfInput].Add(Accept.Instance);
                }
                else
                {
                    var reduction = new Reduce(finalItem.Production);
                    this.Action[state, finalItem.Lookahead].Add(reduction);
                }
            });

        private IEnumerable<ClrItem> GetClrClosureItems(ClrItem item, Production prod)
        {
            var firstSet = this.Grammar.First(item.Production.Right.Skip(item.Cursor + 1));
            foreach (var term in firstSet.Terminals)
            {
                yield return new(prod, 0, term);
            }
        }
    }
}
