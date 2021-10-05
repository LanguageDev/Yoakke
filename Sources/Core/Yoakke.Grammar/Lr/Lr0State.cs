// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.Lr
{
    /// <summary>
    /// Represents an LR 0 state in the DFA.
    /// </summary>
    public sealed record Lr0State(int Index, IReadOnlyCollection<Lr0Item> ItemSet)
    {
        /// <inheritdoc/>
        public bool Equals(Lr0State other) => this.Index == other.Index;

        /// <inheritdoc/>
        public override int GetHashCode() => this.Index.GetHashCode();
    }
}
