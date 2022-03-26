// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// A regular expression parser.
/// </summary>
public sealed class RegExParser
{
    /// <summary>
    /// Attempts to parse a regular expression.
    /// </summary>
    /// <param name="text">The regex text to parse.</param>
    /// <param name="node">The resulting regex AST.</param>
    /// <returns>True, if the parse was succesful.</returns>
    public static bool TryParse(string text, [MaybeNullWhen(false)] out RegExNode<char> node) =>
        TryParse(RegExSettings.Default, text, out node);

    /// <summary>
    /// Attempts to parse a regular expression.
    /// </summary>
    /// <param name="settings">The settings to use.</param>
    /// <param name="text">The regex text to parse.</param>
    /// <param name="node">The resulting regex AST.</param>
    /// <returns>True, if the parse was succesful.</returns>
    public static bool TryParse(
        RegExSettings settings,
        string text,
        [MaybeNullWhen(false)] out RegExNode<char> node)
    {
        try
        {
            node = Parse(settings, text);
            return true;
        }
        catch (RegExParseException)
        {
            node = null;
            return false;
        }
    }

    /// <summary>
    /// Parses a regular expression.
    /// </summary>
    /// <param name="text">The regex text to parse.</param>
    /// <returns>The parsed regex AST.</returns>
    public static RegExNode<char> Parse(string text) => Parse(RegExSettings.Default, text);

    /// <summary>
    /// Parses a regular expression.
    /// </summary>
    /// <param name="settings">The settings to use.</param>
    /// <param name="text">The regex text to parse.</param>
    /// <returns>The parsed regex AST.</returns>
    public static RegExNode<char> Parse(RegExSettings settings, string text)
    {
        var parser = new RegExParser(settings, text);
        return parser.Parse();
    }

    private readonly RegExSettings settings;
    private readonly string text;

    private RegExParser(RegExSettings settings, string text)
    {
        this.settings = settings;
        this.text = text;
    }

    private RegExNode<char> Parse()
    {
        var offset = 0;
        var result = this.ParseAlternation(ref offset);
        if (offset != this.text.Length) this.Error(offset, "did not consume until EOF");
        return result;
    }

    private RegExNode<char> ParseAlternation(ref int offset)
    {
        var result = this.ParseSeq(ref offset);
        while (this.Matches('|', ref offset))
        {
            var right = this.ParseSeq(ref offset);
            result = RegEx.Alternation(right);
        }
        return result;
    }

    private RegExNode<char> ParseSeq(ref int offset)
    {
        RegExNode<char>? result = null;
        while (true)
        {
            var e = this.ParseElement(ref offset);
            if (e is null) break;
            if (result is null) result = e;
            else result = RegEx.Sequence(result, e);
        }
        return result ?? RegEx.Empty<char>();
    }

    private RegExNode<char>? ParseElement(ref int offset)
    {
        if (offset >= this.text.Length || this.text[offset] == '|') return null;
        var atom = this.ParseAtom(ref offset);
        return this.ParseQuantifier(atom, ref offset);
    }

    private RegExNode<char> ParseAtom(ref int offset)
    {
        // Try grouping first
        var offset1 = offset;
        if (this.Matches('(', ref offset1))
        {
            var alt = this.ParseAlternation(ref offset1);
            if (this.Matches(')', ref offset1))
            {
                offset = offset1;
                return alt;
            }
        }
        // Then the rest
        if (this.Matches(']', ref offset)) return RegEx.Literal(']');
        if (this.Matches('.', ref offset)) return this.ConstructDotMatcher();
        RegExNode<char>? result;
        // NOTE: Order matters here
        if (this.ParseCharacterClass(ref offset, out result)) return result;
        if (this.ParseSharedAtom(ref offset, out result)) return result;
        if (this.ParseSharedLiteral(ref offset, out result)) return result;

        // TODO: ^, if we even want to handle it
        this.Error(offset, "unexpected input");
        return null!;
    }

