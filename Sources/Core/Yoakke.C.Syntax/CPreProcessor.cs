// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lexer;
using Yoakke.Utilities;

namespace Yoakke.C.Syntax
{
    /// <summary>
    /// A default <see cref="IPreProcessor"/> that tries to mimic what the standard pre-processors do.
    /// </summary>
    public class CPreProcessor : IPreProcessor
    {
        private enum Condition
        {
            NotYetSatisfied,
            SatisfiedBefore,
            CurrentlySatisfied,
        }

        private readonly ILexer<CToken> lexer;
        private readonly RingBuffer<CToken> outputBuffer;
        private readonly Dictionary<string, IMacro> macros;
        private readonly Stack<Condition> conditionStack;

        /// <summary>
        /// The last consumed <see cref="CToken"/> from the <see cref="inputBuffer"/>.
        /// </summary>
        private CToken? lastInputToken;

        private Condition ConditionState => conditionStack.TryPeek(out var condition)
            ? condition
            : Condition.CurrentlySatisfied;

        /// <summary>
        /// Initializes a new instance of the <see cref="CPreProcessor"/> class.
        /// </summary>
        /// <param name="lexer">The <see cref="ILexer{CToken}"/> to get the <see cref="CToken"/>s from.</param>
        public CPreProcessor(ILexer<CToken> lexer)
        {
            this.lexer = lexer;
            this.outputBuffer = new();
            this.macros = new();
            this.conditionStack = new();
        }

        public bool IsDefined(string name) => this.macros.ContainsKey(name);

        public IMacro? Define(IMacro macro) => this.macros.Remove(macro.Name, out var oldMacro)
            ? oldMacro
            : null;

        public IMacro? Undefine(string name) => this.macros.Remove(name, out var macro)
            ? macro
            : null;

        public CToken Next()
        {
        begin:
            var peek = this.Peek();
            if (peek.Kind == CTokenType.End) return peek;

            if (this.TryParseDirective(out var name))
            {
                this.HandleDirective(name);
                goto begin;
            }

            if (IsIdentifier(peek.Kind))
            {
                // TODO: Might be a macro invocation
            }

            // Just consume
            return this.Skip();
        }

