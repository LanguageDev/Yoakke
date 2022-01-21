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
using Microsoft.CodeAnalysis.Text;
using System.Threading;
using System.Collections.Immutable;
using Yoakke.SynKit.Lexer.Attributes;
using Scriban.Runtime;

namespace Yoakke.SynKit.Lexer.Generator;

/// <summary>
/// Source generator for lexers.
/// Generates a DFA-driven lexer from annotated token types.
/// </summary>
[Generator]
public class LexerSourceGenerator : IIncrementalGenerator
{
    private class LexerAttributeModel
    {
        public INamedTypeSymbol? TokenType { get; set; }
    }

    private class RegexAttributeModel
    {
        public string Regex { get; set; } = string.Empty;
    }

    private class TokenAttributeModel
    {
        public string Text { get; set; } = string.Empty;
    }

    private record class Symbols(
        INamedTypeSymbol LexerAttribute,
        INamedTypeSymbol CharSourceAttribute,
        INamedTypeSymbol RegexAttribute,
        INamedTypeSymbol TokenAttribute,
        INamedTypeSymbol EndAttribute,
        INamedTypeSymbol ErrorAttribute,
        INamedTypeSymbol IgnoreAttribute);

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Inject source code
        context.RegisterEmbeddedSourceCodeInjection("InjectedSources.");
        
        // Incremental pipeline
        var typeDeclarations = context.SyntaxProvider
            .CreateAttributedSyntaxProvider<TypeDeclarationSyntax>(typeof(LexerAttribute));

