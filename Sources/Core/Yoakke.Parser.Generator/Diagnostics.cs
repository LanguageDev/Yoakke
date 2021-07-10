// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.CodeAnalysis;

namespace Yoakke.Parser.Generator
{
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
            category: "Yoakke.Parser.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

#pragma warning restore RS2008 // Enable analyzer release tracking
    }
}
