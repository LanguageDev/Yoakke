// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Xunit;
using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Lexer.Attributes;
using Yoakke.SynKit.Parser.Attributes;
using IgnoreAttribute = Yoakke.SynKit.Lexer.Attributes.IgnoreAttribute;

namespace Yoakke.SynKit.Parser.Tests;

/**
 NOTE: The following typo caused a very big headache
 [Rule("gouping: Identifier")]

 Proposal: When we detect direct left-recursion that we can't resolve, error out and suggest that it's a possible typo
 */
public partial class LeftRecursionTests
{
    internal enum TokenType
    {
        [End] End,
        [Error] Error,
        [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

        [Regex(Regexes.Identifier)] Identifier,
    }

    [Lexer(typeof(TokenType))]
    internal partial class Lexer
    {
    }

    [Parser(typeof(TokenType))]
    internal partial class Parser
    {
        [Rule("grouping : grouping Identifier")]
        private static string Group(string group, IToken next) => $"({group}, {next.Text})";

        [Rule("grouping: Identifier")]
        private static string Ident(IToken t) => t.Text;
    }

    private static string Parse(string source) =>
        new Parser(new Lexer(source)).ParseGrouping().Ok.Value;

    [Theory]
    [InlineData("a", "a")]
    [InlineData("(a, b)", "a b")]
    [InlineData("((a, b), c)", "a b c")]
    [InlineData("(((a, b), c), d)", "a b c d")]
    public void Tests(string expected, string input) => Assert.Equal(expected, Parse(input));
}
