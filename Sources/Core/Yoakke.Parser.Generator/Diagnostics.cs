// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.CodeAnalysis;

namespace Yoakke.Parser.Generator
{
    internal static class Diagnostics
    {
        // TODO: Not sure what this would be good for, consider it

#pragma warning disable RS2008 // Enable analyzer release tracking
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
