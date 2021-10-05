// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Grammar.Cfg;

namespace Yoakke.Grammar.Lr.Clr
{
    /// <summary>
    /// A canonical LR (aka. LR(1)) item.
    /// </summary>
    public sealed record ClrItem(Production Production, int Cursor, Terminal Lookahead) : ILrItem
    {
        /// <inheritdoc/>
        public IEnumerable<Terminal> Lookaheads => new[] { this.Lookahead };

        /// <inheritdoc/>
        public bool IsInitial => this.Cursor == 0;

        /// <inheritdoc/>
        public bool IsFinal => this.Cursor == this.Production.Right.Count;

        /// <inheritdoc/>
        public Symbol? AfterCursor => this.IsFinal ? null : this.Production.Right[this.Cursor];

        /// <summary>
        /// Retrieves the next item, with the cursor advanced one.
        /// </summary>
        public ClrItem Next => new(this.Production, Math.Min(this.Cursor + 1, this.Production.Right.Count), this.Lookahead);

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"[{this.Production.Left} ->");
            for (var i = 0; i < this.Production.Right.Count; ++i)
            {
                if (this.Cursor == i) sb.Append(" _");
                sb.Append($" {this.Production.Right[i]}");
            }
            if (this.IsFinal) sb.Append(" _");
            sb.Append($", {this.Lookahead}]");
            return sb.ToString();
        }
    }
}
