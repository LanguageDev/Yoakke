// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Lexer
{
    /// <summary>
    /// Common regular expression constants. This can be used for the declarative, attribute-based API.
    /// </summary>
    public static class Regex
    {
        /// <summary>
        /// C identifier.
        /// </summary>
        public const string Identifier = "[A-Za-z_][A-Za-z0-9_]*";

        /// <summary>
        /// Whitespace characters.
        /// </summary>
        public const string Whitespace = "[ \t\r\n]";

        /// <summary>
        /// Decimal integer.
        /// </summary>
        public const string IntLiteral = "[0-9]+";

        /// <summary>
        /// C-style hexadecimal integer literal.
        /// </summary>
        public const string HexLiteral = "0x[0-9a-fA-F]+";

        /// <summary>
        /// A single-line string.
        /// </summary>
        public const string StringLiteral = @"""((\\[^\n\r])|[^\r\n\\""])*""";

        /// <summary>
        /// C-style line-comments.
        /// </summary>
        public const string LineComment = @"//[^\r\n]*";

        /// <summary>
        /// C-style multiline comments.
        /// </summary>
        public const string MultilineComment = @"/\*+([^*]|(\*+[^*/]))*\*+/";
    }
}
