using System;

namespace Yoakke.Lexer.Attributes
{
    /// <summary>
    /// An attribute to define a token type in terms of a literal string to match.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
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
            this.Text = text;
        }
    }
}
