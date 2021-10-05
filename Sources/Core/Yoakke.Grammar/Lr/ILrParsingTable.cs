// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Grammar.Cfg;

namespace Yoakke.Grammar.Lr
{
    /// <summary>
    /// Represents an LR(k) parsing table.
    /// </summary>
    /// <typeparam name="TItem">The LR item type.</typeparam>
    public interface ILrParsingTable<TItem>
        where TItem : ILrItem
    {
        /// <summary>
        /// The grammar this table was built for.
        /// </summary>
        public IReadOnlyCfg Grammar { get; }

        /// <summary>
        /// The number of states in the table.
        /// </summary>
        public int StateCount { get; }

        /// <summary>
        /// The state allocator.
        /// </summary>
        public LrStateAllocator<TItem> StateAllocator { get; }

        /// <summary>
        /// The action table.
        /// </summary>
        public LrActionTable Action { get; }

        /// <summary>
        /// The goto table.
        /// </summary>
        public LrGotoTable Goto { get; }

        /// <summary>
        /// True, if the table has conflicts.
        /// </summary>
        public bool HasConflicts { get; }

        /// <summary>
        /// Converts this table to a DFA representation Graphviz DOT code.
        /// </summary>
        /// <returns>The Graphviz DOT code of the DFA this table describes.</returns>
        public string ToDotDfa();

        /// <summary>
        /// Converts this table to HTML.
        /// </summary>
        /// <returns>The HTML code of this table.</returns>
        public string ToHtmlTable();

        /// <summary>
        /// Calculates the closure of some LR item.
        /// </summary>
        /// <param name="item">The item to calculate the closure of.</param>
        /// <returns>The LR closure of <paramref name="item"/>.</returns>
        public ISet<TItem> Closure(TItem item);

        /// <summary>
        /// Calculates the closure of an LR item set.
        /// </summary>
        /// <param name="set">The item set to calculate the closure of.</param>
        /// <returns>The LR closure of <paramref name="set"/>.</returns>
        public ISet<TItem> Closure(IEnumerable<TItem> set);

        /// <summary>
        /// Builds out the table from <see cref="Grammar"/>.
        /// </summary>
        public void Build();
    }
}
