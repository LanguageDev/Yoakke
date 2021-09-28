// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Grammar.BnfAst;

namespace Yoakke.Grammar
{
    /// <summary>
    /// Represents a single rule inside a grammar.
    /// </summary>
    public record Rule(string Name, IReadOnlyList<RuleAlternative> Alternatives)
    {
        /// <inheritdoc/>
        public override string ToString() => string.Join("\n", this.Alternatives.Select(alt => $"{this.Name} -> {alt}"));
    }
}
