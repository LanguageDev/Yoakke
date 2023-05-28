// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Xunit;
using Yoakke.SynKit.Lexer.Attributes;
using Yoakke.SynKit.Lexer.Tests;
using Yoakke.SynKit.Lexer;
using IgnoreAttribute = Yoakke.SynKit.Lexer.Attributes.IgnoreAttribute;


public enum NoNamespaceTokenType
{
    [Ignore][Regex(Regexes.Whitespace)] Whitespace,

    [Error] Error,
    [End] End,

    [Token("IF")][Token("if")] KwIf,
    [Token("else")] KwElse,
    [Regex(Regexes.Identifier)] Identifier,
    [Token("+")] Plus,
    [Token("-")] Minus,
    [Regex(Regexes.IntLiteral)] Number,
}

[Lexer(typeof(NoNamespaceTokenType))]
internal partial class NoNamespaceLexer
{
}

public partial class NoNamespaceLexerGeneratorTests : TestBase<NoNamespaceTokenType>
{

    [Fact]
    public void Empty()
    {
        var lexer = new NoNamespaceLexer(string.Empty);
        Assert.Equal(Token(string.Empty, NoNamespaceTokenType.End, Range((0, 0), 0)), lexer.Next());
    }

    [Fact]
    public void EmptySpaces()
    {
        var lexer = new NoNamespaceLexer("   ");
        Assert.Equal(Token(string.Empty, NoNamespaceTokenType.End, Range((0, 3), 0)), lexer.Next());
    }

    [Fact]
    public void SimpleSequence()
    {
        var lexer = new NoNamespaceLexer("if    asd+b 123");
        Assert.Equal(Token("if", NoNamespaceTokenType.KwIf, Range((0, 0), 2)), lexer.Next());
        Assert.Equal(Token("asd", NoNamespaceTokenType.Identifier, Range((0, 6), 3)), lexer.Next());
        Assert.Equal(Token("+", NoNamespaceTokenType.Plus, Range((0, 9), 1)), lexer.Next());
        Assert.Equal(Token("b", NoNamespaceTokenType.Identifier, Range((0, 10), 1)), lexer.Next());
        Assert.Equal(Token("123", NoNamespaceTokenType.Number, Range((0, 12), 3)), lexer.Next());
        Assert.Equal(Token(string.Empty, NoNamespaceTokenType.End, Range((0, 15), 0)), lexer.Next());
    }
}
