// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.RegExes;

/// <summary>
/// Settings for regexes.
/// </summary>
public sealed class RegExSettings
{
    /// <summary>
    /// The default settings.
    /// </summary>
    public static RegExSettings Default { get; } = new();
}
