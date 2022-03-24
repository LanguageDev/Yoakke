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
        throw new NotImplementedException();
    }

    private static bool Matches(string text, char ch, ref int offset)
    {
        if (offset >= text.Length || text[offset] != ch) return false;
        ++offset;
        return true;
    }
}
