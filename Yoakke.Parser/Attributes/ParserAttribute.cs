using System;

namespace Yoakke.Parser.Attributes
{
    /// <summary>
    /// An attribute to mark a class as a parser with rule methods inside.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ParserAttribute : Attribute
    {
        /// <summary>
        /// The token kind type to use as a parser element.
        /// </summary>
        public readonly Type? TokenType;

        /// <summary>
        /// Initializes a new <see cref="ParserAttribute"/>.
        /// </summary>
        public ParserAttribute()
        {
        }

        /// <summary>
        /// Initializes a new <see cref="ParserAttribute"/>.
        /// </summary>
        /// <param name="tokenType">The token kind type to use as a parser element.</param>
        public ParserAttribute(Type tokenType)
        {
            TokenType = tokenType;
        }
    }
}
