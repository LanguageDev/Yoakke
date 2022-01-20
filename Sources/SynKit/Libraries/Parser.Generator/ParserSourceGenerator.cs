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
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using Yoakke.SynKit.Parser.Attributes;
using System.Threading;
using Yoakke.SynKit.Parser.Generator.Model;
using RuleSet = Yoakke.SynKit.Parser.Generator.Model.RuleSet;

namespace Yoakke.SynKit.Parser.Generator;

/// <summary>
/// A source generator that generates a parser from rule annotations over transformer functions.
/// </summary>
[Generator]
public class ParserSourceGenerator : IIncrementalGenerator
{
    private class ParserAttributeModel
    {
        public INamedTypeSymbol? TokenType { get; set; }
    }

    private class RuleAttributeModel
    {
        public string Rule { get; set; } = string.Empty;
    }

    private record class Symbols(
        INamedTypeSymbol ParserAttribute,
        INamedTypeSymbol TokenSourceAttribute,
        INamedTypeSymbol RuleAttribute,
        INamedTypeSymbol LeftAttribute,
        INamedTypeSymbol RightAttribute);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Inject source code
        context.RegisterEmbeddedSourceCodeInjection("InjectedSources.");

        // Incremental pipeline
        var typeDeclarations = context.SyntaxProvider
            .CreateAttributedSyntaxProvider<TypeDeclarationSyntax>(typeof(ParserAttribute));

