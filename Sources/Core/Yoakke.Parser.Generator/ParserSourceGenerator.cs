// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Yoakke.Parser.Generator.Ast;
using Yoakke.Parser.Generator.Syntax;
using Yoakke.SourceGenerator.Common;
using Yoakke.SourceGenerator.Common.RoslynExtensions;
using Yoakke.Utilities.Compatibility;

namespace Yoakke.Parser.Generator
{
    /// <summary>
    /// A source generator that generates a parser from rule annotations over transformer functions.
    /// </summary>
    [Generator]
    public class ParserSourceGenerator : GeneratorBase
    {
        private class SyntaxReceiver : ISyntaxReceiver
        {
            public IList<TypeDeclarationSyntax> CandidateTypes { get; } = new List<TypeDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is TypeDeclarationSyntax typeDeclSyntax && typeDeclSyntax.AttributeLists.Count > 0)
                {
                    this.CandidateTypes.Add(typeDeclSyntax);
                }
            }
        }

        private class ParserAttribute
        {
            public INamedTypeSymbol? TokenType { get; set; }
        }

        private class RuleAttribute
        {
            public string Rule { get; set; } = string.Empty;
        }

        private RuleSet? ruleSet;
        private int varIndex;
        private TokenKindSet? tokenKinds;
        private INamedTypeSymbol? parserType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserSourceGenerator"/> class.
        /// </summary>
        public ParserSourceGenerator()
            : base("Yoakke.Parser.Generator")
        {
        }

        /// <inheritdoc/>
        protected override ISyntaxReceiver CreateSyntaxReceiver(GeneratorInitializationContext context) => new SyntaxReceiver();

        /// <inheritdoc/>
        protected override bool IsOwnSyntaxReceiver(ISyntaxReceiver syntaxReceiver) => syntaxReceiver is SyntaxReceiver;

        /// <inheritdoc/>
        protected override void GenerateCode(ISyntaxReceiver syntaxReceiver)
        {
            var receiver = (SyntaxReceiver)syntaxReceiver;

            this.RequireLibrary("Yoakke.Parser");

            var parserAttr = this.LoadSymbol(TypeNames.ParserAttribute);

            foreach (var syntax in receiver.CandidateTypes)
            {
                var model = this.Context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                if (symbol is null) continue;
                // Filter classes without the parser attributes
                if (!symbol.HasAttribute(parserAttr)) continue;
                // Generate code for it
                var generated = this.GenerateImplementation(symbol);
                if (generated == null) continue;
                this.AddSource($"{symbol!.ToDisplayString()}.Generated.cs", generated);
            }
        }

        private string? GenerateImplementation(INamedTypeSymbol parserClass)
        {
            if (!this.RequireDeclarableInside(parserClass)) return null;

            var parserAttr = parserClass.GetAttribute<ParserAttribute>(this.LoadSymbol(TypeNames.ParserAttribute));
            this.tokenKinds = new TokenKindSet(parserAttr.TokenType);
            // Extract rules from the method annotations
            this.ruleSet = this.ExtractRuleSet(parserClass);
            this.parserType = parserClass;
            if (!this.CheckRuleSet()) return null;

            var className = parserClass.Name;

            var parserMethods = new StringBuilder();
            foreach (var rule in this.ruleSet.Rules)
            {
                var key = ToPascalCase(rule.Key);

                // TODO: Check if the return types are all compatible
                var parsedType = rule.Value.Ast.GetParsedType(this.ruleSet, this.tokenKinds);
                var returnType = GetReturnType(parsedType);

                if (rule.Value.PublicApi)
                {
                    // Part of public API

                    // Implement a try... pattern method
                    parserMethods.AppendLine($"public bool TryParse{key}([{TypeNames.MaybeNullWhen}(false)] out {parsedType} value) {{");
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
                parserMethods.AppendLine(this.GenerateRuleParser(rule.Value));
                parserMethods.AppendLine("}");
            }

            // Deduce what ctors to generate
            var ctors = string.Empty;
            if (parserClass.HasNoUserDefinedCtors())
            {
                ctors = $@"
public {className}({TypeNames.ILexer} lexer) : base(lexer) {{ }}
public {className}({TypeNames.IEnumerable}<{TypeNames.IToken}> tokens) : base(tokens) {{ }}
";
            }

            var (prefix, suffix) = parserClass.ContainingSymbol.DeclareInsideExternally();
            var (genericTypes, genericConstraints) = parserClass.GetGenericCrud();
            return $@"
{prefix} 
partial {parserClass.GetTypeKindName()} {className}{genericTypes} : {TypeNames.ParserBase} {genericConstraints}
{{
    {ctors}

#nullable enable
#pragma warning disable CS8600
#pragma warning disable CS8604
#pragma warning disable CS8619
#pragma warning disable CS8632
        {parserMethods}
#pragma warning restore CS8632
#pragma warning restore CS8619
#pragma warning restore CS8604
#pragma warning restore CS8600
#nullable restore
}}
{suffix}
";
        }

        /* Sanity-checks */

        private bool CheckRuleSet() => this.ruleSet!.Rules.Values.All(this.CheckRule);

        private bool CheckRule(Rule rule) => this.CheckBnfAst(rule.Ast);

        // For now we only check if all references are valid (referencing existing rules) or not
        private bool CheckBnfAst(BnfAst ast) => ast switch
        {
            BnfAst.Alt alt => alt.Elements.All(this.CheckBnfAst),
            BnfAst.Seq seq => seq.Elements.All(this.CheckBnfAst),
            BnfAst.FoldLeft foldl => this.CheckBnfAst(foldl.First) && this.CheckBnfAst(foldl.Second),
            BnfAst.Opt opt => this.CheckBnfAst(opt.Subexpr),
            BnfAst.Group grp => this.CheckBnfAst(grp.Subexpr),
            BnfAst.Rep0 rep0 => this.CheckBnfAst(rep0.Subexpr),
            BnfAst.Rep1 rep1 => this.CheckBnfAst(rep1.Subexpr),
            BnfAst.Transform tr => this.CheckBnfAst(tr.Subexpr),
            BnfAst.Call call => this.CheckReferenceValidity(call.Name),
            BnfAst.Literal or BnfAst.Placeholder => true,
            _ => throw new InvalidOperationException(),
        };

        private bool CheckReferenceValidity(string referenceName)
        {
            // If the rule-set has such method, we are in the clear
            if (this.ruleSet!.Rules.ContainsKey(referenceName)) return true;
            // If there is such a terminal, also OK
            if (this.tokenKinds!.Fields.ContainsKey(referenceName)) return true;
            // As a last-effort, check for a "parse<ReferenceName>" in the type definition
            if (this.parserType!.GetMembers($"parse{referenceName}").Length > 0) return true;
            // It is an unknown reference, report it
            this.Report(Diagnostics.UnknownRuleIdentifier, referenceName);
            return false;
        }

        /* Code-generation */

        private string GenerateRuleParser(Rule rule)
        {
            var code = new StringBuilder();
            // By default we are at "index"
            var resultVar = this.GenerateBnf(code, rule, rule.Ast, "offset");
            code.AppendLine($"return {resultVar};");
            return code.ToString();
        }

        private string GenerateBnf(StringBuilder code, Rule rule, BnfAst node, string lastIndex)
        {
            var parsedType = node.GetParsedType(this.ruleSet!, this.tokenKinds!);
            var resultType = GetReturnType(parsedType);
            var resultVar = this.AllocateVarName();
            code.AppendLine($"{resultType} {resultVar};");

            switch (node)
            {
            case BnfAst.Placeholder:
            {
                code.AppendLine($"{resultVar} = placeholder;");
                break;
            }

            case BnfAst.Transform transform:
            {
                var subVar = this.GenerateBnf(code, rule, transform.Subexpr, lastIndex);
                var binder = this.GetTopLevelPattern(transform.Subexpr);
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
                var firstVar = this.GenerateBnf(code, rule, fold.First, lastIndex);
                code.AppendLine($"if ({firstVar}.IsOk) {{");
                code.AppendLine($"    var placeholder = {firstVar};");
                code.AppendLine($"    {resultVar} = {firstVar}.Ok;");
                code.AppendLine("    while (true) {");

                var secondVar = this.GenerateBnf(code, rule, fold.Second, lastIndex);
                code.AppendLine($"if ({secondVar}.IsOk) {{");
                code.AppendLine($"    placeholder = {secondVar};");
                code.AppendLine($"    {resultVar} = {secondVar}.Ok;");
                code.AppendLine("} else {");
                code.AppendLine("    break;");
                code.AppendLine("}");

                code.AppendLine("    }");
                code.AppendLine("} else {");
                code.AppendLine($"    {resultVar} = {firstVar}.Error;");
                code.AppendLine("}");
                break;
            }

            case BnfAst.Alt alt:
            {
                var first = true;
                foreach (var element in alt.Elements)
                {
                    var altVar = this.GenerateBnf(code, rule, element, lastIndex);
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
                var prevVar = this.GenerateBnf(code, rule, seq.Elements[0], lastIndex);
                varStack.Push(prevVar);
                var resultSeq = new StringBuilder($"{prevVar}.Ok.Value");
                for (var i = 1; i < seq.Elements.Count; ++i)
                {
                    code.AppendLine($"if ({prevVar}.IsOk) {{");
                    var nextVar = this.GenerateBnf(code, rule, seq.Elements[i], $"{prevVar}.Ok.Offset");
                    // Update it with the error
                    code.AppendLine($"{nextVar} = MergeError({nextVar}, {prevVar}.Ok.FurthestError);");
                    prevVar = nextVar;
                    varStack.Push(prevVar);
                    resultSeq.Append($", {prevVar}.Ok.Value");
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
                var subVar = this.GenerateBnf(code, rule, opt.Subexpr, lastIndex);
                code.AppendLine($"if ({subVar}.IsOk) {resultVar} = {TypeNames.ParserBase}.Ok<{parsedType}>({subVar}.Ok.Value, {subVar}.Ok.Offset, {subVar}.Ok.FurthestError);");
                code.AppendLine($"else {resultVar} = {TypeNames.ParserBase}.Ok(default({parsedType}), {lastIndex}, {subVar}.Error);");
                break;
            }

            case BnfAst.Group grp:
            {
                var subVar = this.GenerateBnf(code, rule, grp.Subexpr, lastIndex);
                code.AppendLine($"{resultVar} = {subVar};");
                break;
            }

            case BnfAst.Rep0 r0:
            {
                var elementType = r0.Subexpr.GetParsedType(this.ruleSet!, this.tokenKinds!);
                var listVar = this.AllocateVarName();
                var indexVar = this.AllocateVarName();
                var errVar = this.AllocateVarName();
                code.AppendLine($"var {listVar} = new {TypeNames.List}<{elementType}>();");
                code.AppendLine($"var {indexVar} = {lastIndex};");
                code.AppendLine($"{TypeNames.ParseError} {errVar} = null;");
                code.AppendLine("while (true) {");
                var subVar = this.GenerateBnf(code, rule, r0.Subexpr, indexVar);
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
                var elementType = r1.Subexpr.GetParsedType(this.ruleSet!, this.tokenKinds!);
                var listVar = this.AllocateVarName();
                var indexVar = this.AllocateVarName();
                var errVar = this.AllocateVarName();
                var firstVar = this.GenerateBnf(code, rule, r1.Subexpr, lastIndex);
                code.AppendLine($"if ({firstVar}.IsOk) {{");
                code.AppendLine($"    var {listVar} = new {TypeNames.List}<{elementType}>();");
                code.AppendLine($"    {listVar}.Add({firstVar}.Ok.Value);");
                code.AppendLine($"    var {indexVar} = {firstVar}.Ok.Offset;");
                code.AppendLine($"    {TypeNames.ParseError} {errVar} = null;");
                code.AppendLine("    while (true) {");
                var subVar = this.GenerateBnf(code, rule, r1.Subexpr, indexVar);
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
                var calledRule = this.ruleSet!.GetRule(call.Name);
                var peekVar = this.AllocateVarName();
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
                var resultTok = this.AllocateVarName();
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
                    var tokenType = this.tokenKinds!.EnumType!.ToDisplayString();
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
            var ruleAttr = this.LoadSymbol(TypeNames.RuleAttribute);
            var leftAttr = this.LoadSymbol(TypeNames.LeftAttribute);
            var rightAttr = this.LoadSymbol(TypeNames.RightAttribute);

            var result = new RuleSet();

            // Go through the methods in declaration order
            foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>().OrderBy(sym => sym.Locations.First().SourceSpan.Start))
            {
                // Collect associativity attributes in declaration order
                var precedenceTable = method.GetAttributes()
                    .Where(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, leftAttr)
                             || SymbolEqualityComparer.Default.Equals(a.AttributeClass, rightAttr))
                    .OrderBy(a => a.ApplicationSyntaxReference!.GetSyntax().GetLocation().SourceSpan.Start)
                    .Select(a =>
                    {
                        var isLeft = SymbolEqualityComparer.Default.Equals(a.AttributeClass, leftAttr);
                        var operators = a.ConstructorArguments.SelectMany(x => x.Values).Select(x => x.Value).ToHashSet();
                        return (Left: isLeft, Operators: operators);
                    })
                    .ToList();
                // Since there can be multiple get all rule attributes attached to this method
                var ruleAttributes = method.GetAttributes<RuleAttribute>(ruleAttr);
                foreach (var attr in ruleAttributes)
                {
                    var (name, ast) = BnfParser.Parse(attr.Rule, this.tokenKinds!);

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
            BnfAst.Alt alt => $"{this.GetTopLevelPattern(alt.Elements[0])}",
            BnfAst.Seq seq => $"({string.Join(", ", seq.Elements.Select(this.GetTopLevelPattern))})",
               BnfAst.Transform
            or BnfAst.Call
            or BnfAst.Opt
            or BnfAst.Group
            or BnfAst.Rep0
            or BnfAst.Rep1
            or BnfAst.Literal
            or BnfAst.Placeholder => this.AllocateVarName(),
            _ => throw new InvalidOperationException(),
        };

        private string AllocateVarName() => $"a{this.varIndex++}";

        private static string ToPascalCase(string str)
        {
            var result = new StringBuilder();
            var prevUnderscore = true;
            for (var i = 0; i < str.Length; ++i)
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
