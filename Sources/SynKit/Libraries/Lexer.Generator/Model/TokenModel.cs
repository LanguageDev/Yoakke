// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.CodeAnalysis;
using Yoakke.SynKit.Automata.RegExAst;

namespace Yoakke.SynKit.Lexer.Generator.Model;

/// <summary>
/// A single description of what a token is.
/// </summary>
/// <param name="Symbol">The symbol that defines the token type.</param>
/// <param name="Regex">The regex that matches the token.</param>
/// <param name="Ignore">True, if the token-type should be ignored while lexing.</param>
internal record class TokenModel(
    IFieldSymbol Symbol,
    IRegExNode<char> Regex,
    bool Ignore);
