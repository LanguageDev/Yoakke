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
        private int varIndex;

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

            var parserMethods = new StringBuilder();
            foreach (var rule in ruleSet.Rules)
            {
                var key = Capitalize(rule.Key);

                // TODO: Check if the return types are all compatible
                var transformedReturnType = rule.Value.First().Method.ReturnType.ToDisplayString();
                var monadicReturnType = GetReturnType(transformedReturnType);

                // Implement a public method
                parserMethods.AppendLine($"public {transformedReturnType} Parse{key}() {{");
                parserMethods.AppendLine($"    var result = impl_Parse{key}(this.index);");
                // TODO: Error handling
                parserMethods.AppendLine("    if (result == null) throw new System.InvalidOperationException();");
                // We update the index and return the result
                parserMethods.AppendLine("    var (index, node) = result.Value;");
                parserMethods.AppendLine("    this.index = index;");
                parserMethods.AppendLine("    return node;");
                parserMethods.AppendLine("}");

                // Implement a private method
                parserMethods.AppendLine($"private {monadicReturnType} impl_Parse{key}(int index) {{");
                parserMethods.AppendLine(GenerateRuleParser(rule.Value));
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

        {parserMethods}
    }}
}}
";
        }

        private string GenerateRuleParser(IList<Rule> alternatives)
        {
            var transformedReturnType = alternatives.First().Method.ReturnType.ToDisplayString();
            var monadicReturnType = GetReturnType(transformedReturnType);
            // NOTE: This is highly ineffective for now, but it's fine
            // let's just start matching all alternatives and the furthest matched one should be accepted
            var code = new StringBuilder();
            var furthestVar = AllocateVarName();
            code.AppendLine($"{monadicReturnType} {furthestVar} = null;");
            foreach (var alt in alternatives)
            {
                var transformedResultVar = AllocateVarName();
                // Generate code for parsing a single alternative
                var resultVar = GenerateBnf(code, alt.Ast, "index");
                code.AppendLine($"if ({resultVar} != null) {{");
                // Generate a pattern to flatten hierarchy
                var binder = GetTopLevelPattern(alt.Ast);
                code.AppendLine($"    var {binder} = {resultVar}.Value.Result;");
                var flattenedValues = binder.Replace("(", "").Replace(")", "");
                // var transformedResultVar = MethodCall(a, b, c, d, e);
                code.AppendLine($"    var {transformedResultVar} = {alt.Method.Name}({flattenedValues});");
                // Now unify with furthest
                code.AppendLine($"    if ({furthestVar} == null || {furthestVar}.Value.Index < {resultVar}.Value.Index) {furthestVar} = ({resultVar}.Value.Index, {transformedResultVar});");
                code.AppendLine("}");
            }
            code.AppendLine($"return {furthestVar};");
            return code.ToString();
        }

        private string GenerateBnf(StringBuilder code, BnfAst node, string lastIndex)
        {
            var parsedType = GetParsedType(node);
            var resultType = GetReturnType(parsedType);
            var resultVar = AllocateVarName();
            code.AppendLine($"{resultType} {resultVar} = null;");

            switch (node)
            {
            case BnfAst.Alt alt:
            {
                var alt1Var = GenerateBnf(code, alt.First, lastIndex);
                var alt2Var = GenerateBnf(code, alt.First, lastIndex);
                // Now we need to choose which one got further or the one that even succeeded
                code.AppendLine($"if ({alt1Var} != null && {alt2Var} != null) {{");
                code.AppendLine($"    if ({alt1Var}.Value.Index > {alt2Var}.Value.Index) {resultVar} = {alt1Var};");
                code.AppendLine($"    else {resultVar} = {alt2Var};");
                code.AppendLine("    }");
                code.AppendLine("}");
                code.AppendLine($"else if ({alt1Var} != null) {resultVar} = {alt1Var};");
                code.AppendLine($"else {resultVar} = {alt2Var};");
                break;
            }

            case BnfAst.Seq seq:
            {
                var firstVar = GenerateBnf(code, seq.First, lastIndex);
                // Only execute the second half, if the first one succeeded
                code.AppendLine($"if ({firstVar} != null) {{");
                var secondVar = GenerateBnf(code, seq.Second, $"{firstVar}.Value.Index");
                code.AppendLine($"    if ({secondVar} != null) {resultVar} = ({secondVar}.Value.Index, ({firstVar}.Value.Result, {secondVar}.Value.Result));");
                code.AppendLine("}");
                break;
            }

            case BnfAst.Opt opt:
            {
                var subVar = GenerateBnf(code, opt.Subexpr, lastIndex);
                // TODO: Might not be correct, might need to take it apart to reconstruct the tuple here
                code.AppendLine($"if ({subVar} != null) {resultVar} = {subVar};");
                code.AppendLine($"else {resultVar} = ({lastIndex}, default);");
                break;
            }

            case BnfAst.Rep0 rep0:
            {
                var listVar = AllocateVarName();
                var indexVar = AllocateVarName();
                code.AppendLine($"var {listVar} = new {TypeNames.List}<{GetParsedType(rep0.Subexpr)}>();");
                code.AppendLine($"var {indexVar} = {lastIndex};");
                code.AppendLine("while (true) {");
                var subVar = GenerateBnf(code, rep0.Subexpr, indexVar);
                code.AppendLine($"    if ({subVar} == null) break;");
                code.AppendLine($"    {indexVar} = {subVar}.Value.Index;");
                code.AppendLine($"    {listVar}.Add({subVar}.Value.Result);");
                code.AppendLine("}");
                code.AppendLine($"{resultVar} = ({indexVar}, {listVar});");
                break;
            }

            case BnfAst.Rep1 rep1:
            {
                var listVar = AllocateVarName();
                var indexVar = AllocateVarName();
                code.AppendLine($"var {listVar} = new {TypeNames.List}<{GetParsedType(rep1.Subexpr)}>();");
                code.AppendLine($"var {indexVar} = {lastIndex};");
                code.AppendLine("while (true) {");
                var subVar = GenerateBnf(code, rep1.Subexpr, indexVar);
                code.AppendLine($"    if ({subVar} == null) break;");
                code.AppendLine($"    {indexVar} = {subVar}.Value.Index;");
                code.AppendLine($"    {listVar}.Add({subVar}.Value.Result);");
                code.AppendLine("}");
                code.AppendLine($"if ({listVar}.Count > 0) {resultVar} = ({indexVar}, {listVar});");
                break;
            }

            case BnfAst.Call call:
            {
                // TODO: Recognize if token kind match?
                var key = Capitalize(call.Name);
                code.AppendLine($"{resultVar} = impl_Parse{key}({lastIndex});");
                break;
            }

            case BnfAst.Literal lit:
                code.AppendLine($"if (this.tokens[{lastIndex}].Text == \"{lit.Value}\") {resultVar} = ({lastIndex} + 1, this.tokens[{lastIndex}]);");
                break;

            default: throw new InvalidOperationException();
            }

            return resultVar;
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

        private string GetParsedType(BnfAst ast)
        {
            switch (ast)
            {
            case BnfAst.Alt alt:
            {
                var left = GetParsedType(alt.First);
                var right = GetParsedType(alt.Second);
                if (left != right) throw new InvalidOperationException("Incompatible alternative types!");
                return left;
            }
            case BnfAst.Seq seq:
            {
                var first = GetParsedType(seq.First);
                var second = GetParsedType(seq.Second);
                return $"({first}, {second})";
            }
            case BnfAst.Call call:
            {
                if (!ruleSet.Rules.TryGetValue(call.Name, out var rules))
                {
                    throw new InvalidOperationException($"Referencing non-existing rule {call.Name}");
                }
                return rules.First().Method.ReturnType.ToDisplayString();
            }
            case BnfAst.Opt opt:
                return $"{GetParsedType(opt.Subexpr)}?";
            case BnfAst.Rep0 rep0:
                return $"{TypeNames.IList}<{GetParsedType(rep0.Subexpr)}>";
            case BnfAst.Rep1 rep1:
                return $"{TypeNames.IList}<{GetParsedType(rep1.Subexpr)}>";
            case BnfAst.Literal:
                return TypeNames.IToken;

            default: throw new InvalidOperationException();
            }
        }

        // Returns the nested binder expression like ((a, b), (c, (d, e)))
        private string GetTopLevelPattern(BnfAst ast) => ast switch 
        {
            BnfAst.Alt alt => $"{GetTopLevelPattern(alt.First)}",
            BnfAst.Seq seq => $"({GetTopLevelPattern(seq.First)}, {GetTopLevelPattern(seq.Second)})",
               BnfAst.Call 
            or BnfAst.Opt
            or BnfAst.Rep0
            or BnfAst.Rep1
            or BnfAst.Literal => AllocateVarName(),
            _ => throw new InvalidOperationException(),
        };

        private string AllocateVarName() => $"a{varIndex++}";

        private static string GetReturnType(string okType) => $"(int Index, {okType} Result)?";

        private static string Capitalize(string str) => char.ToUpper(str[0]) + str.Substring(1).ToLowerInvariant();
    }
}
