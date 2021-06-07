using System;
using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Lexer.Attributes
{
    /// <summary>
    /// An attribute to define a token type in terms of a regular expression.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    [ExcludeFromCodeCoverage]
    public class RegexAttribute : Attribute
    {
        /// <summary>
        /// The regex to match the token.
        /// </summary>
        public string Regex { get; set; }

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
