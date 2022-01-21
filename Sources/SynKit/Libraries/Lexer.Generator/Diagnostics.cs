// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.CodeAnalysis;

namespace Yoakke.SynKit.Lexer.Generator;

/// <summary>
/// Diagnostics for the lexer generator.
/// </summary>
internal static class Diagnostics
{
#pragma warning disable RS2008 // Enable analyzer release tracking

    public static readonly DiagnosticDescriptor NoAttributeForTokenType = new(
        id: "YKLEXERGEN001",
        title: "No descriptor attribute attached to token type",
        messageFormat: "The enumeration value {0} in enum {1} has no token descriptor attribute attached",
        category: "Lexer generation",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NotPartialType = new(
        id: "YKLEXERGEN002",
        title: "The target lexer type is not partial",
        messageFormat: "The target lexer type {0} or one of its containing types is not marked as partial",
        category: "Lexer generation",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NotAnEnumType = new(
        id: "YKLEXERGEN003",
        title: "The specified type is not an enum",
        messageFormat: "The specified token type {0} for the lexer {1} is not an enum type",
        category: "Lexer generation",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TokenTypeAlreadyDefined = new(
        id: "YKLEXERGEN004",
        title: "Fundamental token type already defined",
        messageFormat: "The enumeration value {0} already defines an {1} token in enum {2}",
        category: "Lexer generation",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor TokenTypeNotDefined = new(
        id: "YKLEXERGEN005",
        title: "Fundamental token type not defined",
        messageFormat: "There has been no {0} token type defined in enum {1}",
        category: "Lexer generation",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor RegexParseError = new(
        id: "YKLEXERGEN006",
        title: "Failed to parse regular expression",
        messageFormat: "Failed to parse regular expression {0}",
        category: "Lexer generation",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

#pragma warning restore RS2008 // Enable analyzer release tracking
}
