// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Lr.Lr0;

namespace Yoakke.Grammar.Lr.Slr
{
    /// <summary>
    /// An SLR parsing table, which is a small improvelemt over LR(0).
    /// </summary>
    public sealed class SlrParsingTable : Lr0ParsingTable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SlrParsingTable"/> class.
        /// </summary>
        /// <param name="grammar">The grammar the table is built for.</param>
        public SlrParsingTable(IReadOnlyCfg grammar)
            : base(grammar)
        {
        }

        /// <inheritdoc/>
        protected override void BuildFinalItem(int state, Lr0Item finalItem)
        {
            var reduction = new Reduce(finalItem.Production);
            var followSet = this.Grammar.Follow(finalItem.Production.Left);
            foreach (var follow in followSet.Terminals) this.Action[state, follow].Add(reduction);
        }
    }
}
