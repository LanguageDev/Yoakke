// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.Streams;

namespace Yoakke.SynKit.Lexer;

/// <summary>
/// Extensions for an <see cref="ILexer{TToken}"/>.
/// </summary>
public static class LexerExtensions
{
    private class LexerStreamAdapter<TToken> : IStream<TToken>
        where TToken : IToken
    {
        public bool IsEnd => this.lexer.IsEnd;

        private readonly ILexer<TToken> lexer;

        public LexerStreamAdapter(ILexer<TToken> lexer)
        {
            this.lexer = lexer;
        }

        public bool TryConsume(out TToken item)
        {
            item = this.lexer.Next();
            return true;
        }

        public int Consume(int amount) => StreamExtensions.Consume(this, amount);
    }

    /// <summary>
    /// Adapts an <see cref="ILexer{TToken}"/> to be a <see cref="IStream{TToken}"/>.
    /// </summary>
    /// <typeparam name="TToken">The token type produced by the lexer.</typeparam>
    /// <param name="lexer">The lexer to adapt.</param>
    /// <returns>An <see cref="IStream{TToken}"/> that reads from <paramref name="lexer"/>.</returns>
    public static IStream<TToken> ToStream<TToken>(this ILexer<TToken> lexer)
        where TToken : IToken => new LexerStreamAdapter<TToken>(lexer);
}
