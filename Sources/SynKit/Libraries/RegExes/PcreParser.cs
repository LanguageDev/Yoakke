// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Yoakke.SynKit.RegExes;

/// <summary>
/// A PCRE parser.
/// </summary>
public sealed class PcreParser
{
    /// <summary>
    /// Attempts to parse a PCRE.
    /// </summary>
    /// <param name="text">The regex text to parse.</param>
    /// <param name="node">The resulting regex AST.</param>
    /// <returns>True, if the parse was succesful.</returns>
    public static bool TryParse(string text, [MaybeNullWhen(false)] out PcreAst node) =>
        TryParse(RegExSettings.Default, text, out node);

    /// <summary>
    /// Attempts to parse a PCRE.
    /// </summary>
    /// <param name="settings">The settings to use.</param>
    /// <param name="text">The regex text to parse.</param>
    /// <param name="node">The resulting regex AST.</param>
    /// <returns>True, if the parse was succesful.</returns>
    public static bool TryParse(
        RegExSettings settings,
        string text,
        [MaybeNullWhen(false)] out PcreAst node)
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
    /// Parses a PCRE.
    /// </summary>
    /// <param name="text">The regex text to parse.</param>
    /// <returns>The parsed regex AST.</returns>
    public static PcreAst Parse(string text) => Parse(RegExSettings.Default, text);

    /// <summary>
    /// Parses a PCRE.
    /// </summary>
    /// <param name="settings">The settings to use.</param>
    /// <param name="text">The regex text to parse.</param>
    /// <returns>The parsed regex AST.</returns>
    public static PcreAst Parse(RegExSettings settings, string text)
    {
        var parser = new PcreParser(settings, text);
        return parser.Parse();
    }

    private readonly RegExSettings settings;
    private readonly string text;

    private PcreParser(RegExSettings settings, string text)
    {
        this.settings = settings;
        this.text = text;
    }

    private PcreAst Parse()
    {
        var offset = 0;
        var result = this.ParseAlternation(ref offset);
        if (offset != this.text.Length) this.Error(offset, "did not consume until EOF");
        return result;
    }

    private PcreAst ParseAlternation(ref int offset)
    {
        var alternatives = new List<PcreAst>();
        alternatives.Add(this.ParseSeq(ref offset));
        while (this.Matches('|', ref offset)) alternatives.Add(this.ParseSeq(ref offset));
        return alternatives.Count == 1
            ? alternatives[0]
            : new PcreAst.Alternation(alternatives);
    }

    private PcreAst ParseSeq(ref int offset)
    {
        var elements = new List<PcreAst>();
        while (true)
        {
            var e = this.ParseElement(ref offset);
            if (e is null) break;
            elements.Add(e);
        }
        return elements.Count == 1
            ? elements[0]
            : new PcreAst.Sequence(elements);
    }

    private PcreAst? ParseElement(ref int offset)
    {
        if (offset >= this.text.Length || this.text[offset] is '|' or ')') return null;
        var result = this.ParseAtom(ref offset);
        var quantifier = this.ParseQuantifier(ref offset);
        if (quantifier is not null) result = new PcreAst.Quantified(result, quantifier);
        return result;
    }

    private PcreAst ParseAtom(ref int offset)
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
        if (this.Matches(']', ref offset)) return new PcreAst.Literal(']');
        if (this.Matches('.', ref offset)) return new PcreAst.MetaSequence(".");
        // NOTE: Order matters here
        if (this.ParseCharacterClass(ref offset, out var result)) return result;
        if (this.ParseSharedAtom(ref offset, out result)) return result;
        if (this.ParseSharedLiteral(ref offset, out result)) return result;

