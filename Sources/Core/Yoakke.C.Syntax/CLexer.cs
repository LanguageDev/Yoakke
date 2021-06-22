// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lexer;

namespace Yoakke.C.Syntax
{
    /// <summary>
    /// A lexer that lexes C source tokens, including preprocessor directives.
    /// </summary>
    public class CLexer : LexerBase<CToken>
    {
        /// <summary>
        /// True, if line continuations should be enabled with '\'.
        /// </summary>
        public bool AllowLineContinuations { get; set; } = true;

        /// <summary>
        /// True, if digraphs should be enabled.
        /// </summary>
        public bool AllowDigraphs { get; set; } = true;

        /// <summary>
        /// True, if trigraphs should be enabled.
        /// </summary>
        public bool AllowTrigraphs { get; set; } = true;

        public CLexer(TextReader reader)
            : base(reader)
        {
        }

        public CLexer(string source)
            : base(source)
        {
        }

        public override CToken Next()
        {
        begin:
            if (this.IsEnd) return this.TakeToken(CTokenType.End, 0, string.Empty);

            throw new NotImplementedException();
        }

        /// <summary>
        /// Matches a literal text similarly to <see cref="LexerBase{TKind}.Matches(string, int)"/>, but it compares
        /// the escaped text.
        /// </summary>
        protected bool Matches(string text, out int length, int offset)
        {
            length = 0;
            for (var i = 0; i < text.Length; ++i)
            {
                if (!this.TryPeek(out var ch, out var charLength, offset + i)) return false;
                if (ch != text[i]) return false;
                length += charLength;
            }
            return true;
        }

        /// <summary>
        /// Analogue of <see cref="LexerBase{TKind}.Peek(int, char)"/>.
        /// </summary>
        protected char Peek(out int length, int offset = 0, char @default = '\0') => this.TryPeek(out var result, out length, offset)
            ? result
            : @default;

        /// <summary>
        /// Tries to peek the next character, skipping line-continuations and escaping trigraphs and digraphs based on settings.
        /// </summary>
        /// <param name="result">The peeked, escaped character.</param>
        /// <param name="length">The length of the parsed characters.</param>
        /// <param name="offset">The offset to peek at.</param>
        /// <returns>True, if there was a character to peek (end was not reached).</returns>
        protected bool TryPeek(out char result, out int length, int offset)
        {
            // Try to parse the first character
            if (!this.TryPeekLineContinuated(out var firstCh, out var firstLen, offset))
            {
                result = default;
                length = 0;
                return false;
            }
            // We succeeded, we have a first character
            // If digraphs are enabled, we need to check for that
            if (this.AllowDigraphs && this.TryPeekLineContinuated(out var secondCh, out var secondLen, offset + firstLen))
            {
                // Check if the 2 characters form a digraph
                char? digraph = (firstCh, secondCh) switch
                {
                    ('<', ':') => '[',
                    (':', '>') => ']',
                    ('<', '%') => '{',
                    ('%', '>') => '}',
                    ('%', ':') => '#',
                    _ => null,
                };
                if (digraph is not null)
                {
                    // It was a digraph
                    length = firstLen + secondLen;
                    result = digraph.Value;
                    return true;
                }
            }
            // No digraph
            result = firstCh;
            length = firstLen;
            return true;
        }

        /// <summary>
        /// Tries to peek the next character, skipping line-continuations if they are enabled with '\' and '??/',
        /// if trigraphs are enabled. If trigraphs are enabled, they are also escaped.
        /// </summary>
        /// <param name="result">The peeked non-line-continuated, escaped character.</param>
        /// <param name="length">The length of the parsed characters, in case it has continuations or trigraphs.</param>
        /// <param name="offset">The offset to peek at.</param>
        /// <returns>True, if there was a character to peek (end was not reached).</returns>
        private bool TryPeekLineContinuated(out char result, out int length, int offset)
        {
            // We could have a line-continuation, a digraph, or a trigraph
            // NOTE: Digraphs can be split by line-continuations, trigraphs can not
            length = 0;
            var originalOffset = offset;

        begin:
            if (this.AllowLineContinuations)
            {
                // A '\' means potential line-continuation
                // If trigraphs are enabled, so is '??/'
                var potentialNextOffset = offset;
                if (base.Peek(offset) == '\\')
                {
                    potentialNextOffset += 1;
                }
                else if (this.AllowTrigraphs && base.Matches("??/"))
                {
                    potentialNextOffset += 3;
                }

                if (potentialNextOffset > offset)
                {
                    // A '\' or '??/' was present, now we need an endline
                    // Note, that we allow for trailing spaces
                    for (; base.Peek(potentialNextOffset) == ' '; ++potentialNextOffset)
                    {
                        // Pass
                    }
                    // We require an endline
                    // First we check for a Windows-style '\r\n'
                    if (base.Matches("\r\n", potentialNextOffset))
                    {
                        // We got a newline, it's a line-continuation
                        offset = potentialNextOffset + 2;
                        goto begin;
                    }
                    // Unix '\n' or OS-X 9 '\r'
                    var newlineChar = base.Peek(potentialNextOffset + 1);
                    if (newlineChar == '\r' || newlineChar == '\n')
                    {
                        // We got a newline, it's a line-continuation
                        offset = potentialNextOffset + 1;
                        goto begin;
                    }
                }
            }

            // We are after all possible line-continuations
            if (this.AllowTrigraphs && base.Matches("??", offset))
            {
                // Trigraphs are allowed, escape them, if needed
                var potentialNextOffset = offset + 2;
                char? trigraph = base.Peek(potentialNextOffset) switch
                {
                    '=' => '#',
                    '/' => '\\',
                    '\'' => '^',
                    '(' => '[',
                    ')' => ']',
                    '!' => '|',
                    '<' => '{',
                    '>' => '}',
                    '-' => '~',
                    _ => null,
                };
                if (trigraph is not null)
                {
                    // It was a trigraph
                    offset = potentialNextOffset + 1;
                    length = offset - originalOffset;
                    result = trigraph.Value;
                    return true;
                }
            }

            // It is not a trigraph, just peek one
            if (base.TryPeek(out result, offset))
            {
                offset += 1;
                length = offset - originalOffset;
                return true;
            }
            else
            {
                return false;
            }
        }

        protected CToken TakeToken(CTokenType type, int length, string text) =>
            base.TakeToken(length, (range, origText) => new CToken(range, text, origText, type));
    }
}
