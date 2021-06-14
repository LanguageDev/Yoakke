// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;

namespace Yoakke.LSP.Generator
{
    [Lexer("TsLexer")]
    internal enum TokenType
    {
        [Error] Error,
        [End] End,

        [Ignore]
        [Regex(Regex.Whitespace)]
        [Regex(Regex.LineComment)]
        Ignore,

        [Regex(Regex.MultilineComment)]
        DocComment,

        [Token("export")] KwExport,
        [Token("interface")] KwInterface,
        [Token("namespace")] KwNamespace,
        [Token("const")] KwConst,
        [Token("extends")] KwExtends,
        [Token("type")] KwType,

        [Token("(")] OpenParen,
        [Token(")")] CloseParen,
        [Token("{")] OpenBrace,
        [Token("}")] CloseBrace,
        [Token("[")] OpenBracket,
        [Token("]")] CloseBracket,
        [Token("|")] Pipe,
        [Token("?")] Qmark,
        [Token(":")] Colon,
        [Token(";")] Semicolon,

        [Regex(Regex.Identifier)] Ident,
        [Regex(@"'[^']*'")] StringLit,
        [Regex(Regex.IntLiteral)] NumLit,
    }
}
