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
        var offset1 = offset;
        if (this.Matches('\\', ref offset1))
        {
            // Anything that starts with a '\'
            // Any single character is accepted, BUT there are special cases:
            //  - \c<ASCII>
            //  - \p{...}
            //  - \P{...}
            //  - \[0-3][0-7][0-7] or \[0-7][0-7]
            //  - \x[0-9a-fA-F][0-9a-fA-F] or \x{[0-9a-fA-F][0-9a-fA-F][0-9a-fA-F]+}
            //  - \Q...\E (block-quoted)
            if (!this.Take(ref offset1, out var escaped)) this.Error(offset, "expected escaped character after '\', but found end of text");
            switch (escaped)
            {
            case 'c':
            {
                // Control character
                if (!this.Matches(IsAscii, ref offset1, out var asciiCh)) break;
                offset = offset1;
                return this.ConstructControlChar(asciiCh);
            }

            case 'p':
            case 'P':
            {
                // Character with or without property
                static bool IsPropChar(char ch) =>
                       ch == '_'
                    || ch is >= 'a' and <= 'z'
                    || ch is >= 'A' and <= 'Z'
                    || ch is >= '0' and <= '9';

                var offset2 = offset1;
                if (!this.Matches('{', ref offset2)) break;
                var prop = this.TakeWhile(IsPropChar, ref offset2);
                if (prop.Length == 0) break;
                if (!this.Matches('}', ref offset2)) break;
                offset = offset2;
                return this.ConstructCharWithProp(escaped == 'P', prop);
            }

            case 'x':
            {
                // Hex code for character
                var offset2 = offset1;
                string hex;
                if (this.Matches('{', ref offset2))
                {
                    // At least 3 hex chars
                    hex = this.TakeWhile(IsHexDigit, ref offset2);
                    if (hex.Length < 3) break;
                    if (!this.Matches('}', ref offset2)) break;
                }
                else
                {
                    if (!this.Matches(IsHexDigit, ref offset2, out var h1)) break;
                    if (!this.Matches(IsHexDigit, ref offset2, out var h2)) break;
                    hex = $"{h1}{h2}";
                }
                offset = offset2;
                return RegEx.Literal((char)int.Parse(hex, NumberStyles.HexNumber));
            }

            case >= '0' and <= '7':
            {
                // Octal code for character
                var offset2 = offset1;
                var allow3 = escaped <= '3';
                string oct;
                if (!this.Matches(IsOctDigit, ref offset2, out var o2)) break;
                if (allow3 && this.Matches(IsOctDigit, ref offset2, out var o3)) oct = $"{escaped}{o2}{o3}";
                else oct = $"{escaped}{o2}";
                offset = offset2;
                // NOTE: Ew, Convert API
                return RegEx.Literal((char)Convert.ToInt32(oct, 8));
            }

            case 'Q':
            {
                // Quoted, terminates with '\E'
                var offset2 = offset1;
                var quotedText = new StringBuilder();
                while (true)
                {
                    if (!this.Take(ref offset2, out var ch)) goto after_switch;
                    if (ch == '\\' && this.Matches('E', ref offset2)) break;
                    quotedText.Append(ch);
                }
                offset = offset2;
                return RegEx.Sequence(quotedText.ToString().Select(RegEx.Literal));
            }

            default:
                break;
            }
        after_switch:
            // We assume it's a single-char escape
            offset = offset1;
            var result = this.ConstructSingleCharEscape(escaped, offset);
            return result;
        }
        else if (this.Matches('(', ref offset1))
        {
            var offset2 = offset1;
            var sub = this.ParseAlternation(ref offset2);
            if (this.Matches(')', ref offset2))
            {
                offset = offset2;
                return sub;
            }
        }
        else if (this.Matches('[', ref offset1))
        {
            // TODO: Character classes
            throw new NotImplementedException();
        }
        // Consume as a single character
        if (!this.Take(ref offset, out var ch)) this.Error(offset, "character expected");
        return RegEx.Literal(ch);
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

    private RegExNode<char> ConstructSingleCharEscape(char ch, int offset) => ch switch
    {
        _ => this.Error<RegExNode<char>>(offset, $"unknown escape sequence '\\{ch}'"),
    };

    private RegExNode<char> ConstructCharWithProp(bool negate, string prop) =>
        throw new NotImplementedException("Character properties are not implemented yet");

    private RegExNode<char> ConstructControlChar(char asciiCh) =>
        throw new NotImplementedException("Control chars are not supported yet");

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

    private bool Matches(char ch, ref int offset) =>
        this.Matches(c => c == ch, ref offset, out _);

    private bool Take(ref int offset, out char ch) =>
        this.Matches(_ => true, ref offset, out ch);

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

    private static bool IsAscii(char ch) => ch >= '\x0' && ch <= '\x7f';
    private static bool IsOctDigit(char ch) => ch is >= '0' and <= '7';
    private static bool IsHexDigit(char ch) =>
           ch is >= '0' and <= '9'
        || ch is >= 'a' and <= 'f'
        || ch is >= 'A' and <= 'F';
}
