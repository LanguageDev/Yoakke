using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.Parser.Generator.Ast;
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

        private RuleSet ruleSet;

        public ParserSourceGenerator()
            : base("Yoakke.Parser.Generator") { }

        protected override ISyntaxReceiver CreateSyntaxReceiver(GeneratorInitializationContext context) => new SyntaxReceiver();
        protected override bool IsOwnSyntaxReceiver(ISyntaxReceiver syntaxReceiver) => syntaxReceiver is SyntaxReceiver;

        protected override void GenerateCode(ISyntaxReceiver syntaxReceiver)
        {
            var receiver = (SyntaxReceiver)syntaxReceiver;

            RequireLibrary("Yoakke.Lexer");
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
            ruleSet = ExtractRuleSet(symbol);
            ruleSet.Normalize();
            var namespaceName = symbol.ContainingNamespace.ToDisplayString();
            var className = symbol.Name;

            // TODO: For now we just generate a bunch of empty methods

            var parserMethods = new StringBuilder();
            foreach (var rule in ruleSet.Rules)
            {
                var key = Capitalize(rule.Key);

                // TODO: Check if the return types are all compatible
                var transformedReturnType = rule.Value.First().Method.ReturnType.ToDisplayString();
                var monadicReturnType = $"(int, {transformedReturnType})?";

                // Implement a public method
                parserMethods.AppendLine($"public {transformedReturnType} Parse{key}() {{");
                parserMethods.AppendLine($"    var result = impl_Parse{key}(0);");
                // TODO: Error handling
                parserMethods.AppendLine("    if (result == null) throw new System.InvalidOperationException();");
                // We update the index and return the result
                parserMethods.AppendLine("    var (offset, node) = result.Value;");
                parserMethods.AppendLine("    index += offset;");
                parserMethods.AppendLine("    return node;");
                parserMethods.AppendLine("}");

                // Implement a private method
                parserMethods.AppendLine($"private {monadicReturnType} impl_Parse{key}(int offset) {{");
                parserMethods.Append(GenerateRuleParser(rule.Value));
                // TODO
                parserMethods.Append("    throw new System.NotImplementedException();");
                parserMethods.AppendLine("}");
            }

            return $@"
namespace {namespaceName} 
{{
    partial class {className}
    {{
        private {TypeNames.IList}<{TypeNames.IToken}> tokens;
        private int index;

        public {className}({TypeNames.IList}<{TypeNames.IToken}> tokens)
        {{
            this.tokens = tokens;
        }}

        private {TypeNames.IToken} Peek(int offset) => tokens[index + offset];

        {parserMethods}
    }}
}}
";
        }

        private string GenerateRuleParser(IList<Rule> alternatives)
        {
            // NOTE: This is highly ineffective for now, but it's fine
            // let's just start matching all alternatives and the first matched one should be accepted
            var code = new StringBuilder();
            foreach (var alt in alternatives)
            {
                // Generate code for parsing a single alternative
                code.AppendLine("// try this {");
                GenerateBnf(code, alt.Ast);
                code.AppendLine($"// pass results to {alt.Method.Name} to transform");
                code.AppendLine("// }");
            }
            return code.ToString();
        }

        private void GenerateBnf(StringBuilder code, BnfAst node)
        {
            switch (node)
            {
            case BnfAst.Alt alt:
                code.AppendLine("// if matches this: {");
                GenerateBnf(code, alt.First);
                code.AppendLine("// } else match this: {");
                GenerateBnf(code, alt.Second);
                code.AppendLine("// }");
                break;

            case BnfAst.Seq seq:
                GenerateBnf(code, seq.First);
                GenerateBnf(code, seq.Second);
                break;

            case BnfAst.Opt opt:
                code.AppendLine("// optionally match this: {");
                GenerateBnf(code, opt.Subexpr);
                code.AppendLine("// }");
                break;

            case BnfAst.Rep0 rep0:
                code.AppendLine("// match this repeatedly 0 or more times: {");
                GenerateBnf(code, rep0.Subexpr);
                code.AppendLine("// }");
                break;

            case BnfAst.Rep1 rep1:
                code.AppendLine("// match this repeatedly 1 or more times: {");
                GenerateBnf(code, rep1.Subexpr);
                code.AppendLine("// }");
                break;

            case BnfAst.Call call:
                code.AppendLine($"// call {call.Name}");
                break;

            case BnfAst.Literal lit:
                code.AppendLine($"// literal '{lit.Value}'");
                break;

            default: throw new InvalidOperationException();
            }
        }

        private RuleSet ExtractRuleSet(INamedTypeSymbol symbol)
        {
            var ruleAttr = LoadSymbol(TypeNames.RuleAttribute);
            var result = new RuleSet();
            foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>())
            {
                // Since there can be multiple get all rule attributes attached to this method
                foreach (var attr in method.GetAttributes().Where(a => SymbolEquals(a.AttributeClass, ruleAttr)))
                {
                    var bnfString = attr.GetCtorValue().ToString();
                    var (name, ast) = BnfParser.Parse(bnfString);

                    var rule = new Rule(name, ast, method);
                    result.Add(rule);
                }
            }
            return result;
        }

        private static string Capitalize(string str) => char.ToUpper(str[0]) + str.Substring(1).ToLowerInvariant();
    }
}
