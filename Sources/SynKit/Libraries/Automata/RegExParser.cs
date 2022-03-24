// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Yoakke.SynKit.Automata;

/// <summary>
/// A regular expression parser.
/// </summary>
public sealed class RegExParser
{
    /// <summary>
    /// The regex settings.
    /// </summary>
    public RegExSettings Settings { get; set; } = RegExSettings.Default;

    /// <summary>
    /// Attempts to parse a regular expression.
    /// </summary>
    /// <param name="text">The regex text to parse.</param>
    /// <param name="node">The resulting regex AST.</param>
    /// <returns>True, if the parse was succesful.</returns>
    public bool TryParse(string text, [MaybeNullWhen(false)] out RegExNode<char> node)
    {
        try
        {
            node = this.Parse(text);
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
    public RegExNode<char> Parse(string text)
    {
        var offset = 0;
        return this.Parse(text, ref offset);
    }

    private RegExNode<char> Parse(string text, ref int offset)
    {
        var result = this.ParseAlternation(text, ref offset);
        if (offset != text.Length) throw new RegExParseException(text, $"did not consume until EOF [stuck at index {offset}]");
        return result;
    }

    private RegExNode<char> ParseAlternation(string text, ref int offset)
    {
        var result = this.ParseSeq(text, ref offset);
        while (Matches(text, '|', ref offset))
        {
            var right = this.ParseSeq(text, ref offset);
            result = RegEx.Alternation(right);
        }
        return result;
    }

    private RegExNode<char> ParseSeq(string text, ref int offset)
    {
        RegExNode<char>? result = null;
        while (true)
        {
            var e = this.ParseElement(text, ref offset);
            if (e is null) break;
            if (result is null) result = e;
            else result = RegEx.Sequence(result, e);
        }
        return result ?? RegEx.Empty<char>();
    }

    private RegExNode<char>? ParseElement(string text, ref int offset)
    {
        if (offset >= text.Length || text[offset] == '|') return null;
        var atom = this.ParseAtom(text, ref offset);
        return this.ParseQuantifier(text, atom, ref offset);
    }

    private RegExNode<char> ParseAtom(string text, ref int offset)
    {
        throw new NotImplementedException();
    }

    private RegExNode<char> ParseQuantifier(string text, RegExNode<char> atom, ref int offset)
    {
        // Trivial quantifiers
        if (Matches(text, '?', ref offset)) return RegEx.Option(atom);
        if (Matches(text, '+', ref offset)) return RegEx.Repeat1(atom);
        if (Matches(text, '*', ref offset)) return RegEx.Repeat0(atom);
        // More complex quantifiers, we can't commit yet
        var offset1 = offset;
        if (!Matches(text, '{', ref offset1)) return atom;
        // Open brace, we try to parse a quantifier
        // - { number } => exactly
        // - { number, } => at least
        // - { , number } => at most
        // - { number , number } => between
        if (Matches(text, ',', ref offset1))
        {
            // Only { , number } is possible here
            var n = ParseNumber(text, ref offset1);
            if (n is null || !Matches(text, '}', ref offset1)) return atom;
            // Success
            offset = offset1;
            return RegEx.AtMost(atom, n.Value);
        }
        // { number }, { number, }, { number , number } are possible
        var atLeast = ParseNumber(text, ref offset1);
        if (atLeast is null) return atom;
        if (Matches(text, '}', ref offset1))
        {
            // Exact
            offset = offset1;
            return RegEx.Exactly(atom, atLeast.Value);
        }
        // Only { number, } and { number , number }
        if (!Matches(text, ',', ref offset1)) return atom;
        if (Matches(text, '}', ref offset1))
        {
            // At least
            offset = offset1;
            return RegEx.AtLeast(atom, atLeast.Value);
        }
        var atMost = ParseNumber(text, ref offset1);
        if (atMost is null || !Matches(text, '}', ref offset1)) return atom;
        // Between
        offset = offset1;
        return RegEx.Between(atom, atLeast.Value, atMost.Value);
    }

    private static int? ParseNumber(string text, ref int offset)
    {
        var intText = MatchWhile(text, char.IsDigit, ref offset);
        if (intText.Length == 0) return null;
        return int.Parse(intText);
    }

    private static string MatchWhile(string text, Predicate<char> predicate, ref int offset)
    {
        var sb = new StringBuilder();
        for (; offset < text.Length && predicate(text[offset]); sb.Append(text[offset++])) ;
        return sb.ToString();
    }

    private static bool Matches(string text, char ch, ref int offset)
    {
        if (offset >= text.Length || text[offset] != ch) return false;
        ++offset;
        return true;
    }
}
