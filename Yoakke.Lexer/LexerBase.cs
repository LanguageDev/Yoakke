using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Collections;
using Yoakke.Text;
using Range = Yoakke.Text.Range;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Base class for implementing a lexer with a simplified API.
    /// </summary>
    /// <typeparam name="T">The token type of the tokens this lexer produces.</typeparam>
    public abstract class LexerBase<T> : ILexer
    {
        public Position Position { get; private set; }

        private TextReader reader;
        private char prevChar;
        private RingBuffer<char> peek;

        /// <summary>
        /// Initializes a new lexer with the given source.
        /// </summary>
        /// <param name="reader">The text reader to read from.</param>
        public LexerBase(TextReader reader)
        {
            this.reader = reader;
            this.peek = new RingBuffer<char>();
        }

        /// <summary>
        /// Initializes a new lexer with the given source.
        /// </summary>
        /// <param name="source">The string to read as source.</param>
        public LexerBase(string source)
            : this(new StringReader(source))
        {
        }

        private static Position NextPosition(Position pos, char lastChar, char currentChar)
        {
            // Windows-style, already advanced line at \r
            if (lastChar == '\r' && currentChar == '\n') return pos;
            if (currentChar == '\r' || currentChar == '\n') return pos.Newline();
            if (char.IsControl(currentChar)) return pos;
            return pos.Advance(1);
        }

        /// <summary>
        /// Peeks ahead into the input.
        /// </summary>
        /// <param name="offset">The amount to peek forward.</param>
        /// <param name="default">The default character to return if the end has been reached.</param>
        /// <returns>The peeked character, or default if the end has been reached.</returns>
        protected char Peek(int offset = 0, char @default = '\0')
        {
            while (this.peek.Count <= offset)
            {
                var next = this.reader.Read();
                if (next == -1) return @default;
                this.peek.AddBack((char)next);
            }
            return this.peek[offset];
        }

        /// <summary>
        /// Skips a character in the input.
        /// </summary>
        /// <returns>The skipped character.</returns>
        protected char Skip()
        {
            var current = this.peek.RemoveFront();
            Position = NextPosition(Position, prevChar, current);
            prevChar = current;
            return current;
        }

        /// <summary>
        /// Skips multiple characters in the input.
        /// </summary>
        /// <param name="length">The amount of characters to skip.</param>
        protected void Skip(int length)
        {
            for (int i = 0; i < length; ++i) Skip();
        }

        /// <summary>
        /// Skips characters in the input and builds a string from the skipped characters.
        /// </summary>
        /// <param name="length">The amount of characters to skip.</param>
        /// <returns>The concatenated characters as a string.</returns>
        protected string Take(int length)
        {
            var result = new StringBuilder();
            for (int i = 0; i < length; ++i) result.Append(Skip());
            return result.ToString();
        }

        /// <summary>
        /// Skips characters in the input and builds a token from the skipped characters.
        /// </summary>
        /// <param name="kind">The token kind to build.</param>
        /// <param name="length">The amount of characters to skip.</param>
        /// <returns>The constructed token.</returns>
        protected Token<T> TakeToken(T kind, int length)
        {
            var start = Position;
            var text = length == 0 ? string.Empty : Take(length);
            var range = new Range(start, Position);
            return new Token<T>(range, text, kind);
        }

        IToken ILexer.Next() => Next();

        /// <summary>
        /// Lexes the next token in the input.
        /// </summary>
        /// <returns>The lexed token.</returns>
        public abstract Token<T> Next();
    }
}
