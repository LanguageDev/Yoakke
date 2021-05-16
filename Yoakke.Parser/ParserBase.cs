using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections;
using Yoakke.Lexer;

namespace Yoakke.Parser
{
    /// <summary>
    /// A base-class for parsers.
    /// </summary>
    public abstract class ParserBase
    {
        private ILexer? lexer;
        private RingBuffer<IToken> peek;
        private bool pushedEnd;

        /// <summary>
        /// Initializes a new <see cref="ParserBase"/>.
        /// </summary>
        /// <param name="lexer">The underlying lexer to use.</param>
        public ParserBase(ILexer lexer)
        {
            this.lexer = lexer;
            this.peek = new RingBuffer<IToken>();
        }

        /// <summary>
        /// Initializes a new <see cref="ParserBase"/>.
        /// </summary>
        /// <param name="tokens">The tokens to lex.</param>
        public ParserBase(IEnumerable<IToken> tokens)
        {
            peek = new RingBuffer<IToken>();
            foreach (var token in tokens) peek.AddBack(token);
        }

        /// <summary>
        /// Checks if a certain token ahead has a certain kind.
        /// </summary>
        /// <typeparam name="T">The token kind type.</typeparam>
        /// <param name="offset">The amount to look ahead.</param>
        /// <param name="kind">The token kind to check for.</param>
        /// <param name="token">The token, if it matched the kind.</param>
        /// <returns>True, if the given token ahead has the certain kind.</returns>
        protected bool TryMatchKind<T>(int offset, T kind, [MaybeNullWhen(false)] out Token<T>? token) where T : notnull
        {
            if (TryPeek(offset, out var itoken) && itoken is Token<T> t && kind!.Equals(t.Kind))
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
            TryPeek(offset, out token) && token!.Text == text;

        /// <summary>
        /// Peeks forward in the input.
        /// </summary>
        /// <param name="offset">The amount to peek forward.</param>
        /// <param name="token">The token the given offset ahead, if there were enough tokens.</param>
        /// <returns>True, if there were enough tokens to peek ahead the amount.</returns>
        protected bool TryPeek(int offset, [MaybeNullWhen(false)] out IToken? token)
        {
            if (lexer == null)
            {
                if (peek.Count <= offset)
                {
                    token = null;
                    return false;
                }
                token = peek[offset];
                return true;
            }
            else
            {
                while (peek.Count <= offset)
                {
                    if (lexer.IsEnd)
                    {
                        if (pushedEnd)
                        {
                            token = null;
                            return false;
                        }
                        else
                        {
                            pushedEnd = true;
                        }
                    }
                    peek.AddBack(lexer.Next());
                }
                token = peek[offset];
                return true;
            }
        }

        /// <summary>
        /// Consumes the next token in the input.
        /// </summary>
        /// <param name="token">The consumed token if succeeded.</param>
        /// <returns>True, if the token was successfully consumed.</returns>
        protected bool TryConsume([MaybeNullWhen(false)] out IToken? token)
        {
            if (!TryPeek(0, out token)) return false;
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
            if (!TryPeek(length - 1, out var _)) return false;
            for (int i = 0; i < length; ++i) this.peek.RemoveFront();
            return true;
        }

        // TODO: Doc
        protected static ParseResult<T> MakeOk<T>(ParseOk<T> ok) => new ParseResult<T>(ok);

        /// <summary>
        /// Utility for constructing a <see cref="ParseResult{T}"/> as a success variant.
        /// </summary>
        /// <typeparam name="T">The parsed value type.</typeparam>
        /// <param name="value">The parsed value.</param>
        /// <param name="offset">The offset in the number of tokens.</param>
        /// <returns>The created parse result.</returns>
        protected static ParseResult<T> MakeOk<T>(T value, int offset, ParseError? furthestError = null) =>
            MakeOk(new ParseOk<T>(value, offset, furthestError));

        /// <summary>
        /// Utility for casting a parse error to a <see cref="ParseResult{T}"/>.
        /// </summary>
        /// <typeparam name="T">The parsed value type.</typeparam>
        /// <param name="error">The error to wrap.</param>
        /// <returns>The created parse result.</returns>
        protected static ParseResult<T> MakeError<T>(ParseError error) => new ParseResult<T>(error);

        /// <summary>
        /// Utility for constructing a <see cref="ParseResult{T}"/> as an error variant.
        /// </summary>
        /// <typeparam name="T">The parsed value type.</typeparam>
        /// <param name="expected">The expected element.</param>
        /// <param name="got">The token encountered instead.</param>
        /// <param name="context">The rule context the error occurred in.</param>
        /// <returns>The created parse result.</returns>
        protected static ParseResult<T> MakeError<T>(object expected, IToken got, string context) =>
            MakeError<T>(new ParseError(expected, got, context));

        // TODO: Doc
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
            return new ParseResult<T>(ParseError.Unify(first.Error, second.Error));
        }

        // TODO: Doc
        protected static ParseResult<T> MergeError<T>(ParseResult<T> result, ParseError error) =>
            result.IsOk ? MergeError(result.Ok, error) : MergeError<T>(result.Error, error);

        // TODO: Doc
        protected static ParseResult<T> MergeError<T>(ParseOk<T> ok, ParseError error)
        {
            if (error == null) return MakeOk(ok);
            if (ok.FurthestError == null) return MakeOk(ok.Value, ok.Offset, error);
            return MakeOk(ok.Value, ok.Offset, ParseError.Unify(ok.FurthestError, error));
        }

        // TODO: Doc
        protected static ParseResult<T> MergeError<T>(ParseError err1, ParseError err2) =>
            MakeError<T>(ParseError.Unify(err1, err2));
    }
}
