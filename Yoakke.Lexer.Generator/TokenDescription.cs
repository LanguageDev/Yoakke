using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Lexer.Generator
{
    internal class TokenDescription
    {
        public IFieldSymbol Symbol { get; set; }
        public string Regex { get; set; }
        public bool Ignore { get; set; }
    }
}
