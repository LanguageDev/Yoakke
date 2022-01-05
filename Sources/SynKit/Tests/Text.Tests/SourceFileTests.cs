// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.IO;
using Xunit;

namespace Yoakke.SynKit.Text.Tests;

public class SourceFileTests
{
    [Fact]
    public void GetAllLinesOfString()
    {
        var sourceFile = new SourceFile(
            "a.txt",
    @"abc
xyz
qwe");
        Assert.Equal(3, sourceFile.AvailableLines);
        Assert.Equal("abc", sourceFile.GetLine(0).TrimEnd());
        Assert.Equal("xyz", sourceFile.GetLine(1).TrimEnd());
        Assert.Equal("qwe", sourceFile.GetLine(2).TrimEnd());
        Assert.Equal("xyz", sourceFile.GetLine(1).TrimEnd());
        Assert.Equal("abc", sourceFile.GetLine(0).TrimEnd());
    }

    [Fact]
    public void GetAllLinesOfStringReader()
    {
        var sourceFile = new SourceFile(
            "a.txt",
    new StringReader(@"abc
xyz
qwe"));
        Assert.Equal(0, sourceFile.AvailableLines);
        Assert.Equal("abc", sourceFile.GetLine(0).TrimEnd());
        Assert.Equal(1, sourceFile.AvailableLines);
        Assert.Equal("xyz", sourceFile.GetLine(1).TrimEnd());
        Assert.Equal(2, sourceFile.AvailableLines);
        Assert.Equal("qwe", sourceFile.GetLine(2).TrimEnd());
        Assert.Equal(3, sourceFile.AvailableLines);
        Assert.Equal("xyz", sourceFile.GetLine(1).TrimEnd());
        Assert.Equal(3, sourceFile.AvailableLines);
        Assert.Equal("abc", sourceFile.GetLine(0).TrimEnd());
        Assert.Equal(3, sourceFile.AvailableLines);
    }

    [Fact]
    public void ReaderPositionUnaffectedByGetLine()
    {
        var sourceFile = new SourceFile(
            "a.txt",
    new StringReader(@"abc
xyz
qwe"));
        sourceFile.GetLine(2);
        Assert.Equal("abc", sourceFile.ReadLine()?.TrimEnd());
    }
}
