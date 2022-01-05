// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Yoakke.SynKit.Text;

namespace Yoakke.SynKit.Reporting.Present;

/// <summary>
/// A syntax highlighter that does nothing. Serves as a default.
/// </summary>
public class NullSyntaxHighlighter : ISyntaxHighlighter
{
    /// <summary>
    /// A default instance for the null syntax highlighter.
    /// </summary>
    public static readonly NullSyntaxHighlighter Instance = new();

    /// <inheritdoc/>
    public SyntaxHighlightStyle Style { get; set; } = SyntaxHighlightStyle.Default;

    /// <inheritdoc/>
    public IReadOnlyList<ColoredToken> GetHighlightingForLine(ISourceFile sourceFile, int line) =>
        Array.Empty<ColoredToken>();
}
