// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Xunit;

namespace Yoakke.SynKit.C.Syntax.Tests;

public class CPreProcessorTests
{
    [Theory]
    [InlineData(
  "a b c",
  "a b c")]
    [InlineData(
  @"#define FOO
FOO",
  "")]
    [InlineData(
  @"#define FOO BAR
FOO",
  "BAR")]
    [InlineData(
  @"#define FOO a b c
FOO",
  "a b c")]
    [InlineData(
  @"#define FOO BAR
#define BAR QUX
FOO",
  "QUX")]
    [InlineData(
  @"#define FOO(x) a x b
FOO(abc)",
  "a abc b")]

    [InlineData(
  @"#define FOO(x) a x b
FOO((x, y, z))",
  "a (x, y, z) b")]
    [InlineData(
  @"#define CAT(a, b) a ## b
CAT(ab, 2)
CAT(L, ""asd"")",
  "ab2 L\"asd\"")]
    [InlineData(
  @"#define FOO() A
#define LP() (
#define RP() )
FOO LP() RP()",
  "FOO ( )")]
    [InlineData(
  @"#define FOO() A
#define EXPAND(x) x
#define LP() (
#define RP() )
EXPAND(FOO LP() RP())",
  "A")]
    [InlineData(
  @"#define FOO(...) __VA_ARGS__
FOO(a, b, c d)",
  "a, b, c d")]
    [InlineData(
  @"#define FOO(...) #__VA_ARGS__
FOO(a, b, c)",
  "\"a, b, c\"")]
    [InlineData(
  @"#define FOO(...) #__VA_ARGS__
FOO(a, b,c)",
  "\"a, b,c\"")]
    [InlineData(
  @"#define FOO(...) #__VA_ARGS__
FOO()",
  "\"\"")]
    [InlineData(
  @"#define FOO(x, ...) x
FOO(hello)",
  "hello")]
    [InlineData(
  @"__COUNTER__
__COUNTER__
__COUNTER__",
  "0 1 2")]
    public void TextEquals(string beforePP, string afterPP)
    {
        var expectLexer = new CLexer(afterPP);
        var pp = new CPreProcessor(new CLexer(beforePP))
            .DefineCounter();
        while (true)
        {
            var expected = expectLexer.Next();
            var got = pp.Next();

            Assert.Equal(expected.Kind, got.Kind);
            Assert.Equal(expected.LogicalText, got.LogicalText);
            if (expected.Kind == CTokenType.End) break;
        }
    }
}
