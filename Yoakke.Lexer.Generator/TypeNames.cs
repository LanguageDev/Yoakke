using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Lexer.Generator
{
    internal static class TypeNames
    {
        private static readonly string CollectionsNamespace = "Yoakke.Collections";
        private static readonly string TextNamespace = "Yoakke.Text";
        private static readonly string LexerNamespace = "Yoakke.Lexer";

        public static readonly string InvalidOperationException = $"System.InvalidOperationException";

        public static readonly string LexerAttribute = $"{LexerNamespace}.LexerAttribute";

        public static readonly string EndAttribute = $"{LexerNamespace}.EndAttribute";
        public static readonly string ErrorAttribute = $"{LexerNamespace}.ErrorAttribute";
        public static readonly string IdentAttribute = $"{LexerNamespace}.IdentAttribute";
        public static readonly string IgnoreAttribute = $"{LexerNamespace}.IgnoreAttribute";
        public static readonly string RegexAttribute = $"{LexerNamespace}.RegexAttribute";
        public static readonly string TokenAttribute = $"{LexerNamespace}.TokenAttribute";

        public static readonly string Token = $"{LexerNamespace}.Token";
        public static readonly string Position = $"{TextNamespace}.Position";
        public static readonly string Range = $"{TextNamespace}.Range";

        public static readonly string RingBuffer = $"{CollectionsNamespace}.RingBuffer";

        public static readonly string TextReader = "System.IO.TextReader";
        public static readonly string StringReader = "System.IO.StringReader";
    }
}
