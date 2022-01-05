// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.SynKit.Automata.Internal;

/// <summary>
/// Helper class for producing the DOT code.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
internal sealed class DotWriter<TState>
{
    /// <summary>
    /// The builder that writes the DOT code.
    /// </summary>
    public StringBuilder Code { get; } = new();

    private readonly Dictionary<TState, string> stateNames;

    /// <summary>
    /// Initializes a new instance of the <see cref="DotWriter{TState}"/> class.
    /// </summary>
    /// <param name="stateComparer">The state comparer to use.</param>
    public DotWriter(IEqualityComparer<TState> stateComparer)
    {
        this.stateNames = new(stateComparer);
    }

    /// <summary>
    /// Writes the start of the DOT code.
    /// </summary>
    /// <param name="name">The name to give the graph.</param>
    public void WriteStart(string name)
    {
        this.Code.AppendLine($"digraph {name} {{");
        // Notate left-to-right layout
        this.Code.AppendLine("    rankdir=LR;");
    }

    /// <summary>
    /// Writes the end of the DOT code.
    /// </summary>
    public void WriteEnd() => this.Code.Append('}');

    /// <summary>
    /// Writes a sequence of non-accepting states to the graph.
    /// </summary>
    /// <param name="states">The sequence of accepting states.</param>
    public void WriteStates(IEnumerable<TState> states)
    {
        if (!states.Any()) return;
        this.Code.AppendLine("    node [shape=circle];");
        this.Code.AppendLine($"    {string.Join(" ", states.Select(s => $"\"{this.GetStateName(s)}\""))};");
    }

    /// <summary>
    /// Writes a sequence of initial states to the graph.
    /// </summary>
    /// <param name="states">The sequence of initial states.</param>
    public void WriteInitialStates(IEnumerable<TState> states)
    {
        if (!states.Any()) return;
        this.Code.AppendLine("    node [shape=point, label=\"\"];");
        var i = 0;
        foreach (var state in states)
        {
            this.Code.AppendLine($"    init_{i} -> \"{this.GetStateName(state)}\";");
            ++i;
        }
    }

    /// <summary>
    /// Writes a sequence of accepting states to the graph.
    /// </summary>
    /// <param name="states">The sequence of accepting states.</param>
    public void WriteAcceptingStates(IEnumerable<TState> states)
    {
        if (!states.Any()) return;
        this.Code.AppendLine("    node [shape=doublecircle];");
        this.Code.AppendLine($"    {string.Join(" ", states.Select(s => $"\"{this.GetStateName(s)}\""))};");
    }

    /// <summary>
    /// Writes a transition to the graph.
    /// </summary>
    /// <param name="from">The source state.</param>
    /// <param name="on">The string representation of the transition symbol(s).</param>
    /// <param name="to">The destination state.</param>
    public void WriteTransition(TState from, string on, TState to)
    {
        this.Code.AppendLine($"    \"{this.GetStateName(from)}\" -> \"{this.GetStateName(to)}\" [label=\"{on}\"];");
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
