using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Lexer.Generator
{
    /// <summary>
    /// Describes a declared lexer.
    /// </summary>
    internal class LexerDescription
    {
        /// <summary>
        /// The symbol used to define an error/unknown token type.
        /// </summary>
        public IFieldSymbol? ErrorSymbol { get; set; }
        /// <summary>
        /// The symbol used to define an end token type.
        /// </summary>
        public IFieldSymbol? EndSymbol { get; set; }
        /// <summary>
        /// The list of <see cref="TokenDescription"/>s.
        /// </summary>
        public IList<TokenDescription> Tokens { get; } = new List<TokenDescription>();
    }
}
