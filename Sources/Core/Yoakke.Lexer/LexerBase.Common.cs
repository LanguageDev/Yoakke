// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.IO;
using System.Text;
using Yoakke.Text;
using Yoakke.Utilities;

namespace Yoakke.Lexer
{
    /// <summary>
    /// For internal use only, use <see cref="LexerBase"/> or <see cref="LexerBase{TKind}"/> instead.
    /// </summary>
    public abstract class LexerBaseCommon
    {
        /// <summary>
        /// The current <see cref="Position"/> the lexer is at.
        /// </summary>
        public Position Position { get; private set; }

        /// <summary>
        /// True, if the end of the source has been reached.
        /// </summary>
        public bool IsEnd => !this.TryPeek(out var _);

        private readonly TextReader reader;
        private readonly RingBuffer<char> peek;
        private char prevChar;

        /// <summary>
        /// Initializes a new instance of the <see cref="LexerBaseCommon"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> that reads the source text.</param>
        protected LexerBaseCommon(TextReader reader)
        {
            this.reader = reader;
            this.peek = new RingBuffer<char>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexerBaseCommon"/> class.
        /// </summary>
        /// <param name="source">The string to lex.</param>
        protected LexerBaseCommon(string source)
            : this(new StringReader(source))
        {
        }

        /// <summary>
        /// Checks, some upcoming text matches a given string.
        /// </summary>
        /// <param name="text">The string to compare with the upcoming text.</param>
        /// <param name="offset">The offset to start the match at in the input.</param>
        /// <returns>True, if there is a full match.</returns>
        protected bool Matches(string text, int offset = 0)
        {
            // To avoid peeking -1, we pre-check empty string
            if (text.Length == 0) return true;
            // Check if we even have enough characters
            if (!this.TryPeek(out var _, offset + text.Length - 1)) return false;
            // If so, we can do a linear match without the overhead of the peek call
            for (var i = 0; i < text.Length; ++i)
            {
                if (text[i] != this.peek[offset + i]) return false;
            }
            return true;
        }

        /// <summary>
        /// Checks, some upcoming character matches a given one.
        /// </summary>
        /// <param name="ch">The character to compare with the upcoming one.</param>
        /// <param name="offset">The offset to start look for match at in the input.</param>
        /// <returns>True, if they match.</returns>
        protected bool Matches(char ch, int offset = 0) => this.TryPeek(out var inInput, offset) && ch == inInput;

        /// <summary>
        /// Peeks ahead some characters into the input.
        /// </summary>
        /// <param name="result">The peeked character is written here, if the method returns true.</param>
        /// <param name="offset">The amount to peek forward. 0 means next character.</param>
        /// <returns>True, if there was a character to peek (the end has not been reached).</returns>
        protected bool TryPeek(out char result, int offset = 0)
        {
            result = this.Peek(offset);
            return this.peek.Count > offset;
        }

        /// <summary>
        /// Peeks ahead some characters into the input.
        /// </summary>
        /// <param name="offset">The amount to peek forward. 0 means next character.</param>
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
            this.Peek();
            var current = this.peek.RemoveFront();
            this.Position = NextPosition(this.Position, this.prevChar, current);
            this.prevChar = current;
            return current;
        }

        /// <summary>
        /// Skips multiple characters in the input.
        /// </summary>
        /// <param name="length">The amount of characters to skip.</param>
        protected void Skip(int length)
        {
            for (var i = 0; i < length; ++i) this.Skip();
        }

        /// <summary>
        /// Skips characters in the input and builds a string from the skipped characters.
        /// </summary>
        /// <param name="length">The amount of characters to skip.</param>
        /// <returns>The concatenated characters as a string.</returns>
        protected string Take(int length)
        {
            var result = new StringBuilder();
            for (var i = 0; i < length; ++i) result.Append(this.Skip());
            return result.ToString();
        }

        /// <summary>
        /// Skips characters in the input and builds a <see cref="IToken{T}"/> with a given factory function.
        /// </summary>
        /// <typeparam name="TToken">The exact token type to produce.</typeparam>
        /// <param name="length">The amount of characters to skip.</param>
        /// <param name="makeToken">The factory function that receives the source <see cref="Text.Range"/> of the skipped characters
        /// and the skipped characters themselves concatenated as a string, and produces an <see cref="IToken"/> from them.</param>
        /// <returns>The constructed <see cref="IToken{TKind}"/> returned from <paramref name="makeToken"/>.</returns>
        protected TToken TakeToken<TToken>(int length, Func<Text.Range, string, TToken> makeToken)
            where TToken : IToken
        {
            var start = this.Position;
            var text = length == 0 ? string.Empty : this.Take(length);
            var range = new Text.Range(start, this.Position);
            return makeToken(range, text);
        }

        /// <summary>
        /// Skips characters in the input and builds a <see cref="Token{TKind}"/> from the skipped characters.
        /// </summary>
        /// <typeparam name="TKind">The kind-type for the produces <see cref="Token{TKind}"/>.</typeparam>
        /// <param name="kind">The token-kind to build.</param>
        /// <param name="length">The amount of characters to skip.</param>
        /// <returns>The constructed <see cref="Token{TKind}"/>.</returns>
        protected Token<TKind> TakeToken<TKind>(TKind kind, int length)
            where TKind : notnull
        {
            var start = this.Position;
            var text = length == 0 ? string.Empty : this.Take(length);
            var range = new Text.Range(start, this.Position);
            return new Token<TKind>(range, text, kind);
        }

        private static Position NextPosition(Position pos, char lastChar, char currentChar)
        {
            // Windows-style, already advanced line at \r
            if (lastChar == '\r' && currentChar == '\n') return pos;
            if (currentChar == '\r' || currentChar == '\n') return pos.Newline();
            if (char.IsControl(currentChar)) return pos;
            return pos.Advance();
        }
    }
}
