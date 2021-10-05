// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Grammar.Lr
{
    /// <summary>
    /// Represents that the current token should be shifted.
    /// </summary>
    public sealed record Shift(int State) : Action
    {
        /// <inheritdoc/>
        public override string ToString() => $"shift({this.State})";
    }
}
