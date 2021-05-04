using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Lexer.Generator
{
    internal class LexerDescription
    {
        public string ErrorName { get; set; }
        public string EndName { get; set; }
        public IList<TokenDescription> Tokens { get; } = new List<TokenDescription>();
    }
}
