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

        /// <summary>
        /// Splits all or nodes into separate alternatives.
        /// </summary>
        /// <returns>The resulting <see cref="Rule"/> that contains no more or nodes.</returns>
        public Rule SplitOrAlternatives() =>
            new(this.Name, this.Alternatives.SelectMany(a => a.SplitOrNodes()).ToList());
    }
}
