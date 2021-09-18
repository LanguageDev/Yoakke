// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Automata.Internal
{
    /// <summary>
    /// Helper class for producing the DOT code.
    /// </summary>
    /// <typeparam name="TState">The state type.</typeparam>
    /// <typeparam name="TSymbol">The symbol type.</typeparam>
    internal class ToDotHelper<TState, TSymbol>
    {
        private readonly IFiniteAutomaton<TState, TSymbol> automaton;
        private readonly Dictionary<TState, string> stateNames;
        private readonly StringBuilder stringBuilder = new();

        /// <summary>
        /// The written code.
        /// </summary>
        public string Code => this.stringBuilder.ToString();

        /// <summary>
        /// Initializes a new instance of the <see cref="ToDotHelper{TState, TSymbol}"/> class.
        /// </summary>
        /// <param name="automaton">The automaton to generate the code for.</param>
        public ToDotHelper(IFiniteAutomaton<TState, TSymbol> automaton)
        {
            this.automaton = automaton;
            this.stateNames = new(automaton.StateComparer);
        }

        /// <summary>
        /// Writes the start of the DOT code.
        /// </summary>
        public void WriteStart()
        {
            this.stringBuilder
                .AppendLine("digraph automata {")
                // Left-to-right layout
                .AppendLine("    rankdir=LR;");
            // Double-circle accepting states
            if (this.automaton.AcceptingStates.Count > 0)
            {
                var acceptingStates = string.Join(" ", this.automaton.AcceptingStates.Select(s => $"\"{this.GetStateName(s)}\""));
                this.stringBuilder.AppendLine($"    node [shape=doublecircle]; {acceptingStates};");
            }
            // Rest are simple circle
            this.stringBuilder.AppendLine($"    node [shape=circle];");
        }

        /// <summary>
        /// Writes the end of the DOT code.
        /// </summary>
        public void WriteEnd()
        {
            // Initial state
            this.stringBuilder
                .AppendLine("    init [label=\"\", shape=point]")
                .AppendLine($"    init -> \"{this.GetStateName(this.automaton.InitialState)}\"")
                .Append('}');
        }

        /// <summary>
        /// Writes a transition to the DOT code.
        /// </summary>
        /// <param name="from">The source state.</param>
        /// <param name="on">The string representation of the transition symbol(s).</param>
        /// <param name="to">The destination state.</param>
        public void WriteTransition(TState from, string on, TState to)
        {
            this.stringBuilder.AppendLine($"    \"{this.GetStateName(from)}\" -> \"{this.GetStateName(to)}\" [label = \"{on}\"];");
        }

        private string GetStateName(TState state)
        {
            if (!this.stateNames!.TryGetValue(state, out var name))
            {
                name = state!.ToString();
                this.stateNames.Add(state, name);
            }
            return name;
        }
    }
}
