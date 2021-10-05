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
    /// Represents an action table in LR parsing.
    /// </summary>
    public class LrActionTable
    {
        private readonly Dictionary<int, Dictionary<Terminal, HashSet<Action>>> underlying = new();

        /// <summary>
        /// Retrieves a collection of actions for a given state and terminal.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="terminal">The terminal.</param>
        /// <returns>The actions to perform on state <paramref name="state"/> and temrinal <paramref name="terminal"/>.</returns>
        public ICollection<Action> this[int state, Terminal terminal]
        {
            get
            {
                if (!this.underlying.TryGetValue(state, out var onMap))
                {
                    onMap = new();
                    this.underlying.Add(state, onMap);
                }
                if (!onMap.TryGetValue(terminal, out var toSet))
                {
                    toSet = new();
                    onMap.Add(terminal, toSet);
                }
                return toSet;
            }
        }
    }
}