        private void HandleDirective(CToken directive)
        {
            // TODO: Skip until end of line everywhere to ignore stray tokens

            // First we handle conditional directives
            switch (directive.LogicalText)
            {
            case "if":
            case "ifdef":
            case "ifndef":
                // #if, #ifdef, #ifndef pushes onto the stack, but is only signficant, if the condition stack signals that
                // we are currently satisfied, as can only change matters in that case
                // Otherwise we just assume it's already satisfied before to skip tokens
                if (this.ConditionState == Condition.CurrentlySatisfied)
                {
                    // We need to evaluate condition
                    if (directive.LogicalText == "if")
                    {
                        // We need to parse a condition
                        // TODO
                        throw new NotImplementedException();
                    }
                    else
                    {
                        // ifdef or ifndef
                        // We need to parse a macro name
                        if (this.TrySkipInline(directive, IsIdentifier, out var name))
                        {
                            // Evaluate the condition
                            var condition = this.IsDefined(name.LogicalText);
                            if (directive.LogicalText == "ifndef") condition = !condition;
                            // Push to the condition stack
                            this.conditionStack.Push(condition ? Condition.CurrentlySatisfied : Condition.NotYetSatisfied);
                        }
                        else
                        {
                            // It's not an identifier or not on the same line
                            // TODO: Proper error handling
                            throw new NotImplementedException("Identifier expected for ifdef or ifndef");
                        }
                    }
                }
                else
                {
                    // Underlying condition not satisfied, skip this entirely
                    this.conditionStack.Push(Condition.SatisfiedBefore);
                }
                return;

            case "elif":
                // If we were satisfied before, we need to switch to satisfied already
                // If satisfied already, stay that way
                // If not yet satisfied, evaluate
                if (this.conditionStack.TryPop(out var conditionState))
                {
                    if (conditionState == Condition.SatisfiedBefore || conditionState == Condition.CurrentlySatisfied)
                    {
                        this.conditionStack.Push(Condition.SatisfiedBefore);
                    }
                    else
                    {
                        // Not yet satisfied, we need to evaluate the condition
                        // TODO
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    // TODO: Proper error handling
                    throw new NotImplementedException("Stray #elif");
                }
                return;

            case "else":
                // If we were satisfied before, we need to switch to satisfied already
                // If satisfied already, stay that way
                // If not yet satisfied, be currently satisfied
                if (this.conditionStack.TryPop(out conditionState))
                {
                    if (conditionState == Condition.SatisfiedBefore || conditionState == Condition.CurrentlySatisfied)
                    {
                        this.conditionStack.Push(Condition.SatisfiedBefore);
                    }
                    else
                    {
                        // Not yet satisfied, now we are
                        this.conditionStack.Push(Condition.CurrentlySatisfied);
                    }
                }
                else
                {
                    // TODO: Proper error handling
                    throw new NotImplementedException("Stray #else");
                }
                return;

            case "endif":
                // Just pop off the condition stack
                if (!this.conditionStack.TryPop(out var _))
                {
                    // TODO: Proper error handling
                    throw new NotImplementedException("Stray #endif");
                }
                return;
            }

            // If we can't eval right now, do nothing
            if (this.ConditionState != Condition.CurrentlySatisfied) return;

            // Remaining directives
            switch (directive.LogicalText)
            {
            case "define":
                // Parse a macro definition
                var macro = this.ParseMacroDefinition(directive);
                // Define it
                this.Define(macro);
                break;

            case "undef":
                if (this.TrySkipInline(directive, IsIdentifier, out var macroName))
                {
                    // We found an identifier, undefine macro
                    this.Undefine(macroName.LogicalText);
                }
                else
                {
                    // TODO: Proper error handling
                    throw new NotImplementedException("Identifier expected for undef");
                }
                break;

            default:
                // TODO: Proper error handling
                throw new NotImplementedException($"Unknown directive {directive.LogicalText}");
            }
        }

        private IMacro ParseMacroDefinition(CToken defineToken)
        {
            if (!this.TrySkipInline(defineToken, IsIdentifier, out var macroName))
            {
                // TODO: Proper error handling
                throw new NotImplementedException("macro name expected for define");
            }

            List<string>? args = null;
            if (this.TrySkipInline(macroName, CTokenType.OpenParen, out var _))
            {
                // We have an argument list
                args = new();
                // TODO
                throw new NotImplementedException();
            }
            // Now we parse tha macro-body, which goes until the end of line
            var body = new List<CToken>();
            for (; this.TrySkipInline(macroName, out var element); body.Add(element))
            {
                // Pass
            }
            // Construct result
            return new UserMacro(macroName.LogicalText, args, body);
            throw new NotImplementedException();
        }

        private bool TryParseDirective([MaybeNullWhen(false)] out CToken directive)
        {
            var peek = this.Peek();
            var peek2 = this.Peek(1);

            if (peek.Kind == CTokenType.Hash
             && this.lastInputToken?.LogicalRange.End.Line != peek.LogicalRange.Start.Line
             && IsIdentifier(peek2.Kind))
            {
                // It's a hash on a fresh line with an identifier following, we consider it a pre-processor directive
                // First skip hash
                this.Skip();
                // Store identifier
                directive = this.Skip();
                return true;
            }

            directive = null;
            return false;
        }

        private bool TrySkipInline(CToken inlineWith, [MaybeNullWhen(false)] out CToken result) =>
            this.TrySkipInline(inlineWith.LogicalRange.End.Line, out result);

        private bool TrySkipInline(int line, [MaybeNullWhen(false)] out CToken result) =>
            this.TrySkipInline(line, _ => true, out result);

        private bool TrySkipInline(CToken inlineWith, CTokenType type, [MaybeNullWhen(false)] out CToken result) =>
            this.TrySkipInline(inlineWith, t => t.Kind == type, out result);

        private bool TrySkipInline(CToken inlineWith, Predicate<CToken> predicate, [MaybeNullWhen(false)] out CToken result) =>
            this.TrySkipInline(inlineWith.LogicalRange.End.Line, predicate, out result);

        private bool TrySkipInline(int line, Predicate<CToken> predicate, [MaybeNullWhen(false)] out CToken result)
        {
            var peek = this.Peek();
            // NOTE: We don't allow the end token so macros won't accidentally include it
            if (peek.Kind != CTokenType.End && peek.LogicalRange.Start.Line == line && predicate(peek))
            {
                result = this.Skip();
                return true;
            }
            else
            {
                result = null;
                return false;
            }
        }

        private CToken Skip()
        {
            this.Peek();
            this.lastInputToken = this.outputBuffer.RemoveFront();
            return this.lastInputToken;
        }

        private CToken Peek(int offset = 0)
        {
            while (this.outputBuffer.Count <= offset)
            {
                var token = this.lexer.Next();
                if (token.Kind == CTokenType.End) return token;
                this.outputBuffer.AddBack(token);
            }
            return this.outputBuffer[offset];
        }

        private static bool IsIdentifier(CToken token) => IsIdentifier(token.Kind);

        private static bool IsIdentifier(CTokenType tokenType) =>
               tokenType == CTokenType.Identifier
            || ((int)CTokenType.KeywordAuto <= (int)tokenType && (int)tokenType <= (int)CTokenType.KeywordWhile);
    }
}