    private RegExNode<char> ParseQuantifier(RegExNode<char> atom, ref int offset)
    {
        // Trivial quantifiers
        if (this.Matches('?', ref offset)) return RegEx.Option(atom);
        if (this.Matches('+', ref offset)) return RegEx.Repeat1(atom);
        if (this.Matches('*', ref offset)) return RegEx.Repeat0(atom);
        // More complex quantifiers, we can't commit yet
        var offset1 = offset;
        if (!this.Matches('{', ref offset1)) return atom;
        // Open brace, we try to parse a quantifier
        // - { number } => exactly
        // - { number, } => at least
        // - { , number } => at most
        // - { number , number } => between
        if (this.Matches(',', ref offset1))
        {
            // Only { , number } is possible here
            if (!this.ParseNumber(ref offset1, out var n)) return atom;
            if (!this.Matches('}', ref offset1)) return atom;
            // Success
            offset = offset1;
            return RegEx.AtMost(atom, n);
        }
        // { number }, { number, }, { number , number } are possible
        if (!this.ParseNumber(ref offset1, out var atLeast)) return atom;
        if (this.Matches('}', ref offset1))
        {
            // Exact
            offset = offset1;
            return RegEx.Exactly(atom, atLeast);
        }
        // Only { number, } and { number , number }
        if (!this.Matches(',', ref offset1)) return atom;
        if (this.Matches('}', ref offset1))
        {
            // At least
            offset = offset1;
            return RegEx.AtLeast(atom, atLeast);
        }
        if (!this.ParseNumber(ref offset1, out var atMost)) return atom;
        if (!this.Matches('}', ref offset1)) return atom;
        // Between
        offset = offset1;
        return RegEx.Between(atom, atLeast, atMost);
    }

    private bool ParseCharacterClass(
        ref int offset,
        [MaybeNullWhen(false)] out RegExNode<char> result)
    {
        // TODO
        throw new NotImplementedException();
    }

    private bool ParseSharedAtom(
        ref int offset,
        [MaybeNullWhen(false)] out RegExNode<char> result)
    {
        var offset1 = offset;
        if (this.Matches('\\', ref offset1))
        {
            // Could be
            //  - \c<ASCII>
            //  - \p{property} or \P{property}
            //  - \<ANYTHING>

            if (!this.Take(ref offset1, out var escaped)) goto not_atom;

            switch (escaped)
            {
            case 'c':
            {
                // Control character
                if (!this.Matches(IsAscii, ref offset1, out var asciiCh)) break;
                result = this.ConstructControlChar(offset, asciiCh);
                offset = offset1;
                return true;
            }

            case 'p' or 'P':
            {
                // Character with or without property
                static bool IsPropChar(char ch) => IsAsciiAlnum(ch) || ch == '_';

                var offset2 = offset1;
                if (!this.Matches('{', ref offset2)) break;
                var prop = this.TakeWhile(IsPropChar, ref offset2);
                if (prop.Length == 0) break;
                if (!this.Matches('}', ref offset2)) break;
                result = this.ConstructCharWithProp(offset, escaped == 'P', prop);
                offset = offset2;
                return true;
            }
            }

            // Single-character
            // NOTE: This conversion can fail
            result = this.ConstructEscapedAtom(offset, escaped);
            if (result is null) return false;
            offset = offset1;
            return true;
        }
        else
        {
            // Could be
            //  - [[:classname:]]
            //  - [[:^classname:]]

            if (!this.Matches("[[:", ref offset1)) goto not_atom;
            var negate = this.Matches('^', ref offset1);
            var setName = this.TakeWhile(IsAsciiAlnum, ref offset1);
            if (setName.Length == 0) goto not_atom;
            if (!this.Matches(":]]", ref offset1)) goto not_atom;
            result = this.ConstructPosixNamedSet(offset, negate, setName);
            offset = offset1;
            return true;
        }
    not_atom:
        result = null;
        return false;
    }

    private bool ParseSharedLiteral(
        ref int offset,
        [MaybeNullWhen(false)] out RegExNode<char> result)
    {
        var offset1 = offset;
        if (this.Matches('\\', ref offset1))
        {
            // Escaped, could be
            //  - octal escaped char
            //  - hex escaped char
            //  - \~[a-zA-Z0-9]
            //  - \Q.*?\E
            //  - \a, \e, \f, \r, \n, \t

            if (!this.Take(ref offset1, out var escaped)) goto not_escaped;

            switch (escaped)
            {
            case 'x':
            {
                // Hex code for character
                var offset2 = offset1;
                string hex;
                if (this.Matches('{', ref offset2))
                {
                    // At least 3 hex chars
                    hex = this.TakeWhile(IsHexDigit, ref offset2);
                    if (hex.Length < 3) goto not_escaped;
                    if (!this.Matches('}', ref offset2)) goto not_escaped;
                }
                else
                {
                    if (!this.Matches(IsHexDigit, ref offset2, out var h1)) goto not_escaped;
                    if (!this.Matches(IsHexDigit, ref offset2, out var h2)) goto not_escaped;
                    hex = $"{h1}{h2}";
                }
                result = RegEx.Literal((char)int.Parse(hex, NumberStyles.HexNumber));
                offset = offset2;
                return true;
            }

            case 'Q':
            {
                // Quoted, terminates with '\E'
                var offset2 = offset1;
                var quotedText = new StringBuilder();
                while (true)
                {
                    if (!this.Take(ref offset2, out var ch)) goto not_escaped;
                    if (ch == '\\' && this.Matches('E', ref offset2)) break;
                    quotedText.Append(ch);
                }
                result = RegEx.Sequence(quotedText.ToString().Select(RegEx.Literal));
                offset = offset2;
                return true;
            }

            case >= '0' and <= '7':
            {
                // Octal code for character
                var offset2 = offset1;
                var allow3 = escaped <= '3';
                string oct;
                if (!this.Matches(IsOctDigit, ref offset2, out var o2)) goto not_escaped;
                if (allow3 && this.Matches(IsOctDigit, ref offset2, out var o3)) oct = $"{escaped}{o2}{o3}";
                else oct = $"{escaped}{o2}";
                // NOTE: Ew, Convert API
                result = RegEx.Literal((char)Convert.ToInt32(oct, 8));
                offset = offset2;
                return true;
            }

            case 'a' or 'e' or 'f' or 'r' or 'n' or 't':
            {
                result = RegEx.Literal(escaped switch
                {
                    'a' => '\a',
                    'e' => (char)0x1b,
                    'f' => '\f',
                    'r' => '\r',
                    'n' => '\n',
                    't' => '\t',
                    _ => throw new InvalidOperationException(),
                });
                offset = offset1;
                return true;
            }

            case char ch when !IsAsciiAlnum(ch):
            {
                // Quoted
                result = RegEx.Literal(ch);
                offset = offset1;
                return true;
            }

            default:
                goto not_escaped;
            }
        }
    not_escaped:
        // Otherwise we just consume a single character
        if (!this.Take(ref offset, out var ch1))
        {
            result = null;
            return false;
        }
        // There is a character, consume it literally
        result = RegEx.Literal(ch1);
        return true;
    }

