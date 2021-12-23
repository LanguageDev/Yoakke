// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Range = Yoakke.Text.Range;

namespace Yoakke.Lexer;

/// <summary>
/// A default implementation for <see cref="IToken{TKind}"/>.
/// </summary>
/// <typeparam name="TKind">The kind type this <see cref="Token{TKind}"/> uses. Usually an enumeration type.</typeparam>
public sealed class Token<TKind> : IToken<TKind>, IEquatable<Token<TKind>>
    where TKind : notnull
{
  /// <inheritdoc/>
  public Range Range { get; }

  /// <inheritdoc/>
  public string Text { get; }

  /// <inheritdoc/>
  public TKind Kind { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="Token{TKind}"/> class.
  /// </summary>
  /// <param name="range">The <see cref="Text.Range"/> of the <see cref="Token{TKind}"/> in the source.</param>
  /// <param name="text">The text the <see cref="Token{TKind}"/> was parsed from.</param>
  /// <param name="kind">The <typeparamref name="TKind"/> of the <see cref="Token{TKind}"/>.</param>
  public Token(Range range, string text, TKind kind)
  {
    this.Range = range;
    this.Text = text;
    this.Kind = kind;
  }

  /// <inheritdoc/>
  public override bool Equals(object? obj) => this.Equals(obj as Token<TKind>);

  /// <inheritdoc/>
  public bool Equals(IToken? other) => this.Equals(other as Token<TKind>);

  /// <inheritdoc/>
  public bool Equals(IToken<TKind>? other) => this.Equals(other as Token<TKind>);

  /// <inheritdoc/>
  public bool Equals(Token<TKind>? other) =>
         other is not null
      && this.Range == other.Range
      && this.Text == other.Text
      && this.Kind.Equals(other.Kind);

  /// <inheritdoc/>
  public override int GetHashCode() => HashCode.Combine(this.Range, this.Text, this.Kind);
}
