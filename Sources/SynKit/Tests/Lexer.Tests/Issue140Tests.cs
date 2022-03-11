// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.IO;
using Xunit;
using Yoakke.SynKit.Lexer.Attributes;

namespace Yoakke.SynKit.Lexer.Tests;

// https://github.com/LanguageDev/Yoakke/issues/140
public partial class Issue140Tests
{
    internal enum TokenType
    {
        [Error] Error,
        [End] End,
        [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

        [Regex(@"'((\\[^\n\r])|[^\r\n\\'])*'")] Test,
    }

    [Lexer(typeof(TokenType))]
    internal partial class Lexer
    {
    }

    [Theory]
    [InlineData(@"'hello'")]
    [InlineData(@"'hello \' bye'")]
    public void Tests(string input)
    {
        var lexer = new Lexer(input);
        var t = lexer.Next();
        Assert.Equal(input, t.Text);
        Assert.Equal(TokenType.Test, t.Kind);
        Assert.Equal(TokenType.End, lexer.Next().Kind);
    }
}
