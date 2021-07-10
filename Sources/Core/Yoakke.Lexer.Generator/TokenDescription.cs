// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.CodeAnalysis;

namespace Yoakke.Lexer.Generator
{
    /// <summary>
    /// A single description of what a token is.
    /// </summary>
    internal class TokenDescription
    {
        /// <summary>
        /// The symbol that defines the token type.
        /// </summary>
        public IFieldSymbol Symbol { get; }

        /// <summary>
        /// The regex that matches the token.
        /// </summary>
        public string Regex { get; }

        /// <summary>
        /// True, if the token-type should be ignored while lexing.
        /// </summary>
        public bool Ignore { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenDescription"/> class.
        /// </summary>
        /// <param name="symbol">The corresponding token type symbol.</param>
        /// <param name="regex">The regex that matches this token.</param>
        /// <param name="ignore">True, if this token should be ignored, when matched.</param>
        public TokenDescription(IFieldSymbol symbol, string regex, bool ignore)
        {
            this.Symbol = symbol;
            this.Regex = regex;
            this.Ignore = ignore;
        }
    }
}
