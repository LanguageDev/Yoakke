// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Lexer
{
    /// <summary>
    /// Common regular expression constants. This can be used for the declarative, attribute-based API.
    /// </summary>
    public static class Regexes
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
        /// Decimal number with an optional fractionary part and optional scientific notation.
        /// </summary>
        /// <remarks>
        /// Accepts both decimal numbers that are integers and numbers that have a fractionary part (e.g. both <c>2</c> and <c>2.0</c> are valid).
        /// If the whole part is zero, it may be omitted (<c>.12</c> means <c>0.12</c>).
        /// This does not capture the leading sign (<c>+</c> or <c>-</c>), which must be handled separately.
        /// Scientific notation is supported by writting <c>e</c> or <c>E</c> and then using an integer exponent, which may be optionally negative.
        ///
        /// Note that this CANNOT represent all IEEE-754 floating point values, as special values such as infinity or NaN are not supported.
        /// </remarks>
        public const string RealNumberLiteral = "(([0-9]*.)|([0-9]+[0-9_]*.))?[0-9]+[0-9_]*((e|E)-?[0-9]+)?";

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
