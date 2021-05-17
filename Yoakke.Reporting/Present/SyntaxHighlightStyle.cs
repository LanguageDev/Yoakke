using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Reporting.Present
{
    /// <summary>
    /// Styles for <see cref="ISyntaxHighlighter"/>s.
    /// </summary>
    public class SyntaxHighlightStyle
    {
        /// <summary>
        /// The default syntax highlight style.
        /// </summary>
        public static readonly SyntaxHighlightStyle Default = new SyntaxHighlightStyle();

        /// <summary>
        /// The colors for the different token kinds.
        /// </summary>
        public IDictionary<TokenKind, ConsoleColor> TokenColors { get; set; } = new Dictionary<TokenKind, ConsoleColor>
        {
            { TokenKind.Comment    , ConsoleColor.DarkGreen },
            { TokenKind.Keyword    , ConsoleColor.Magenta   },
            { TokenKind.Literal    , ConsoleColor.Blue      },
            { TokenKind.Name       , ConsoleColor.Cyan      },
            { TokenKind.Operator   , ConsoleColor.DarkCyan  },
            { TokenKind.Punctuation, ConsoleColor.White     },
            { TokenKind.Other      , ConsoleColor.White     },
        };
        /// <summary>
        /// The default color to use.
        /// </summary>
        public ConsoleColor DefaultColor { get; set; } = ConsoleColor.White;

        public ConsoleColor GetTokenColor(TokenKind kind) =>
            TokenColors.TryGetValue(kind, out var col) ? col : DefaultColor;
    }
}
