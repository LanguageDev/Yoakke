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
    /// Some action to perform for the LR parser.
    /// </summary>
    public abstract record Action
    {
        /// <summary>
        /// Represents that the current token should be shifted.
        /// </summary>
        public sealed record Shift : Action
        {
            /// <summary>
            /// A singleton instance to use.
            /// </summary>
            public static Shift Instance { get; } = new();

            private Shift()
            {
            }

            /// <inheritdoc/>
            public override string ToString() => "shift";
        }

        /// <summary>
        /// Represents that the stack should be reduced using some rule.
        /// </summary>
        public sealed record Reduce(Production Rule) : Action
        {
            /// <inheritdoc/>
            public override string ToString() => $"reduce({this.Rule})";
        }
    }
}
