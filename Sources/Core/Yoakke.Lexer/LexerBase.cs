// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.IO;
using System.Text;
using Yoakke.Collections;
using Yoakke.Text;
using Range = Yoakke.Text.Range;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Base-class to provide common functionality for lexers, if a custom solution is needed.
    /// </summary>
    public abstract class LexerBase : LexerBaseCommon, ILexer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LexerBase"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> that reads the source text.</param>
        protected LexerBase(TextReader reader)
            : base(reader)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexerBase"/> class.
        /// </summary>
        /// <param name="source">The string to lex.</param>
        protected LexerBase(string source)
            : base(source)
        {
        }

        /// <summary>
        /// Lexes the next <see cref="IToken"/> in the input.
        /// </summary>
        /// <returns>The lexed <see cref="IToken"/>.</returns>
        public abstract IToken Next();
    }
}
