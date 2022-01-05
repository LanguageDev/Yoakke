// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;

namespace Yoakke.SynKit.Lexer;

/// <summary>
/// Represents an atom in a language grammar as the lowest level element (atom/terminal) of parsing.
/// </summary>
/// <typeparam name="TKind">The kind type this <see cref="IToken{TKind}"/> uses. Usually an enumeration type.</typeparam>
public interface IToken<TKind> : IToken, IEquatable<IToken<TKind>>
{
    /// <summary>
    /// The kind tag of this <see cref="IToken{TKind}"/>.
    /// </summary>
    public TKind Kind { get; }
}
