using System;
using System.Collections.Generic;
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
        private ILexer lexer;
        private RingBuffer<IToken> peek;

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
        protected bool TryMatchKind<T>(int offset, T kind, out Token<T> token)
        {
            if (TryPeek(offset, out var itoken) && itoken is Token<T> t && t.Kind.Equals(kind))
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
        protected bool TryMatchText(int offset, string text, out IToken token) =>
            TryPeek(offset, out token) && token.Text == text;

        /// <summary>
        /// Peeks forward in the input.
        /// </summary>
        /// <param name="offset">The amount to peek forward.</param>
        /// <param name="token">The token the given offset ahead, if there were enough tokens.</param>
        /// <returns>True, if there were enough tokens to peek ahead the amount.</returns>
        protected bool TryPeek(int offset, out IToken token)
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
                        token = null;
                        return false;
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
        protected bool TryConsume(out IToken token)
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

        /// <summary>
        /// Utility for constructing a <see cref="ParseResult{T}"/> as a success variant.
        /// </summary>
        /// <typeparam name="T">The parsed value type.</typeparam>
        /// <param name="value">The parsed value.</param>
        /// <param name="offset">The offset in the number of tokens.</param>
        /// <returns>The created parse result.</returns>
        protected static ParseResult<T> MakeSuccess<T>(T value, int offset) =>
            new ParseResult<T>(new ParseSuccess<T>(value, offset));

        /// <summary>
        /// Utility for constructing a <see cref="ParseResult{T}"/> as an error variant.
        /// </summary>
        /// <typeparam name="T">The parsed value type.</typeparam>
        /// <param name="expected">The expected element.</param>
        /// <param name="got">The token encountered instead.</param>
        /// <param name="context">The rule context the error occurred in.</param>
        /// <returns>The created parse result.</returns>
        protected static ParseResult<T> MakeError<T>(object expected, IToken got, string context) =>
            new ParseResult<T>(new ParseError(expected, got, context));
    }
}
