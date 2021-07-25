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
using Yoakke.SourceGenerator.Common;
using Yoakke.SourceGenerator.Common.RoslynExtensions;
using Yoakke.Utilities.FiniteAutomata;
using Yoakke.Utilities.Intervals;
using Yoakke.Utilities.RegEx;

namespace Yoakke.Lexer.Generator
{
    /// <summary>
    /// Source generator for lexers.
    /// Generates a DFA-driven lexer from annotated token types.
    /// </summary>
    [Generator]
    public class LexerSourceGenerator : GeneratorBase
    {
        private class SyntaxReceiver : ISyntaxReceiver
        {
            public IList<EnumDeclarationSyntax> CandidateEnums { get; } = new List<EnumDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is EnumDeclarationSyntax enumDeclSyntax
                    && enumDeclSyntax.AttributeLists.Count > 0)
                {
                    this.CandidateEnums.Add(enumDeclSyntax);
                }
            }
        }

        private class LexerAttribute
        {
            public string ClassName { get; set; } = string.Empty;
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
            : base("Yoakke.Lexer.Generator")
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

            this.RequireLibrary("Yoakke.Lexer");

            var lexerAttribute = this.LoadSymbol(TypeNames.LexerAttribute);

            foreach (var syntax in receiver.CandidateEnums)
            {
                var model = this.Context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                if (symbol is null) continue;
                // Filter enums without the lexer attributes
                if (!symbol.HasAttribute(lexerAttribute)) continue;
                // Generate code for it
                var generated = this.GenerateImplementation(symbol!);
                if (generated == null) continue;
                this.AddSource($"{symbol!.ToDisplayString()}.Generated.cs", generated);
            }
        }

        private string? GenerateImplementation(INamedTypeSymbol symbol)
        {
            var lexerAttribute = this.LoadSymbol(TypeNames.LexerAttribute);
            var lexerAttributeParams = symbol.GetAttribute(lexerAttribute).ParseInto<LexerAttribute>();

            var accessibility = symbol.DeclaredAccessibility.ToString().ToLowerInvariant();
            var lexerClassName = lexerAttributeParams.ClassName;
            var namespaceName = symbol.ContainingNamespace.ToDisplayString();
            var enumName = symbol.ToDisplayString();
            var tokenName = $"{TypeNames.Token}<{enumName}>";

            // Extract the lexer from the attributes
            var description = this.ExtractLexerDescription(symbol);
            if (description is null) return null;

            // Build the DFA and state -> token associations from the description
            var dfaResult = this.BuildDfa(description);
            if (dfaResult is null) return null;
            var (dfa, dfaStateToToken) = dfaResult.Value;

            // Allocate unique numbers for each DFA state as we have the hierarchical numbers from determinization
            var dfaStateIdents = new Dictionary<State, int>();
            foreach (var state in dfa.States) dfaStateIdents.Add(state, dfaStateIdents.Count);

            // For each state we need to build the transition table
            var transitionTable = new StringBuilder();
            transitionTable.AppendLine("switch (currentState) {");
            foreach (var state in dfa.States)
            {
                transitionTable.AppendLine($"case {dfaStateIdents[state]}:");
                if (!dfa.TryGetTransitionsFrom(state, out var transitions))
                {
                    // There's no way to continue from here
                    transitionTable.AppendLine("goto end_loop;");
                    continue;
                }
                // For the current state we need transitions to a new state based on the current character
                transitionTable.AppendLine("switch (currentChar) {");
                foreach (var kv in transitions)
                {
                    var interval = kv.Key;
                    var destState = kv.Value;
                    var (lower, upper) = ToInclusive(interval);

                    // In the library it is rational to have an interval like ('a'; 'b'), but in practice
                    // this means that this transition might as well not exist, as these are discrete values,
                    // there can't be anything in between
                    if (lower != null && upper != null && lower.Value > upper.Value) continue;

                    var matchCondition = (lower, upper) switch
                    {
                        (char l, char h) => $"case char ch when '{Escape(l)}' <= currentChar && currentChar <= '{Escape(h)}':",
                        (char l, null) => $"case char ch when '{Escape(l)}' <= currentChar:",
                        (null, char h) => $"case char ch when currentChar <= '{Escape(h)}':",
                        (null, null) => "case char ch:",
                    };
                    transitionTable.AppendLine(matchCondition);
                    transitionTable.AppendLine($"currentState = {dfaStateIdents[destState]};");
                    if (dfaStateToToken.TryGetValue(destState, out var token))
                    {
                        // The destination is an accepting state, save it
                        transitionTable.AppendLine("lastOffset = currentOffset;");
                        if (token.Ignore)
                        {
                            // Ignore means clear out the token type
                            transitionTable.AppendLine("lastTokenType = null;");
                        }
                        else
                        {
                            // Save token type
                            transitionTable.AppendLine($"lastTokenType = {enumName}.{token.Symbol!.Name};");
                        }
                    }
                    transitionTable.AppendLine("break;");
                }
                // Add a default arm to break out of the loop on non-matching character
                transitionTable.AppendLine("default: goto end_loop;");
                transitionTable.AppendLine("}");
                transitionTable.AppendLine("break;");
            }
            // Add a default arm to panic on illegal state
            transitionTable.AppendLine($"default: throw new {TypeNames.InvalidOperationException}();");
            transitionTable.AppendLine("}");

            // TODO: Consuming a single token on error might not be the best strategy
            // Also we might want to report if there was a token type that was being matched, while the error occurred
            return $@"
namespace {namespaceName}
{{
    {accessibility} class {lexerClassName} : {TypeNames.LexerBase}<{tokenName}>
    {{
        public {lexerClassName}({TypeNames.TextReader} reader)
            : base(reader)
        {{
        }}

        public {lexerClassName}(string text)
            : base(text)
        {{
        }}

        public override {tokenName} Next()
        {{
begin:
            if (this.Peek() == '\0') 
            {{
                return this.TakeToken({enumName}.{description.EndSymbol!.Name}, 0);
            }}

            var currentState = {dfaStateIdents[dfa.InitalState]};
            var currentOffset = 0;

            {enumName}? lastTokenType = null;
            var lastOffset = 0;

            while (true)
            {{
                var currentChar = this.Peek(currentOffset);
                if (currentChar == '\0') break;
                ++currentOffset;
                {transitionTable}
            }}
end_loop:
            if (lastOffset > 0)
            {{
                if (lastTokenType == null) 
                {{
                    this.Skip(lastOffset);
                    goto begin;
                }}
                return this.TakeToken(lastTokenType.Value, lastOffset);
            }}
            else
            {{
                return this.TakeToken({enumName}.{description.ErrorSymbol!.Name}, 1);
            }}
        }}
    }}
}}
";
        }

        private (DenseDfa<char> Dfa, Dictionary<State, TokenDescription> StateToToken)? BuildDfa(LexerDescription description)
        {
            // Store which token corresponds to which end state
            var tokenToNfaState = new Dictionary<TokenDescription, State>();
            // Construct the NFA from the regexes
            var nfa = new DenseNfa<char>();
            nfa.InitalState = nfa.NewState();
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
                    var (start, end) = regex.ThompsonConstruct(nfa);
                    // Wire the initial state to the start of the construct
                    nfa.AddTransition(nfa.InitalState, Epsilon.Default, start);
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
            // Now we have to figure out which new accepting states correspond to which token
            var dfaStateToToken = new Dictionary<State, TokenDescription>();
            // We go in the order of each token because this ensures the precedence in which order the tokens were declared
            foreach (var token in description.Tokens)
            {
                var nfaAccepting = tokenToNfaState[token];
                var dfaAccepting = dfa.AcceptingStates.Where(dfaState => nfaAccepting.IsSubstateOf(dfaState));
                foreach (var dfaState in dfaAccepting)
                {
                    // This check ensures the unambiuous accepting states
                    if (!dfaStateToToken.ContainsKey(dfaState)) dfaStateToToken.Add(dfaState, token);
                }
            }

            return (dfa, dfaStateToToken);
        }

        private LexerDescription? ExtractLexerDescription(INamedTypeSymbol symbol)
        {
            var regexAttr = this.LoadSymbol(TypeNames.RegexAttribute);
            var tokenAttr = this.LoadSymbol(TypeNames.TokenAttribute);
            var endAttr = this.LoadSymbol(TypeNames.EndAttribute);
            var errorAttr = this.LoadSymbol(TypeNames.ErrorAttribute);
            var ignoreAttr = this.LoadSymbol(TypeNames.IgnoreAttribute);

            var result = new LexerDescription();
            foreach (var member in symbol.GetMembers().OfType<IFieldSymbol>())
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
                var relevantAttribs = member.GetAttributes()
                    .Where(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, regexAttr)
                                || SymbolEqualityComparer.Default.Equals(attr.AttributeClass, tokenAttr))
                    .ToList();
                foreach (var attr in relevantAttribs)
                {
                    if (SymbolEqualityComparer.Default.Equals(attr.AttributeClass, regexAttr))
                    {
                        // Regex
                        var regex = attr.ParseInto<RegexAttribute>().Regex;
                        result.Tokens.Add(new TokenDescription(member, regex, ignore));
                    }
                    else
                    {
                        // Token
                        var regex = RegExParser.Escape(attr.ParseInto<TokenAttribute>().Text);
                        result.Tokens.Add(new TokenDescription(member, regex, ignore));
                    }
                }
                if (relevantAttribs.Count == 0)
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
                    symbol.Locations.First(),
                    result.EndSymbol is null ? "end" : "error",
                    result.EndSymbol is null ? "EndAttribute" : "ErrorAttribute");
                return null;
            }
            return result;
        }

        private static (char? Lower, char? Upper) ToInclusive(Interval<char> interval)
        {
            char? lower = interval.Lower.Type switch
            {
                BoundType.Inclusive => interval.Lower.Value,
                BoundType.Exclusive => (char)(interval.Lower.Value + 1),
                BoundType.Unbounded => null,
                _ => throw new InvalidOperationException(),
            };
            char? upper = interval.Upper.Type switch
            {
                BoundType.Inclusive => interval.Upper.Value,
                BoundType.Exclusive => (char)(interval.Upper.Value - 1),
                BoundType.Unbounded => null,
                _ => throw new InvalidOperationException(),
            };
            return (lower, upper);
        }

        private static string Escape(char ch) => ch switch
        {
            '\'' => @"\'",
            '\n' => @"\n",
            '\r' => @"\r",
            '\t' => @"\t",
            '\0' => @"\0",
            '\\' => @"\\",
            _ => ch.ToString(),
        };
    }
}
