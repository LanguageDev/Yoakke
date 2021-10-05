// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Internal;
using Yoakke.Grammar.Lr.Lr0;

namespace Yoakke.Grammar.Lr.Slr
{
    /// <summary>
    /// An SLR parsing table, which is a small improvelemt over LR(0).
    /// </summary>
    public sealed class SlrParsingTable : ILrParsingTable<Lr0Item>
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
        /// Initializes a new instance of the <see cref="SlrParsingTable"/> class.
        /// </summary>
        /// <param name="grammar">The grammar for the table.</param>
        public SlrParsingTable(IReadOnlyCfg grammar)
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
            (state, finalItem) =>
            {
                var reduction = new Reduce(finalItem.Production);
                var followSet = this.Grammar.Follow(finalItem.Production.Left);
                foreach (var follow in followSet.Terminals) this.Action[state, follow].Add(reduction);
            });
    }
}
