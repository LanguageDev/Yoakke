// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Text;
using Yoakke.Collections;
using Yoakke.Grammar.Cfg;

namespace Yoakke.Grammar.Lr.Lalr
{
    /// <summary>
    /// A LALR item.
    /// </summary>
    public sealed record LalrItem(Production Production, int Cursor, ISet<Terminal> Lookaheads) : ILrItem
    {
        /// <inheritdoc/>
        public bool IsInitial => this.Cursor == 0;

        /// <inheritdoc/>
        public bool IsFinal => this.Cursor == this.Production.Right.Count;

        /// <inheritdoc/>
        public Symbol? AfterCursor => this.IsFinal ? null : this.Production.Right[this.Cursor];

        /// <summary>
        /// Retrieves the next item, with the cursor advanced one.
        /// </summary>
        public LalrItem Next =>
            new(this.Production, Math.Min(this.Cursor + 1, this.Production.Right.Count), this.Lookaheads.ToHashSet());

        /// <inheritdoc/>
        public bool Equals(LalrItem other) =>
               this.Production.Equals(other.Production)
            && this.Cursor == other.Cursor
            && SetEqualityComparer<Terminal>.Default.Equals(this.Lookaheads, other.Lookaheads);

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            var h = default(HashCode);
            h.Add(this.Production);
            h.Add(this.Cursor);
            h.Add(this.Lookaheads, SetEqualityComparer<Terminal>.Default);
            return h.ToHashCode();
        }

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
            sb.Append($", {(this.Lookaheads.Count == 0 ? "ε" : string.Join("/", this.Lookaheads))}");
            return sb.ToString();
        }
    }
}
