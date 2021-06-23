// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            // EOF
            if (this.IsEnd) return this.TakeToken(CTokenType.End, 0, string.Empty);

            // Peek a character to have a clue what is coming up
            var peek = this.PeekDigraph(out var peekLength);

            // Handling newlines
            if (peek == '\r' || peek == '\n')
            {
                // NOTE: It's ok to use the escape-less peek here
                // It is a Windows-style newline
                if (peek == '\r' && base.Peek(peekLength) == '\n') this.Skip(peekLength + 1);
                // OS-X 9 or Unix-style
                else this.Skip(peekLength);

                // Either way, it was a newline
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
                else if (peek2 == '=')
                {
                    // Divide-assign operator
                    return this.TakeToken(CTokenType.DivideAssign, peekLength + peekLength2, "/=");
                }
                else
                {
                    // Division operator
                    return this.TakeToken(CTokenType.Divide, peekLength, "/");
                }
            }

            case '#':
            {
                // It's either a hashmark or a double-hashmark
                // But for that we use the digraph-escaping peek, because %:%: can specify ## with digraphs
                var peek2 = this.PeekDigraph(out var peekLength2, peekLength);
                return peek2 == '#'
                    // It's a ##
                    ? this.TakeToken(CTokenType.HashHash, peekLength + peekLength2, "##")
                    // It's a simple #
                    : this.TakeToken(CTokenType.Hash, peekLength, "#");
            }

            case '(': return this.TakeToken(CTokenType.OpenParen, peekLength, "(");
            case ')': return this.TakeToken(CTokenType.CloseParen, peekLength, ")");
            case '[': return this.TakeToken(CTokenType.OpenBracket, peekLength, "[");
            case ']': return this.TakeToken(CTokenType.CloseBracket, peekLength, "]");
            case '{': return this.TakeToken(CTokenType.OpenBrace, peekLength, "{");
            case '}': return this.TakeToken(CTokenType.CloseBrace, peekLength, "}");
            case '~': return this.TakeToken(CTokenType.BitNot, peekLength, "~");
            case ',': return this.TakeToken(CTokenType.Comma, peekLength, ",");
            case ':': return this.TakeToken(CTokenType.Colon, peekLength, ":");
            case ';': return this.TakeToken(CTokenType.Semicolon, peekLength, ";");
            case '?': return this.TakeToken(CTokenType.QuestionMark, peekLength, "?");

            case '+':
            {
                var peek2 = this.PeekTrigraph(out var peekLength2, peekLength);
                if (peek2 == '=') return this.TakeToken(CTokenType.AddAssign, peekLength + peekLength2, "+=");
                else if (peek2 == '+') return this.TakeToken(CTokenType.Increment, peekLength + peekLength2, "++");
                else return this.TakeToken(CTokenType.Add, peekLength, "+");
            }
            case '-':
            {
                var peek2 = this.PeekTrigraph(out var peekLength2, peekLength);
                if (peek2 == '=') return this.TakeToken(CTokenType.SubtractAssign, peekLength + peekLength2, "-=");
                else if (peek2 == '-') return this.TakeToken(CTokenType.Decrement, peekLength + peekLength2, "--");
                else if (peek2 == '>') return this.TakeToken(CTokenType.Arrow, peekLength + peekLength2, "->");
                else return this.TakeToken(CTokenType.Subtract, peekLength, "-");
            }
            case '*':
            {
                return this.PeekTrigraph(out var peekLength2, peekLength) == '='
                    ? this.TakeToken(CTokenType.MultiplyAssign, peekLength + peekLength2, "*=")
                    : this.TakeToken(CTokenType.Multiply, peekLength, "*");
            }
            // NOTE: '/' is handled above
            case '%':
            {
                return this.PeekTrigraph(out var peekLength2, peekLength) == '='
                    ? this.TakeToken(CTokenType.ModuloAssign, peekLength + peekLength2, "%=")
                    : this.TakeToken(CTokenType.Modulo, peekLength, "%");
            }
            case '^':
            {
                return this.PeekTrigraph(out var peekLength2, peekLength) == '='
                    ? this.TakeToken(CTokenType.BitXorAssign, peekLength + peekLength2, "^=")
                    : this.TakeToken(CTokenType.BitXor, peekLength, "^");
            }
            case '!':
            {
                return this.PeekTrigraph(out var peekLength2, peekLength) == '='
                    ? this.TakeToken(CTokenType.NotEqual, peekLength + peekLength2, "!=")
                    : this.TakeToken(CTokenType.LogicalNot, peekLength, "!");
            }
            case '=':
            {
                return this.PeekTrigraph(out var peekLength2, peekLength) == '='
                    ? this.TakeToken(CTokenType.Equal, peekLength + peekLength2, "==")
                    : this.TakeToken(CTokenType.Assign, peekLength, "=");
            }

            case '&':
            {
                var peek2 = this.PeekTrigraph(out var peekLength2, peekLength);
                if (peek2 == '=') return this.TakeToken(CTokenType.BitAndAssign, peekLength + peekLength2, "&=");
                else if (peek2 == '&') return this.TakeToken(CTokenType.LogicalAnd, peekLength + peekLength2, "&&");
                else return this.TakeToken(CTokenType.BitAnd, peekLength, "&");
            }
            case '|':
            {
                var peek2 = this.PeekTrigraph(out var peekLength2, peekLength);
                if (peek2 == '=') return this.TakeToken(CTokenType.BitOrAssign, peekLength + peekLength2, "|=");
                else if (peek2 == '|') return this.TakeToken(CTokenType.LogicalOr, peekLength + peekLength2, "||");
                else return this.TakeToken(CTokenType.BitOr, peekLength, "|");
            }

            case '>':
            {
                var peek2 = this.PeekTrigraph(out var peekLength2, peekLength);
                if (peek2 == '>')
                {
                    // Shift or shift-assign
                    if (this.PeekTrigraph(out var peekLength3, peekLength + peekLength2) == '=')
                    {
                        return this.TakeToken(CTokenType.ShiftRightAssign, peekLength + peekLength2 + peekLength3, ">>=");
                    }
                    else
                    {
                        return this.TakeToken(CTokenType.ShiftRight, peekLength + peekLength2, ">>");
                    }
                }
                else if (peek2 == '=')
                {
                    // Cmp-equal
                    return this.TakeToken(CTokenType.GreaterEqual, peekLength + peekLength2, ">=");
                }
                else
                {
                    // Just cmp
                    return this.TakeToken(CTokenType.Greater, peekLength, ">");
                }
            }

            case '<':
            {
                var peek2 = this.PeekTrigraph(out var peekLength2, peekLength);
                if (peek2 == '<')
                {
                    // Shift or shift-assign
                    if (this.PeekTrigraph(out var peekLength3, peekLength + peekLength2) == '=')
                    {
                        return this.TakeToken(CTokenType.ShiftLeftAssign, peekLength + peekLength2 + peekLength3, "<<=");
                    }
                    else
                    {
                        return this.TakeToken(CTokenType.ShiftLeft, peekLength + peekLength2, "<<");
                    }
                }
                else if (peek2 == '=')
                {
                    // Cmp-equal
                    return this.TakeToken(CTokenType.LessEqual, peekLength + peekLength2, "<=");
                }
                else
                {
                    // Just cmp
                    return this.TakeToken(CTokenType.Less, peekLength, "<");
                }
            }

            case '.':
            {
                var peek2 = this.PeekTrigraph(out var peekLength2, peekLength);
                if (peek2 == '.' && this.PeekTrigraph(out var peekLength3, peekLength + peekLength2) == '.')
                {
                    return this.TakeToken(CTokenType.Ellipsis, peekLength + peekLength2 + peekLength3, "...");
                }
                else if (!char.IsDigit(peek2))
                {
                    // If it was a digit, we would consider it a float literal
                    return this.TakeToken(CTokenType.Dot, peekLength, ".");
                }
                break;
            }
            }

            if (char.IsDigit(peek))
            {
                // Number literals
                var offset = peekLength;
                var number = new StringBuilder(peek);

                if (peek == '0' && this.TakeIf(number, ref offset, IsX))
                {
                    // Hex literal
                    this.TakeWhile(number, ref offset, IsHexDigit);
                    // Only consider it a real hex number, if at least 1 hex digit was present
                    // That means a length greater than 2, as '0x' is 2 chars
                    if (number.Length > 2)
                    {
                        // Now we can take arbitrary amount of integer suffixes
                        this.TakeWhile(number, ref offset, IsIntSuffix);
                        // We are done
                        return this.TakeToken(CTokenType.IntLiteral, offset, number.ToString());
                    }
                }
                else
                {
                    // Any other decimal number literal
                    bool isFloat = false;
                    this.TakeWhile(number, ref offset, char.IsDigit);
                    // We have consumed all decimal digits available
                    if (this.TakeIf(number, ref offset, '.'))
                    {
                        // Decimal point
                        isFloat = true;
                        this.TakeWhile(number, ref offset, char.IsDigit);
                    }
                    // Exponent
                    if (this.TakeExponent(number, ref offset)) isFloat = true;

                    if (isFloat)
                    {
                        // Only allow float suffixes
                        this.TakeWhile(number, ref offset, IsFloatSuffix);
                    }
                    else
                    {
                        // Only allow integer suffixes
                        this.TakeWhile(number, ref offset, IsIntSuffix);
                    }

                    // We are done
                    return this.TakeToken(isFloat ? CTokenType.FloatLiteral : CTokenType.IntLiteral, offset, number.ToString());
                }
            }

            if (peek == '.')
            {
                // It can only be a floating point number
                var offset = peekLength;
                var number = new StringBuilder(peek);
                this.TakeWhile(number, ref offset, char.IsDigit);
                Debug.Assert(number.Length > 1, "digit expected after dot");
                // Exponent
                this.TakeExponent(number, ref offset);
                // Suffixes
                this.TakeWhile(number, ref offset, IsFloatSuffix);
                // We are done
                return this.TakeToken(CTokenType.FloatLiteral, offset, number.ToString());
            }

            // Char literal
            if (peek == '\'')
            {
                var literal = new StringBuilder(peek);
                var offset = peekLength;
                if (this.TakeLiteral(literal, ref offset, '\'') && literal.Length > 2)
                {
                    // This is only successful, if there was at least one character besides the open and close character
                    return this.TakeToken(CTokenType.CharLiteral, offset, literal.ToString());
                }
            }

            // String literal
            if (peek == '"')
            {
                var literal = new StringBuilder(peek);
                var offset = peekLength;
                if (this.TakeLiteral(literal, ref offset, '"'))
                {
                    return this.TakeToken(CTokenType.StringLiteral, offset, literal.ToString());
                }
            }

            if (IsIdent(peek))
            {
                var ident = new StringBuilder(peek);
                var offset = peekLength;

                if (this.TakeIf(ident, ref offset, '\''))
                {
                    // Char literal
                    if (this.TakeLiteral(ident, ref offset, '\'') && ident.Length > 2)
                    {
                        // This is only successful, if there was at least one character besides the open and close character
                        return this.TakeToken(CTokenType.CharLiteral, offset, ident.ToString());
                    }
                }
                else if (this.TakeIf(ident, ref offset, '"'))
                {
                    // String literal
                    if (this.TakeLiteral(ident, ref offset, '"'))
                    {
                        return this.TakeToken(CTokenType.StringLiteral, offset, ident.ToString());
                    }
                }
                else
                {
                    // Identifier
                    this.TakeWhile(ident, ref offset, IsIdent);
                    var text = ident.ToString();

                    // Determine the token-type
                    var tokenType = text switch
                    {
                        "auto" => CTokenType.KeywordAuto,
                        "break" => CTokenType.KeywordBreak,
                        "case" => CTokenType.KeywordCase,
                        "char" => CTokenType.KeywordChar,
                        "const" => CTokenType.KeywordConst,
                        "continue" => CTokenType.KeywordContinue,
                        "default" => CTokenType.KeywordDefault,
                        "do" => CTokenType.KeywordDo,
                        "double" => CTokenType.KeywordDouble,
                        "else" => CTokenType.KeywordElse,
                        "enum" => CTokenType.KeywordEnum,
                        "extern" => CTokenType.KeywordExtern,
                        "float" => CTokenType.KeywordFloat,
                        "for" => CTokenType.KeywordFor,
                        "goto" => CTokenType.KeywordGoto,
                        "if" => CTokenType.KeywordIf,
                        "int" => CTokenType.KeywordInt,
                        "long" => CTokenType.KeywordLong,
                        "register" => CTokenType.KeywordRegister,
                        "return" => CTokenType.KeywordReturn,
                        "short" => CTokenType.KeywordShort,
                        "signed" => CTokenType.KeywordSigned,
                        "sizeof" => CTokenType.KeywordSizeof,
                        "static" => CTokenType.KeywordStatic,
                        "struct" => CTokenType.KeywordStruct,
                        "switch" => CTokenType.KeywordSwitch,
                        "typedef" => CTokenType.KeywordTypedef,
                        "union" => CTokenType.KeywordUnion,
                        "unsigned" => CTokenType.KeywordUnsigned,
                        "void" => CTokenType.KeywordVoid,
                        "volatile" => CTokenType.KeywordVolatile,
                        "while" => CTokenType.KeywordWhile,
                        _ => CTokenType.Identifier,
                    };

                    return this.TakeToken(tokenType, offset, text);
                }
            }

            // Unknown
            return this.TakeToken(CTokenType.Unknown, 1, peek.ToString());
        }

        private static bool IsX(char ch) => ch == 'x' || ch == 'X';

        private static bool IsE(char ch) => ch == 'e' || ch == 'E';

        private static bool IsSign(char ch) => ch == '+' || ch == '-';

        private static bool IsHexDigit(char ch) => char.IsDigit(ch) || "abcdef".Contains(char.ToLower(ch));

        private static bool IsIntSuffix(char ch) => ch == 'u' || ch == 'U' || ch == 'l' || ch == 'L';

        private static bool IsFloatSuffix(char ch) => ch == 'f' || ch == 'F' || ch == 'l' || ch == 'L';

        private static bool IsIdent(char ch) => char.IsLetterOrDigit(ch) || ch == '_';

        private bool TakeIf(out char @char, ref int offset, Predicate<char> predicate)
        {
            var peek = this.PeekTrigraph(out var length, offset);
            if (predicate(peek))
            {
                offset += length;
                @char = peek;
                return true;
            }
            else
            {
                @char = default;
                return false;
            }
        }

        private bool TakeIf(StringBuilder target, ref int offset, Predicate<char> predicate)
        {
            if (this.TakeIf(out var ch, ref offset, predicate))
            {
                target.Append(ch);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TakeIf(StringBuilder target, ref int offset, char what) =>
            this.TakeIf(target, ref offset, ch => ch == what);

        private int TakeWhile(StringBuilder target, ref int offset, Predicate<char> predicate)
        {
            int i = 0;
            for (; this.TakeIf(target, ref offset, predicate); ++i)
            {
                // Pass
            }
            return i;
        }

        private bool TakeExponent(StringBuilder target, ref int offset)
        {
            var count = target.Length;
            if (this.TakeIf(target, ref offset, IsE))
            {
                this.TakeIf(target, ref offset, IsSign);
                if (this.TakeWhile(target, ref offset, char.IsDigit) > 0) return true;
            }
            target.Remove(count, target.Length - count);
            return false;
        }

        private bool TakeOneLiteral(StringBuilder target, ref int offset, char forbidden)
        {
            var peek1 = this.PeekTrigraph(out var length, offset, forbidden);
            if (peek1 == '\\')
            {
                // Escaped
                if (this.TryPeekTrigraph(out var ch, out var length2, offset + length))
                {
                    // There was a character to escape
                    offset += length + length2;
                    target.Append('\\').Append(ch);
                    return true;
                }
            }
            else if (peek1 != forbidden)
            {
                offset += length;
                target.Append(peek1);
                return true;
            }
            return false;
        }

        private bool TakeLiteral(StringBuilder target, ref int offset, char close)
        {
            int i = 0;
            for (; this.TakeOneLiteral(target, ref offset, close); ++i)
            {
                // Pass
            }
            if (this.TakeIf(target, ref offset, close))
            {
                // There was a proper close character
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Consumes a <see cref="CToken"/> from the input.
        /// </summary>
        /// <param name="type">The <see cref="CTokenType"/> of the token.</param>
        /// <param name="length">The length of the <see cref="CToken"/> in actual characters.</param>
        /// <param name="logicalText">The logical, meaningful text of the <see cref="CToken"/>.</param>
        /// <returns>The created <see cref="CToken"/>.</returns>
        private CToken TakeToken(CTokenType type, int length, string logicalText)
        {
            var lastPosition = this.LogicalPosition;
            this.LogicalPosition = this.LogicalPosition.Advance(logicalText.Length);
            var logicalRange = new Text.Range(lastPosition, this.LogicalPosition);
            return this.TakeToken(type, length, logicalRange, logicalText);
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
