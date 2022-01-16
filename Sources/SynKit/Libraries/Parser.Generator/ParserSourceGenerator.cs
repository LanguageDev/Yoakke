// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Yoakke.SynKit.Parser.Generator.Ast;
using Yoakke.SynKit.Parser.Generator.Syntax;
using Yoakke.SourceGenerator.Common;
using Yoakke.SourceGenerator.Common.RoslynExtensions;
using System.IO;
using System.Reflection;
using Scriban;

namespace Yoakke.SynKit.Parser.Generator;

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
    private string sourceField = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParserSourceGenerator"/> class.
    /// </summary>
    public ParserSourceGenerator()
        : base("Yoakke.SynKit.Parser.Generator")
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

        var assembly = Assembly.GetExecutingAssembly();
        var sourcesToInject = assembly
            .GetManifestResourceNames()
            .Where(m => m.StartsWith("InjectedSources."));
        this.InjectSources(sourcesToInject
            .Select(s => (s, new StreamReader(assembly.GetManifestResourceStream(s)).ReadToEnd()))
            .ToList());

        this.RequireLibrary("Yoakke.SynKit.Parser");

        var parserAttr = this.LoadSymbol(TypeNames.ParserAttribute);

        foreach (var syntax in receiver.CandidateTypes)
        {
            var model = this.Compilation!.GetSemanticModel(syntax.SyntaxTree);
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

        var tokenSourceAttr = this.LoadSymbol(TypeNames.TokenSourceAttribute);
        var parserAttr = parserClass.GetAttribute<ParserAttribute>(this.LoadSymbol(TypeNames.ParserAttribute));

        var source = parserClass.GetMembers()
            .Where(m => m.HasAttribute(tokenSourceAttr))
            .FirstOrDefault();
        this.sourceField = source?.Name ?? "TokenStream";

        this.tokenKinds = new TokenKindSet(parserAttr.TokenType);
        // Extract rules from the method annotations
        this.ruleSet = this.ExtractRuleSet(parserClass);
        this.parserType = parserClass;
        if (!this.CheckRuleSet()) return null;

        var assembly = Assembly.GetExecutingAssembly();
        var templateText = new StreamReader(assembly.GetManifestResourceStream("Templates.lexer.sbncs")).ReadToEnd();
        var template = Template.Parse(templateText);

        if (template.HasErrors)
        {
            var errors = string.Join(" | ", template.Messages.Select(x => x.Message));
            throw new InvalidOperationException($"Template parse error: {template.Messages}");
        }

        var model = new
        {
        };

        var result = template.Render(model: model, memberRenamer: member => member.Name);
        result = SyntaxFactory
            .ParseCompilationUnit(result)
            .NormalizeWhitespace()
            .GetText()
            .ToString();

        return result;
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
