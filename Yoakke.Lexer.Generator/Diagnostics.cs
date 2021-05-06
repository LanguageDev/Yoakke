using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Lexer.Generator
{
    internal static class Diagnostics
    {
        public static readonly DiagnosticDescriptor FundamentalTokenTypeAlreadyDefined = new(
            id: "YKLEXERGEN001",
            title: "Fundamental token type already defined",
            messageFormat: "The enumeration value '{0}' already defines an {1} token",
            category: "Yoakke.Lexer.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor NoAttributeForTokenType = new(
            id: "YKLEXERGEN002",
            title: "No attribute attached to token type",
            messageFormat: "The enumeration value '{0}' has no descriptor attribute attached",
            category: "Yoakke.Lexer.Generator",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor FundamentalTokenTypeNotDefined = new(
            id: "YKLEXERGEN003",
            title: "Fundamental token type not defined",
            messageFormat: "There has been no {0} token type defined using the attribute {1}",
            category: "Yoakke.Lexer.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public static readonly DiagnosticDescriptor FailedToParseRegularExpression = new(
            id: "YKLEXERGEN004",
            title: "Failed to parse regular expression",
            messageFormat: "Failed to parse regular expression: {0}",
            category: "Yoakke.Lexer.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}
