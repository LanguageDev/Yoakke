// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Represents a single epsilon-transition.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
public readonly struct EpsilonTransition<TState>
{
    /// <summary>
    /// The source state.
    /// </summary>
    public TState Source { get; }

    /// <summary>
    /// The destination state.
    /// </summary>
    public TState Destination { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EpsilonTransition{TState}"/> struct.
    /// </summary>
    /// <param name="source">The source state.</param>
    /// <param name="destination">The destination state.</param>
    public EpsilonTransition(TState source, TState destination)
    {
        this.Source = source;
        this.Destination = destination;
    }

    /// <summary>
    /// Deconstructs the transition into its elements.
    /// </summary>
    /// <param name="source">The <see cref="Source"/> gets written here.</param>
    /// <param name="destination">The <see cref="Destination"/> gets written here.</param>
    public void Deconstruct(out TState source, out TState destination)
    {
        source = this.Source;
        destination = this.Destination;
    }
}
