// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Automata
{
    /// <summary>
    /// Represents a set of states that result from transformations merging multiple states.
    /// Useful for keeping readability.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    public sealed record StateSet<TState>(ISet<TState> States)
    {
        /// <inheritdoc/>
        public override string ToString() => string.Join(", ", this.States);
    }
}
