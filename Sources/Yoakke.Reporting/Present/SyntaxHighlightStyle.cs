// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;

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
            this.TokenColors.TryGetValue(kind, out var col) ? col : this.DefaultColor;
    }
}
