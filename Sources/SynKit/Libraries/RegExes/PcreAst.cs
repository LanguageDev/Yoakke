// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.SynKit.RegExes;

/// <summary>
/// AST for PCRE constructs.
/// </summary>
public abstract record class PcreAst
{
    /// <summary>
    /// Desugars this PCRE into plain regex.
    /// </summary>
    /// <param name="settings">The regex settings.</param>
    /// <returns>The equivalent plain regex construct.</returns>
    public abstract RegExAst<char> ToPlainRegex(RegExSettings settings);
}
