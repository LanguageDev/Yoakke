// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Yoakke.Lexer.Generator
{
    /// <summary>
    /// Describes a declared lexer.
    /// </summary>
    internal class LexerDescription
    {
        /// <summary>
        /// The symbol containing the source character stream.
        /// </summary>
        public ISymbol? SourceSymbol { get; set; }

        /// <summary>
        /// The symbol used to define an error/unknown token type.
        /// </summary>
        public IFieldSymbol? ErrorSymbol { get; set; }

        /// <summary>
        /// The symbol used to define an end token type.
        /// </summary>
        public IFieldSymbol? EndSymbol { get; set; }

        /// <summary>
        /// The list of <see cref="TokenDescription"/>s.
        /// </summary>
        public IList<TokenDescription> Tokens { get; } = new List<TokenDescription>();
    }
}
