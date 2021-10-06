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

namespace Yoakke.Grammar.Lr.Lr0
{
    /// <summary>
    /// An LR(0) parsing table.
    /// </summary>
    public sealed class Lr0ParsingTable : ILrParsingTable<Lr0Item>
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
        public bool HasConflicts => TrivialImpl.HasConflicts(this);

        /// <summary>
        /// Initializes a new instance of the <see cref="Lr0ParsingTable"/> class.
        /// </summary>
        /// <param name="grammar">The grammar for the table.</param>
        public Lr0ParsingTable(IReadOnlyCfg grammar)
        {
            this.Grammar = grammar;
        }

        /// <inheritdoc/>
        public string ToDotDfa() => LrTablePrinter.ToDotDfa(this);

        /// <inheritdoc/>
        public string ToHtmlTable() => LrTablePrinter.ToHtmlTable(this);

        /// <inheritdoc/>
        public ISet<Lr0Item> Closure(Lr0Item item) => this.Closure(new[] { item });

        /// <inheritdoc/>
        public ISet<Lr0Item> Closure(IEnumerable<Lr0Item> set) =>
            TrivialImpl.Closure(this, set, (item, prod) => new[] { new Lr0Item(prod, 0) });

        /// <inheritdoc/>
        public void Build() => TrivialImpl.Build(
            this,
            prod => new(prod, 0),
            item => item.Next,
            set => set,
            (state, finalItem) =>
            {
                if (finalItem.Production.Left.Equals(this.Grammar.StartSymbol))
                {
                    this.Action[state, Terminal.EndOfInput].Add(Accept.Instance);
                }
                else
                {
                    var reduction = new Reduce(finalItem.Production);
                    foreach (var term in this.Grammar.Terminals) this.Action[state, term].Add(reduction);
                }
            });

        /// <inheritdoc/>
        public bool IsKernel(Lr0Item item) => TrivialImpl.IsKernel(this, item);
    }
}
