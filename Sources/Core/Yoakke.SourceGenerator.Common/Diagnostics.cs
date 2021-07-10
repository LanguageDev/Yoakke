// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.CodeAnalysis;

namespace Yoakke.SourceGenerator.Common
{
    /// <summary>
    /// Common <see cref="DiagnosticDescriptor"/>s.
    /// </summary>
    internal static class Diagnostics
    {
#pragma warning disable RS2008 // Enable analyzer release tracking

        /// <summary>
        /// Descriptor for a <see cref="Diagnostic"/> when a required library is not referenced by the user.
        /// </summary>
        public static readonly DiagnosticDescriptor RequiredLibraryNotReferenced = new(
            id: "YKGEN001",
            title: "Required library not referenced",
            messageFormat: "Required library {0} is not referenced in the project",
            category: "Yoakke.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        /// <summary>
        /// Descriptor for a <see cref="Diagnostic"/> when a type definition is required to be partial but is not.
        /// </summary>
        public static readonly DiagnosticDescriptor TypeDefinitionIsNotPartial = new(
            id: "YKGEN002",
            title: "Defined type is not partial",
            messageFormat: "The type definition {0} is not partial",
            category: "Yoakke.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        /// <summary>
        /// Descriptor for a <see cref="Diagnostic"/> when a symbol is required not to be a nested type but it is.
        /// </summary>
        public static readonly DiagnosticDescriptor SymbolIsNested = new(
            id: "YKGEN003",
            title: "Symbol is nested",
            messageFormat: "The symbol {0} is not partial",
            category: "Yoakke.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

#pragma warning restore RS2008 // Enable analyzer release tracking
    }
}
