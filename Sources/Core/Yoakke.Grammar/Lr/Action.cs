// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

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
            /// <inheritdoc/>
            public override string ToString() => "shift";
        }

        /// <summary>
        /// Represents that the stack should be reduced using some rule.
        /// </summary>
        public sealed record Reduce : Action
        {
            /// <inheritdoc/>
            public override string ToString() => "reduce";
        }
    }
}
