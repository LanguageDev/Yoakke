// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.Parser.Generator.Syntax;

/// <summary>
/// A single token (terminal) in the BNF grammar.
/// </summary>
internal class BnfToken
{
    /// <summary>
    /// 0-based index in the BNF source.
    /// </summary>
    public int Index { get; }

    /// <summary>
    /// The textual value of this <see cref="BnfToken"/>.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// The kind of this <see cref="BnfToken"/>.
    /// </summary>
    public BnfTokenType Type { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BnfToken"/> class.
    /// </summary>
    /// <param name="index">The start index of the token in the source text.</param>
    /// <param name="value">The text value of the token.</param>
    /// <param name="type">The <see cref="BnfTokenType"/> of the token.</param>
    public BnfToken(int index, string value, BnfTokenType type)
    {
        this.Index = index;
        this.Value = value;
        this.Type = type;
    }
}
