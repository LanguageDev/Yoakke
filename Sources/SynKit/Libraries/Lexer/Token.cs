// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Range = Yoakke.SynKit.Text.Range;
using Location = Yoakke.SynKit.Text.Location;

namespace Yoakke.SynKit.Lexer;

/// <summary>
/// A default implementation for <see cref="IToken{TKind}"/>.
/// </summary>
/// <typeparam name="TKind">The kind type this <see cref="Token{TKind}"/> uses. Usually an enumeration type.</typeparam>
public sealed record Token<TKind>(Range Range, Location Location, string Text, TKind Kind) : IToken<TKind>, IEquatable<Token<TKind>>
    where TKind : notnull
{
    /// <inheritdoc/>
    public bool Equals(IToken? other) => this.Equals(other as Token<TKind>);

    /// <inheritdoc/>
    public bool Equals(IToken<TKind>? other) => this.Equals(other as Token<TKind>);
    public bool Equals(Token<TKind>? other) =>
           other is not null
        && this.Range == other.Range
        && this.Location.File?.Path == other.Location.File?.Path
        && this.Text == other.Text
        && this.Kind.Equals(other.Kind);

    /// <inheritdoc/>
    public override int GetHashCode() =>
        HashCode.Combine(this.Range, this.Text, this.Kind, this.Location.File?.Path);
}
