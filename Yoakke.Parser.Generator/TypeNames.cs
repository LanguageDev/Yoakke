namespace Yoakke.Parser.Generator
{
    /// <summary>
    /// Common type-names so we don't have hardcoded strings in the source-generator.
    /// </summary>
    internal static class TypeNames
    {
        private static readonly string ParserNamespace = "Yoakke.Parser";
        private static readonly string LexerNamespace = "Yoakke.Lexer";

        public static readonly string ParserAttribute = $"{ParserNamespace}.Attributes.ParserAttribute";
        public static readonly string RuleAttribute = $"{ParserNamespace}.Attributes.RuleAttribute";
        public static readonly string LeftAttribute = $"{ParserNamespace}.Attributes.LeftAttribute";
        public static readonly string RightAttribute = $"{ParserNamespace}.Attributes.RightAttribute";

        public static readonly string ParserBase = $"{ParserNamespace}.ParserBase";
        public static readonly string ParseError = $"{ParserNamespace}.ParseError";
        public static readonly string ParseErrorElement = $"{ParserNamespace}.ParseErrorElement";
        public static readonly string ParseSuccess = $"{ParserNamespace}.ParseSuccess";
        public static readonly string ParseResult = $"{ParserNamespace}.ParseResult";

        public static readonly string IToken = $"{LexerNamespace}.IToken";
        public static readonly string Token = $"{LexerNamespace}.Token";
        public static readonly string ILexer = $"{LexerNamespace}.ILexer";

        public static readonly string IReadOnlyList = "System.Collections.Generic.IReadOnlyList";
        public static readonly string IList = "System.Collections.Generic.IList";
        public static readonly string List = "System.Collections.Generic.List";
        public static readonly string IEnumerable = "System.Collections.Generic.IEnumerable";
    }
}
