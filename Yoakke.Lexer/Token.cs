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
    /// </summary>
    /// <typeparam name="TKind">The kind type this token uses. Usually an enumeration.</typeparam>
    public class Token<TKind> : IToken, IEquatable<Token<TKind>>
    {
        public Range Range { get; }
        public string Text { get; }

        /// <summary>
        /// The kind tag of this token.
        /// </summary>
        public readonly TKind Kind;

        /// <summary>
        /// Initializes a new token.
        /// </summary>
        /// <param name="range">The range of the token in the source.</param>
        /// <param name="text">The text the token was parsed from.</param>
        /// <param name="kind">The kind tag of the token.</param>
        public Token(Range range, string text, TKind kind)
        {
            Range = range;
            Text = text;
            Kind = kind;
        }

        public override bool Equals(object obj) => obj is Token<TKind> t && Equals(t);
        public bool Equals(IToken other) => other is Token<TKind> t && Equals(t);
        public bool Equals(Token<TKind> other) =>
               Range == other.Range
            && Text == other.Text
            && Kind.Equals(other.Kind);

        public override int GetHashCode() => HashCode.Combine(Range, Text);
    }
}
