// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Yoakke.SynKit.Lexer;
using Yoakke.SynKit.Lexer.Attributes;
using Yoakke.SynKit.Parser.Attributes;
using IgnoreAttribute = Yoakke.SynKit.Lexer.Attributes.IgnoreAttribute;
using Token = Yoakke.SynKit.Lexer.IToken<Yoakke.SynKit.Parser.Tests.PunctuatedTests.TokenType>;

namespace Yoakke.SynKit.Parser.Tests;

public partial class PunctuatedTests
{
    internal enum TokenType
    {
        [End] End,
        [Error] Error,
        [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

        [Token("(")] Lparen,
        [Token(")")] Rparen,
        [Token(",")] Comma,
        [Regex(Regexes.Identifier)] Identifier,
    }

    [Lexer(typeof(TokenType))]
    internal partial class Lexer
    {
    }

    [Parser(typeof(TokenType))]
    internal partial class Parser
    {
        [Rule("any0_no_trailing : Lparen (Identifier (',' Identifier)*)? Rparen")]
        private static List<string> ZeroOrMoreNoTrailing(IToken _lp, Punctuated<Token, Token> elements, IToken _rp) =>
            elements.Values.Select(t => t.Text).ToList();

        [Rule("any1_no_trailing : Lparen (Identifier (',' Identifier)*) Rparen")]
        private static List<string> OneOrMoreNoTrailing(IToken _lp, Punctuated<Token, Token> elements, IToken _rp) =>
            elements.Values.Select(t => t.Text).ToList();
    }

    private static List<string> Any0NoTrailing(string source) => new Parser(new Lexer(source)).ParseAny0NoTrailing().Ok.Value;

    private static List<string> Any1NoTrailing(string source) => new Parser(new Lexer(source)).ParseAny1NoTrailing().Ok.Value;

    [Fact]
    public void Empty0NoTrailing() => Assert.True(Any0NoTrailing("()").SequenceEqual(Array.Empty<string>()));

    [Fact]
    public void One0NoTrailing() => Assert.True(Any0NoTrailing("(a)").SequenceEqual(new string[] { "a" }));

    [Fact]
    public void Two0NoTrailing() => Assert.True(Any0NoTrailing("(a, b)").SequenceEqual(new string[] { "a", "b" }));

    [Fact]
    public void Many0NoTrailing() => Assert.True(Any0NoTrailing("(a, b, c, d, e)").SequenceEqual(new string[] { "a", "b", "c", "d", "e" }));

    [Fact]
    public void One1NoTrailing() => Assert.True(Any1NoTrailing("(a)").SequenceEqual(new string[] { "a" }));

    [Fact]
    public void Two1NoTrailing() => Assert.True(Any1NoTrailing("(a, b)").SequenceEqual(new string[] { "a", "b" }));

    [Fact]
    public void Many1NoTrailing() => Assert.True(Any1NoTrailing("(a, b, c, d, e)").SequenceEqual(new string[] { "a", "b", "c", "d", "e" }));
}
