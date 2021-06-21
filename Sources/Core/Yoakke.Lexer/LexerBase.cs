// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.IO;
using System.Text;
using Yoakke.Collections;
using Yoakke.Text;
using Range = Yoakke.Text.Range;

namespace Yoakke.Lexer
{
    /// <summary>
    /// Base-class to provide common functionality for lexers, if a custom solution is needed.
    /// </summary>
    /// <typeparam name="TKind">The token-kind of the <see cref="IToken{TKind}"/>s this <see cref="LexerBase{TKind}"/> produces.</typeparam>
    public abstract class LexerBase<TKind> : ILexer
        where TKind : notnull
    {
        public Position Position { get; private set; }

        public bool IsEnd => !this.TryPeek(out var _);

        private readonly TextReader reader;
        private readonly RingBuffer<char> peek;
        private char prevChar;

        /// <summary>
        /// Initializes a new instance of the <see cref="LexerBase{T}"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="TextReader"/> that reads the source text.</param>
        protected LexerBase(TextReader reader)
        {
            this.reader = reader;
            this.peek = new RingBuffer<char>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexerBase{T}"/> class.
        /// </summary>
        /// <param name="source">The string to lex.</param>
        protected LexerBase(string source)
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
        /// Checks, some upcoming text matches a given string.
        /// </summary>
        /// <param name="text">The string to compare with the upcoming text.</param>
        /// <param name="offset">The offset to start the match at in the input.</param>
        /// <returns>True, if there is a full match.</returns>
        protected bool Matches(string text, int offset = 0)
        {
            for (var i = 0; i < text.Length; ++i)
            {
                if (!this.TryPeek(out var ch, offset + i)) return false;
                if (ch != text[i]) return false;
            }
            return true;
        }

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
        /// <param name="length">The amount of characters to skip.</param>
        /// <param name="makeToken">The factory function that receives the source <see cref="Range"/> of the skipped characters
        /// and the skipped characters themselves concatenated as a string, and produces an <see cref="IToken{T}"/> from them.</param>
        /// <returns>The constructed <see cref="IToken{TKind}"/> returned from <paramref name="makeToken"/>.</returns>
        protected IToken<TKind> TakeToken(int length, Func<Range, string, IToken<TKind>> makeToken)
        {
            var start = this.Position;
            var text = length == 0 ? string.Empty : this.Take(length);
            var range = new Range(start, this.Position);
            return makeToken(range, text);
        }

        /// <summary>
        /// Skips characters in the input and builds a <see cref="Token{T}"/> from the skipped characters.
        /// </summary>
        /// <param name="kind">The token-kind to build.</param>
        /// <param name="length">The amount of characters to skip.</param>
        /// <returns>The constructed <see cref="Token{T}"/>.</returns>
        protected Token<TKind> TakeToken(TKind kind, int length)
        {
            var start = this.Position;
            var text = length == 0 ? string.Empty : this.Take(length);
            var range = new Range(start, this.Position);
            return new Token<TKind>(range, text, kind);
        }

        IToken ILexer.Next() => this.Next();

        /// <summary>
        /// Lexes the next <see cref="IToken{T}"/> in the input.
        /// </summary>
        /// <returns>The lexed <see cref="IToken{T}"/>.</returns>
        public abstract IToken<TKind> Next();
    }
}
