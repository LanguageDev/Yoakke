using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;

namespace Yoakke.LSP.Generator
{
    [Lexer("TsLexer")]
    enum TokenType
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
