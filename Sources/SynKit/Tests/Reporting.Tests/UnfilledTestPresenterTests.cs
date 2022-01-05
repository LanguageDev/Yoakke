// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.IO;
using Xunit;
using Yoakke.SynKit.Reporting.Present;
using Yoakke.SynKit.Text;

namespace Yoakke.SynKit.Reporting.Tests;

public class UnfilledTextPresenterTests
{
    private static Location Loc(ISourceFile source, int line, int column, int length) =>
        new(source, new Range(new Position(line, column), length));

    [Fact]
    public void NoErrorCode()
    {
        var src = new SourceFile(
            "simple.txt",
    @"line 1
prev line
this is a line of text
next line
some other line");
        var diag = new Diagnostics()
            .WithSeverity(Severity.Error)
            .WithMessage("Some error message")
            .WithSourceInfo(Loc(src, line: 2, column: 10, length: 4), Severity.Error, "some annotation");
        var result = new StringWriter();
        var renderer = new TextDiagnosticsPresenter(result);
        renderer.Present(diag);
        AssertUtils.AreEqualIgnoreNewlineEncoding(
            @"error: Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation
4 │ next line
  │
", result.ToString());
    }

    [Fact]
    public void NoSeverity()
    {
        var src = new SourceFile(
            "simple.txt",
    @"line 1
prev line
this is a line of text
next line
some other line");
        var diag = new Diagnostics()
            .WithCode("E0001")
            .WithMessage("Some error message")
            .WithSourceInfo(Loc(src, line: 2, column: 10, length: 4), Severity.Error, "some annotation");
        var result = new StringWriter();
        var renderer = new TextDiagnosticsPresenter(result);
        renderer.Present(diag);
        AssertUtils.AreEqualIgnoreNewlineEncoding(
            @"E0001: Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation
4 │ next line
  │
", result.ToString());
    }

    [Fact]
    public void NoErrorCoreAndSeverity()
    {
        var src = new SourceFile(
            "simple.txt",
    @"line 1
prev line
this is a line of text
next line
some other line");
        var diag = new Diagnostics()
            .WithMessage("Some error message")
            .WithSourceInfo(Loc(src, line: 2, column: 10, length: 4), Severity.Error, "some annotation");
        var result = new StringWriter();
        var renderer = new TextDiagnosticsPresenter(result);
        renderer.Present(diag);
        AssertUtils.AreEqualIgnoreNewlineEncoding(
            @"Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation
4 │ next line
  │
", result.ToString());
    }

    [Fact]
    public void NoErrorMessage()
    {
        var src = new SourceFile(
            "simple.txt",
    @"line 1
prev line
this is a line of text
next line
some other line");
        var diag = new Diagnostics()
            .WithSeverity(Severity.Error)
            .WithCode("E0001")
            .WithSourceInfo(Loc(src, line: 2, column: 10, length: 4), Severity.Error, "some annotation");
        var result = new StringWriter();
        var renderer = new TextDiagnosticsPresenter(result);
        renderer.Present(diag);
        AssertUtils.AreEqualIgnoreNewlineEncoding(
            @"error[E0001]
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation
4 │ next line
  │
", result.ToString());
    }
}
