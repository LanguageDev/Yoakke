using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Lexer.Generator
{
    internal static class TypeNames
    {
        private static readonly string NamespaceBase = "Yoakke.Lexer";

        public static readonly string InvalidOperationException = $"System.InvalidOperationException";

        public static readonly string LexerAttribute = $"{NamespaceBase}.LexerAttribute";

        public static readonly string EndAttribute = $"{NamespaceBase}.EndAttribute";
        public static readonly string ErrorAttribute = $"{NamespaceBase}.ErrorAttribute";
        public static readonly string IdentAttribute = $"{NamespaceBase}.IdentAttribute";
        public static readonly string IgnoreAttribute = $"{NamespaceBase}.IgnoreAttribute";
        public static readonly string RegexAttribute = $"{NamespaceBase}.RegexAttribute";
        public static readonly string TokenAttribute = $"{NamespaceBase}.TokenAttribute";
    }
}