        // TODO: ^, if we even want to handle it
        this.Error(offset, "unexpected input");
        return null!;
    }

    private Quantifier? ParseQuantifier(ref int offset)
    {
        // Trivial quantifiers
        if (this.Matches('?', ref offset)) return Quantifier.Optional.Instance;
        if (this.Matches('+', ref offset)) return Quantifier.OneOrMore.Instance;
        if (this.Matches('*', ref offset)) return Quantifier.ZeroOrMore.Instance;
        // More complex quantifiers, we can't commit yet
        var offset1 = offset;
        if (!this.Matches('{', ref offset1)) return null;
        // Open brace, we try to parse a quantifier
        // - { number } => exactly
        // - { number, } => at least
        // - { , number } => at most
        // - { number , number } => between
        if (this.Matches(',', ref offset1))
        {
            // Only { , number } is possible here
            if (!this.ParseNumber(ref offset1, out var n)) return null;
            if (!this.Matches('}', ref offset1)) return null;
            // Success
            offset = offset1;
            return new Quantifier.AtMost(n);
        }
        // { number }, { number, }, { number , number } are possible
        if (!this.ParseNumber(ref offset1, out var atLeast)) return null;
        if (this.Matches('}', ref offset1))
        {
            // Exact
            offset = offset1;
            return new Quantifier.Exactly(atLeast);
        }
        // Only { number, } and { number , number }
        if (!this.Matches(',', ref offset1)) return null;
        if (this.Matches('}', ref offset1))
        {
            // At least
            offset = offset1;
            return new Quantifier.AtLeast(atLeast);
        }
        if (!this.ParseNumber(ref offset1, out var atMost)) return null;
        if (!this.Matches('}', ref offset1)) return null;
        // Between
        offset = offset1;
        return new Quantifier.Between(atLeast, atMost);
    }

    private bool ParseCharacterClass(
        ref int offset,
        [MaybeNullWhen(false)] out PcreAst result)
    {
        // Can be
        //  - [^]-cc_atom+]
        //  - [^]cc_atom*]
        //  - [^cc_atom+]
        //  - []-cc_atom+]
        //  - []cc_atom*]
        //  - [cc_atom+]
        result = null;
        var offset1 = offset;
        if (!this.Matches('[', ref offset1)) return false;
        var invert = this.Matches('^', ref offset1);
        var elements = new List<PcreAst>();
        if (this.Matches(']', ref offset1))
        {
            // Can be
            //  - [^]-cc_atom+]
            //  - [^]cc_atom*]
            //  - []-cc_atom+]
            //  - []cc_atom*]
            // and we are after ]
            if (this.Matches('-', ref offset1))
            {
                if (!this.ParseCharacterClassLiteral(ref offset1, out var to)) return false;
                if (to is not PcreAst.Literal toLit) return false;
                elements.Add(new PcreAst.CharacterClassRange(']', toLit.Char));
            }
            else
            {
                elements.Add(new PcreAst.Literal(']'));
            }
            while (offset1 < this.text.Length && !this.Matches(']', ref offset1))
            {
                if (!this.ParseCharacterClassAtom(ref offset1, out var atom)) return false;
                elements.Add(atom);
            }
        }
        else
        {
            // Can be
            //  - [^cc_atom+]
            //  - [cc_atom+]
            while (offset1 < this.text.Length && !this.Matches(']', ref offset1))
            {
                if (!this.ParseCharacterClassAtom(ref offset1, out var atom)) return false;
                elements.Add(atom);
            }
            if (elements.Count == 0) return false;
        }
        offset = offset1;
        result = new PcreAst.CharacterClass(invert, elements);
        return true;
    }

    private bool ParseCharacterClassAtom(
        ref int offset,
        [MaybeNullWhen(false)] out PcreAst result)
    {
        if (!this.ParseCharacterClassLiteral(ref offset, out result)) return false;
        var offset1 = offset;
        // Singleton result, if there is no '-'
        if (!this.Matches('-', ref offset1)) return true;
        // If there is a character class literal, there's a chance it's a range
        if (!this.ParseCharacterClassLiteral(ref offset1, out var to)) return true;
        // Try to extract range literals
        if (!TryGetCharForCcLiteralRange(result, out var lFrom)
         || !TryGetCharForCcLiteralRange(to, out var lTo)) return true;
        // They are a range
        result = new PcreAst.CharacterClassRange(lFrom, lTo);
        offset = offset1;
        return true;
    }

    private static bool TryGetCharForCcLiteralRange(PcreAst ast, out char lit)
    {
        if (ast is PcreAst.Literal l)
        {
            lit = l.Char;
            return true;
        }
        if (ast is PcreAst.Quoted q && q.Text.Length > 0)
        {
            lit = q.Text[0];
            return true;
        }

        lit = default;
        return false;
    }

    private bool ParseCharacterClassLiteral(
        ref int offset,
        [MaybeNullWhen(false)] out PcreAst result)
    {
        if (this.ParseSharedAtom(ref offset, out result)) return true;
        if (this.ParseSharedLiteral(ref offset, out result)) return true;
        return false;
    }

    private bool ParseSharedAtom(
        ref int offset,
        [MaybeNullWhen(false)] out PcreAst result)
    {
        result = null;
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
                // TODO
                result = (PcreAst)null! ?? throw new NotImplementedException();
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
                result = new PcreAst.CharProperty(escaped == 'P', prop);
                offset = offset2;
                return true;
            }
            }

            // Single-character
            // NOTE: This conversion can fail
            if (this.settings.MetaSequences.TryGetValue($"\\{escaped}", out var seq))
            {
                result = new PcreAst.Desugared(seq);
                offset = offset1;
                return true;
            }
            result = null;
            return false;
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
            result = new PcreAst.NamedCharacterClass(negate, setName);
            offset = offset1;
            return true;
        }
    not_atom:
        result = null;
        return false;
    }

    private bool ParseSharedLiteral(
        ref int offset,
        [MaybeNullWhen(false)] out PcreAst result)
    {
        result = null;
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
                result = new PcreAst.Literal((char)int.Parse(hex, NumberStyles.HexNumber));
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
                result = new PcreAst.Quoted(quotedText.ToString());
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
                result = new PcreAst.Literal((char)Convert.ToInt32(oct, 8));
                offset = offset2;
                return true;
            }

            case 'a' or 'e' or 'f' or 'r' or 'n' or 't':
            {
                result = new PcreAst.Literal(this.settings.Escapes[escaped]);
                offset = offset1;
                return true;
            }

            case char ch when !IsAsciiAlnum(ch):
            {
                // Quoted
                result = new PcreAst.Literal(ch);
                offset = offset1;
                return true;
            }

            default:
                goto not_escaped;
            }
        }
    not_escaped:
        // Otherwise we just consume a single character
        offset1 = offset;
        if (!this.Take(ref offset1, out var ch1)) return false;
        // We don't allow '(' and ')' by themselves
        if (ch1 is '(' or ')') return false;
        // There is a character, consume it literally
        result = new PcreAst.Literal(ch1);
        offset = offset1;
        return true;
    }

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
