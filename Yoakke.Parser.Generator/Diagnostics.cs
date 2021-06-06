using Microsoft.CodeAnalysis;

namespace Yoakke.Parser.Generator
{
    internal static class Diagnostics
    {
        public static readonly DiagnosticDescriptor UnknownRuleIdentifier = new(
            id: "YKPARSERGEN001",
            title: "Identifier is neither a rule, nor a terminal",
            messageFormat: "The identifier {0} matches no usable rule or token-kind to match",
            category: "Yoakke.Parser.Generator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);
    }
}
