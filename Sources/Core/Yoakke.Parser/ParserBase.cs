// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Yoakke.Collections;
using Yoakke.Lexer;

namespace Yoakke.Parser
{
    /// <summary>
    /// A base-class for parsers.
    /// </summary>
    public abstract class ParserBase
    {
        private readonly ILexer lexer;
        private readonly RingBuffer<IToken> peek;
        private bool pushedEnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserBase"/> class.
        /// </summary>
        /// <param name="lexer">The underlying lexer to use.</param>
        protected ParserBase(ILexer lexer)
        {
            this.lexer = lexer;
            this.peek = new RingBuffer<IToken>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserBase"/> class.
        /// </summary>
        /// <param name="tokens">The tokens to lex.</param>
        protected ParserBase(IEnumerable<IToken> tokens)
            : this(new MemoryLexer(tokens))
        {
        }

        /// <summary>
        /// Checks if a certain token ahead has a certain kind.
        /// </summary>
        /// <typeparam name="T">The token kind type.</typeparam>
        /// <param name="offset">The amount to look ahead.</param>
        /// <param name="kind">The token kind to check for.</param>
        /// <param name="token">The token, if it matched the kind.</param>
        /// <returns>True, if the given token ahead has the certain kind.</returns>
        protected bool TryMatchKind<T>(int offset, T kind, [MaybeNullWhen(false)] out IToken<T>? token)
            where T : notnull
        {
            if (this.TryPeek(offset, out var itoken) && itoken is IToken<T> t && kind!.Equals(t.Kind))
            {
                token = t;
                return true;
            }
            token = null;
            return false;
        }

        /// <summary>
        /// Checks if a certain token ahead has a certain text value.
        /// </summary>
        /// <param name="offset">The amount to look ahead.</param>
        /// <param name="text">The token text to check for.</param>
        /// <param name="token">The token, if it matched the kind.</param>
        /// <returns>True, if the given token ahead has the certain text.</returns>
        protected bool TryMatchText(int offset, string text, [MaybeNullWhen(false)] out IToken? token) =>
            this.TryPeek(offset, out token) && token!.Text == text;

        /// <summary>
        /// Peeks forward in the input.
        /// </summary>
        /// <param name="offset">The amount to peek forward.</param>
        /// <param name="token">The token the given offset ahead, if there were enough tokens.</param>
        /// <returns>True, if there were enough tokens to peek ahead the amount.</returns>
        protected bool TryPeek(int offset, [MaybeNullWhen(false)] out IToken? token)
        {
            while (this.peek.Count <= offset)
            {
                if (this.lexer.IsEnd)
                {
                    if (this.pushedEnd)
                    {
                        token = null;
                        return false;
                    }
                    else
                    {
                        this.pushedEnd = true;
                    }
                }
                this.peek.AddBack(this.lexer.Next());
            }
            token = this.peek[offset];
            return true;
        }

        /// <summary>
        /// Consumes the next token in the input.
        /// </summary>
        /// <param name="token">The consumed token if succeeded.</param>
        /// <returns>True, if the token was successfully consumed.</returns>
        protected bool TryConsume([MaybeNullWhen(false)] out IToken? token)
        {
            if (!this.TryPeek(0, out token)) return false;
            this.peek.RemoveFront();
            return true;
        }

        /// <summary>
        /// Consumes multiple tokens from the input.
        /// </summary>
        /// <param name="length">The amount of tokens to consume.</param>
        /// <returns>True, if the tokens were successfully consumed.</returns>
        protected bool TryConsume(int length)
        {
            if (length == 0) return true;
            if (!this.TryPeek(length - 1, out _)) return false;
            for (var i = 0; i < length; ++i) this.peek.RemoveFront();
            return true;
        }

        /// <summary>
        /// Utility for constructing a <see cref="ParseOk{T}"/>.
        /// </summary>
        /// <typeparam name="T">The parsed value type.</typeparam>
        /// <param name="value">The parsed value.</param>
        /// <param name="offset">The offset in the number of tokens.</param>
        /// <param name="furthestError">The furthest advanced <see cref="ParseError"/>, if any.</param>
        /// <returns>The created <see cref="ParseOk{T}"/>.</returns>
        protected static ParseOk<T> Ok<T>(T value, int offset, ParseError? furthestError = null) =>
            new(value, offset, furthestError);

        /// <summary>
        /// Utility for constructing a <see cref="ParseError"/>.
        /// </summary>
        /// <param name="expected">The expected element.</param>
        /// <param name="got">The token encountered instead.</param>
        /// <param name="context">The rule context the error occurred in.</param>
        /// <returns>The created <see cref="ParseError"/>.</returns>
        protected static ParseError Error(object expected, IToken? got, string context) =>
            new(expected, got, context);

        /// <summary>
        /// Merges alternative <see cref="ParseResult{T}"/>s that happened from the same starting position.
        /// </summary>
        /// <typeparam name="T">The element type of the parse.</typeparam>
        /// <param name="first">The first alternative result.</param>
        /// <param name="second">The second alternative result.</param>
        /// <returns>The chosen/constructed <see cref="ParseResult{T}"/> constructed from the alternatives.</returns>
        protected static ParseResult<T> MergeAlternatives<T>(ParseResult<T> first, ParseResult<T> second)
        {
            if (first.IsOk && second.IsOk)
            {
                if (second.Ok.Offset > first.Ok.Offset) return second;
                // NOTE: Even if they are equal, we return the first
                return first;
            }
            if (first.IsOk) return first;
            if (second.IsOk) return second;
            // Both are errors
            return new ParseResult<T>(ParseError.Unify(first.Error, second.Error)!);
        }

        /// <summary>
        /// Merges a <see cref="ParseError"/> into a <see cref="ParseResult{T}"/>.
        /// </summary>
        /// <typeparam name="T">The element type of the parse.</typeparam>
        /// <param name="result">The <see cref="ParseResult{T}"/> to merge into.</param>
        /// <param name="error">The <see cref="ParseError"/> to merge.</param>
        /// <returns>A new <see cref="ParseResult{T}"/> that has the <see cref="ParseError"/> merged in.</returns>
        protected static ParseResult<T> MergeError<T>(ParseResult<T> result, ParseError error) =>
            result.IsOk ? MergeError(result.Ok, error) : ParseError.Unify(result.Error, error)!;

        /// <summary>
        /// Merges a <see cref="ParseError"/> with a <see cref="ParseOk{T}"/>.
        /// </summary>
        /// <typeparam name="T">The element type of the parse.</typeparam>
        /// <param name="ok">The <see cref="ParseOk{T}"/> to merge.</param>
        /// <param name="error">The <see cref="ParseError"/> to merge.</param>
        /// <returns>A new <see cref="ParseResult{T}"/> that has the <see cref="ParseOk{T}"/>
        /// and the <see cref="ParseError"/> merged.</returns>
        protected static ParseResult<T> MergeError<T>(ParseOk<T> ok, ParseError? error)
        {
            if (error is null) return ok;
            if (ok.FurthestError == null) return Ok(ok.Value, ok.Offset, error);
            return Ok(ok.Value, ok.Offset, ParseError.Unify(ok.FurthestError, error));
        }
    }
}
