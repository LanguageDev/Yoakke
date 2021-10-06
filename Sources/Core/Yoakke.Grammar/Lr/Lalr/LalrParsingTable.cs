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
        public int StateCount => this.StateAllocator.StateCount;

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
        public void Build() => TrivialImpl.Build(
            this,
            prod => new(prod, 0, new HashSet<Terminal>()),
            item => item.Next,
            // Filter for kernel items
            set => set.Where(this.IsKernel).ToHashSet(),
            (state, finalItem) => { });

        /// <inheritdoc/>
        public bool IsKernel(LalrItem item) => TrivialImpl.IsKernel(this, item);
    }
}
