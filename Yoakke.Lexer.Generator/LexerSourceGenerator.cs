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
using Yoakke.SourceGenerator.Common;

namespace Yoakke.Lexer.Generator
{
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
                    CandidateEnums.Add(enumDeclSyntax);
                }
            }
        }

        public LexerSourceGenerator()
            : base("Yoakke.Lexer.Generator") { }

        protected override ISyntaxReceiver CreateSyntaxReceiver(GeneratorInitializationContext context) => new SyntaxReceiver();
        protected override bool IsOwnSyntaxReceiver(ISyntaxReceiver syntaxReceiver) => syntaxReceiver is SyntaxReceiver;

        protected override void GenerateCode(ISyntaxReceiver syntaxReceiver)
        {
            //Debugger.Launch();

            var receiver = (SyntaxReceiver)syntaxReceiver;

            RequireLibrary("Yoakke.Lexer");

            foreach (var syntax in receiver.CandidateEnums)
            {
                var model = Context.Compilation.GetSemanticModel(syntax.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(syntax) as INamedTypeSymbol;
                // Filter enums without the lexer attributes
                if (!HasAttribute(symbol, TypeNames.LexerAttribute)) continue;
                // Generate code for it
                var generated = GenerateImplementation(syntax, symbol);
                if (generated == null) continue;
                AddSource($"{symbol.Name}.Generated.cs", generated);
            }
        }

        private string GenerateImplementation(EnumDeclarationSyntax syntax, INamedTypeSymbol symbol)
        {
            var accessibility = symbol.DeclaredAccessibility.ToString().ToLowerInvariant();
            var lexerClassName = GetAttribute(symbol, TypeNames.LexerAttribute).GetCtorValue().ToString();
            var namespaceName = symbol.ContainingNamespace.ToDisplayString();
            var enumName = symbol.ToDisplayString();
            var tokenName = $"{TypeNames.Token}<{enumName}>";

            var description = ExtractLexerDescription(symbol);
            if (description == null) return null;

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
                    Report(Diagnostics.FailedToParseRegularExpression, token.Symbol.Locations.First(), ex.Message);
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
                        transitionTable.AppendLine("lastLexerState = currentLexerState;");
                        transitionTable.AppendLine("lastOffset = currentOffset;");
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
        private struct State 
        {{
            public {TypeNames.Position} position;
            public char? lastChar;
        }}

        private {TypeNames.TextReader} reader;
        private {TypeNames.RingBuffer}<char> peek;
        private State state;

        public {lexerClassName}({TypeNames.TextReader} reader)
        {{
            this.reader = reader;
            this.peek = new {TypeNames.RingBuffer}<char>();
        }}

        public {lexerClassName}(string text)
            : this(new {TypeNames.StringReader}(text))
        {{
        }}

        private static {TypeNames.Position} NextPosition({TypeNames.Position} pos, char? last, char current)
        {{
            // Windows-style, already advanced at \r
            if (last == '\r' && current == '\n') return pos;
            if (current == '\r' || current == '\n') return pos.Newline();
            if (char.IsControl(current)) return pos;
            return pos.Advance(1);
        }}

        private char? Peek(int offset = 0)
        {{
            while (this.peek.Count <= offset)
            {{
                var next = this.reader.Read();
                if (next == -1) return null;
                this.peek.AddBack((char)next);
            }}
            return this.peek[offset];
        }}

        private void SkipPeek(int length)
        {{
            for (int i = 0; i < length; ++i) this.peek.RemoveFront();
        }}

        private string PeekToString(int length)
        {{
            var result = string.Empty;
            for (int i = 0; i < length; ++i) result += this.peek.RemoveFront();
            return result;
        }}

        public {tokenName} Next()
        {{
begin:
            if (this.Peek() == null) 
            {{
                return new {tokenName}(new {TypeNames.Range}(state.position, 0), string.Empty, {enumName}.{description.EndName});
            }}

            var currentState = {dfaStateIdents[dfa.InitalState]};
            var currentLexerState = state;
            var currentOffset = 0;

            State lastLexerState = state;
            {enumName}? lastTokenType = null;
            var lastOffset = 0;

            while (true)
            {{
                var peek = this.Peek(currentOffset);
                if (peek == null) break;
                var currentChar = peek.Value;
                currentLexerState.position = NextPosition(currentLexerState.position, currentLexerState.lastChar, currentChar);
                currentLexerState.lastChar = currentChar;
                ++currentOffset;
                {transitionTable}
            }}
end_loop:
            if (lastOffset > 0)
            {{
                if (lastTokenType == null) {{
                    state = lastLexerState;
                    this.SkipPeek(lastOffset);
                    goto begin;
                }}
                var result = new {tokenName}(
                    new {TypeNames.Range}(state.position, lastLexerState.position), 
                    this.PeekToString(lastOffset), 
                    lastTokenType.Value);
                state = lastLexerState;
                return result;
            }}
            else
            {{
                var faultyChar = this.peek.RemoveFront();
                var result = new {tokenName}(
                    new {TypeNames.Range}(state.position, 1), 
                    faultyChar.ToString(), 
                    {enumName}.{description.ErrorName});
                state.position = NextPosition(state.position, state.lastChar, faultyChar);
                return result;
            }}
        }}
    }}
}}
";
        }

        private LexerDescription ExtractLexerDescription(INamedTypeSymbol symbol)
        {
            var result = new LexerDescription();
            foreach (var member in symbol.GetMembers().OfType<IFieldSymbol>())
            {
                // End token
                if (HasAttribute(member, TypeNames.EndAttribute))
                { 
                    if (result.EndName == null)
                    {
                        result.EndName = member.Name;
                    }
                    else
                    {
                        Report(Diagnostics.FundamentalTokenTypeAlreadyDefined, member.Locations.First(), result.EndName, "end");
                        return null;
                    }
                    continue;
                }
                // End
                if (HasAttribute(member, TypeNames.ErrorAttribute))
                {
                    if (result.ErrorName == null)
                    {
                        result.ErrorName = member.Name;
                    }
                    else
                    {
                        Report(Diagnostics.FundamentalTokenTypeAlreadyDefined, member.Locations.First(), result.EndName, "error");
                        return null;
                    }
                    continue;
                }
                // Regular token
                var ignore = HasAttribute(member, TypeNames.IgnoreAttribute);
                // C identifier
                if (HasAttribute(member, TypeNames.IdentAttribute))
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
                if (TryGetAttribute(member, TypeNames.RegexAttribute, out var attr))
                {
                    result.Tokens.Add(new TokenDescription
                    {
                        Symbol = (IFieldSymbol)member,
                        Regex = attr.GetCtorValue().ToString(),
                        Ignore = ignore,
                    });
                    continue;
                }
                // Token
                if (TryGetAttribute(member, TypeNames.TokenAttribute, out attr))
                {
                    result.Tokens.Add(new TokenDescription
                    {
                        Symbol = (IFieldSymbol)member,
                        Regex = RegExParser.Escape(attr.GetCtorValue().ToString()),
                        Ignore = ignore,
                    });
                    continue;
                }
                // No attribute, warn
                Report(Diagnostics.NoAttributeForTokenType, member.Locations.First(), member.Name);
            }
            // Check if everything has been filled out
            if (result.EndName == null || result.ErrorName == null)
            {
                Report(
                    Diagnostics.FundamentalTokenTypeNotDefined,
                    symbol.Locations.First(),
                    result.EndName == null ? "end" : "error",
                    result.EndName == null ? "EndAttribute" : "ErrorAttribute");
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