    private RegExNode<char> ConstructDotMatcher() =>
        throw new NotImplementedException("'.' is not supported yet");

    private RegExNode<char>? ConstructEscapedAtom(int offset, char ch) => ch switch
    {
        _ => throw new NotImplementedException($"\\{ch} is not supported yet"),
    };

    private RegExNode<char> ConstructControlChar(int offset, char ch) => ch switch
    {
        _ => throw new NotImplementedException($"Control character {ch} is not supported yet"),
    };

    private RegExNode<char> ConstructCharWithProp(int offset, bool negate, string prop) =>
        throw new NotImplementedException("Character properties are not supported yet");

    private RegExNode<char> ConstructPosixNamedSet(int offset, bool negate, string setName) =>
        throw new NotImplementedException("Posix named sets are not supported yet");

    private bool ParseNumber(ref int offset, out int number)
    {
        var intText = this.TakeWhile(char.IsDigit, ref offset);
        if (intText.Length == 0)
        {
            number = 0;
            return false;
        }
        else
        {
            number = int.Parse(intText);
            return true;
        }
    }

    private bool Matches(string text, ref int offset)
    {
        if (offset + text.Length > this.text.Length) return false;
        for (var i = 0; i < text.Length; ++i)
        {
            if (text[i] != this.text[offset + i]) return false;
        }
        offset += text.Length;
        return true;
    }

    private bool Matches(char ch, ref int offset)
    {
        if (offset >= this.text.Length || this.text[offset] != ch) return false;
        ++offset;
        return true;
    }

    private bool Take(ref int offset, out char ch)
    {
        if (offset >= this.text.Length)
        {
            ch = default;
            return false;
        }
        else
        {
            ch = this.text[offset++];
            return true;
        }
    }

    private string TakeWhile(Predicate<char> predicate, ref int offset)
    {
        var sb = new StringBuilder();
        while (true)
        {
            if (!this.Matches(predicate, ref offset, out var ch)) break;
            sb.Append(ch);
        }
        return sb.ToString();
    }

    private bool Matches(Predicate<char> predicate, ref int offset, out char result)
    {
        if (offset >= this.text.Length || !predicate(this.text[offset]))
        {
            result = default;
            return false;
        }
        else
        {
            result = this.text[offset++];
            return true;
        }
    }

    [DoesNotReturn]
    private void Error(int offset, string message) =>
        this.Error<int>(offset, message);

    [DoesNotReturn]
    private T Error<T>(int offset, string message) =>
        throw new RegExParseException(this.text, $"{message} [at index {offset}]");

    private static bool IsAscii(char ch) => ch is >= '\x0' and <= '\x7f';
    private static bool IsAsciiAlnum(char ch) =>
           ch is >= 'a' and <= 'z'
        || ch is >= 'A' and <= 'Z'
        || ch is >= '0' and <= '9';
    private static bool IsOctDigit(char ch) => ch is >= '0' and <= '7';
    private static bool IsHexDigit(char ch) =>
           ch is >= '0' and <= '9'
        || ch is >= 'a' and <= 'f'
        || ch is >= 'A' and <= 'F';
}
