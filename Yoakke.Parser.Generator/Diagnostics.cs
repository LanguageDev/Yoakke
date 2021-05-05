using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator
{
    internal static class Diagnostics
    {
        public static readonly DiagnosticDescriptor RequiredDependencyNotReferenced = new(
            id: "YKPARSERGEN001",
            title: "Required dependency not referenced",
            messageFormat: "Required library {0} is not referenced in the project",
            category: "Yoakke.Parser.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor ParserClassMustBePartial = new DiagnosticDescriptor(
            id: "YKPARSERGEN002",
            title: "Parser class definitions must be partial",
            messageFormat: "Parser class '{0}' definition must be partial",
            category: "Yoakke.Parser.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor ParserClassMustBeTopLevel = new DiagnosticDescriptor(
            id: "YKPARSERGEN003",
            title: "Parser class definitions must be top-level",
            messageFormat: "Parser class '{0}' definition must be a top-level definition",
            category: "Yoakke.Parser.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}
