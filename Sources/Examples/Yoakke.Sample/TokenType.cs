// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;

namespace Yoakke.Sample
{
    public enum TokenType
    {
        [Error] Error,
        [End] End,

        [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

        [Token("func")] KwFunc,
        [Token("if")] KwIf,
        [Token("else")] KwElse,
        [Token("while")] KwWhile,
        [Token("return")] KwReturn,
        [Token("var")] KwVar,
        [Token("and")] KwAnd,
        [Token("or")] KwOr,
        [Token("not")] KwNot,

        [Token("{")] OpenBrace,
        [Token("}")] CloseBrace,
        [Token("(")] OpenParen,
        [Token(")")] CloseParen,
        [Token(";")] Semicol,

        [Token("+")] Plus,
        [Token("-")] Minus,
        [Token("*")] Star,
        [Token("/")] Slash,
        [Token("%")] Percent,
        [Token("=")] Assign,
        [Token("==")] Equal,
        [Token("!=")] NotEqual,
        [Token(">")] Greater,
        [Token("<")] Less,
        [Token(">=")] GreaterEqual,
        [Token("<=")] LessEqual,
        [Token("!")] Exclamation,
        [Token(",")] Comma,

        [Regex(Regexes.Identifier)] Ident,
        [Regex(Regexes.IntLiteral)] IntLit,
        [Regex(Regexes.StringLiteral)] StrLit,
    }

    [Lexer(typeof(TokenType))]
    public partial class Lexer
    {
    }
}
