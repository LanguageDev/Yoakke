// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Xunit;

namespace Yoakke.SynKit.Parser.Tests;

public class ParseErrorTests
{
    [Fact]
    public void MergeSameError()
    {
        var firstError = new ParseError("^", null, 12, "expression");
        var secondError = new ParseError("^", null, 12, "expression");

        var result = firstError | secondError;

        Assert.NotNull(result);
        Assert.Null(result.Got);
        Assert.Equal(12, result.Position);
        Assert.Equal(1, result.Elements.Count);
        Assert.True(result.Elements.ContainsKey("expression"));
        Assert.Equal(1, result.Elements["expression"].Expected.Count);
        Assert.True(result.Elements["expression"].Expected.Contains("^"));
    }

    [Fact]
    public void MergeSameErrorDifferentExpectations()
    {
        var firstError = new ParseError("^", null, 12, "expression");
        var secondError = new ParseError("|", null, 12, "expression");

        var result = firstError | secondError;

        Assert.NotNull(result);
        Assert.Null(result.Got);
        Assert.Equal(12, result.Position);
        Assert.Equal(1, result.Elements.Count);
        Assert.True(result.Elements.ContainsKey("expression"));
        Assert.Equal("expression", result.Elements["expression"].Context);
        Assert.Equal(2, result.Elements["expression"].Expected.Count);
        Assert.True(result.Elements["expression"].Expected.Contains("^"));
        Assert.True(result.Elements["expression"].Expected.Contains("|"));
    }

    [Fact]
    public void MergeThreeErrors()
    {
        var firstError = new ParseError("^", null, 12, "expression");
        var secondError = new ParseError("|", null, 12, "expression");
        var thirdError = new ParseError(":", null, 12, "expression");

        var result = firstError | secondError | thirdError;

        Assert.NotNull(result);
        Assert.Null(result.Got);
        Assert.Equal(12, result.Position);
        Assert.Equal(1, result.Elements.Count);
        Assert.True(result.Elements.ContainsKey("expression"));
        Assert.Equal(3, result.Elements["expression"].Expected.Count);
        Assert.True(result.Elements["expression"].Expected.Contains("^"));
        Assert.True(result.Elements["expression"].Expected.Contains("|"));
        Assert.True(result.Elements["expression"].Expected.Contains(":"));
    }

    [Fact]
    public void MergeTwoErrorsFromDifferentExpressions()
    {
        var firstError = new ParseError("^", null, 12, "expression");
        var secondError = new ParseError("^", null, 12, "other_expression");

        var result = firstError | secondError;

        Assert.NotNull(result);
        Assert.Null(result.Got);
        Assert.Equal(12, result.Position);
        Assert.Equal(2, result.Elements.Count);
        Assert.True(result.Elements.ContainsKey("expression"));
        Assert.Equal(1, result.Elements["expression"].Expected.Count);
        Assert.True(result.Elements["expression"].Expected.Contains("^"));
        Assert.Equal(1, result.Elements["other_expression"].Expected.Count);
        Assert.True(result.Elements["other_expression"].Expected.Contains("^"));
    }
}
