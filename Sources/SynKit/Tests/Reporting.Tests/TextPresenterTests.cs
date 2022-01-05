// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.IO;
using Xunit;
using Yoakke.SynKit.Reporting.Present;
using Yoakke.SynKit.Text;

namespace Yoakke.SynKit.Reporting.Tests;

public class TextPresenterTests
{
    private static Location Loc(ISourceFile source, int line, int column, int length) =>
        new(source, new Range(new Position(line, column), length));

    [Fact]
    public void BasicSingleAnnotation()
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
            .WithMessage("Some error message")
            .WithSourceInfo(Loc(src, line: 2, column: 10, length: 4), Severity.Error, "some annotation");
        var result = new StringWriter();
        var renderer = new TextDiagnosticsPresenter(result);
        renderer.Present(diag);
        AssertUtils.AreEqualIgnoreNewlineEncoding(
            @"error[E0001]: Some error message
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
    public void TwoAnnotationsRightUnderEachother()
    {
        var src = new SourceFile(
            "simple.txt",
    @"line 1
prev line
this is a line of text
some other line
last line");
        var diag = new Diagnostics()
            .WithSeverity(Severity.Error)
            .WithCode("E0001")
            .WithMessage("Some error message")
            .WithSourceInfo(Loc(src, line: 2, column: 10, length: 4), Severity.Error, "some annotation1")
            .WithSourceInfo(Loc(src, line: 3, column: 5, length: 5), "some annotation2");
        var result = new StringWriter();
        var renderer = new TextDiagnosticsPresenter(result);
        renderer.Present(diag);
        AssertUtils.AreEqualIgnoreNewlineEncoding(
            @"error[E0001]: Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation1
4 │ some other line
  │      ----- some annotation2
5 │ last line
  │
", result.ToString());
    }

    [Fact]
    public void TwoAnnotationsClose()
    {
        var src = new SourceFile(
            "simple.txt",
    @"line 1
prev line
this is a line of text
next line
some other line
last line");
        var diag = new Diagnostics()
            .WithSeverity(Severity.Error)
            .WithCode("E0001")
            .WithMessage("Some error message")
            .WithSourceInfo(Loc(src, line: 2, column: 10, length: 4), Severity.Error, "some annotation1")
            .WithSourceInfo(Loc(src, line: 4, column: 5, length: 5), "some annotation2");
        var result = new StringWriter();
        var renderer = new TextDiagnosticsPresenter(result);
        renderer.Present(diag);
        AssertUtils.AreEqualIgnoreNewlineEncoding(
            @"error[E0001]: Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation1
4 │ next line
5 │ some other line
  │      ----- some annotation2
6 │ last line
  │
", result.ToString());
    }

    [Fact]
    public void TwoAnnotationsTouching()
    {
        var src = new SourceFile(
            "simple.txt",
    @"line 1
prev line
this is a line of text
next line
next line2
some other line
last line");
        var diag = new Diagnostics()
            .WithSeverity(Severity.Error)
            .WithCode("E0001")
            .WithMessage("Some error message")
            .WithSourceInfo(Loc(src, line: 2, column: 10, length: 4), Severity.Error, "some annotation1")
            .WithSourceInfo(Loc(src, line: 5, column: 5, length: 5), "some annotation2");
        var result = new StringWriter();
        var renderer = new TextDiagnosticsPresenter(result);
        renderer.Present(diag);
        AssertUtils.AreEqualIgnoreNewlineEncoding(
            @"error[E0001]: Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation1
4 │ next line
5 │ next line2
6 │ some other line
  │      ----- some annotation2
7 │ last line
  │
", result.ToString());
    }

    [Fact]
    public void TwoAnnotationsAlmostDotted()
    {
        var src = new SourceFile(
            "simple.txt",
    @"line 1
prev line
this is a line of text
next line
line between
next line2
some other line
last line");
        var diag = new Diagnostics()
            .WithSeverity(Severity.Error)
            .WithCode("E0001")
            .WithMessage("Some error message")
            .WithSourceInfo(Loc(src, line: 2, column: 10, length: 4), Severity.Error, "some annotation1")
            .WithSourceInfo(Loc(src, line: 6, column: 5, length: 5), "some annotation2");
        var result = new StringWriter();
        var renderer = new TextDiagnosticsPresenter(result);
        renderer.Present(diag);
        AssertUtils.AreEqualIgnoreNewlineEncoding(
            @"error[E0001]: Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation1
4 │ next line
5 │ line between
6 │ next line2
7 │ some other line
  │      ----- some annotation2
8 │ last line
  │
", result.ToString());
    }

    [Fact]
    public void TwoAnnotationsDotted()
    {
        var src = new SourceFile(
            "simple.txt",
    @"line 1
prev line
this is a line of text
next line
line between
line between2
next line2
some other line
last line");
        var diag = new Diagnostics()
            .WithSeverity(Severity.Error)
            .WithCode("E0001")
            .WithMessage("Some error message")
            .WithSourceInfo(Loc(src, line: 2, column: 10, length: 4), Severity.Error, "some annotation1")
            .WithSourceInfo(Loc(src, line: 7, column: 5, length: 5), "some annotation2");
        var result = new StringWriter();
        var renderer = new TextDiagnosticsPresenter(result);
        renderer.Present(diag);
        AssertUtils.AreEqualIgnoreNewlineEncoding(
            @"error[E0001]: Some error message
  ┌─ simple.txt:3:11
  │
2 │ prev line
3 │ this is a line of text
  │           ^^^^ some annotation1
4 │ next line
  │ ...
7 │ next line2
8 │ some other line
  │      ----- some annotation2
9 │ last line
  │
", result.ToString());
    }

    [Fact]
    public void EmptyLineTrimming()
    {
        var src = new SourceFile(
            "simple.txt",
    @"


error here

context



");
        var diag = new Diagnostics()
            .WithSeverity(Severity.Error)
            .WithCode("E0001")
            .WithMessage("Some error message")
            .WithSourceInfo(Loc(src, line: 3, column: 6, length: 4), Severity.Error, "some annotation");
        var result = new StringWriter();
        var renderer = new TextDiagnosticsPresenter(result);
        renderer.Style.TrimEmptySourceLinesAtEdges = true;
        renderer.Present(diag);
        AssertUtils.AreEqualIgnoreNewlineEncoding(
        @"error[E0001]: Some error message
  ┌─ simple.txt:4:7
  │
4 │ error here
  │       ^^^^ some annotation
  │
", result.ToString());
    }

    [Fact]
    public void EmptyLineTrimmingWithDots()
    {
        var src = new SourceFile(
            "simple.txt",
    @"
hello





bye


");
        var diag = new Diagnostics()
            .WithSeverity(Severity.Error)
            .WithCode("E0001")
            .WithMessage("Some error message")
            .WithSourceInfo(Loc(src, line: 1, column: 0, length: 5), Severity.Error, "some annotation1")
            .WithSourceInfo(Loc(src, line: 7, column: 0, length: 3), Severity.Error, "some annotation2");
        var result = new StringWriter();
        var renderer = new TextDiagnosticsPresenter(result);
        renderer.Style.TrimEmptySourceLinesAtEdges = true;
        renderer.Present(diag);
        AssertUtils.AreEqualIgnoreNewlineEncoding(
        @"error[E0001]: Some error message
  ┌─ simple.txt:2:1
  │
2 │ hello
  │ ^^^^^ some annotation1
  │ ...
8 │ bye
  │ ^^^ some annotation2
  │
", result.ToString());
    }
}
