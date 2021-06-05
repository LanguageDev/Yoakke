using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Text;
using Range = Yoakke.Text.Range;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Represents an atom in a language grammar as the lowest level element (atom/terminal) of parsing.
    /// Usually tokens have a kind/category tag attached to them, for that <see cref="Token{TKind}"/>.
    /// </summary>
    public interface IToken : IEquatable<IToken>
    {
        /// <summary>
        /// The <see cref="Text.Range"/> that the token can be found at in the source.
        /// </summary>
        public Range Range { get; }
        /// <summary>
        /// The text this token was parsed from.
        /// </summary>
        public string Text { get; }
    }
}
