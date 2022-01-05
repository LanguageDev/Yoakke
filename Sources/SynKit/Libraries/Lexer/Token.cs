// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Range = Yoakke.SynKit.Text.Range;

namespace Yoakke.SynKit.Lexer;

/// <summary>
/// A default implementation for <see cref="IToken{TKind}"/>.
/// </summary>
/// <typeparam name="TKind">The kind type this <see cref="Token{TKind}"/> uses. Usually an enumeration type.</typeparam>
public sealed record Token<TKind>(Range Range, string Text, TKind Kind) : IToken<TKind>, IEquatable<Token<TKind>>
    where TKind : notnull
{
    /// <inheritdoc/>
    public bool Equals(IToken? other) => this.Equals(other as Token<TKind>);

    /// <inheritdoc/>
    public bool Equals(IToken<TKind>? other) => this.Equals(other as Token<TKind>);
}
