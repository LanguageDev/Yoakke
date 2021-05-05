using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yoakke.Parser.Generator
{
    [Generator]
    public class ParserSourceGenerator : ISourceGenerator
    {
        private class SyntaxReceiver : ISyntaxReceiver
        {
            public IList<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is ClassDeclarationSyntax classDeclSyntax
                    && classDeclSyntax.AttributeLists.Count > 0)
                {
                    CandidateClasses.Add(classDeclSyntax);
                }
            }
        }

        private INamedTypeSymbol parserAttributeSymbol;
        private INamedTypeSymbol ruleAttributeSymbol;

        public void Initialize(GeneratorInitializationContext context) =>
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

        public void Execute(GeneratorExecutionContext context)
        {
            // TODO: We could factor out some common source-generator patterns, like requiring a library, ...

            if (context.SyntaxReceiver is not SyntaxReceiver receiver) return;

            var compilation = context.Compilation;

            // check that the users compilation references the expected library 
            if (!compilation.ReferencedAssemblyNames.Any(ai => ai.Name.Equals("Yoakke.Parser", StringComparison.OrdinalIgnoreCase)))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.RequiredDependencyNotReferenced,
                    null,
                    "Yoakke.Parser"));
            }

            // Load relevant symbols
            parserAttributeSymbol = compilation.GetTypeByMetadataName(TypeNames.ParserAttribute);
            ruleAttributeSymbol = compilation.GetTypeByMetadataName(TypeNames.RuleAttribute);

            foreach (var syntax in receiver.CandidateClasses)
            {
                var model = compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                // Filter classes without the parser attributes
                if (!HasAttribute(symbol, parserAttributeSymbol)) continue;
                // Generate code for it
                var generated = GenerateImplementation(context, syntax, symbol);
                if (generated != null) context.AddSource($"{symbol.Name}.Generated.cs", generated);
            }
        }

        private string GenerateImplementation(
            GeneratorExecutionContext context,
            ClassDeclarationSyntax syntax,
            INamedTypeSymbol symbol)
        {
            if (!syntax.Modifiers.Any(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PartialKeyword))
            {
                // No partial keyword, error to the user
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.ParserClassMustBePartial,
                    syntax.GetLocation(),
                    syntax.Identifier.ValueText));
                return null;
            }
            if (!SymbolEqualityComparer.Default.Equals(symbol.ContainingSymbol, symbol.ContainingNamespace))
            {
                // Non-top-level declaration, error to the user
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.ParserClassMustBeTopLevel,
                    syntax.GetLocation(),
                    syntax.Identifier.ValueText));
                return null;
            }

            // Extract rules from the method annotations
            var ruleSet = ExtractRuleSet(context, symbol);
            var namespaceName = symbol.ContainingNamespace.ToDisplayString();
            var className = symbol.Name;

            // TODO: For now we just generate a bunch of empty methods

            var parserMethods = new StringBuilder();
            foreach (var rule in ruleSet.Rules)
            {
                parserMethods.AppendLine($"public void Parse{Capitalize(rule.Key)}() {{ }}");
            }

            return $@"
namespace {namespaceName} 
{{
    partial class {className}
    {{
        {parserMethods}
    }}
}}
";
        }

        private RuleSet ExtractRuleSet(GeneratorExecutionContext context, INamedTypeSymbol symbol)
        {
            var result = new RuleSet();
            foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>())
            {
                if (!HasAttribute(method, ruleAttributeSymbol)) continue;

                var bnfString = GetAttributeParam(method, ruleAttributeSymbol);
                var (name, ast) = BnfParser.Parse(bnfString);

                var rule = new Rule(name, ast, method);
                result.Add(rule);
            }
            return result;
        }

        private static bool HasAttribute(ISymbol symbol, INamedTypeSymbol searchedAttr) => symbol
            .GetAttributes()
            .Any(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, searchedAttr));

        private static string GetAttributeParam(ISymbol symbol, INamedTypeSymbol searchedAttr) => symbol
            .GetAttributes()
            .Where(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, searchedAttr))
            .Select(attr => attr.ConstructorArguments.First().Value.ToString())
            .First();

        private static string Capitalize(string str) => char.ToUpper(str[0]) + str.Substring(1).ToLowerInvariant();
    }
}
