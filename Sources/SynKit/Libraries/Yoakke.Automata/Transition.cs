// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Automata;

/// <summary>
/// Represents a generic transition in a sparse state machine.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
public readonly struct Transition<TState, TSymbol>
{
  /// <summary>
  /// The source state.
  /// </summary>
  public TState Source { get; }

  /// <summary>
  /// The symbol the transition triggers on.
  /// </summary>
  public TSymbol Symbol { get; }

  /// <summary>
  /// The destination state.
  /// </summary>
  public TState Destination { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Transition{TState, TSymbol}"/> struct.
  /// </summary>
  /// <param name="source">The source state.</param>
  /// <param name="symbol">The symbol the transition triggers on.</param>
  /// <param name="destination">The destination state.</param>
  public Transition(TState source, TSymbol symbol, TState destination)
  {
    this.Source = source;
    this.Symbol = symbol;
    this.Destination = destination;
  }

  /// <summary>
  /// Deconstructs the transition into its elements.
  /// </summary>
  /// <param name="source">The <see cref="Source"/> gets written here.</param>
  /// <param name="symbol">The <see cref="Symbol"/> gets written here.</param>
  /// <param name="destination">The <see cref="Destination"/> gets written here.</param>
  public void Deconstruct(out TState source, out TSymbol symbol, out TState destination)
  {
    source = this.Source;
    symbol = this.Symbol;
    destination = this.Destination;
  }
}
