// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;

namespace Yoakke.Lexer
{
    /// <summary>
    /// A simple <see cref="ILexer"/> implementation that yields the given stream.
    /// </summary>
    public class MemoryLexer : MemoryLexer<IToken>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryLexer"/> class.
        /// </summary>
        /// <param name="tokens">The sequence of <see cref="IToken"/>s to yield as lexed results.</param>
        public MemoryLexer(IEnumerable<IToken> tokens)
            : base(tokens)
        {
        }
    }
}
