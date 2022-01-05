// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.Parser.Generator.Syntax;

/// <summary>
/// The possible different kinds for a <see cref="BnfToken"/>.
/// </summary>
internal enum BnfTokenType
{
    /// <summary>
    /// End of grammar.
    /// </summary>
    End,

    /// <summary>
    /// An identifier.
    /// </summary>
    Identifier,

    /// <summary>
    /// ':'.
    /// </summary>
    Colon,

    /// <summary>
    /// '|'.
    /// </summary>
    Pipe,

    /// <summary>
    /// '('.
    /// </summary>
    OpenParen,

    /// <summary>
    /// ')'.
    /// </summary>
    CloseParen,

    /// <summary>
    /// A literal for matching token texts.
    /// </summary>
    StringLiteral,

    /// <summary>
    /// '+' (repeat 1 or more).
    /// </summary>
    Plus,

    /// <summary>
    /// '*' (repeat 0 or more).
    /// </summary>
    Star,

    /// <summary>
    /// '?' (optional).
    /// </summary>
    QuestionMark,
}
