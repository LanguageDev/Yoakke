// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.SynKit.Lexer.Attributes;

namespace Yoakke.SynKit.Lexer.Sample;

public enum TokenType
{
    [Error] Error,
    [End] End,
    [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

    [Token("if")] KwIf,
    [Token("else")] KwElse,
    [Token("func")] KwFunc,
    [Regex(Regexes.Identifier)] Ident,

    [Token("{")] OpenBrace,
    [Token("}")] CloseBrace,
    [Token("(")] OpenParen,
    [Token(")")] CloseParen,

    [Token("+")] Plus,
    [Token("-")] Minus,

    [Regex(Regexes.IntLiteral)] IntLiteral,
}

[Lexer(typeof(TokenType))]
internal partial class Lexer
{
}

internal class Program
{
    private static void Main(string[] args)
    {
        var lexer = new Lexer(Console.In);
        while (true)
        {
            var t = lexer.Next();
            Console.WriteLine($"{t.Text} [{t.Kind}] ({t.Range.Start})");
        }
    }
}
