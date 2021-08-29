// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Lexer.Streams
{
    /// <summary>
    /// An <see cref="ITokenStream"/> implementation with a backing buffer.
    /// Only assumes the underlying stream to implement <see cref="ITokenStream{IToken}.IsEnd"/> and
    /// <see cref="ITokenStream{IToken}.Advance()"/>.
    /// </summary>
    public class BufferedTokenStream : BufferedTokenStream<IToken>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedTokenStream"/> class.
        /// </summary>
        /// <param name="underlying">The underlying stream.</param>
        public BufferedTokenStream(ITokenStream underlying)
            : base(underlying)
        {
        }
    }
}
