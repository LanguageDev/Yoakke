// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// A factory interface that creates states.
/// </summary>
/// <typeparam name="TState">The state type.</typeparam>
public interface IStateCreator<TState>
{
    /// <summary>
    /// Creates a new, unique state.
    /// </summary>
    /// <returns>The created state.</returns>
    public TState Create();
}
