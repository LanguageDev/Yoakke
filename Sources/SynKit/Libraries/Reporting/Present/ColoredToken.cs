// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.Reporting.Present;

/// <summary>
/// A single, colored token in the source code.
/// </summary>
public readonly struct ColoredToken
{
    /// <summary>
    /// The start index of the token.
    /// </summary>
    public readonly int Start;

    /// <summary>
    /// The length of the token.
    /// </summary>
    public readonly int Length;

    /// <summary>
    /// The kind of the token.
    /// </summary>
    public readonly TokenKind Kind;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColoredToken"/> struct.
    /// </summary>
    /// <param name="start">The starting index of the token in the line.</param>
    /// <param name="length">The length of the token in characters.</param>
    /// <param name="kind">The <see cref="TokenKind"/> of the token.</param>
    public ColoredToken(int start, int length, TokenKind kind)
    {
        this.Start = start;
        this.Length = length;
        this.Kind = kind;
    }
}
