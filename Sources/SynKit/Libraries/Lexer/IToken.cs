// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Range = Yoakke.Text.Range;

namespace Yoakke.Lexer;

/// <summary>
/// Represents an atom in a language grammar as the lowest level element (atom/terminal) of parsing.
/// Usually tokens have a kind/category tag attached to them, for that <see cref="IToken{TKind}"/>.
/// </summary>
public interface IToken : IEquatable<IToken>
{
    /// <summary>
    /// The <see cref="Text.Range"/> that the token can be found at in the source.
    /// </summary>
    public Range Range { get; }

    /// <summary>
    /// The text this token was parsed from.
    /// </summary>
    public string Text { get; }
}
