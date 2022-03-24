// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// Settings for regular expressions.
/// </summary>
public sealed class RegExSettings
{
    /// <summary>
    /// Default regex settings.
    /// </summary>
    public static RegExSettings Default { get; } = new();
}
