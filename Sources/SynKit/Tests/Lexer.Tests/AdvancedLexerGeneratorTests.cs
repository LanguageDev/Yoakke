// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Xunit;
using Yoakke.SynKit.Lexer.Attributes;
using IgnoreAttribute = Yoakke.SynKit.Lexer.Attributes.IgnoreAttribute;

namespace Yoakke.SynKit.Lexer.Tests;

public partial class AdvancedLexerGeneratorTests : TestBase<AdvancedLexerGeneratorTests.TokenType>
{
    public enum TokenType
    {
        [Ignore][Regex(Regexes.Whitespace)] Whitespace,

        [Error] Error,
        [End] End,

        [Regex("[\\-]")] MinusInRegexGroup,
    }

    [Lexer(typeof(TokenType))]
    internal partial class AdvancedLexer
    {
    }

    [Fact]
    public void AdvancedRulesSequence()
    {
        var lexer = new AdvancedLexer("-");
        Assert.Equal(Token("-", TokenType.MinusInRegexGroup, Range((0, 0), 1)), lexer.Next());
        Assert.Equal(Token(string.Empty, TokenType.End, Range((0, 1), 0)), lexer.Next());
    }
}
