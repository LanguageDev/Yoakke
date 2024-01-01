// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.SynKit.Lexer;

namespace Yoakke.SynKit.C.Syntax;

/// <summary>
/// The token produced by lexing C source code.
/// </summary>
public sealed class CToken : IToken<CTokenType>, IEquatable<CToken>
{
    /// <inheritdoc/>
    public Text.Range Range => this.Location.Range;

    /// <inheritdoc/>
    public Text.Location Location { get; }

    /// <inheritdoc/>
    public string Text { get; }

    /// <inheritdoc/>
    public CTokenType Kind { get; }

    /// <summary>
    /// The logical (escaped) <see cref="Text.Range"/> that the token can be found at.
    /// </summary>
    public Text.Range LogicalRange { get; }

    /// <summary>
    /// The logical (escaped) text value without line-continuations and trigraphs.
    /// </summary>
    public string LogicalText { get; }

    /// <summary>
    /// The <see cref="CToken"/> that this one was expanded from, if this <see cref="CToken"/> originates from
    /// a macro expansion.
    /// </summary>
    public CToken? ExpandedFrom { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CToken"/> class.
    /// </summary>
    /// <param name="range">The <see cref="Text.Range"/> of the <see cref="CToken"/> in the source.</param>
    /// <param name="text">The text of the <see cref="CToken"/>.</param>
    /// <param name="logicalRange">The logical <see cref="Text.Range"/> of the <see cref="CToken"/> in the source.</param>
    /// <param name="logicalText">The original text the <see cref="CToken"/> was parsed from.</param>
    /// <param name="kind">The <see cref="CTokenType"/> of the <see cref="CToken"/>.</param>
    /// <param name="expandedFrom">The <see cref="CToken"/> that this one was expanded from.</param>
    public CToken(Text.Location range, string text, Text.Range logicalRange, string logicalText, CTokenType kind, CToken? expandedFrom)
    {
        this.Location = range;
        this.Text = text;
        this.Kind = kind;
        this.LogicalRange = logicalRange;
        this.LogicalText = logicalText;
        this.ExpandedFrom = expandedFrom;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CToken"/> class.
    /// </summary>
    /// <param name="range">The <see cref="Text.Range"/> of the <see cref="CToken"/> in the source.</param>
    /// <param name="text">The text of the <see cref="CToken"/>.</param>
    /// <param name="logicalRange">The logical <see cref="Text.Range"/> of the <see cref="CToken"/> in the source.</param>
    /// <param name="logicalText">The original text the <see cref="CToken"/> was parsed from.</param>
    /// <param name="kind">The <see cref="CTokenType"/> of the <see cref="CToken"/>.</param>
    public CToken(Text.Location range, string text, Text.Range logicalRange, string logicalText, CTokenType kind)
        : this(range, text, logicalRange, logicalText, kind, null)
    {
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => this.Equals(obj as CToken);

    /// <inheritdoc/>
    public bool Equals(IToken? other) => this.Equals(other as CToken);

    /// <inheritdoc/>
    public bool Equals(IToken<CTokenType>? other) => this.Equals(other as CToken);

    /// <inheritdoc/>
    public bool Equals(CToken? other) =>
           other is not null
        && this.Range == other.Range
        && this.Location.File.Path == other.Location.File.Path
        && this.Text == other.Text
        && this.Kind == other.Kind
        && this.LogicalRange == other.LogicalRange
        && this.LogicalText == other.LogicalText;

    /// <inheritdoc/>
    public override int GetHashCode() =>
        HashCode.Combine(this.Range, this.Text, this.Kind, this.LogicalRange, this.LogicalText);
}
