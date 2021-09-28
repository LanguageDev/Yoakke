// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Yoakke.Grammar.BnfAst;

namespace Yoakke.Grammar
{
    /// <summary>
    /// A single alternative of a rule.
    /// </summary>
    public record RuleAlternative(IBnfNode Ast)
    {
        /// <inheritdoc/>
        public override string ToString() => this.Ast.ToString();
    }
}
