using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Common regular expression constants.
    /// </summary>
    public static class Regex
    {
        /// <summary>
        /// C identifier.
        /// </summary>
        public const string Ident = "[A-Za-z_][A-Za-z0-9_]*";
        /// <summary>
        /// Whitespace characters.
        /// </summary>
        public const string Space = "[ \t\r\n]";
        /// <summary>
        /// Decimal integer.
        /// </summary>
        public const string DecInt = "[0-9]+";
    }
}
