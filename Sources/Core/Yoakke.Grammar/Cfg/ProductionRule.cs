// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Collections.Values;

namespace Yoakke.Grammar.Cfg
{
    /// <summary>
    /// Represents a single production rule in a context-free grammar.
    /// </summary>
    public sealed record ProductionRule(string Name, IReadOnlyValueList<Symbol> Symbols)
    {
        /// <inheritdoc/>
        public override string ToString() =>
            $"{this.Name} -> {(this.Symbols.Count == 0 ? "ε" : string.Join(" ", this.Symbols))}";
    }
}
