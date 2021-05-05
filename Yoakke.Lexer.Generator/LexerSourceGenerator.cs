using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Yoakke.Collections.FiniteAutomata;
using Yoakke.Collections.Intervals;
using Yoakke.Collections.RegEx;

namespace Yoakke.Lexer.Generator
{
    [Generator]
    public class LexerSourceGenerator : ISourceGenerator
    {
        private class SyntaxReceiver : ISyntaxReceiver
        {
            public IList<EnumDeclarationSyntax> CandidateEnums { get; } = new List<EnumDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (syntaxNode is EnumDeclarationSyntax enumDeclSyntax
                    && enumDeclSyntax.AttributeLists.Count > 0)
                {
                    CandidateEnums.Add(enumDeclSyntax);
                }
            }
        }

        private INamedTypeSymbol lexerAttributeSymbol;
        private INamedTypeSymbol endAttributeSymbol;
        private INamedTypeSymbol errorAttributeSymbol;
        private INamedTypeSymbol identAttributeSymbol;
        private INamedTypeSymbol ignoreAttributeSymbol;
        private INamedTypeSymbol regexAttributeSymbol;
        private INamedTypeSymbol tokenAttributeSymbol;

        public void Initialize(GeneratorInitializationContext context) =>
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());

        public void Execute(GeneratorExecutionContext context)
        {
            if (context.SyntaxReceiver is not SyntaxReceiver receiver) return;

            var compilation = context.Compilation;

            // check that the users compilation references the expected library 
            if (!compilation.ReferencedAssemblyNames.Any(ai => ai.Name.Equals("Yoakke.Lexer", StringComparison.OrdinalIgnoreCase)))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.FailedToParseRegularExpression,
                    null,
                    "Yoakke.Lexer"));
            }

            // Load relevant symbols
            lexerAttributeSymbol = compilation.GetTypeByMetadataName(TypeNames.LexerAttribute);
            endAttributeSymbol = compilation.GetTypeByMetadataName(TypeNames.EndAttribute);
            errorAttributeSymbol = compilation.GetTypeByMetadataName(TypeNames.ErrorAttribute);
            identAttributeSymbol = compilation.GetTypeByMetadataName(TypeNames.IdentAttribute);
            ignoreAttributeSymbol = compilation.GetTypeByMetadataName(TypeNames.IgnoreAttribute);
            regexAttributeSymbol = compilation.GetTypeByMetadataName(TypeNames.RegexAttribute);
            tokenAttributeSymbol = compilation.GetTypeByMetadataName(TypeNames.TokenAttribute);

            foreach (var syntax in receiver.CandidateEnums)
            {
                var model = compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                // Filter interfaces without the query group attributes
                if (!HasAttribute(symbol, lexerAttributeSymbol)) continue;
                // Generate code for it
                var generated = GenerateImplementation(context, syntax, symbol);
                context.AddSource($"{symbol.Name}.Generated.cs", generated);
            }
        }

        private string GenerateImplementation(
            GeneratorExecutionContext context,
            EnumDeclarationSyntax syntax,
            INamedTypeSymbol symbol)
        {
            var accessibility = AccessibilityToString(symbol.DeclaredAccessibility);
            var lexerClassName = GetAttributeParam(symbol, lexerAttributeSymbol);
            var namespaceName = symbol.ContainingNamespace.ToDisplayString();
            var enumName = GetFullPath(symbol);
            var tokenName = $"{TypeNames.Token}<{enumName}>";

            if (   symbol.DeclaredAccessibility == Accessibility.Private
                || symbol.DeclaredAccessibility == Accessibility.Protected
                || symbol.DeclaredAccessibility == Accessibility.ProtectedAndInternal)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.IllegalVisibility,
                    symbol.Locations.First(),
                    accessibility));
            }

            var description = ExtractLexerDescription(context, symbol);

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
                    var regex = regexParser.Parse(token.Regex);
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
                    context.ReportDiagnostic(Diagnostic.Create(
                        Diagnostics.FailedToParseRegularExpression,
                        token.Symbol.Locations.First(),
                        ex.Message));
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
                        // +1 because the offset was not incremented yet
                        transitionTable.AppendLine("lastLexerState = currentLexerState;");
                        if (token.Ignore)
                        {
                            // Ignore means clear out the token type
                            transitionTable.AppendLine("lastTokenType = null;");
                        }
                        else
                        {
                            // Save token type
                            transitionTable.AppendLine($"lastTokenType = {enumName}.{token.Symbol.Name};");
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

            return $@"
namespace {namespaceName}
{{
    {accessibility} class {lexerClassName}
    {{
        private struct State {{
            public int index;
            public {TypeNames.Position} position;
            public char? lastChar;
        }}

        private string source;
        private State state;

        public {lexerClassName}(string source)
        {{
            this.source = source;
        }}

        private {TypeNames.Position} NextPosition({TypeNames.Position} pos, char? last, char current)
        {{
            // Windows-style, already advanced at \r
            if (last == '\r' && current == '\n') return pos;
            if (current == '\r' || current == '\n') return pos.Newline();
            if (char.IsControl(current)) return pos;
            return pos.Advance(1);
        }}

        public {tokenName} Next()
        {{
begin:
            if (state.index >= source.Length) return new {tokenName}(
                                                        new {TypeNames.Range}(state.position, 0), 
                                                        string.Empty, 
                                                        {enumName}.{description.EndName});

            var currentState = {dfaStateIdents[dfa.InitalState]};
            var currentLexerState = state;

            State lastLexerState = state;
            {enumName}? lastTokenType = null;

            while (currentLexerState.index < source.Length)
            {{
                var currentChar = source[currentLexerState.index];
                currentLexerState.position = NextPosition(currentLexerState.position, currentLexerState.lastChar, currentChar);
                currentLexerState.lastChar = currentChar;
                currentLexerState.index += 1;
                {transitionTable}
            }}
end_loop:
            if (lastLexerState.index > state.index)
            {{
                if (lastTokenType == null) {{
                    state = lastLexerState;
                    goto begin;
                }}
                var result = new {tokenName}(
                    new {TypeNames.Range}(state.position, lastLexerState.position), 
                    source.Substring(state.index, lastLexerState.index - state.index), 
                    lastTokenType.Value);
                state = lastLexerState;
                return result;
            }}
            else
            {{
                var result = new {tokenName}(
                    new {TypeNames.Range}(state.position, 1), 
                    source.Substring(state.index, 1), 
                    {enumName}.{description.ErrorName});
                state.position = NextPosition(state.position, state.lastChar, source[state.index]);
                state.index += 1;
                return result;
            }}
        }}
    }}
}}
";
        }

        private LexerDescription ExtractLexerDescription(GeneratorExecutionContext context, INamedTypeSymbol symbol)
        {
            var result = new LexerDescription();
            foreach (var member in symbol.GetMembers())
            {
                // End token
                if (HasAttribute(member, endAttributeSymbol))
                { 
                    if (result.EndName == null)
                    {
                        result.EndName = member.Name;
                    }
                    else
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                           Diagnostics.FundamentalTokenTypeAlreadyDefined,
                           member.Locations.First(),
                           result.EndName,
                           "end"));
                    }
                    continue;
                }
                // End
                if (HasAttribute(member, errorAttributeSymbol))
                {
                    if (result.ErrorName == null)
                    {
                        result.ErrorName = member.Name;
                    }
                    else
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                           Diagnostics.FundamentalTokenTypeAlreadyDefined,
                           member.Locations.First(),
                           result.EndName,
                           "error"));
                    }
                    continue;
                }
                // Regular token
                var ignore = HasAttribute(member, ignoreAttributeSymbol);
                // C identifier
                if (HasAttribute(member, identAttributeSymbol))
                {
                    result.Tokens.Add(new TokenDescription
                    {
                        Symbol = (IFieldSymbol)member,
                        Regex = "[A-Za-z_][A-Za-z0-9_]*",
                        Ignore = ignore,
                    });
                    continue;
                }
                // Regex
                if (HasAttribute(member, regexAttributeSymbol))
                {
                    result.Tokens.Add(new TokenDescription
                    {
                        Symbol = (IFieldSymbol)member,
                        Regex = GetAttributeParam(member, regexAttributeSymbol),
                        Ignore = ignore,
                    });
                    continue;
                }
                // Token
                if (HasAttribute(member, tokenAttributeSymbol))
                {
                    result.Tokens.Add(new TokenDescription
                    {
                        Symbol = (IFieldSymbol)member,
                        Regex = RegExParser.Escape(GetAttributeParam(member, tokenAttributeSymbol)),
                        Ignore = ignore,
                    });
                    continue;
                }
                // No attribute, warn
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.NoAttributeForTokenType,
                    member.Locations.First(),
                    member.Name));
            }
            // Check if everything has been filled out
            if (result.EndName == null || result.ErrorName == null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    Diagnostics.NoAttributeForTokenType,
                    symbol.Locations.First(),
                    result.EndName == null ? "end" : "error",
                    result.EndName == null ? "EndAttribute" : "ErrorAttribute"));
            }
            return result;
        }

        private static bool HasAttribute(ISymbol symbol, INamedTypeSymbol searchedAttr) => symbol
            .GetAttributes()
            .Any(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, searchedAttr));

        private static string GetAttributeParam(ISymbol symbol, INamedTypeSymbol searchedAttr) => symbol
            .GetAttributes()
            .Where(attr => SymbolEqualityComparer.Default.Equals(attr.AttributeClass, searchedAttr))
            .Select(attr => attr.ConstructorArguments.First().Value.ToString())
            .First();

        private static string AccessibilityToString(Accessibility accessibility) =>
            accessibility.ToString().ToLowerInvariant();

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

        private static string GetFullPath(ISymbol symbol)
        {
            var prefix = symbol.ContainingNamespace.ToDisplayString();
            var result = symbol.Name;
            while (symbol.ContainingType != null)
            {
                symbol = symbol.ContainingType;
                result = $"{symbol.Name}.{result}";
            }
            if (prefix.Length > 0) return $"{prefix}.{result}";
            return result;
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
