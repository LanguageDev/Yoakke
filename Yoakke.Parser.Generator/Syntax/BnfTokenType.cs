using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator.Syntax
{
    /// <summary>
    /// The possible different kinds for a <see cref="BnfToken"/>.
    /// </summary>
    internal enum BnfTokenType
    {
        End,
        Identifier,
        Colon,
        Pipe,
        OpenParen,
        CloseParen,
        StringLiteral,
        Plus,
        Star,
        QuestionMark,
    }
}
