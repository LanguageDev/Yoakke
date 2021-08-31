// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Yoakke.Lexer;
using Yoakke.Lexer.Streams;
using Yoakke.Text;

namespace Yoakke.Ir.Syntax
{
    /// <summary>
    /// Lexer for the IR code.
    /// </summary>
    public class IrLexer : ILexer<IToken<IrTokenType>>
    {
        /// <inheritdoc/>
        public Position Position => this.source.Position;

        /// <inheritdoc/>
        public bool IsEnd => this.source.IsEnd;

        private readonly ICharStream source;

        /// <summary>
        /// Initializes a new instance of the <see cref="IrLexer"/> class.
        /// </summary>
        /// <param name="source">The source to lex.</param>
        public IrLexer(ICharStream source)
        {
            this.source = source;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IrLexer"/> class.
        /// </summary>
        /// <param name="source">The source to lex.</param>
        public IrLexer(TextReader source)
            : this(new TextReaderCharStream(source))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IrLexer"/> class.
        /// </summary>
        /// <param name="source">The source to lex.</param>
        public IrLexer(string source)
            : this(new StringReader(source))
        {
        }

        /// <inheritdoc/>
        public IToken<IrTokenType> Next()
        {
        begin:
            // End of file
            if (this.IsEnd) return this.source.ConsumeToken(IrTokenType.End, 0);
            // Whitespace
            if (char.IsWhiteSpace(this.source.Peek()))
            {
                this.source.Advance();
                goto begin;
            }
            // Line comment
            if (this.source.Matches("//"))
            {
                var i = 0;
                for (; this.source.LookAhead(i, '\n') != '\n'; ++i)
                {
                    // Pass
                }
                this.source.Advance(i);
                goto begin;
            }

            // Punctuation
            switch (this.source.Peek())
            {
            case '(': return this.source.ConsumeToken(IrTokenType.OpenParen, 1);
            case ')': return this.source.ConsumeToken(IrTokenType.CloseParen, 1);
            case '[': return this.source.ConsumeToken(IrTokenType.OpenBracket, 1);
            case ']': return this.source.ConsumeToken(IrTokenType.CloseBracket, 1);
            case '{': return this.source.ConsumeToken(IrTokenType.OpenBrace, 1);
            case '}': return this.source.ConsumeToken(IrTokenType.CloseBrace, 1);
            case '.': return this.source.ConsumeToken(IrTokenType.Dot, 1);
            case ',': return this.source.ConsumeToken(IrTokenType.Comma, 1);
            case ':': return this.source.ConsumeToken(IrTokenType.Colon, 1);
            case ';': return this.source.ConsumeToken(IrTokenType.Semicolon, 1);
            case '=': return this.source.ConsumeToken(IrTokenType.Assign, 1);
            case '*': return this.source.ConsumeToken(IrTokenType.Star, 1);
            }

            // Number literal
            if (char.IsDigit(this.source.Peek()))
            {
                var length = 1;
                for (; char.IsDigit(this.source.LookAhead(length)); ++length)
                {
                    // Pass
                }
                return this.source.ConsumeToken(IrTokenType.IntLiteral, length);
            }

            // Identifier
            if (char.IsLetter(this.source.Peek()))
            {
                var length = 1;
                for (; char.IsLetterOrDigit(this.source.LookAhead(length)); ++length)
                {
                    // Pass
                }
                var text = this.source.ConsumeText(length, out var range);
                var tokenType = text switch
                {
                    "block" => IrTokenType.KeywordBlock,
                    "proc" => IrTokenType.KeywordProc,
                    _ => IrTokenType.Identifier,
                };
                return new Token<IrTokenType>(range, text, tokenType);
            }

            // Unknown
            return this.source.ConsumeToken(IrTokenType.Unknown, 1);
        }
    }
}
