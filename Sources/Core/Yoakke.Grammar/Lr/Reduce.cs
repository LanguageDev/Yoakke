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
    /// Represents that the stack should be reduced using some rule.
    /// </summary>
    public sealed record Reduce(Production Production) : Action
    {
        /// <inheritdoc/>
        public override string ToString() => $"reduce({this.Production})";
    }
}
