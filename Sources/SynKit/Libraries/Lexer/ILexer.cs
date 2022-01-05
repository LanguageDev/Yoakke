// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Yoakke.SynKit.Text;

namespace Yoakke.SynKit.Lexer;

/// <summary>
/// Represents a general lexer.
/// It's a stateful iterator-like object that reads in <see cref="IToken"/>s from a text source.
/// </summary>
/// <typeparam name="TToken">The exact type of token the <see cref="ILexer{TToken}"/> produces.</typeparam>
public interface ILexer<out TToken>
    where TToken : IToken
{
    /// <summary>
    /// The current <see cref="Text.Position"/> the <see cref="ILexer{TToken}"/> is at in the source.
    /// </summary>
    public Position Position { get; }

    /// <summary>
    /// True, if all of the input has been consumed.
    /// </summary>
    public bool IsEnd { get; }

    /// <summary>
    /// Lexes the next <typeparamref name="TToken"/>. If the source text has been depleted, it should produce some default
    /// end-signaling <typeparamref name="TToken"/>.
    /// </summary>
    /// <returns>The lexed <typeparamref name="TToken"/>.</returns>
    public TToken Next();
}
