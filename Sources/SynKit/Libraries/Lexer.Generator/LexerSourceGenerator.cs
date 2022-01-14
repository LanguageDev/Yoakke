// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Collections.Generic.Polyfill;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Yoakke.SynKit.Automata;
using Yoakke.SynKit.Automata.Dense;
using Yoakke.Collections.Intervals;
using Yoakke.SourceGenerator.Common;
using Yoakke.SourceGenerator.Common.RoslynExtensions;
using Yoakke.SynKit.Lexer.Generator.Model;
using System.IO;
using System.Reflection;
using Scriban;
using Microsoft.CodeAnalysis.CSharp;

namespace Yoakke.SynKit.Lexer.Generator;

/// <summary>
/// Source generator for lexers.
/// Generates a DFA-driven lexer from annotated token types.
/// </summary>
[Generator]
public class LexerSourceGenerator : GeneratorBase
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

    private class LexerAttribute
    {
        public INamedTypeSymbol? TokenType { get; set; }
    }

    private class RegexAttribute
    {
        public string Regex { get; set; } = string.Empty;
    }

    private class TokenAttribute
    {
        public string Text { get; set; } = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LexerSourceGenerator"/> class.
    /// </summary>
    public LexerSourceGenerator()
        : base("Yoakke.SynKit.Lexer.Generator")
    {
    }

    /// <inheritdoc/>
    protected override ISyntaxReceiver CreateSyntaxReceiver(GeneratorInitializationContext context) => new SyntaxReceiver();

    /// <inheritdoc/>
    protected override bool IsOwnSyntaxReceiver(ISyntaxReceiver syntaxReceiver) => syntaxReceiver is SyntaxReceiver;

    /// <inheritdoc/>
    protected override void GenerateCode(ISyntaxReceiver syntaxReceiver)
    {
        // Debugger.Launch();

        var assembly = Assembly.GetExecutingAssembly();
        var sourcesToInject = assembly
            .GetManifestResourceNames()
            .Where(m => m.StartsWith("InjectedSources."));
        this.InjectSources(sourcesToInject
            .Select(s => (s, new StreamReader(assembly.GetManifestResourceStream(s)).ReadToEnd()))
            .ToList());

        var receiver = (SyntaxReceiver)syntaxReceiver;

        this.RequireLibrary("Yoakke.SynKit.Lexer");

        var lexerAttribute = this.LoadSymbol(TypeNames.LexerAttribute);

        foreach (var syntax in receiver.CandidateTypes)
        {
            var model = this.Compilation!.GetSemanticModel(syntax.SyntaxTree);
            var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
            if (symbol is null) continue;
            // Filter classes without the lexer attributes
            if (!symbol.TryGetAttribute(lexerAttribute, out LexerAttribute? attr)) continue;
            // Generate code for it
            var generated = this.GenerateImplementation(symbol!, attr!.TokenType!);
            if (generated is null) continue;
            this.AddSource($"{symbol!.ToDisplayString()}.Generated.cs", generated);
        }
    }

    private string? GenerateImplementation(INamedTypeSymbol lexerClass, INamedTypeSymbol tokenKind)
    {
        if (!this.RequireDeclarableInside(lexerClass)) return null;

        // Extract the lexer from the attributes
        var description = this.ExtractLexerDescription(lexerClass, tokenKind);
        if (description is null) return null;

        // Build the DFA and state -> token associations from the description
        var dfaResult = this.BuildDfa(description);
        if (dfaResult is null) return null;
        var (dfa, dfaStateToToken) = dfaResult.Value;

        // Allocate unique numbers for each DFA state as we have the hierarchical numbers from determinization
        var dfaStateIdents = new Dictionary<StateSet<int>, int>();
        foreach (var state in dfa.States) dfaStateIdents.Add(state, dfaStateIdents.Count);

        // Group the transitions by source and destination states
        var transitionsByState = dfa.Transitions
            .GroupBy(t => t.Source)
            .ToDictionary(
                group => group.Key,
                group => group
                    .GroupBy(t => t.Destination)
                    .ToDictionary(
                        group => group.Key,
                        group => group.Select(t => t.Symbol).ToList()));

        // TODO: Consuming a single character on error might not be the best strategy
        // Also we might want to report if there was a token type that was being matched, while the error occurred

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
            LibraryVersion = assembly.GetName().Version.ToString(),
            Namespace = lexerClass.ContainingNamespace?.ToDisplayString(),
            ContainingTypes = lexerClass
                .GetContainingTypeChain()
                .Select(c => new
                {
                    Kind = c.GetTypeKindName(),
                    Name = c.Name,
                    GenericArgs = c.TypeArguments.Select(t => t.Name).ToList(),
                }),
            LexerType = new
            {
                Kind = lexerClass.GetTypeKindName(),
                Name = lexerClass.Name,
                GenericArgs = lexerClass.TypeArguments.Select(t => t.Name).ToList(),
            },
            TokenType = tokenKind.ToDisplayString(),
            ImplicitConstructor = lexerClass.HasNoUserDefinedCtors() && description.SourceSymbol is null,
            SourceName = description.SourceSymbol?.Name ?? "CharStream",
            EndTokenName = description.EndSymbol!.Name,
            ErrorTokenName = description.ErrorSymbol!.Name,
            InitialState = new
            {
                Id = dfaStateIdents[dfa.InitialState],
            },
            States = transitionsByState
                .Select(kv => new
                {
                    Id = dfaStateIdents[kv.Key],
                    Destinations = kv.Value
                        .Select(kv2 => new
                        {
                            Id = dfaStateIdents[kv2.Key],
                            Token = dfaStateToToken.TryGetValue(kv2.Key, out var t) ? t.Symbol.Name : null,
                            IsAccepting = t is not null,
                            Ignore = t?.Ignore ?? false,
                            Intervals = kv2.Value
                                .Select(ToInclusive)
                                .Where(iv => iv.Lower is null || iv.Upper is null || iv.Lower <= iv.Upper)
                                .Select(iv => new
                                {
                                    Start = iv.Lower,
                                    End = iv.Upper,
                                })
                                .ToList(),
                        })
                        .Where(d => d.Intervals.Count > 0)
                        .ToList(),
                }).ToList(),
        };

        var result = template.Render(model: model, memberRenamer: member => member.Name);
        result = SyntaxFactory
            .ParseCompilationUnit(result)
            .NormalizeWhitespace()
            .GetText()
            .ToString();

        return result;
    }

    private (IDenseDfa<StateSet<int>, char> Dfa, Dictionary<StateSet<int>, TokenModel> StateToToken)?
        BuildDfa(LexerModel description)
    {
        var rnd = new Random();
        var occupiedStates = new HashSet<int>();
        int MakeState()
        {
            while (true)
            {
                var s = rnd.Next();
                if (occupiedStates.Add(s)) return s;
            }
        }

        // Store which token corresponds to which end state
        var tokenToNfaState = new Dictionary<TokenModel, int>();
        // Construct the NFA from the regexes
        var nfa = new DenseNfa<int, char>();
        var initialState = MakeState();
        nfa.InitialStates.Add(initialState);
        var regexParser = new RegExParser();
        foreach (var token in description.Tokens)
        {
            try
            {
                // Parse the regex
                var regex = regexParser.Parse(token.Regex!);
                // Desugar it
                regex = regex.Desugar();
                // Construct it into the NFA
                var (start, end) = regex.ThompsonsConstruct(nfa, MakeState);
                // Wire the initial state to the start of the construct
                nfa.AddEpsilonTransition(initialState, start);
                // Mark the state as accepting
                nfa.AcceptingStates.Add(end);
                // Save the final state as a state that accepts this token
                tokenToNfaState.Add(token, end);
            }
            catch (Exception ex)
            {
                this.Report(Diagnostics.FailedToParseRegularExpression, token.Symbol!.Locations.First(), ex.Message);
                return null;
            }
        }

        // Determinize it
        var dfa = nfa.Determinize();
        var minDfa = dfa.Minimize(StateCombiner<int>.DefaultSetCombiner, dfa.AcceptingStates);
        // Now we have to figure out which new accepting states correspond to which token
        var dfaStateToToken = new Dictionary<StateSet<int>, TokenModel>();
        // We go in the order of each token because this ensures the precedence in which order the tokens were declared
        foreach (var token in description.Tokens)
        {
            var nfaAccepting = tokenToNfaState[token];
            var dfaAccepting = minDfa.AcceptingStates.Where(dfaState => dfaState.Contains(nfaAccepting));
            foreach (var dfaState in dfaAccepting)
            {
                // This check ensures the unambiguous accepting states
                if (!dfaStateToToken.ContainsKey(dfaState)) dfaStateToToken.Add(dfaState, token);
            }
        }

        return (minDfa, dfaStateToToken);
    }

    private LexerModel? ExtractLexerDescription(INamedTypeSymbol lexerClass, INamedTypeSymbol tokenKind)
    {
        var sourceAttr = this.LoadSymbol(TypeNames.CharSourceAttribute);
        var regexAttr = this.LoadSymbol(TypeNames.RegexAttribute);
        var tokenAttr = this.LoadSymbol(TypeNames.TokenAttribute);
        var endAttr = this.LoadSymbol(TypeNames.EndAttribute);
        var errorAttr = this.LoadSymbol(TypeNames.ErrorAttribute);
        var ignoreAttr = this.LoadSymbol(TypeNames.IgnoreAttribute);

        var result = new LexerModel();

        // Search for the source field in the lexer class
        var sourceField = lexerClass.GetMembers()
            .Where(field => field.HasAttribute(sourceAttr))
            .FirstOrDefault();
        result.SourceSymbol = sourceField;

        // Deal with the enum members
        foreach (var member in tokenKind.GetMembers().OfType<IFieldSymbol>())
        {
            // End token
            if (member.HasAttribute(endAttr))
            {
                if (result.EndSymbol is null)
                {
                    result.EndSymbol = member;
                }
                else
                {
                    this.Report(Diagnostics.FundamentalTokenTypeAlreadyDefined, member.Locations.First(), result.EndSymbol.Name, "end");
                    return null;
                }
                continue;
            }
            // Error token
            if (member.HasAttribute(errorAttr))
            {
                if (result.ErrorSymbol is null)
                {
                    result.ErrorSymbol = member;
                }
                else
                {
                    this.Report(Diagnostics.FundamentalTokenTypeAlreadyDefined, member.Locations.First(), result.ErrorSymbol.Name, "error");
                    return null;
                }
                continue;
            }
            // Regular token
            var ignore = member.HasAttribute(ignoreAttr);
            // Ask for all regex and token attributes
            var regexAttribs = member.GetAttributes<RegexAttribute>(regexAttr);
            var tokenAttribs = member.GetAttributes<TokenAttribute>(tokenAttr);
            foreach (var attr in regexAttribs) result.Tokens.Add(new(member, attr.Regex, ignore));
            foreach (var attr in tokenAttribs) result.Tokens.Add(new(member, RegExParser.Escape(attr.Text), ignore));

            if (regexAttribs.Count == 0 && tokenAttribs.Count == 0)
            {
                // No attribute, warn
                this.Report(Diagnostics.NoAttributeForTokenType, member.Locations.First(), member.Name);
            }
        }
        // Check if everything has been filled out
        if (result.EndSymbol is null || result.ErrorSymbol is null)
        {
            this.Report(
                Diagnostics.FundamentalTokenTypeNotDefined,
                tokenKind.Locations.First(),
                result.EndSymbol is null ? "end" : "error",
                result.EndSymbol is null ? "EndAttribute" : "ErrorAttribute");
            return null;
        }

        return result;
    }

    private static (char? Lower, char? Upper) ToInclusive(Interval<char> interval)
    {
        char? lower = interval.Lower switch
        {
            LowerBound<char>.Inclusive i => i.Value,
            LowerBound<char>.Exclusive e => (char)(e.Value + 1),
            LowerBound<char>.Unbounded => null,
            _ => throw new ArgumentOutOfRangeException(),
        };
        char? upper = interval.Upper switch
        {
            UpperBound<char>.Inclusive i => i.Value,
            UpperBound<char>.Exclusive e => (char)(e.Value - 1),
            UpperBound<char>.Unbounded => null,
            _ => throw new ArgumentOutOfRangeException(),
        };
        return (lower, upper);
    }
}
