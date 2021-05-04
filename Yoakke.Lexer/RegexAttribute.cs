using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lexer
{
    /// <summary>
    /// An attribute to define a token type in terms of a regular expression.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class RegexAttribute : Attribute
    {
        /// <summary>
        /// The regex to match the token.
        /// </summary>
        public readonly string Regex;

        /// <summary>
        /// Initializes a new <see cref="RegexAttribute"/>.
        /// </summary>
        /// <param name="regex">The regular expression.</param>
        public RegexAttribute(string regex)
        {
            Regex = regex;
        }
    }
}
