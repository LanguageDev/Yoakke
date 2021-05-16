using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Lexer.Generator
{
    internal class TokenDescription
    {
        public readonly IFieldSymbol Symbol;
        public readonly string Regex;
        public readonly bool Ignore;

        public TokenDescription(IFieldSymbol symbol, string regex, bool ignore)
        {
            Symbol = symbol;
            Regex = regex;
            Ignore = ignore;
        }
    }
}
