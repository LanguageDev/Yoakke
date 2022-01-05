// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Xunit;
using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Lexer.Attributes;
using Yoakke.SynKit.Parser.Attributes;
using IgnoreAttribute = Yoakke.SynKit.Lexer.Attributes.IgnoreAttribute;

namespace Yoakke.SynKit.Parser.Tests;

public partial class ExpressionParserTests
{
    internal enum TokenType
    {
        [End] End,
        [Error] Error,
        [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

        [Token("+")] Add,
        [Token("-")] Sub,
        [Token("*")] Mul,
        [Token("/")] Div,
        [Token("^")] Exp,
        [Regex(Regexes.IntLiteral)] Number,
        [Token("(")] Lparen,
        [Token(")")] Rparen,
    }

    [Lexer(typeof(TokenType))]
    internal partial class Lexer
    {
    }

    [Parser(typeof(TokenType))]
    internal partial class Parser
    {
        [Rule("top_expression : expression End")]
        private static int TopLevel(int n, IToken _) => n;

        [Right("^")]
        [Left("*", "/")]
        [Left("+", "-")]
        [Rule("expression")]
        private static int Op(int left, IToken op, int right) => op.Text switch
        {
            "+" => left + right,
            "-" => left - right,
            "*" => left * right,
            "/" => left / right,
            "^" => (int)Math.Pow(left, right),
            _ => throw new InvalidOperationException(),
        };

        [Rule("expression : '(' expression ')'")]
        private static int Grouping(IToken _1, int n, IToken _2) => n;

        [Rule("expression : Number")]
        private static int Number(IToken tok) => int.Parse(tok.Text);
    }

    private static int Eval(string s) => new Parser(new Lexer(s)).ParseTopExpression().Ok.Value;

    [Theory]
    [InlineData(3, "1+2")]
    [InlineData(7, "1+2*3")]
    [InlineData(9, "(1+2)*3")]
    [InlineData(0, "3-2-1")]
    [InlineData(2, "3-(2-1)")]
    [InlineData(6561, "3^2^3")]
    [InlineData(729, "(3^2)^3")]
    public void Tests(int value, string text) => Assert.Equal(value, Eval(text));
}
