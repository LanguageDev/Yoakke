using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;

namespace Yoakke.Sample
{
    [Lexer("Lexer")]
    public enum TokenType
    {
        [Error] Error,
        [End] End,

        [Ignore] [Regex(Regex.Whitespace)] Whitespace,

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

        [Regex(Regex.Identifier)] Ident,
        [Regex(Regex.IntLiteral)] IntLit,
        [Regex(Regex.StringLiteral)] StrLit,
    }
}
