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
    /// Represents a goto table in LR parsing.
    /// </summary>
    public class LrGotoTable
    {
        private readonly Dictionary<int, Dictionary<Nonterminal, int>> underlying = new();

        /// <summary>
        /// The state to go to on a nonterminal.
        /// </summary>
        /// <param name="from">The source state.</param>
        /// <param name="nonterminal">The nontemrinal.</param>
        /// <returns>The destination state from state <paramref name="from"/> on nonterminal
        /// <paramref name="nonterminal"/>.</returns>
        public int? this[int from, Nonterminal nonterminal]
        {
            get => this.underlying.TryGetValue(from, out var onMap)
                && onMap.TryGetValue(nonterminal, out var to)
                    ? to
                    : null;
            set
            {
                if (!this.underlying.TryGetValue(from, out var on))
                {
                    on = new();
                    this.underlying.Add(from, on);
                }
                if (value is null) on.Remove(nonterminal);
                else on[nonterminal] = value.Value;
            }
        }
    }
}
