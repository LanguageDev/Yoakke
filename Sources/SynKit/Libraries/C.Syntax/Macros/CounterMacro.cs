// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.SynKit.C.Syntax.Macros;

/// <summary>
/// Implementation of __COUNTER__.
/// </summary>
public class CounterMacro : IMacro
{
    private int count = 0;

    /// <inheritdoc/>
    public string Name => "__COUNTER__";

    /// <inheritdoc/>
    public IReadOnlyList<string>? Parameters => null;

    /// <inheritdoc/>
    public IEnumerable<CToken> Expand(IPreProcessor preProcessor, IReadOnlyList<IReadOnlyList<CToken>> arguments)
    {
        var str = (this.count++).ToString();
        // TODO: Proper range?
        var token = new CToken(default, str, default, str, CTokenType.IntLiteral);
        return new[] { token };
    }
}
