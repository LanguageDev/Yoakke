// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Xunit;
using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Lexer.Attributes;
using Yoakke.SynKit.Parser.Attributes;
using IgnoreAttribute = Yoakke.SynKit.Lexer.Attributes.IgnoreAttribute;

namespace Yoakke.SynKit.Parser.Tests;

// https://github.com/LanguageDev/Yoakke/issues/62
public partial class Issue62Tests
{
    internal enum TokenType
    {
        [End] End,
        [Error] Error,
        [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

        [Regex(Regexes.Identifier)] Identifier,
        [Token(";")] Semicolon,
    }

    [Lexer(typeof(TokenType))]
    internal partial class Lexer
    {
    }

    [Parser(typeof(TokenType))]
    internal partial class Parser
    {
        [Rule("program : statement*")]
        public static string Program(IReadOnlyList<string> idents) => string.Join(", ", idents);

        [Rule("statement : Identifier ';'")]
        public static string Statement(IToken<TokenType> identifier, IToken<TokenType> _) => identifier.Text;
    }

    private static string Parse(string source) =>
       new Parser(new Lexer(source)).ParseProgram().Ok.Value;

    [Theory]
    [InlineData("", "; ^")]
    [InlineData("x", "x ;")]
    [InlineData("x, y", "x; y;")]
    public void Tests(string expected, string input) => Assert.Equal(expected, Parse(input));
}
