// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Grammar.Cfg;

namespace Yoakke.Grammar.Lr.Lr0
{
    /// <summary>
    /// An LR(0) item.
    /// </summary>
    public sealed record Lr0Item(Production Production, int Cursor) : ILrItem
    {
        /// <inheritdoc/>
        public IEnumerable<Terminal> Lookaheads => Array.Empty<Terminal>();

        /// <inheritdoc/>
        public bool IsInitial => this.Cursor == 0;

        /// <inheritdoc/>
        public bool IsFinal => this.Cursor == this.Production.Right.Count;

        /// <inheritdoc/>
        public Symbol? AfterCursor => this.IsFinal ? null : this.Production.Right[this.Cursor];

        /// <inheritdoc/>
        public string ToTex() => throw new NotImplementedException();
    }
}
