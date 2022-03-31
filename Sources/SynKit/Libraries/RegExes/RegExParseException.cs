// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.RegExes;

/// <summary>
/// An error that happened during regex parsing.
/// </summary>
public sealed class RegExParseException : Exception
{
    /// <summary>
    /// Initializes a new <see cref="RegExParseException"/>.
    /// </summary>
    /// <param name="regex">The parsed regex.</param>
    /// <param name="error">The parse error.</param>
    public RegExParseException(string regex, string error)
        : base($"{error} while parsing {regex}")
    {
    }
}
