// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Lexer.Generator
{
    /// <summary>
    /// Common type-names so we don't have hardcoded strings in the source-generator.
    /// </summary>
    internal static class TypeNames
    {
        private const string CollectionsNamespace = "Yoakke.Collections";
        private const string TextNamespace = "Yoakke.Text";
        private const string LexerNamespace = "Yoakke.Lexer";

        public const string InvalidOperationException = "System.InvalidOperationException";

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

        public const string RingBuffer = "{CollectionsNamespace}.RingBuffer";

        public const string TextReader = "System.IO.TextReader";
    }
}
