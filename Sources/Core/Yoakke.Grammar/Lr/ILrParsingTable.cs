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
    public interface ILrParsingTable
    {
        /// <summary>
        /// The grammar this table was built for.
        /// </summary>
        public IReadOnlyCfg Grammar { get; }

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
        /// Builds out the table from <see cref="Grammar"/>.
        /// </summary>
        public void Build();
    }
}
