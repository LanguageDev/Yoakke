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
        private const string LexerNamespace = "Yoakke.Lexer";

        /// <summary>
        /// System.InvalidOperationException.
        /// </summary>
        public const string InvalidOperationException = "System.InvalidOperationException";

        /// <summary>
        /// System.IO.TextReader.
        /// </summary>
        public const string TextReader = "System.IO.TextReader";

        /// <summary>
        /// The attribute that turns an enumeration into a generated lexer.
        /// </summary>
        public static readonly string LexerAttribute = $"{LexerNamespace}.Attributes.LexerAttribute";

        /// <summary>
        /// Annotates that a token type should be used as the end token.
        /// </summary>
        public static readonly string EndAttribute = $"{LexerNamespace}.Attributes.EndAttribute";

        /// <summary>
        /// Annotates that a token type should be used as the error - or unknown - token.
        /// </summary>
        public static readonly string ErrorAttribute = $"{LexerNamespace}.Attributes.ErrorAttribute";

        /// <summary>
        /// Annotates that a token should be skipped on match.
        /// </summary>
        public static readonly string IgnoreAttribute = $"{LexerNamespace}.Attributes.IgnoreAttribute";

        /// <summary>
        /// Annotates a token definition that is based on a regex.
        /// </summary>
        public static readonly string RegexAttribute = $"{LexerNamespace}.Attributes.RegexAttribute";

        /// <summary>
        /// Annotates a token definition that is based on a literal string.
        /// </summary>
        public static readonly string TokenAttribute = $"{LexerNamespace}.Attributes.TokenAttribute";

        /// <summary>
        /// The LexerBase class that defines common lexer operations.
        /// </summary>
        public static readonly string LexerBase = $"{LexerNamespace}.LexerBase";

        /// <summary>
        /// The pre-defined token class.
        /// </summary>
        public static readonly string Token = $"{LexerNamespace}.Token";
    }
}
