using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator
{
    internal static class TypeNames
    {
        private static readonly string ParserNamespace = "Yoakke.Parser";
        private static readonly string LexerNamespace = "Yoakke.Lexer";

        public static readonly string ParserAttribute = $"{ParserNamespace}.ParserAttribute";
        public static readonly string RuleAttribute = $"{ParserNamespace}.RuleAttribute";

        public static readonly string IToken = $"{LexerNamespace}.IToken";

        public static readonly string IList = "System.Collections.Generic.IList";
    }
}
