// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Lr.Clr;
using Yoakke.Grammar.Lr.Lr0;
using Yoakke.Grammar.Lr.Slr;

namespace Yoakke.Grammar.Lr
{
    /// <summary>
    /// Utilities for constructing different LR parsing tables.
    /// </summary>
    public static class LrParsingTable
    {
        /// <summary>
        /// Builds an LR(0) parsing table.
        /// </summary>
        /// <param name="grammar">The grammar to build the table for.</param>
        /// <returns>The LR(0) table for <paramref name="grammar"/>.</returns>
        public static Lr0ParsingTable Lr0(IReadOnlyCfg grammar)
        {
            var table = new Lr0ParsingTable(grammar);
            table.Build();
            return table;
        }

        /// <summary>
        /// Builds an SLR parsing table.
        /// </summary>
        /// <param name="grammar">The grammar to build the table for.</param>
        /// <returns>The SLR table for <paramref name="grammar"/>.</returns>
        public static SlrParsingTable Slr(IReadOnlyCfg grammar)
        {
            var table = new SlrParsingTable(grammar);
            table.Build();
            return table;
        }

        /// <summary>
        /// Builds a CLR (aka. LR(1)) parsing table.
        /// </summary>
        /// <param name="grammar">The grammar to build the table for.</param>
        /// <returns>The CLR table for <paramref name="grammar"/>.</returns>
        public static ClrParsingTable Clr(IReadOnlyCfg grammar)
        {
            var table = new ClrParsingTable(grammar);
            table.Build();
            return table;
        }
    }
}
