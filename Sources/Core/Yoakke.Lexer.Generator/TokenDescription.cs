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

        public TokenDescription(IFieldSymbol symbol, string regex, bool ignore)
        {
            this.Symbol = symbol;
            this.Regex = regex;
            this.Ignore = ignore;
        }
    }
}
