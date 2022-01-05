// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using Yoakke.SynKit.Reporting.Present;
using Yoakke.SynKit.Text;

namespace Yoakke.SynKit.Reporting.Sample;

internal class MyHighlighter : ISyntaxHighlighter
{
    /// <inheritdoc/>
    public SyntaxHighlightStyle Style { get; set; } = SyntaxHighlightStyle.Default;

    /// <inheritdoc/>
    public IReadOnlyList<ColoredToken> GetHighlightingForLine(ISourceFile sourceFile, int line)
    {
        if (line == 1)
        {
            return new ColoredToken[]
            {
                new ColoredToken(0, 4, TokenKind.Keyword),
                new ColoredToken(5, 3, TokenKind.Name),
            };
        }
        else if (line == 2)
        {
            return new ColoredToken[]
            {
                new ColoredToken(4, 6, TokenKind.Keyword),
                new ColoredToken(11, 1, TokenKind.Literal),
                new ColoredToken(12, 1, TokenKind.Punctuation),
            };
        }
        return Array.Empty<ColoredToken>();
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        var src = new SourceFile("test.txt", @"
func foo() {
    return 0;
}
");

        var presenter = new TextDiagnosticsPresenter(Console.Error)
        {
            SyntaxHighlighter = new MyHighlighter(),
        };

        var diag = new Diagnostics()
            .WithCode("E001")
            .WithSeverity(Severity.Error)
            .WithMessage("you made a mistake")
            .WithSourceInfo(new Location(src, new Text.Range(new Position(2, 4), 6)), Severity.Error, "mistake made here");

        presenter.Present(diag);
    }
}
