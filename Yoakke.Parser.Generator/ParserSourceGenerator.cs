using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.SourceGenerator.Common;

namespace Yoakke.Parser.Generator
{
    [Generator]
    public class ParserSourceGenerator : GeneratorBase
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

        public ParserSourceGenerator()
            : base("Yoakke.Parser.Generator") { }

        protected override ISyntaxReceiver CreateSyntaxReceiver(GeneratorInitializationContext context) => new SyntaxReceiver();
        protected override bool IsOwnSyntaxReceiver(ISyntaxReceiver syntaxReceiver) => syntaxReceiver is SyntaxReceiver;

        protected override void GenerateCode(ISyntaxReceiver syntaxReceiver)
        {
            var receiver = (SyntaxReceiver)syntaxReceiver;

            RequireLibrary("Yoakke.Parser");

            foreach (var syntax in receiver.CandidateClasses)
            {
                var model = Context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                // Filter classes without the parser attributes
                if (!HasAttribute(symbol, TypeNames.ParserAttribute)) continue;
                // Generate code for it
                var generated = GenerateImplementation(syntax, symbol);
                if (generated == null) continue;
                AddSource($"{symbol.Name}.Generated.cs", generated);
            }
        }

        private string GenerateImplementation(ClassDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            if (!RequirePartial(syntax) || !RequireNonNested(symbol)) return null;

            // Extract rules from the method annotations
            var ruleSet = ExtractRuleSet(symbol);
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

        private RuleSet ExtractRuleSet(INamedTypeSymbol symbol)
        {
            var result = new RuleSet();
            foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>())
            {
                if (!TryGetAttribute(method, TypeNames.RuleAttribute, out var attr)) continue;

                var bnfString = attr.GetCtorValue().ToString();
                var (name, ast) = BnfParser.Parse(bnfString);

                var rule = new Rule(name, ast, method);
                result.Add(rule);
            }
            return result;
        }

        private static string Capitalize(string str) => char.ToUpper(str[0]) + str.Substring(1).ToLowerInvariant();
    }
}
