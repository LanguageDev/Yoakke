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
using Yoakke.Text;

namespace Yoakke.C.Syntax
{
    /// <summary>
    /// A lexer that lexes C source tokens, including preprocessor directives.
    /// </summary>
    public class CLexer : LexerBase<CToken>
    {
        /// <summary>
        /// The logical <see cref="Position"/> in the source.
        /// This takes line continuations as being in the same line as the previous one for example.
        /// </summary>
        public Position LogicalPosition { get; private set; }

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
            // Current logical position
            var currentPosition = this.LogicalPosition;

            // Ease token creation by updating the logical position
            CToken TakeToken(CTokenType type, int length, string logicalText)
            {
                var logicalRange = new Text.Range(this.LogicalPosition, currentPosition);
                this.LogicalPosition = currentPosition;
                return this.TakeToken(type, length, logicalRange, logicalText);
            }

            // EOF
            if (this.IsEnd) return TakeToken(CTokenType.End, 0, string.Empty);

            // Peek a character to have a clue what is coming up
            var peek = this.PeekDigraph(out var peekLength);

            // Handling newlines
            if (peek == '\r')
            {
                // We use the primitive peek method, it is enough
                var peek2 = this.Peek(peekLength);

                // It is a Windows-style newline
                if (peek2 == '\n') this.Skip(peekLength + 1);
                // OS-X 9 style
                else this.Skip(peekLength);

                // Either way, it was a newline
                this.LogicalPosition = this.LogicalPosition.Newline();
                goto begin;
            }
            if (peek == '\n')
            {
                // Unix-style newline
                this.Skip(peekLength);
                this.LogicalPosition = this.LogicalPosition.Newline();
                goto begin;
            }

            // A control character means nothing, no logical position steps
            if (char.IsControl(peek))
            {
                this.Skip(peekLength);
                goto begin;
            }

            // Whitespace on the other hand is advancement
            // Also the NUL character for some reason
            if (char.IsWhiteSpace(peek) || peek == '\0')
            {
                this.Skip(peekLength);
                this.LogicalPosition = this.LogicalPosition.Advance();
                goto begin;
            }

            // Look up what it could be based on the single peek
            switch (peek)
            {
            case '/':
            {
                // Can be a division operator ('/'), a line comment ('//') and a multiline-comment ('/* ... */')
                // To find out, trigraph-peeking is enough
                var peek2 = this.PeekTrigraph(out var peekLength2, peekLength);
                if (peek2 == '/')
                {
                    // Line comment
                    // We don't deal with escaping digraphs from here, nor do we care about logical positioning
                    // Logical position will essentially update by a space
                    int offset = peekLength + peekLength2;
                    while (true)
                    {
                        var ch = this.PeekTrigraph(out var length, offset, '\n');
                        // End of comment
                        if (ch == '\r' || ch == '\n') break;
                        // Skip character
                        offset += length;
                    }
                    // Advance logical position by a space, skip
                    this.Skip(offset);
                    this.LogicalPosition = this.LogicalPosition.Advance();
                    goto begin;
                }
                else if (peek2 == '*')
                {
                    // Multiline comment
                    // We don't deal with escaping digraphs from here, nor do we care about logical positioning
                    // Logical position will essentially update by a space
                    int offset = peekLength + peekLength2;
                    while (true)
                    {
                        if (!this.TryPeekTrigraph(out var ch1, out var length1, offset))
                        {
                            // EOF, but comment did not end
                            // TODO: Handle properly
                            break;
                        }
                        offset += length1;
                        if (ch1 == '*')
                        {
                            // Potentially end of comment
                            var ch2 = this.PeekTrigraph(out var length2, offset);
                            if (ch2 == '/')
                            {
                                // End of comment
                                offset += length2;
                                break;
                            }
                        }
                    }
                    // Advance logical position by a space, skip
                    this.Skip(offset);
                    this.LogicalPosition = this.LogicalPosition.Advance();
                    goto begin;
                }
                else
                {
                    // Division operator
                    return TakeToken(CTokenType.Slash, peekLength2, "/");
                }
            }

            case '#':
            {
                // It's either a hashmark or a double-hashmark
                // But for that we use the digraph-escaping peek, because %:%: can specify ## with digraphs
                var peek2 = this.PeekDigraph(out var peekLength2, peekLength);
                return peek2 == '#'
                    // It's a ##
                    ? TakeToken(CTokenType.HashHash, peekLength + peekLength2, "##")
                    // It's a simple #
                    : TakeToken(CTokenType.Hash, peekLength, "#");
            }
            }

            // Unknown
            return TakeToken(CTokenType.Unknown, 1, peek.ToString());
        }

        /// <summary>
        /// Peeks while escaping digraphs and trigraphs. Wrapper for <see cref="TryPeekDigraph(out char, out int, int)"/>.
        /// </summary>
        protected char PeekDigraph(out int length, int offset = 0, char @default = '\0') => this.TryPeekDigraph(out var result, out length, offset)
            ? result
            : @default;

        /// <summary>
        /// Peeks while escaping trigraphs only. Wrapper for <see cref="TryPeekTrigraph(out char, out int, int)"/>.
        /// </summary>
        protected char PeekTrigraph(out int length, int offset = 0, char @default = '\0') => this.TryPeekTrigraph(out var result, out length, offset)
            ? result
            : @default;

        /// <summary>
        /// Tries to peek the next character, skipping line-continuations and escaping trigraphs and digraphs based on settings.
        /// </summary>
        /// <param name="result">The peeked, escaped character.</param>
        /// <param name="length">The length of the parsed characters.</param>
        /// <param name="offset">The offset to peek at.</param>
        /// <returns>True, if there was a character to peek (end was not reached).</returns>
        protected bool TryPeekDigraph(out char result, out int length, int offset)
        {
            // Try to parse the first character
            if (!this.TryPeekTrigraph(out var firstCh, out var firstLen, offset))
            {
                result = default;
                length = 0;
                return false;
            }
            // We succeeded, we have a first character
            // If digraphs are enabled, we need to check for that
            if (this.AllowDigraphs && this.TryPeekTrigraph(out var secondCh, out var secondLen, offset + firstLen))
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
        private bool TryPeekTrigraph(out char result, out int length, int offset)
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

        protected CToken TakeToken(CTokenType type, int length, Text.Range logicalRange, string logicalText) =>
            base.TakeToken(length, (origRange, origText) => new CToken(logicalRange, logicalText, origRange, origText, type));
    }
}
