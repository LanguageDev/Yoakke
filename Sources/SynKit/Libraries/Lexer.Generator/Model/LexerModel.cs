// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Yoakke.SynKit.Lexer.Generator.Model;

/// <summary>
/// Describes a declared lexer.
/// </summary>
/// <param name="SourceField">The symbol containing the source character stream.</param>
/// <param name="ErrorVariant">The symbol used to define an error/unknown token type.</param>
/// <param name="EndVariant">The symbol used to define an end token type.</param>
/// <param name="Tokens">The list of <see cref="TokenModel"/>s.</param>
internal record class LexerModel(
    INamedTypeSymbol LexerType,
    INamedTypeSymbol TokenType,
    ISymbol? SourceField,
    IFieldSymbol ErrorVariant,
    IFieldSymbol EndVariant,
    IReadOnlyList<TokenModel> Tokens);
