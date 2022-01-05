// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Xunit;
using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Lexer.Attributes;
using Yoakke.SynKit.Parser.Attributes;
using IgnoreAttribute = Yoakke.SynKit.Lexer.Attributes.IgnoreAttribute;
using Token = Yoakke.SynKit.Lexer.IToken<Yoakke.SynKit.Parser.Tests.Issue59Tests.TokenType>;

namespace Yoakke.SynKit.Parser.Tests;

// https://github.com/LanguageDev/Yoakke/issues/59
public partial class Issue59Tests
{
    internal enum TokenType
    {
        [End] End,
        [Error] Error,
        [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

        [Regex(Regexes.Identifier)] Identifier,

        [Token(".")] Dot,
        [Token("(")] OpenParen,
        [Token(")")] CloseParen,
    }

    [Lexer(typeof(TokenType))]
    internal partial class Lexer
    {
    }

    [Parser(typeof(TokenType))]
    internal partial class Parser
    {
        [Rule("expression : call")]
        [Rule("call : primary")]
        public static string Identity(string expression) => expression;

        [Rule("call : call '(' ')'")]
        public static string FunctionCall(string callee, Token _open, Token _close) => $"({callee}())";

        [Rule("call : call '.' Identifier")]
        public static string MemberAccess(string @object, Token _dot, Token identifier) => $"({@object}.{identifier.Text})";

        [Rule("primary : Identifier")]
        public static string Primary(Token atom) => atom.Text;
    }

    private static string Parse(string source) =>
       new Parser(new Lexer(source)).ParseCall().Ok.Value;

    [Theory]
    [InlineData("x", "x")]
    [InlineData("(x())", "x()")]
    [InlineData("(x.y)", "x.y")]
    [InlineData("(((((((x.y)())()).z).w)()).q)", "x.y()().z.w().q")]
    public void Tests(string expected, string input) => Assert.Equal(expected, Parse(input));
}
