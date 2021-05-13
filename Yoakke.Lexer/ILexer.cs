using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Text;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Represents a general lexer.
    /// </summary>
    public interface ILexer
    {
        /// <summary>
        /// The current position the lexer is at in the source.
        /// </summary>
        public Position Position { get; }
        /// <summary>
        /// Lexes the next token.
        /// </summary>
        /// <returns>The lexed token.</returns>
        public IToken Next();
    }
}
