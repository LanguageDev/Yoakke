using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lexer
{
    /// <summary>
    /// An attribute to mark an enum to generate a lexer for it's token types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
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
