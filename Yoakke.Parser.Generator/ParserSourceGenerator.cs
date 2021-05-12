using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Yoakke.Collections.Compatibility;
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
            var namespaceName = symbol.ContainingNamespace.ToDisplayString();
            var className = symbol.Name;

            var parserMethods = new StringBuilder();
            foreach (var rule in ruleSet.Rules)
            {
                var key = Capitalize(rule.Key);

                // TODO: Check if the return types are all compatible
                var parsedType = rule.Value.Ast.GetParsedType(ruleSet);
                var returnType = GetReturnType(parsedType);

                if (rule.Value.PublicApi)
                {
                    // Implement a public method
                    parserMethods.AppendLine($"public {parsedType} Parse{key}() {{");
                    parserMethods.AppendLine($"    var result = impl_Parse{key}(this.index);");
                    // TODO: Error handling
                    parserMethods.AppendLine("    if (result == null) throw new System.InvalidOperationException();");
                    // We update the index and return the result
                    parserMethods.AppendLine("    var (index, node) = result.Value;");
                    parserMethods.AppendLine("    this.index = index;");
                    parserMethods.AppendLine("    return node;");
                    parserMethods.AppendLine("}");
                }

                // Implement a private method
                parserMethods.AppendLine($"private {returnType} impl_Parse{key}(int index) {{");
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

        private string GenerateRuleParser(Rule rule)
        {
            var code = new StringBuilder();
            // "index" is the parameter, that's our default
            var resultVar = GenerateBnf(code, rule.Ast, "index");
            code.AppendLine($"return {resultVar};");
            return code.ToString();
        }

        private string GenerateBnf(StringBuilder code, BnfAst node, string lastIndex)
        {
            var parsedType = node.GetParsedType(ruleSet);
            var resultType = GetReturnType(parsedType);
            var resultVar = AllocateVarName();
            code.AppendLine($"{resultType} {resultVar} = null;");

            switch (node)
            {
            case BnfAst.Transform transform:
            {
                var subVar = GenerateBnf(code, transform.Subexpr, lastIndex);
                var binder = GetTopLevelPattern(transform.Subexpr);
                var flattenedValues = FlattenBind(binder);
                code.AppendLine($"if ({subVar} !=  null) {{");
                code.AppendLine($"    var {binder} = {subVar}.Value.Result;");
                code.AppendLine($"    {resultVar} = ({subVar}.Value.Index, {transform.Method.Name}({flattenedValues}));");
                code.AppendLine("}");
                break;
            }

            case BnfAst.FoldLeft fold:
            {
                var binder = GetTopLevelPattern(fold.Second);
                var flattenedValues = FlattenBind(binder);
                var firstVar = GenerateBnf(code, fold.First, lastIndex);
                code.AppendLine($"if ({firstVar} != null) {{");
                code.AppendLine($"    {resultVar} = {firstVar};");
                code.AppendLine($"    while (true) {{");
                var secondVar = GenerateBnf(code, fold.Second, $"{resultVar}.Value.Index");
                code.AppendLine($"        if ({secondVar} == null) break;");
                code.AppendLine($"        var {binder} = {secondVar}.Value.Result;");
                code.AppendLine($"        {resultVar} = ({secondVar}.Value.Index, {fold.Method.Name}({resultVar}.Value.Result, {flattenedValues}));");
                code.AppendLine("    }");
                code.AppendLine("}");
                break;
            }

            case BnfAst.Alt alt:
            {
                foreach (var element in alt.Elements)
                {
                    var altVar = GenerateBnf(code, element, lastIndex);
                    // Pick the one that got the furthest
                    code.AppendLine($"if ({altVar} != null && ({resultVar} == null || {resultVar}.Value.Index < {altVar}.Value.Index)) {resultVar} = {altVar};");
                }
                break;
            }

            case BnfAst.Seq seq:
            {
                var prevVar = GenerateBnf(code, seq.Elements[0], lastIndex);
                var resultSeq = $"{prevVar}.Value.Result";
                for (int i = 1; i < seq.Elements.Count; ++i)
                {
                    code.AppendLine($"if ({prevVar} != null) {{");
                    var nextVar = GenerateBnf(code, seq.Elements[i], $"{prevVar}.Value.Index");
                    prevVar = nextVar;
                    resultSeq += $", {prevVar}.Value.Result";
                }
                // Unify last
                code.AppendLine($"if ({prevVar} != null) {{");
                code.AppendLine($"{resultVar} = ({prevVar}.Value.Index, ({resultSeq}));");
                // Close nesting
                for (int i = 0; i < seq.Elements.Count; ++i) code.AppendLine("}");
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

            case BnfAst.Rep0:
            case BnfAst.Rep1:
            {
                var subexpr = node is BnfAst.Rep0 r0 ? r0.Subexpr : ((BnfAst.Rep1)node).Subexpr;
                var listVar = AllocateVarName();
                var indexVar = AllocateVarName();
                code.AppendLine($"var {listVar} = new {TypeNames.List}<{subexpr.GetParsedType(ruleSet)}>();");
                code.AppendLine($"var {indexVar} = {lastIndex};");
                code.AppendLine("while (true) {");
                var subVar = GenerateBnf(code, subexpr, indexVar);
                code.AppendLine($"    if ({subVar} == null) break;");
                code.AppendLine($"    {indexVar} = {subVar}.Value.Index;");
                code.AppendLine($"    {listVar}.Add({subVar}.Value.Result);");
                code.AppendLine("}");
                if (node is BnfAst.Rep0)
                {
                    code.AppendLine($"{resultVar} = ({indexVar}, {listVar});");
                }
                else
                {
                    code.AppendLine($"if ({listVar}.Count > 0) {resultVar} = ({indexVar}, {listVar});");
                }
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
            var leftAttr = LoadSymbol(TypeNames.LeftAttribute);
            var rightAttr = LoadSymbol(TypeNames.RightAttribute);

            var result = new RuleSet();

            // Go through the methods in declaration order
            foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>().OrderBy(sym => sym.Locations.First().SourceSpan.Start))
            {
                // Collect associativity attributes in declaration order
                var precedenceTable = method.GetAttributes()
                    .Where(a => SymbolEquals(a.AttributeClass, leftAttr) || SymbolEquals(a.AttributeClass, rightAttr))
                    .OrderBy(a => a.ApplicationSyntaxReference.GetSyntax().GetLocation().SourceSpan.Start)
                    .Select(a => (Left: SymbolEquals(a.AttributeClass, leftAttr), Operators: a.ConstructorArguments.SelectMany(x => x.Values).Select(x => x.Value).ToHashSet()))
                    .ToList();
                // Since there can be multiple get all rule attributes attached to this method
                foreach (var attr in method.GetAttributes().Where(a => SymbolEquals(a.AttributeClass, ruleAttr)))
                {
                    var bnfString = attr.GetCtorValue().ToString();
                    var (name, ast) = BnfParser.Parse(bnfString);

                    if (precedenceTable.Count > 0)
                    {
                        result.AddPrecedence(name, precedenceTable, method);
                        precedenceTable.Clear();
                    }

                    if (ast == null) continue;

                    var rule = new Rule(name, new BnfAst.Transform(ast, method));
                    result.Add(rule);
                }
            }

            result.Desugar();
            return result;
        }

        // Returns the nested binder expression like ((a, b), (c, (d, e)))
        private string GetTopLevelPattern(BnfAst ast) => ast switch 
        {
            BnfAst.Alt alt => $"{GetTopLevelPattern(alt.Elements[0])}",
            BnfAst.Seq seq => $"({string.Join(", ", seq.Elements.Select(GetTopLevelPattern))})",
               BnfAst.Transform
            or BnfAst.Call 
            or BnfAst.Opt
            or BnfAst.Rep0
            or BnfAst.Rep1
            or BnfAst.Literal => AllocateVarName(),
            _ => throw new InvalidOperationException(),
        };

        private string AllocateVarName() => $"a{varIndex++}";

        private static string GetReturnType(string okType) => $"(int Index, {okType} Result)?";
        private static string Capitalize(string str) => char.ToUpper(str[0]) + str.Substring(1).ToLowerInvariant();
        private static string FlattenBind(string bind) => bind.Replace("(", string.Empty).Replace(")", string.Empty);
    }
}
