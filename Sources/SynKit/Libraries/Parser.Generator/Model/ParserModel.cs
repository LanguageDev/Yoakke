// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Yoakke.SynKit.Parser.Generator.Model;

internal record class ParserModel(
    INamedTypeSymbol ParserType,
    ISymbol? SourceField,
    RuleSet RuleSet,
    TokenKindSet TokenKinds);
