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
    /// Represents a production with a cursor.
    /// </summary>
    public sealed record LrItem(Production Production, int Cursor)
    {
        /// <summary>
        /// True, if this is an initial item, meaning the cursor is at the start.
        /// </summary>
        public bool IsInitial => this.Cursor == 0;

        /// <summary>
        /// True, if this is a final item, meaning the cursor is at the end.
        /// </summary>
        public bool IsFinal => this.Cursor == this.Production.Symbols.Count;

        /// <summary>
        /// The symbol after the cursor.
        /// </summary>
        public Symbol? AfterCursor => this.IsFinal ? null : this.Production.Symbols[this.Cursor];

        /// <summary>
        /// Retrieves the next item, with the cursor advanced one.
        /// </summary>
        public LrItem Next => new(this.Production, Math.Min(this.Cursor + 1, this.Production.Symbols.Count));

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{this.Production.Name} ->");
            for (var i = 0; i < this.Production.Symbols.Count; ++i)
            {
                if (this.Cursor == i) sb.Append(" _");
                sb.Append($" {this.Production.Symbols[i]}");
            }
            if (this.IsFinal) sb.Append(" _");
            return sb.ToString();
        }
    }
}
