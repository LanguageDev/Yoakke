// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Text;

namespace Yoakke.Lexer
{
    /// <summary>
    /// A simple <see cref="ILexer"/> implementation that yields the given stream.
    /// </summary>
    /// <typeparam name="TToken">The exact type of token this <see cref="MemoryLexer{TToken}"/> produces.</typeparam>
    public class MemoryLexer<TToken> : ILexer<TToken>
        where TToken : IToken
    {
        private readonly IEnumerator<TToken> tokens;
        private TToken last;

        /// <inheritdoc/>
        public Position Position => this.last.Range.Start;

        /// <inheritdoc/>
        public bool IsEnd { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryLexer{TToken}"/> class.
        /// </summary>
        /// <param name="tokens">The sequence of <see cref="IToken"/>s to yield as lexed results.</param>
        public MemoryLexer(IEnumerable<TToken> tokens)
        {
            this.tokens = tokens.GetEnumerator();
            this.IsEnd = !this.tokens.MoveNext();
            if (this.IsEnd) throw new ArgumentException("The provided sequence contains no tokens. At least an end token must be contained.", nameof(tokens));
            this.last = this.tokens.Current;
        }

        /// <inheritdoc/>
        public TToken Next()
        {
            this.IsEnd = !this.tokens.MoveNext();
            if (!this.IsEnd) this.last = this.tokens.Current;
            return this.last;
        }

        /// <inheritdoc/>
        IToken ILexer.Next() => this.Next();
    }
}
