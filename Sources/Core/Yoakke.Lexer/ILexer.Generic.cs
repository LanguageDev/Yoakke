// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

namespace Yoakke.Lexer
{
    /// <summary>
    /// Represents a general lexer.
    /// It's a stateful iterator-like object that reads in <see cref="IToken{TKind}"/>s from a text source.
    /// </summary>
    /// <typeparam name="TToken">The exact type of token the <see cref="ILexer{TKind}"/> produces.</typeparam>
    public interface ILexer<TToken> : ILexer
        where TToken : IToken
    {
        /// <summary>
        /// Lexes the next <typeparamref name="TToken"/>. If the source text has been depleted, it should produce some default
        /// end-signaling <typeparamref name="TToken"/>.
        /// </summary>
        /// <returns>The lexed <typeparamref name="TToken"/>.</returns>
        public new TToken Next();
    }
}
