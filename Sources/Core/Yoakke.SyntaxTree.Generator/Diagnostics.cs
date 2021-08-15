// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Yoakke.SyntaxTree.Generator
{
    /// <summary>
    /// Diagnostics for the syntax tree generator.
    /// </summary>
    internal static class Diagnostics
    {
#pragma warning disable RS2008 // Enable analyzer release tracking

        /// <summary>
        /// Happens, when the visitor specifies both a return type and a generic return type.
        /// </summary>
        public static readonly DiagnosticDescriptor BothReturnTypeAndGenericReturnTypeSpecified = new(
            id: "YKSYNTAXTREEGEN001",
            title: "Cannot specify both a return type and a generic return type",
            messageFormat: "Cannot specify both a return type and a generic return type for a visitor",
            category: "Yoakke.SyntaxTree.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        /// <summary>
        /// Happens, when the visitor specifies a generic return type that is not defined by the visitor.
        /// </summary>
        public static readonly DiagnosticDescriptor GenericTypeNotDefined = new(
            id: "YKSYNTAXTREEGEN002",
            title: "Generic type not defined",
            messageFormat: "The class '{0}' does not define a generic parameter named '{1}'",
            category: "Yoakke.SyntaxTree.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

#pragma warning restore RS2008 // Enable analyzer release tracking
    }
}
