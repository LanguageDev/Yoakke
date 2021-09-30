// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.Cfg
{
    /// <summary>
    /// Represents some symbol in the production rule of the context-free grammar.
    /// </summary>
    public abstract record Symbol
    {
        /// <summary>
        /// Represents some terminal symbol.
        /// </summary>
        public sealed record Terminal(object Value) : Symbol
        {
            /// <inheritdoc/>
            public override string ToString() => this.Value.ToString();
        }

        /// <summary>
        /// Represents some nonterminal symbol referencing a rule.
        /// </summary>
        public sealed record Nonterminal(string Rule) : Symbol
        {
            /// <inheritdoc/>
            public override string ToString() => this.Rule;
        }
    }
}
