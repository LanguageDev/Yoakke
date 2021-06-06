using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Yoakke.Collections.Compatibility;
using Yoakke.Parser.Generator.Ast;
using Yoakke.Parser.Generator.Syntax;
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

        private RuleSet? ruleSet;
        private int varIndex;
        private TokenKindSet? tokenKinds;

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
                if (!HasAttribute(symbol!, TypeNames.ParserAttribute)) continue;
                // Generate code for it
                var generated = GenerateImplementation(syntax, symbol!);
                if (generated == null) continue;
                AddSource($"{symbol!.ToDisplayString()}.Generated.cs", generated);
            }
        }

        private string? GenerateImplementation(ClassDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            if (!RequirePartial(syntax) || !RequireNonNested(symbol)) return null;

            var parserAttr = GetAttribute(symbol, TypeNames.ParserAttribute);
            if (parserAttr.ConstructorArguments.Length > 0)
            {
                var tokenType = (INamedTypeSymbol)parserAttr.ConstructorArguments.First().Value!;
                tokenKinds = new TokenKindSet(tokenType);
            }
            else
            {
                tokenKinds = new TokenKindSet();
            }
            // Extract rules from the method annotations
            ruleSet = ExtractRuleSet(symbol);
            var namespaceName = symbol.ContainingNamespace.ToDisplayString();
            var className = symbol.Name;

            var parserMethods = new StringBuilder();
            foreach (var rule in ruleSet.Rules)
            {
                var key = ToPascalCase(rule.Key);

                // TODO: Check if the return types are all compatible
                var parsedType = rule.Value.Ast.GetParsedType(ruleSet, tokenKinds);
                var returnType = GetReturnType(parsedType);

                if (rule.Value.PublicApi)
                {
                    // Part of public API

                    // Implement a try... pattern method
                    parserMethods.AppendLine($"public bool TryParse{key}(out {parsedType} value) {{");
                    parserMethods.AppendLine($"    var result = parse{key}(0);");
                    // Failure case
                    parserMethods.AppendLine("    if (result.IsError) {");
                    parserMethods.AppendLine("        value = default;");
                    parserMethods.AppendLine("        return false;");
                    parserMethods.AppendLine("    }");
                    // Success case
                    parserMethods.AppendLine("    value = result.Ok.Value;");
                    parserMethods.AppendLine("    this.TryConsume(result.Ok.Offset);");
                    parserMethods.AppendLine("    return true;");
                    parserMethods.AppendLine("}");

                    // Implement a regular parse-result method
                    parserMethods.AppendLine($"public {returnType} Parse{key}() {{");
                    parserMethods.AppendLine($"    var result = parse{key}(0);");
                    parserMethods.AppendLine("    if (result.IsOk) {");
                    parserMethods.AppendLine("        this.TryConsume(result.Ok.Offset);");
                    parserMethods.AppendLine("    } else {");
                    // Try to consume one so the parser won't get stuck
                    // TODO: Maybe let the user do this or be smarter about it?
                    parserMethods.AppendLine("        this.TryConsume(1);");
                    parserMethods.AppendLine("    }");
                    parserMethods.AppendLine("    return result;");
                    parserMethods.AppendLine("}");
                }

                // Implement a private method
                parserMethods.AppendLine($"private {returnType} parse{key}(int offset) {{");
                parserMethods.AppendLine(GenerateRuleParser(rule.Value));
                parserMethods.AppendLine("}");
            }

            return $@"
namespace {namespaceName} 
{{
    partial class {className} : {TypeNames.ParserBase}
    {{
        public {className}({TypeNames.ILexer} lexer)
            : base(lexer)
        {{
        }}

        public {className}({TypeNames.IEnumerable}<{TypeNames.IToken}> tokens)
            : base(tokens)
        {{
        }}

        {parserMethods}
    }}
}}
";
        }

        private string GenerateRuleParser(Rule rule)
        {
            var code = new StringBuilder();
            // By default we are at "index"
            var resultVar = GenerateBnf(code, rule, rule.Ast, "offset");
            code.AppendLine($"return {resultVar};");
            return code.ToString();
        }

        private string GenerateBnf(StringBuilder code, Rule rule, BnfAst node, string lastIndex)
        {
            var parsedType = node.GetParsedType(ruleSet!, tokenKinds!);
            var resultType = GetReturnType(parsedType);
            var resultVar = AllocateVarName();
            code.AppendLine($"{resultType} {resultVar};");

            switch (node)
            {
            case BnfAst.Transform transform:
            {
                var subVar = GenerateBnf(code, rule, transform.Subexpr, lastIndex);
                var binder = GetTopLevelPattern(transform.Subexpr);
                var flattenedValues = FlattenBind(binder);
                var call = $"{transform.Method.Name}({flattenedValues})";
                code.AppendLine($"if ({subVar}.IsOk) {{");
                code.AppendLine($"    var {binder} = {subVar}.Ok.Value;");
                code.AppendLine($"    {resultVar} = {TypeNames.ParserBase}.Ok({call}, {subVar}.Ok.Offset, {subVar}.Ok.FurthestError);");
                code.AppendLine("} else {");
                code.AppendLine($"    {resultVar} = {subVar}.Error;");
                code.AppendLine("}");
                break;
            }

            case BnfAst.FoldLeft fold:
            {
                var binder = GetTopLevelPattern(fold.Second);
                var flattenedValues = FlattenBind(binder);
                var firstVar = GenerateBnf(code, rule, fold.First, lastIndex);
                var call = $"{fold.Method.Name}({resultVar}.Ok.Value, {flattenedValues})";
                code.AppendLine($"if ({firstVar}.IsOk) {{");
                code.AppendLine($"    {resultVar} = {firstVar};");
                code.AppendLine("    while (true) {");
                var secondVar = GenerateBnf(code, rule, fold.Second, $"{resultVar}.Ok.Offset");
                var errMerge = $"{TypeNames.ParseError}.Unify({resultVar}.FurthestError, {secondVar}.FurthestError)";
                code.AppendLine($"        if ({secondVar}.IsError) {{");
                code.AppendLine($"            {resultVar} = {TypeNames.ParserBase}.MergeError({resultVar}.Ok, {secondVar}.Error);");
                code.AppendLine("            break;");
                code.AppendLine("        }");
                code.AppendLine($"        var {binder} = {secondVar}.Ok.Value;");
                code.AppendLine($"        {resultVar} = {TypeNames.ParserBase}.Ok({call}, {secondVar}.Ok.Offset, {errMerge});");
                code.AppendLine("    }");
                code.AppendLine("} else {");
                code.AppendLine($"    {resultVar} = {firstVar}.Error;");
                code.AppendLine("}");
                break;
            }

            case BnfAst.Alt alt:
            {
                bool first = true;
                foreach (var element in alt.Elements)
                {
                    var altVar = GenerateBnf(code, rule, element, lastIndex);
                    if (first)
                    {
                        // First, just keep that
                        code.AppendLine($"{resultVar} = {altVar};");
                        first = false;
                    }
                    else
                    {
                        // Pick the one that got the furthest
                        code.AppendLine($"{resultVar} = {TypeNames.ParserBase}.MergeAlternatives({resultVar}, {altVar});");
                    }
                }
                break;
            }

            case BnfAst.Seq seq:
            {
                var varStack = new Stack<string>();
                var prevVar = GenerateBnf(code, rule, seq.Elements[0], lastIndex);
                varStack.Push(prevVar);
                var resultSeq = $"{prevVar}.Ok.Value";
                for (int i = 1; i < seq.Elements.Count; ++i)
                {
                    code.AppendLine($"if ({prevVar}.IsOk) {{");
                    var nextVar = GenerateBnf(code, rule, seq.Elements[i], $"{prevVar}.Ok.Offset");
                    // Update it with the error
                    code.AppendLine($"{nextVar} = MergeError({nextVar}, {prevVar}.Ok.FurthestError);");
                    prevVar = nextVar;
                    varStack.Push(prevVar);
                    resultSeq += $", {prevVar}.Ok.Value";
                }
                // Unify last
                code.AppendLine($"if ({prevVar}.IsOk) {{");
                code.AppendLine($"    {resultVar} = {TypeNames.ParserBase}.Ok(({resultSeq}), {prevVar}.Ok.Offset, {prevVar}.Ok.FurthestError);");
                code.AppendLine("} else {");
                varStack.Pop();
                code.AppendLine($"    {resultVar} = {prevVar}.Error;");
                code.AppendLine("}");
                // Close nesting and errors
                while (varStack.TryPop(out var top))
                {
                    code.AppendLine("} else {");
                    code.AppendLine($"    {resultVar} = {top}.Error;");
                    code.AppendLine("}");
                }
                break;
            }

            case BnfAst.Opt opt:
            {
                var subVar = GenerateBnf(code, rule, opt.Subexpr, lastIndex);
                code.AppendLine($"if ({subVar}.IsOk) {resultVar} = {TypeNames.ParserBase}.Ok<{parsedType}>({subVar}.Ok.Value, {subVar}.Ok.Offset, {subVar}.Ok.FurthestError);");
                code.AppendLine($"else {resultVar} = {TypeNames.ParserBase}.Ok(default({parsedType}), {lastIndex}, {subVar}.Error);");
                break;
            }

            case BnfAst.Rep0 r0:
            {
                var elementType = r0.Subexpr.GetParsedType(ruleSet!, tokenKinds!);
                var listVar = AllocateVarName();
                var indexVar = AllocateVarName();
                var errVar = AllocateVarName();
                code.AppendLine($"var {listVar} = new {TypeNames.List}<{elementType}>();");
                code.AppendLine($"var {indexVar} = {lastIndex};");
                code.AppendLine($"{TypeNames.ParseError} {errVar} = null;");
                code.AppendLine("while (true) {");
                var subVar = GenerateBnf(code, rule, r0.Subexpr, indexVar);
                code.AppendLine($"    if ({subVar}.IsError) {{");
                code.AppendLine($"        {errVar} = {TypeNames.ParseError}.Unify({errVar}, {subVar}.Error);");
                code.AppendLine("        break;");
                code.AppendLine("    }");
                code.AppendLine($"    {indexVar} = {subVar}.Ok.Offset;");
                code.AppendLine($"    {listVar}.Add({subVar}.Ok.Value);");
                code.AppendLine($"    {errVar} = {TypeNames.ParseError}.Unify({errVar}, {subVar}.Ok.FurthestError);");
                code.AppendLine("}");
                code.AppendLine($"{resultVar} = {TypeNames.ParserBase}.Ok(({parsedType}){listVar}, {indexVar}, {errVar});");
                break;
            }

            case BnfAst.Rep1 r1:
            {
                var elementType = r1.Subexpr.GetParsedType(ruleSet!, tokenKinds!);
                var listVar = AllocateVarName();
                var indexVar = AllocateVarName();
                var errVar = AllocateVarName();
                var firstVar = GenerateBnf(code, rule, r1.Subexpr, lastIndex);
                code.AppendLine($"if ({firstVar}.IsOk) {{");
                code.AppendLine($"    var {listVar} = new {TypeNames.List}<{elementType}>();");
                code.AppendLine($"    {listVar}.Add({firstVar}.Ok.Value);");
                code.AppendLine($"    var {indexVar} = {firstVar}.Ok.Offset;");
                code.AppendLine($"    {TypeNames.ParseError} {errVar} = null;");
                code.AppendLine("    while (true) {");
                var subVar = GenerateBnf(code, rule, r1.Subexpr, indexVar);
                code.AppendLine($"        if ({subVar}.IsError) {{");
                code.AppendLine($"            {errVar} = {TypeNames.ParseError}.Unify({errVar}, {subVar}.Error);");
                code.AppendLine("            break;");
                code.AppendLine("        }");
                code.AppendLine($"        {indexVar} = {subVar}.Ok.Offset;");
                code.AppendLine($"        {listVar}.Add({subVar}.Ok.Value);");
                code.AppendLine($"        {errVar} = {TypeNames.ParseError}.Unify({errVar}, {subVar}.Ok.FurthestError);");
                code.AppendLine("    }");
                code.AppendLine($"    {resultVar} = {TypeNames.ParserBase}.Ok(({parsedType}){listVar}, {indexVar}, {errVar});");
                code.AppendLine("} else {");
                code.AppendLine($"    {resultVar} = {firstVar}.Error;");
                code.AppendLine("}");
                break;
            }

            case BnfAst.Call call:
            {
                // TODO: Check if it even exists
                var calledRule = ruleSet!.GetRule(call.Name);
                var peekVar = AllocateVarName();
                var key = ToPascalCase(call.Name);
                code.AppendLine($"{resultVar} = parse{key}({lastIndex});");
                // HEURISTIC: If the parse didn't advance anything, replace error
                code.AppendLine($"if ({resultVar}.IsError && (!TryPeek({lastIndex}, out var {peekVar}) || ReferenceEquals({peekVar}, {resultVar}.Error.Got))) {{");
                code.AppendLine($"    {resultVar} = {TypeNames.ParserBase}.Error(\"{calledRule.VisualName}\", {resultVar}.Error.Got, \"{rule.VisualName}\");");
                code.AppendLine("}");
                break;
            }

            case BnfAst.Literal lit:
            {
                var resultTok = AllocateVarName();
                if (lit.Value is string)
                {
                    // Match text
                    code.AppendLine($"if (this.TryMatchText({lastIndex}, \"{lit.Value}\", out var {resultTok})) {{");
                    code.AppendLine($"    {resultVar} = {TypeNames.ParserBase}.Ok(({parsedType}){resultTok}, {lastIndex} + 1);");
                    code.AppendLine("} else {");
                    code.AppendLine($"    this.TryPeek({lastIndex}, out var got);");
                    code.AppendLine($"    {resultVar} = {TypeNames.ParserBase}.Error(\"{lit.Value}\", got, \"{rule.VisualName}\");");
                    code.AppendLine("}");
                }
                else
                {
                    // Match token type
                    var tokenType = tokenKinds!.EnumType!.ToDisplayString();
                    var tokVariant = $"{tokenType}.{((IFieldSymbol)lit.Value).Name}";
                    code.AppendLine($"if (this.TryMatchKind({lastIndex}, {tokVariant}, out var {resultTok})) {{");
                    code.AppendLine($"    {resultVar} = {TypeNames.ParserBase}.Ok({resultTok}, {lastIndex} + 1);");
                    code.AppendLine("} else {");
                    code.AppendLine($"    this.TryPeek({lastIndex}, out var got);");
                    code.AppendLine($"    {resultVar} = {TypeNames.ParserBase}.Error({tokVariant}, got, \"{rule.VisualName}\");");
                    code.AppendLine("}");
                }
                break;
            }

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
                    .OrderBy(a => a.ApplicationSyntaxReference!.GetSyntax().GetLocation().SourceSpan.Start)
                    .Select(a => (Left: SymbolEquals(a.AttributeClass, leftAttr), Operators: a.ConstructorArguments.SelectMany(x => x.Values).Select(x => x.Value).ToHashSet()))
                    .ToList();
                // Since there can be multiple get all rule attributes attached to this method
                foreach (var attr in method.GetAttributes().Where(a => SymbolEquals(a.AttributeClass, ruleAttr)))
                {
                    var bnfString = attr.GetCtorValue()!.ToString();
                    var (name, ast) = BnfParser.Parse(bnfString, tokenKinds!);

                    if (precedenceTable.Count > 0)
                    {
                        result.AddPrecedence(name, precedenceTable!, method);
                        precedenceTable.Clear();
                    }

                    if (ast == null) continue;

                    var rule = new Rule(name, new BnfAst.Transform(ast, method)) { VisualName = name };
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

        private static string ToPascalCase(string str)
        {
            var result = new StringBuilder();
            bool prevUnderscore = true;
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] == '_')
                {
                    prevUnderscore = true;
                }
                else
                {
                    result.Append(prevUnderscore ? char.ToUpper(str[i]) : str[i]);
                    prevUnderscore = false;
                }
            }
            return result.ToString();
        }

        private static string GetReturnType(string okType) => $"{TypeNames.ParseResult}<{okType}>";
        private static string FlattenBind(string bind) => bind.Replace("(", string.Empty).Replace(")", string.Empty);
    }
}
