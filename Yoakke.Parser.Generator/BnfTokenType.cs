using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator
{
    internal enum BnfTokenType
    {
        End,
        Identifier,
        Colon,
        Pipe,
        OpenParen,
        CloseParen,
        StringLiteral,
        KindLiteral,
        Plus,
        Star,
        QuestionMark,
    }
}
