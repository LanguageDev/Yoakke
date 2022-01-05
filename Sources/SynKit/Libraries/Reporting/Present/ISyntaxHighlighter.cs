// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Yoakke.SynKit.Text;

namespace Yoakke.SynKit.Reporting.Present;

/// <summary>
/// An interface to provide custom syntax highlighting for source text.
/// </summary>
public interface ISyntaxHighlighter
{
    /// <summary>
    /// The style to use.
    /// </summary>
    public SyntaxHighlightStyle Style { get; set; }

    /// <summary>
    /// Asks for syntax highlighting for a single source line.
    /// </summary>
    /// <param name="sourceFile">The source that contains the line.</param>
    /// <param name="line">The index of the line in the source.</param>
    /// <returns>A list of <see cref="ColoredToken"/>s. Their order does not matter and not all characters have to
    /// belong to a token.</returns>
    public IReadOnlyList<ColoredToken> GetHighlightingForLine(ISourceFile sourceFile, int line);
}
