// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.CodeAnalysis;

namespace Yoakke.Lexer.Generator
{
    /// <summary>
    /// Diagnostics for the lexer generator.
    /// </summary>
    internal static class Diagnostics
    {
#pragma warning disable RS2008 // Enable analyzer release tracking

        /// <summary>
        /// Happens, when the 'End' or 'Error' is already defined.
        /// </summary>
        public static readonly DiagnosticDescriptor FundamentalTokenTypeAlreadyDefined = new(
            id: "YKLEXERGEN001",
            title: "Fundamental token type already defined",
            messageFormat: "The enumeration value '{0}' already defines an {1} token",
            category: "Yoakke.Lexer.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        /// <summary>
        /// A warning when a token type element has no annotation at all.
        /// </summary>
        public static readonly DiagnosticDescriptor NoAttributeForTokenType = new(
            id: "YKLEXERGEN002",
            title: "No attribute attached to token type",
            messageFormat: "The enumeration value '{0}' has no descriptor attribute attached",
            category: "Yoakke.Lexer.Generator",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        /// <summary>
        /// Happens, when 'End' or 'Error' is not defined.
        /// </summary>
        public static readonly DiagnosticDescriptor FundamentalTokenTypeNotDefined = new(
            id: "YKLEXERGEN003",
            title: "Fundamental token type not defined",
            messageFormat: "There has been no {0} token type defined using the attribute {1}",
            category: "Yoakke.Lexer.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        /// <summary>
        /// Happens, when a regex fails to parse in the generator.
        /// </summary>
        public static readonly DiagnosticDescriptor FailedToParseRegularExpression = new(
            id: "YKLEXERGEN004",
            title: "Failed to parse regular expression",
            messageFormat: "Failed to parse regular expression: {0}",
            category: "Yoakke.Lexer.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

#pragma warning restore RS2008 // Enable analyzer release tracking
    }
}
