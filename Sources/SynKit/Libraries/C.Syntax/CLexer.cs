// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.IO;
using System.Polyfill;
using System.Text;
using Yoakke.SynKit.Lexer;
using Yoakke.Streams;
using Yoakke.SynKit.Text;

namespace Yoakke.SynKit.C.Syntax;

/// <summary>
/// A lexer that lexes C source tokens, including preprocessor directives.
/// </summary>
public class CLexer : ILexer<CToken>
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

    /// <summary>
    /// True, if unicode characters in identifiers should be enabled.
    /// </summary>
    public bool AllowUnicodeCharacters { get; set; } = true;

    /// <inheritdoc/>
    public Position Position => this.source.Position;

    /// <inheritdoc/>
    public bool IsEnd => this.source.IsEnd;

    private readonly ICharStream source;

    /// <summary>
    /// Initializes a new instance of the <see cref="CLexer"/> class.
    /// </summary>
    /// <param name="source">The <see cref="ICharStream"/> to read the source from.</param>
    public CLexer(ICharStream source)
    {
        this.source = source;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CLexer"/> class.
    /// </summary>
    /// <param name="sourceFile">The <see cref="ISourceFile"/> to read the source from.</param>
    public CLexer(ISourceFile sourceFile)
        : this(new TextReaderCharStream(sourceFile))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CLexer"/> class.
    /// </summary>
    /// <param name="sourceFile">The <see cref="ISourceFile"/> to read the source from.</param>
    public CLexer(SourceFile sourceFile)
        : this(new TextReaderCharStream(sourceFile))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CLexer"/> class.
    /// </summary>
    /// <param name="reader">The <see cref="TextReader"/> to read the source from.</param>
    public CLexer(TextReader reader)
        : this(new TextReaderCharStream(new SourceFile("<no-location>", reader)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CLexer"/> class.
    /// </summary>
    /// <param name="source">The text to read.</param>
    public CLexer(string source)
        : this((ISourceFile)new SourceFile("<no-location>", source))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CLexer"/> class.
    /// </summary>
    /// <param name="path">Path to source location.</param>
    /// <param name="source">The text to read.</param>
    public CLexer(string path, string source)
        : this(new SourceFile(path, source))
    {
    }

    /// <inheritdoc/>
    public CToken Next()
    {
        var offset = 0;

        CToken Make(CTokenType type, string text) => this.TakeToken(type, offset, text);

    begin:
        // Since we can jump back here, we need to reset
        this.source.Consume(offset);
        offset = 0;

        // EOF
        if (!this.TryParseEscaped(out var peek, ref offset)) return this.TakeToken(CTokenType.End, 0, string.Empty);

        // Check for newline
        if (peek == '\r' || peek == '\n')
        {
            // If it was a '\r', consume '\n' because it might be Windows-style
            if (peek == '\r') this.MatchesEscaped('\n', ref offset);
            // Was a newline anyway
            this.LogicalPosition = this.LogicalPosition.Newline();
            goto begin;
        }

        // Whitespace is horizontal skip, as well as the NUL character
        if (char.IsWhiteSpace(peek) || peek == '\0')
        {
            this.LogicalPosition = this.LogicalPosition.Advance();
            goto begin;
        }

        // Control character is just skip
        if (char.IsControl(peek)) goto begin;

        // Check for comments
        if (peek == '/' && this.MatchesEscaped('/', ref offset))
        {
            // Line-comment, go until the end of line
            while (this.MatchesEscaped(ch => !IsNewline(ch), out _, ref offset))
            {
                // Pass
            }
            // A line comment becomes a space
            this.LogicalPosition = this.LogicalPosition.Advance();
            goto begin;
        }
        if (peek == '/' && this.MatchesEscaped('*', ref offset))
        {
            // Multi-line comment
            while (true)
            {
                if (!this.TryParseEscaped(out var ch, ref offset)) break;
                if (ch == '*' && this.MatchesEscaped('/', ref offset)) break;
            }
            // A multi-line comment becomes a space too
            this.LogicalPosition = this.LogicalPosition.Advance();
            goto begin;
        }

        // Try to decide on the current character
        switch (peek)
        {
        case ',': return Make(CTokenType.Comma, ",");
        case ';': return Make(CTokenType.Semicolon, ";");
        case '?': return Make(CTokenType.QuestionMark, "?");
        case '~': return Make(CTokenType.BitNot, "~");
        case '(': return Make(CTokenType.OpenParen, "(");
        case ')': return Make(CTokenType.CloseParen, ")");
        case '{': return Make(CTokenType.OpenBrace, "{");
        case '}': return Make(CTokenType.CloseBrace, "}");
        case '[': return Make(CTokenType.OpenBracket, "[");
        case ']': return Make(CTokenType.CloseBracket, "]");

        case ':':
            return this.AllowDigraphs && this.MatchesEscaped('>', ref offset)
          ? Make(CTokenType.CloseBracket, ":>")
          : Make(CTokenType.Colon, ":");
        case '*':
            return this.MatchesEscaped('=', ref offset)
          ? Make(CTokenType.MultiplyAssign, "*=")
          : Make(CTokenType.Multiply, "*");
        case '/':
            return this.MatchesEscaped('=', ref offset)
          ? Make(CTokenType.DivideAssign, "/=")
          : Make(CTokenType.Divide, "/");
        case '!':
            return this.MatchesEscaped('=', ref offset)
          ? Make(CTokenType.NotEqual, "!=")
          : Make(CTokenType.LogicalNot, "!");
        case '^':
            return this.MatchesEscaped('=', ref offset)
          ? Make(CTokenType.BitXorAssign, "^=")
          : Make(CTokenType.BitXor, "^");
        case '=':
            return this.MatchesEscaped('=', ref offset)
          ? Make(CTokenType.Equal, "==")
          : Make(CTokenType.Assign, "=");
        case '#':
            return this.MatchesEscaped('#', ref offset)
          ? Make(CTokenType.HashHash, "##")
          : Make(CTokenType.Hash, "#");

        case '+':
            if (this.MatchesEscaped('+', ref offset)) return Make(CTokenType.Increment, "++");
            if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.AddAssign, "+=");
            return Make(CTokenType.Add, "+");
        case '|':
            if (this.MatchesEscaped('|', ref offset)) return Make(CTokenType.LogicalOr, "||");
            if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.BitOrAssign, "|=");
            return Make(CTokenType.BitOr, "|");
        case '&':
            if (this.MatchesEscaped('&', ref offset)) return Make(CTokenType.LogicalAnd, "&&");
            if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.BitAndAssign, "&=");
            return Make(CTokenType.BitAnd, "&");

        case '-':
            if (this.MatchesEscaped('-', ref offset)) return Make(CTokenType.Decrement, "--");
            if (this.MatchesEscaped('>', ref offset)) return Make(CTokenType.Arrow, "->");
            if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.SubtractAssign, "-=");
            return Make(CTokenType.Subtract, "-");
        case '>':
            if (this.MatchesEscaped(">=", ref offset)) return Make(CTokenType.ShiftRightAssign, ">>=");
            if (this.MatchesEscaped('>', ref offset)) return Make(CTokenType.ShiftRight, ">>");
            if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.GreaterEqual, ">=");
            return Make(CTokenType.Greater, ">");

        case '<':
            if (this.MatchesEscaped("<=", ref offset)) return Make(CTokenType.ShiftLeftAssign, "<<=");
            if (this.MatchesEscaped('<', ref offset)) return Make(CTokenType.ShiftLeft, "<<");
            if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.LessEqual, "<=");
            if (this.AllowDigraphs && this.MatchesEscaped(':', ref offset)) return Make(CTokenType.OpenBracket, "<:");
            if (this.AllowDigraphs && this.MatchesEscaped('%', ref offset)) return Make(CTokenType.OpenBrace, "<%");
            return Make(CTokenType.Less, "<");

        case '%':
            if (this.AllowDigraphs && this.MatchesEscaped(":%:", ref offset)) return Make(CTokenType.HashHash, "%:%:");
            if (this.AllowDigraphs && this.MatchesEscaped(':', ref offset)) return Make(CTokenType.Hash, "%:");
            if (this.AllowDigraphs && this.MatchesEscaped('>', ref offset)) return Make(CTokenType.CloseBrace, "%>");
            if (this.MatchesEscaped('=', ref offset)) return Make(CTokenType.ModuloAssign, "%=");
            return Make(CTokenType.Modulo, "%");

        case '.':
            if (this.MatchesEscaped("..", ref offset)) return Make(CTokenType.Ellipsis, "...");
            // If it's a digit, it's a float, don't handle it here
            if (!char.IsDigit(this.ParseEscaped(offset, out _))) return Make(CTokenType.Dot, ".");
            break;
        }

        if (char.IsDigit(peek) || peek == '.')
        {
            var oldOffset = offset;
            // It's number
            var text = new StringBuilder().Append(peek);
            if (peek == '0' && this.MatchesEscaped(IsX, out var x, ref offset))
            {
                // Hex number
                text.Append(x);
                // At least one hex digit is required
                if (this.TakeWhile(text, IsHex, ref offset) > 0)
                {
                    this.TakeWhile(text, IsIntSuffix, ref offset);
                    return Make(CTokenType.IntLiteral, text.ToString());
                }
                else
                {
                    // Reset the offset, let the rest be handled by the lexer
                    offset = oldOffset;
                }
            }
            else
            {
                // Any other number
                var isFloat = peek == '.';
                if (peek != '.')
                {
                    this.TakeWhile(text, char.IsDigit, ref offset);
                    if (this.MatchesEscaped('.', ref offset))
                    {
                        text.Append('.');
                        isFloat = true;
                    }
                }
                this.TakeWhile(text, char.IsDigit, ref offset);
                if (this.TryParseExponent(text, ref offset)) isFloat = true;
                // Proper suffixes
                if (isFloat) this.TakeWhile(text, IsFloatSuffix, ref offset);
                else this.TakeWhile(text, IsIntSuffix, ref offset);
                // Done
                return Make(isFloat ? CTokenType.FloatLiteral : CTokenType.IntLiteral, text.ToString());
            }
        }

        // Literals
        if (IsLiteralSeparator(peek) || this.IsIdent(peek))
        {
            var isLiteral = false;
            var text = new StringBuilder().Append(peek);
            var separatorChar = peek;
            if (IsLiteralSeparator(peek))
            {
                isLiteral = true;
            }
            else if (this.MatchesEscaped(IsLiteralSeparator, out var sep, ref offset))
            {
                isLiteral = true;
                text.Append(sep);
                separatorChar = sep;
            }

            // It is a literal
            if (isLiteral)
            {
                while (true)
                {
                    if (this.MatchesEscaped(separatorChar, ref offset))
                    {
                        text.Append(separatorChar);
                        break;
                    }
                    if (this.MatchesEscaped('\\', ref offset))
                    {
                        // Anything
                        text.Append('\\');
                        text.Append(this.ParseEscaped(ref offset));
                    }
                    else if (this.MatchesEscaped(ch => !char.IsControl(ch), out var ch, ref offset))
                    {
                        // Just a single character to consume
                        text.Append(ch);
                    }
                    else
                    {
                        // There's not even a character to get
                        break;
                    }
                }
                return Make(separatorChar == '\'' ? CTokenType.CharLiteral : CTokenType.StringLiteral, text.ToString());
            }
        }

        if (this.IsIdent(peek))
        {
            var text = new StringBuilder().Append(peek);
            this.TakeWhile(text, this.IsIdent, ref offset);
            var str = text.ToString();
            // Determine the token-type
            var tokenType = str switch
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
                "inline" => CTokenType.KeywordInline,
                "int" => CTokenType.KeywordInt,
                "long" => CTokenType.KeywordLong,
                "register" => CTokenType.KeywordRegister,
                "restrict" => CTokenType.KeywordRestrict,
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
                "_Alignas" => CTokenType.KeywordAlignAs,
                "_Alignof" => CTokenType.KeywordAlignOf,
                "_Atomic" => CTokenType.KeywordAtomic,
                "_Bool" => CTokenType.KeywordBool,
                "_Complex" => CTokenType.KeywordComplex,
                "_Generic" => CTokenType.KeywordGeneric,
                "_Imaginary" => CTokenType.KeywordImaginary,
                "_Noreturn" => CTokenType.KeywordNoReturn,
                "_Static_assert" => CTokenType.KeywordStaticAssert,
                "_Thread_local" => CTokenType.KeywordThreadLocal,
                _ => CTokenType.Identifier,
            };

            return Make(tokenType, str);
        }

        // Unknown
        return this.TakeToken(CTokenType.Unknown, offset, peek.ToString());
    }

    private bool TryParseExponent(StringBuilder result, ref int offset) =>
        this.TryParseExponent(result, offset, out offset);

    private bool TryParseExponent(StringBuilder result, int offset, out int nextOffset)
    {
        var initial = offset;
        if (this.MatchesEscaped(IsE, out var e, ref offset))
        {
            result.Append(e);
            if (this.MatchesEscaped(IsSign, out var sign, ref offset)) result.Append(sign);
            if (this.TakeWhile(result, char.IsDigit, ref offset) > 0)
            {
                nextOffset = offset;
                return true;
            }
        }
        nextOffset = initial;
        return false;
    }

    private int TakeWhile(StringBuilder result, Predicate<char> predicate, ref int offset) =>
        this.TakeWhile(result, predicate, offset, out offset);

    private int TakeWhile(StringBuilder result, Predicate<char> predicate, int offset, out int nextOffset)
    {
        var length = 0;
        for (; this.MatchesEscaped(predicate, out var ch, ref offset); ++length)
        {
            result.Append(ch);
        }
        nextOffset = offset;
        return length;
    }

    /// <summary>
    /// Wrapper to <see cref="MatchesEscaped(Predicate{char}, out char, int, out int)"/>.
    /// </summary>
    private bool MatchesEscaped(Predicate<char> predicate, out char result, ref int offset) =>
        this.MatchesEscaped(predicate, out result, offset, out offset);

    /// <summary>
    /// Tries to match an escaped character with a predicate.
    /// </summary>
    /// <param name="predicate">The predicate to match the character with.</param>
    /// <param name="result">The result character gets written here.</param>
    /// <param name="offset">The offset to match at.</param>
    /// <param name="nextOffset">The offset gets written here after the match.</param>
    /// <returns>True, if the character matches.</returns>
    private bool MatchesEscaped(Predicate<char> predicate, out char result, int offset, out int nextOffset)
    {
        if (this.TryParseEscaped(out var ch, offset, out var offset2) && predicate(ch))
        {
            nextOffset = offset2;
            result = ch;
            return true;
        }
        else
        {
            nextOffset = offset;
            result = default;
            return false;
        }
    }

    /// <summary>
    /// Wrapper for <see cref="MatchesEscaped(string, int, out int)"/>.
    /// </summary>
    private bool MatchesEscaped(string text, ref int offset) =>
        this.MatchesEscaped(text, offset, out offset);

    /// <summary>
    /// Tries to match an escaped string from the input.
    /// </summary>
    /// <param name="text">The text to match.</param>
    /// <param name="offset">The offset to start the match from.</param>
    /// <param name="nextOffset">The offset gets written here after the match.</param>
    /// <returns>True, if the text matches.</returns>
    private bool MatchesEscaped(string text, int offset, out int nextOffset)
    {
        var initial = offset;
        for (var i = 0; i < text.Length; ++i)
        {
            if (!this.MatchesEscaped(text[i], offset, out nextOffset))
            {
                nextOffset = initial;
                return false;
            }
            offset = nextOffset;
        }
        nextOffset = offset;
        return true;
    }

    /// <summary>
    /// Wrapper for <see cref="MatchesEscaped(char, int, out int)"/>.
    /// </summary>
    private bool MatchesEscaped(char ch, ref int offset) =>
        this.MatchesEscaped(ch, offset, out offset);

    /// <summary>
    /// Tries to match an escaped character in the input.
    /// </summary>
    /// <param name="ch">The character to match.</param>
    /// <param name="offset">The offset to match at.</param>
    /// <param name="nextOffset">The offset gets written here after the match.</param>
    /// <returns>True, if the character matches.</returns>
    private bool MatchesEscaped(char ch, int offset, out int nextOffset)
    {
        if (this.TryParseEscaped(out var got, offset, out var offset2) && got == ch)
        {
            nextOffset = offset2;
            return true;
        }
        else
        {
            nextOffset = offset;
            return false;
        }
    }

    /// <summary>
    /// Wrapper for <see cref="ParseEscaped(int, out int, char)"/>.
    /// </summary>
    private char ParseEscaped(ref int offset, char @default = '\0') =>
        this.ParseEscaped(offset, out offset, @default);

    /// <summary>
    /// Parses an escaped character from the input.
    /// </summary>
    /// <param name="offset">The offset to start the parse from.</param>
    /// <param name="nextOffset">The offset gets written here after the parse.</param>
    /// <param name="default">The default character that gets returned if the end of input is reached.</param>
    /// <returns>The parsed character, or <paramref name="default"/>, if the end of input is reached.</returns>
    private char ParseEscaped(int offset, out int nextOffset, char @default = '\0') =>
        this.TryParseEscaped(out var ch, offset, out nextOffset) ? ch : @default;

    /// <summary>
    /// Wrapper for <see cref="TryParseEscaped(out char, int, out int)"/>.
    /// </summary>
    private bool TryParseEscaped(out char result, ref int offset) =>
        this.TryParseEscaped(out result, offset, out offset);

    /// <summary>
    /// Tries to parse the next escaped character from the input.
    /// Escaped means line-continuated - if enabled - and trighraph-converted - also if enabled.
    /// </summary>
    /// <param name="result">The resulting escaped character is written here.</param>
    /// <param name="offset">The offset to start the parse from.</param>
    /// <param name="nextOffset">The offset that ends the character is written here.</param>
    /// <returns>True, if there was a character to parse, false otherwise.</returns>
    private bool TryParseEscaped(out char result, int offset, out int nextOffset)
    {
    begin:
        // If there is no character, just return immediately
        if (!this.source.TryLookAhead(offset, out var ch))
        {
            result = default;
            nextOffset = offset;
            return false;
        }

        if (this.AllowLineContinuations)
        {
            // Line-continuations are enabled, if there is a '\', means a potential line-continuation
            var backslashLength = 0;
            if (ch == '\\') backslashLength = 1;
            else if (this.AllowTrigraphs && this.source.Matches("??/", offset)) backslashLength = 3;

            // A length > 0 means that there was a '\' or equivalent trigraph
            if (backslashLength > 0)
            {
                var length = backslashLength;
                // Consume whitespace, we allow trailing spaces before the newline
                for (; IsSpace(this.source.LookAhead(offset + length)); ++length)
                {
                    // Pass
                }
                // Now we expect a newline
                var newline = 0;
                if (this.source.Matches("\r\n", offset + length)) newline = 2;
                else if (this.source.Matches('\r', offset + length) || this.source.Matches('\n', offset + length)) newline = 1;
                // If we got a newline, it's a line-continuation
                if (newline > 0)
                {
                    // It's a line-continuation, retry after consume
                    offset += length + newline;
                    goto begin;
                }
                // Otherwise we just eat the backslash
                result = '\\';
                nextOffset = offset + backslashLength;
                return true;
            }
        }

        // It could be a trigraph
        if (this.AllowTrigraphs && this.source.Matches("??", offset))
        {
            char? trigraph = this.source.TryLookAhead(offset + 2, out var ch2) ? ch2 switch
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
            } : null;
            if (trigraph is not null)
            {
                // It was a trigraph
                result = trigraph.Value;
                nextOffset = offset + 3;
                return true;
            }
        }

        // Out of ideas, just return the character raw
        result = ch;
        nextOffset = offset + 1;
        return true;
    }

    /// <summary>
    /// Constructs a new <see cref="CToken"/>.
    /// </summary>
    /// <param name="type">The <see cref="CTokenType"/>.</param>
    /// <param name="length">The physical length to consume.</param>
    /// <param name="logicalText">The logical (escaped) text of the token.</param>
    /// <returns>The constructed <see cref="CToken"/>.</returns>
    private CToken TakeToken(CTokenType type, int length, string logicalText)
    {
        // Construct the logical range
        var startPosition = this.LogicalPosition;
        this.LogicalPosition = this.LogicalPosition.Advance(logicalText.Length);
        var logicalRange = new Text.Range(startPosition, this.LogicalPosition);
        // Construct the token
        return this.source.ConsumeToken<CToken>(length, (range, text) => new(range, text, logicalRange, logicalText, type));
    }

    private bool IsIdent(char ch) => char.IsDigit(ch) || ch == '_'
        || (this.AllowUnicodeCharacters ? char.IsLetter(ch) : (ch >= 'a' && ch <= 'z') || (ch >= 'A' && ch <= 'Z'));

    private static bool IsSpace(char ch) => ch == ' ' || ch == '\0';

    private static bool IsNewline(char ch) => ch == '\n' || ch == '\r';

    private static bool IsHex(char ch) => "0123456789abcdefABCDEF".Contains(ch);

    private static bool IsX(char ch) => "xX".Contains(ch);

    private static bool IsE(char ch) => "eE".Contains(ch);

    private static bool IsSign(char ch) => "+-".Contains(ch);

    private static bool IsFloatSuffix(char ch) => "fFlL".Contains(ch);

    private static bool IsIntSuffix(char ch) => "uUlL".Contains(ch);

    private static bool IsLiteralSeparator(char ch) => "'\"".Contains(ch);
}
