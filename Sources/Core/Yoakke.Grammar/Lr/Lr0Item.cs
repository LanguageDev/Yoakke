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
    /// Represents a production with a cursor, without any lookahead.
    /// </summary>
    public sealed record Lr0Item(Production Production, int Cursor)
    {
        /// <summary>
        /// True, if this is an initial item, meaning the cursor is at the start.
        /// </summary>
        public bool IsInitial => this.Cursor == 0;

        /// <summary>
        /// True, if this is a final item, meaning the cursor is at the end.
        /// </summary>
        public bool IsFinal => this.Cursor == this.Production.Right.Count;

        /// <summary>
        /// The symbol after the cursor.
        /// </summary>
        public Symbol? AfterCursor => this.IsFinal ? null : this.Production.Right[this.Cursor];

        /// <summary>
        /// Retrieves the next item, with the cursor advanced one.
        /// </summary>
        public Lr0Item Next => new(this.Production, Math.Min(this.Cursor + 1, this.Production.Right.Count));

        /// <inheritdoc/>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"{this.Production.Left} ->");
            for (var i = 0; i < this.Production.Right.Count; ++i)
            {
                if (this.Cursor == i) sb.Append(" _");
                sb.Append($" {this.Production.Right[i]}");
            }
            if (this.IsFinal) sb.Append(" _");
            return sb.ToString();
        }

        /// <summary>
        /// Converts this LR item to TeX code.
        /// </summary>
        /// <returns>The TeX code to represents this item.</returns>
        public string ToTex()
        {
            var sb = new StringBuilder();
            sb.Append($"{this.Production.Left} \\rightarrow");
            for (var i = 0; i < this.Production.Right.Count; ++i)
            {
                if (this.Cursor == i) sb.Append(" \\textbullet \\ ");
                sb.Append($" {this.Production.Right[i]}");
            }
            if (this.IsFinal) sb.Append(" \\textbullet");
            return sb.ToString();
        }
    }
}
