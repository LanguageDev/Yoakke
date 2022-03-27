// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.RegExes;

/// <summary>
/// Represents the most basic regex AST imaginable. All other regexes desugar to this.
/// </summary>
/// <typeparam name="TSymbol">The regex symbol type.</typeparam>
public abstract record class RegExAst<TSymbol>
{
}
