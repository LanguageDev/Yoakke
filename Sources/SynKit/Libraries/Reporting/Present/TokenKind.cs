// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.SynKit.Reporting.Present;

/// <summary>
/// Different token categories for presenting.
/// </summary>
public enum TokenKind
{
    /// <summary>
    /// A comment.
    /// </summary>
    Comment,

    /// <summary>
    /// A language keyword.
    /// </summary>
    Keyword,

    /// <summary>
    /// Some literal.
    /// </summary>
    Literal,

    /// <summary>
    /// A name or identifier.
    /// </summary>
    Name,

    /// <summary>
    /// Punctuation token.
    /// </summary>
    Punctuation,

    /// <summary>
    /// An operator.
    /// </summary>
    Operator,

    /// <summary>
    /// Something else.
    /// </summary>
    Other,
}
