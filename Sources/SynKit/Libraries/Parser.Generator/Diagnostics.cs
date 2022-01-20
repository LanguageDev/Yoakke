// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.CodeAnalysis;

namespace Yoakke.SynKit.Parser.Generator;

/// <summary>
/// Diagnostics produced by the parser generator.
/// </summary>
internal static class Diagnostics
{
#pragma warning disable RS2008 // Enable analyzer release tracking

    /// <summary>
    /// Happens whan an identifier has no matching rule or token-type.
    /// </summary>
    public static readonly DiagnosticDescriptor UnknownRuleIdentifier = new(
        id: "YKPARSERGEN001",
        title: "Identifier is neither a rule, nor a terminal",
        messageFormat: "The identifier {0} matches no usable rule or token-kind to match",
        category: "Parser generation",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NotPartialType = new(
        id: "YKPARSERGEN002",
        title: "The target parser type is not partial",
        messageFormat: "The target parser type {0} or one of its containing types is not marked as partial",
        category: "Parser generation",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

#pragma warning restore RS2008 // Enable analyzer release tracking
}
