using System;
using System.Diagnostics.CodeAnalysis;

namespace Yoakke.Lexer.Attributes
{
    /// <summary>
    /// An attribute to mark an enum to generate a lexer for it's token types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    [ExcludeFromCodeCoverage]
    public class LexerAttribute : Attribute
    {
        /// <summary>
        /// The lexer classes name to generate.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Initializes a new <see cref="LexerAttribute"/>.
        /// </summary>
        /// <param name="className">The lexer classes name to generate.</param>
        public LexerAttribute(string className)
        {
            ClassName = className;
        }
    }
}
