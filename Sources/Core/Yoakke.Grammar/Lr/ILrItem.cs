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
    /// A generic LR(k) item interface.
    /// </summary>
    public interface ILrItem
    {
        /// <summary>
        /// The production this item references.
        /// </summary>
        public Production Production { get; }

        /// <summary>
        /// The cursor index standing in the production.
        /// </summary>
        public int Cursor { get; }

        /// <summary>
        /// The lookaheads in this item. The amount of lookaheads relate to the parameter k in LR(k).
        /// </summary>
        public IEnumerable<Terminal> Lookaheads { get; }

        /// <summary>
        /// True, if this is an initial item, meaning the cursor is at the start.
        /// </summary>
        public bool IsInitial { get; }

        /// <summary>
        /// True, if this is a final item, meaning the cursor is at the end.
        /// </summary>
        public bool IsFinal { get; }

        /// <summary>
        /// The symbol after the cursor. Null, if the cursor is at the end.
        /// </summary>
        public Symbol? AfterCursor { get; }

        /// <summary>
        /// Converts this LR item to TeX code.
        /// </summary>
        /// <returns>The TeX code to represents this item.</returns>
        public string ToTex();
    }
}