        // Generate the sources
        context.RegisterTwoPhaseSourceOutput(
            values: typeDeclarations,
            loadExtra: LoadSymbols,
            toGenerationModel: GetGenerationModel,
            toScribanModel: ToScribanModel,
            loadScribanTemplate: _ => Assembly
                .GetExecutingAssembly()
                .ReadEmbeddedScribanTemplate("Templates.parser.sbncs"));
    }

    private static Symbols LoadSymbols(Compilation compilation) => new(
        ParserAttribute: compilation.GetRequiredType(typeof(ParserAttribute)),
        TokenSourceAttribute: compilation.GetRequiredType(typeof(TokenSourceAttribute)),
        RuleAttribute: compilation.GetRequiredType(typeof(RuleAttribute)),
        LeftAttribute: compilation.GetRequiredType(typeof(LeftAttribute)),
        RightAttribute: compilation.GetRequiredType(typeof(RightAttribute))
    );

    private static ParserModel? GetGenerationModel(
        SourceProductionContext context,
        Symbols symbols,
        ISymbol symbol,
        CancellationToken cancellationToken)
    {
        // If it's not even a named type symbol, return
        if (symbol is not INamedTypeSymbol parserSymbol) return null;

        // Check, if the parser class can have external code injected (all elements in the chain are partial)
        if (!parserSymbol.CanDeclareInsideExternally())
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: Diagnostics.NotPartialType,
                location: parserSymbol.Locations.FirstOrDefault(),
                messageArgs: new[] { parserSymbol.Name }));
            return null;
        }

        // Extract the token kinds
        if (!parserSymbol.TryGetAttribute(symbols.ParserAttribute, out ParserAttributeModel? parserAttr)) return null;

        var source = parserSymbol.GetMembers()
            .Where(m => m.HasAttribute(symbols.TokenSourceAttribute))
            .FirstOrDefault();

        var tokenKinds = new TokenKindSet(parserAttr!.TokenType);
        var result = new RuleSet();

        // Go through the methods in declaration order
        foreach (var method in parserSymbol
            .GetMembers()
            .OfType<IMethodSymbol>()
            .OrderBy(sym => sym.Locations.First().SourceSpan.Start))
        {
            // Collect associativity attributes in declaration order
            var precedenceTable = method.GetAttributes()
                .Where(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, symbols.LeftAttribute)
                         || SymbolEqualityComparer.Default.Equals(a.AttributeClass, symbols.RightAttribute))
                .OrderBy(a => a.ApplicationSyntaxReference!.GetSyntax().GetLocation().SourceSpan.Start)
                .Select(a =>
                {
                    var isLeft = SymbolEqualityComparer.Default.Equals(a.AttributeClass, symbols.LeftAttribute);
                    var operators = a.ConstructorArguments.SelectMany(x => x.Values).Select(x => x.Value).ToHashSet();
                    return (Left: isLeft, Operators: operators);
                })
                .ToList();
            // Since there can be multiple get all rule attributes attached to this method
            var ruleAttributes = method.GetAttributes<RuleAttributeModel>(symbols.RuleAttribute);
            foreach (var attr in ruleAttributes)
            {
                var (name, ast) = BnfParser.Parse(attr.Rule, tokenKinds);

                if (precedenceTable.Count > 0)
                {
                    result.AddPrecedence(name, precedenceTable!, method);
                    precedenceTable.Clear();
                }

                if (ast is null) continue;

                var rule = new Rule(name, new BnfAst.Transform(ast, method)) { VisualName = name };
                result.Add(rule);
            }
        }

        // TODO: Do sanity checks

        result.Desugar();
        return new(
            ParserType: parserSymbol,
            SourceField: source,
            RuleSet: result,
            TokenKinds: tokenKinds);
    }

    private static object? ToScribanModel(
        ParserModel parserModel,
        CancellationToken cancellationToken) => new
    {
        LibraryVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
        Namespace = parserModel.ParserType.ContainingNamespace?.ToDisplayString(),
        ContainingTypes = parserModel.ParserType
            .GetContainingTypeChain()
            .Select(c => new
            {
                Kind = c.GetTypeKindName(),
                Name = c.Name,
                GenericArgs = c.TypeArguments.Select(t => t.Name).ToList(),
            }),
        ParserType = new
        {
            Kind = parserModel.ParserType.GetTypeKindName(),
            Name = parserModel.ParserType.Name,
            GenericArgs = parserModel.ParserType.TypeArguments.Select(t => t.Name).ToList(),
        },
        TokenType = parserModel.TokenKinds.EnumType is null
            ? "IToken"
            : $"IToken<{parserModel.TokenKinds.EnumType.ToDisplayString()}>",
        ImplicitConstructor = parserModel.ParserType.HasNoUserDefinedCtors() && parserModel.SourceField is null,
        SourceName = parserModel.SourceField?.Name ?? "TokenStream",
        ParserRules = parserModel.RuleSet.Rules.Select(r => new
        {
            PublicApi = r.Value.PublicApi,
            Name = r.Value.VisualName,
            MethodName = ToPascalCase(r.Key),
            Ast = TranslateAst(parserModel, r.Value.Ast),
        }),
    };

    private static object TranslateAst(ParserModel model, BnfAst ast) => ast switch
    {
        BnfAst.Placeholder p => new
        {
            Type = "Placeholder",
            ParsedType = ast.GetParsedType(model.RuleSet, model.TokenKinds),
        },
        BnfAst.Transform t => new
        {
            Type = "Transform",
            ParsedType = ast.GetParsedType(model.RuleSet, model.TokenKinds),
            Subexpr = TranslateAst(model, t.Subexpr),
            MethodName = t.Method.Name,
        },
        BnfAst.FoldLeft f => new
        {
            Type = "FoldLeft",
            ParsedType = ast.GetParsedType(model.RuleSet, model.TokenKinds),
            First = TranslateAst(model, f.First),
            Second = TranslateAst(model, f.Second),
        },
        BnfAst.Alt a => new
        {
            Type = "Alt",
            ParsedType = ast.GetParsedType(model.RuleSet, model.TokenKinds),
            Elements = a.Elements.Select(e => TranslateAst(model, e)).ToList(),
        },
        BnfAst.Seq s => new
        {
            Type = "Seq",
            ParsedType = ast.GetParsedType(model.RuleSet, model.TokenKinds),
            Elements = s.Elements.Select(e => TranslateAst(model, e)).ToList(),
        },
        BnfAst.Opt o => new
        {
            Type = "Opt",
            ParsedType = ast.GetParsedType(model.RuleSet, model.TokenKinds),
            Subexpr = TranslateAst(model, o.Subexpr),
        },
        BnfAst.Group g => new
        {
            Type = "Group",
            ParsedType = ast.GetParsedType(model.RuleSet, model.TokenKinds),
            Subexpr = TranslateAst(model, g.Subexpr),
        },
        BnfAst.Rep0 r => new
        {
            Type = "Rep0",
            ParsedType = ast.GetParsedType(model.RuleSet, model.TokenKinds),
            Subexpr = TranslateAst(model, r.Subexpr),
        },
        BnfAst.Rep1 r => new
        {
            Type = "Rep1",
            ParsedType = ast.GetParsedType(model.RuleSet, model.TokenKinds),
            Subexpr = TranslateAst(model, r.Subexpr),
        },
        BnfAst.Call c => new
        {
            Type = "Call",
            ParsedType = ast.GetParsedType(model.RuleSet, model.TokenKinds),
            RuleName = c.Name,
            RuleMethodName = ToPascalCase(c.Name),
        },
        BnfAst.Literal lit => new
        {
            Type = lit.Value is string ? "Text" : "Token",
            ParsedType = ast.GetParsedType(model.RuleSet, model.TokenKinds),
            Value = lit.Value,
        },
        _ => throw new ArgumentOutOfRangeException(nameof(ast)),
    };

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

    // TODO: Add back sanity checks!
}
