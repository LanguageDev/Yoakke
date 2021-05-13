using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator
{
    internal static class TypeNames
    {
        private static readonly string ParserNamespace = "Yoakke.Parser";
        private static readonly string LexerNamespace = "Yoakke.Lexer";

        public static readonly string ParserAttribute = $"{ParserNamespace}.Attributes.ParserAttribute";
        public static readonly string RuleAttribute = $"{ParserNamespace}.Attributes.RuleAttribute";
        public static readonly string LeftAttribute = $"{ParserNamespace}.Attributes.LeftAttribute";
        public static readonly string RightAttribute = $"{ParserNamespace}.Attributes.RightAttribute";

        public static readonly string ParserBase = $"{ParserNamespace}.ParserBase";

        public static readonly string IToken = $"{LexerNamespace}.IToken";
        public static readonly string Token = $"{LexerNamespace}.Token";
        public static readonly string ILexer = $"{LexerNamespace}.ILexer";

        public static readonly string IList = "System.Collections.Generic.IList";
        public static readonly string List = "System.Collections.Generic.List";
        public static readonly string IEnumerable = "System.Collections.Generic.IEnumerable";
    }
}
