// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Represents a generic transition in a sparse state machine.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
/// <typeparam name="TSymbol">The symbol type.</typeparam>
/// <param name="Source">The source state.</param>
/// <param name="Symbol">The symbol the transition triggers on.</param>
/// <param name="Destination">The destination state.</param>
public readonly record struct Transition<TState, TSymbol>(TState Source, TSymbol Symbol, TState Destination);
