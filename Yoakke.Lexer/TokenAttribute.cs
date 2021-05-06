using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lexer
{
    /// <summary>
    /// An attribute to define a token type in terms of a literal string to match.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class TokenAttribute : Attribute
    {
        /// <summary>
        /// The text to match the token.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Initializes a new <see cref="TokenAttribute"/>.
        /// </summary>
        /// <param name="text">The text to match.</param>
        public TokenAttribute(string text)
        {
            Text = text;
        }
    }
}
