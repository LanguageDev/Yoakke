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
        public readonly IFieldSymbol Symbol;
        /// <summary>
        /// The regex that matches the token.
        /// </summary>
        public readonly string Regex;
        /// <summary>
        /// True, if the token-type should be ignored while lexing.
        /// </summary>
        public readonly bool Ignore;

        public TokenDescription(IFieldSymbol symbol, string regex, bool ignore)
        {
            this.Symbol = symbol;
            this.Regex = regex;
            this.Ignore = ignore;
        }
    }
}
