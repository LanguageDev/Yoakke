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

        public static readonly string LexerAttribute = $"{LexerNamespace}.Attributes.LexerAttribute";

        public static readonly string EndAttribute = $"{LexerNamespace}.Attributes.EndAttribute";
        public static readonly string ErrorAttribute = $"{LexerNamespace}.Attributes.ErrorAttribute";
        public static readonly string IgnoreAttribute = $"{LexerNamespace}.Attributes.IgnoreAttribute";
        public static readonly string RegexAttribute = $"{LexerNamespace}.Attributes.RegexAttribute";
        public static readonly string TokenAttribute = $"{LexerNamespace}.Attributes.TokenAttribute";

        public static readonly string LexerBase = $"{LexerNamespace}.LexerBase";
        public static readonly string Token = $"{LexerNamespace}.Token";
        public static readonly string Position = $"{TextNamespace}.Position";
        public static readonly string Range = $"{TextNamespace}.Range";

        public static readonly string RingBuffer = $"{CollectionsNamespace}.RingBuffer";

        public static readonly string TextReader = "System.IO.TextReader";
    }
}