        // Generate the sources
        context.RegisterTwoPhaseSourceOutput(
            values: typeDeclarations,
            loadExtra: LoadSymbols,
            toGenerationModel: GetGenerationModel,
            toScribanModel: ToScribanModel,
            loadScribanTemplate: _ => Assembly
                .GetExecutingAssembly()
                .ReadEmbeddedScribanTemplate("Templates.lexer.sbncs"));
    }

    private static Symbols LoadSymbols(Compilation compilation) => new(
        LexerAttribute: compilation.GetRequiredType(typeof(LexerAttribute)),
        CharSourceAttribute: compilation.GetRequiredType(typeof(CharSourceAttribute)),
        RegexAttribute: compilation.GetRequiredType(typeof(RegexAttribute)),
        TokenAttribute: compilation.GetRequiredType(typeof(TokenAttribute)),
        EndAttribute: compilation.GetRequiredType(typeof(EndAttribute)),
        ErrorAttribute: compilation.GetRequiredType(typeof(ErrorAttribute)),
        IgnoreAttribute: compilation.GetRequiredType(typeof(IgnoreAttribute))
    );

    private static LexerModel? GetGenerationModel(
        SourceProductionContext context,
        Symbols symbols,
        ISymbol symbol,
        CancellationToken cancellationToken)
    {
        // If it's not even a named type symbol, return
        if (symbol is not INamedTypeSymbol lexerSymbol) return null;

        // Check, if the lexer class can have external code injected (all elements in the chain are partial)
        if (!lexerSymbol.CanDeclareInsideExternally())
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: Diagnostics.NotPartialType,
                location: lexerSymbol.Locations.FirstOrDefault(),
                messageArgs: new[] { lexerSymbol.Name }));
            return null;
        }

        // We extract all information from the lexer attribute
        if (!lexerSymbol.TryGetAttribute(symbols.LexerAttribute, out LexerAttributeModel? lexerAttribute)) return null;

        // From this we get to know the token type, which we inspect for the fields
        // Check, if it's even an enumeration type
        var tokenType = lexerAttribute!.TokenType;
        if (tokenType is null) return null;
        if (tokenType.TypeKind != TypeKind.Enum)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                descriptor: Diagnostics.NotAnEnumType,
                location: tokenType.Locations.FirstOrDefault(),
                messageArgs: new[] { tokenType.Name, lexerSymbol.Name }));
            return null;
        }

        // The source field is easy to extract from the lexer class
        // It is optional, so a null is acceptable here
        var sourceField = lexerSymbol
            .GetMembers()
            .FirstOrDefault(f => f.HasAttribute(symbols.CharSourceAttribute));

        // Time to collect out all the other lexer members
        IFieldSymbol? endVariant = null;
        IFieldSymbol? errorVariant = null;
        var tokens = new List<TokenModel>();
        var regexParser = new RegExParser();

        // Go through all enum members
        // Assign each in the proper category
        foreach (var member in tokenType.GetMembers().OfType<IFieldSymbol>())
        {
            var hasAttribute = false;

            if (member.HasAttribute(symbols.EndAttribute))
            {
                // End token
                hasAttribute = true;
                if (endVariant is not null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        descriptor: Diagnostics.TokenTypeAlreadyDefined,
                        location: member.Locations.FirstOrDefault(),
                        messageArgs: new[] { endVariant.Name, "end", tokenType.Name }));
                    return null;
                }
                endVariant = member;
            }

            if (member.HasAttribute(symbols.ErrorAttribute))
            {
                // Error token
                hasAttribute = true;
                if (errorVariant is not null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        descriptor: Diagnostics.TokenTypeAlreadyDefined,
                        location: member.Locations.FirstOrDefault(),
                        messageArgs: new[] { errorVariant.Name, "error", tokenType.Name }));
                    return null;
                }
                errorVariant = member;
            }

            // Determine if should be ignored
            var ignore = member.HasAttribute(symbols.IgnoreAttribute);

            // Add all token attributes
            var regexAttribs = member.GetAttributes<RegexAttributeModel>(symbols.RegexAttribute);
            var tokenAttribs = member.GetAttributes<TokenAttributeModel>(symbols.TokenAttribute);
            try
            {
                foreach (var attr in regexAttribs)
                {
                    hasAttribute = true;
                    var regex = regexParser.Parse(attr.Regex);
                    tokens.Add(new(member, regex, ignore));
                }
                foreach (var attr in tokenAttribs)
                {
                    hasAttribute = true;
                    var regex = regexParser.Parse(RegExParser.Escape(attr.Text));
                    tokens.Add(new(member, regex, ignore));
                }
            }
            catch (Exception ex)
            {
                // TODO: The API is a bit bad here, we shouldn't catch a geneic exception, we should be able
                // to ask the parsed result what the error was
                context.ReportDiagnostic(Diagnostic.Create(
                    descriptor: Diagnostics.RegexParseError,
                    location: member.Locations.FirstOrDefault(),
                    messageArgs: new[] { ex.Message }));
                return null;
            }

            // Warn, if the enum variant is not meaningful
            if (!hasAttribute)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    descriptor: Diagnostics.NoAttributeForTokenType,
                    location: member.Locations.FirstOrDefault(),
                    messageArgs: new[] { member.Name, tokenType.Name }));
            }
        }

        // Check, if either of the required attributes are not defined
        if (endVariant is null || errorVariant is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                    descriptor: Diagnostics.TokenTypeNotDefined,
                    location: tokenType.Locations.FirstOrDefault(),
                    messageArgs: new[] { endVariant is null ? "end" : "error", tokenType.Name }));
            return null;
        }

        // We have succeeded
        return new(
            LexerType: lexerSymbol,
            TokenType: tokenType,
            SourceField: sourceField,
            ErrorVariant: errorVariant,
            EndVariant: endVariant,
            Tokens: tokens);
    }

    private static object? ToScribanModel(
        LexerModel lexerModel,
        CancellationToken cancellationToken)
    {
        // Constructing the DFA
        // This RNG assigns a random number for each state
        // This is to avoid hash-attacking ourselves
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

        // Helper for intervals
        static (char? Lower, char? Upper) ToInclusive(Interval<char> interval)
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

        // Check if should terminate
        cancellationToken.ThrowIfCancellationRequested();

        // Store which token corresponds to which end state
        var tokenToNfaState = new Dictionary<TokenModel, int>();
        // Construct the NFA from the regexes
        var nfa = new DenseNfa<int, char>();
        var initialState = MakeState();
        nfa.InitialStates.Add(initialState);
        var regexParser = new RegExParser();
        foreach (var token in lexerModel.Tokens)
        {
            // Desugar the regex
            var regex = token.Regex.Desugar();
            // Construct it into the NFA
            var (start, end) = regex.ThompsonsConstruct(nfa, MakeState);
            // Wire the initial state to the start of the construct
            nfa.AddEpsilonTransition(initialState, start);
            // Mark the state as accepting
            nfa.AcceptingStates.Add(end);
            // Save the final state as a state that accepts this token
            tokenToNfaState.Add(token, end);
        }

        // Check if should terminate
        cancellationToken.ThrowIfCancellationRequested();

        // TODO: Determinization should accept a cancellation token, it's a potentially long process
        // Determinize it
        var dfa = nfa.Determinize();

        // Check if should terminate
        cancellationToken.ThrowIfCancellationRequested();

        // TODO: Minimization should accept a cancellation token, it's a potentially long process
        // Minimize it
        var minDfa = dfa.Minimize(StateCombiner<int>.DefaultSetCombiner, dfa.AcceptingStates);

        // Check if should terminate
        cancellationToken.ThrowIfCancellationRequested();

        // Now we have to figure out which new accepting states correspond to which token
        var dfaStateToToken = new Dictionary<StateSet<int>, TokenModel>();
        // We go in the order of each token because this ensures the precedence in which order the tokens were declared
        foreach (var token in lexerModel.Tokens)
        {
            var nfaAccepting = tokenToNfaState[token];
            var dfaAccepting = minDfa.AcceptingStates.Where(dfaState => dfaState.Contains(nfaAccepting));
            foreach (var dfaState in dfaAccepting)
            {
                // This check ensures the unambiguous accepting states
                if (!dfaStateToToken.ContainsKey(dfaState)) dfaStateToToken.Add(dfaState, token);
            }
        }

        // Allocate unique numbers for each DFA state as we have the hierarchical numbers from determinization
        var dfaStateIdents = new Dictionary<StateSet<int>, int>();
        foreach (var state in dfa.States) dfaStateIdents.Add(state, dfaStateIdents.Count);

        // Check if should terminate
        cancellationToken.ThrowIfCancellationRequested();

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

        // Check if should terminate
        cancellationToken.ThrowIfCancellationRequested();

        return new
        {
            LibraryVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(),
            TypeEnclosure = lexerModel.LexerType.GetTypeEnclosure(),
            LexerType = new
            {
                Kind = lexerModel.LexerType.GetTypeKindName(),
                Name = lexerModel.LexerType.Name,
                GenericArgs = lexerModel.LexerType.TypeArguments.Select(t => t.Name).ToList(),
            },
            TokenType = lexerModel.TokenType.ToDisplayString(),
            ImplicitConstructor = lexerModel.LexerType.HasNoUserDefinedCtors() && lexerModel.SourceField is null,
            SourceName = lexerModel.SourceField?.Name ?? "CharStream",
            EndTokenName = lexerModel.EndVariant.Name,
            ErrorTokenName = lexerModel.ErrorVariant.Name,
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
    }
}
