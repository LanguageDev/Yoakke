// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Yoakke.SynKit.Automata.RegExAst;
using Yoakke.Collections.Intervals;

namespace Yoakke.SynKit.Lexer.Generator;

/// <summary>
/// A parser that can parse regular expressions into <see cref="IRegExNode{Char}"/>s.
/// </summary>
public class RegExParser
{
    private string? source;
    private int index;

    private bool IsEnd => this.source is null || this.index >= this.source.Length;

    /// <summary>
    /// Parses a string into a regex AST.
    /// </summary>
    /// <param name="source">The string to parse.</param>
    /// <returns>The resulting regex AST.</returns>
    public IRegExNode<char> Parse(string source)
    {
        this.source = source;
        this.index = 0;
        return this.ParseAlt();
    }

    private IRegExNode<char> ParseAlt()
    {
        var seq = this.ParseSeq();
        if (this.Match('|')) return RegEx.Or(seq, this.ParseAlt());
        return seq;
    }

    private IRegExNode<char> ParseSeq()
    {
        var postfx = this.ParsePostfix();
        if (!this.IsEnd && this.Peek() != '|' && this.Peek() != ')') return RegEx.Seq(postfx, this.ParseSeq());
        return postfx;
    }

    private IRegExNode<char> ParsePostfix()
    {
        var atom = this.ParseAtom();
        if (this.Match('?')) return RegEx.Opt(atom);
        if (this.Match('*')) return RegEx.Rep0(atom);
        if (this.Match('+')) return RegEx.Rep1(atom);
        if (this.Match('{'))
        {
            if (this.Match(','))
            {
                var atMost = this.ParseNumber();
                this.Expect('}');
                return RegEx.AtMost(atMost, atom);
            }
            else
            {
                var atLeast = this.ParseNumber();
                if (this.Match('}')) return RegEx.Exactly(atLeast, atom);
                if (this.Match(','))
                {
                    if (this.Match('}')) return RegEx.AtLeast(atLeast, atom);
                    var atMost = this.ParseNumber();
                    this.Expect('}');
                    return RegEx.Between(atLeast, atMost, atom);
                }
            }
        }
        return atom;
    }

    private IRegExNode<char> ParseAtom()
    {
        if (this.Match('('))
        {
            var sub = this.ParseAlt();
            this.Expect(')');
            return sub;
        }
        if (this.Peek() == '[') return this.ParseLiteralRange();
        return this.ParseLiteral();
    }

    private IRegExNode<char> ParseLiteralRange()
    {
        this.Expect('[');
        var negate = this.Match('^');
        var ranges = new List<Interval<char>>();
        ranges.Add(this.ParseSingleLiteralRange());
        while (!this.Match(']')) ranges.Add(this.ParseSingleLiteralRange());
        return RegEx.Range(negate, ranges);
    }

    private Interval<char> ParseSingleLiteralRange()
    {
        var from = this.ParseLiteralRangeAtom();
        if (this.Match('-'))
        {
            var to = this.ParseLiteralRangeAtom();
            return new Interval<char>(new LowerBound<char>.Inclusive(from), new UpperBound<char>.Inclusive(to));
        }
        return Interval<char>.Singleton(from);
    }

    private char ParseLiteralRangeAtom()
    {
        if (this.Match('\\'))
        {
            var ch = this.Consume();
            var escaped = Escape(ch);
            if (ch == '-') return ch;
            if (ch == '[') return ch;
            if (ch == ']') return ch;
            if (escaped == null) throw new FormatException($"invalid escape \\{ch} in grouping (position {this.index - 2})");
            return escaped.Value;
        }
        return this.Consume();
    }

    private IRegExNode<char> ParseLiteral()
    {
        if (this.Match('\\'))
        {
            var ch = this.Consume();
            if (IsSpecial(ch)) return RegEx.Lit(ch);
            var escaped = Escape(ch);
            if (escaped == null) throw new FormatException($"invalid escape \\{ch} (position {this.index - 2})");
            return RegEx.Lit(escaped.Value);
        }
        else
        {
            var ch = this.Consume();
            if (IsSpecial(ch)) throw new FormatException($"special character {ch} must be escaped (position {this.index - 1})");
            return RegEx.Lit(ch);
        }
    }

    private int ParseNumber()
    {
        var result = new StringBuilder();
        for (; char.IsDigit(this.Peek()); result.Append(this.Consume()))
        {
            // Blank
        }
        if (result.Length == 0) throw new FormatException($"number expected (position {this.index})");
        return int.Parse(result.ToString());
    }

    private char Peek(int offset = 0, char @default = '\0') =>
        this.index + offset >= this.source!.Length ? @default : this.source[this.index + offset];

    private char Consume()
    {
        if (this.IsEnd) throw new FormatException("unexpected end of string");

        var ch = this.Peek();
        ++this.index;
        return ch;
    }

    private bool Match(char ch)
    {
        if (this.Peek() == ch)
        {
            this.Consume();
            return true;
        }
        return false;
    }

    private void Expect(char ch)
    {
        if (!this.Match(ch)) throw new FormatException($"{ch} expected (position {this.index})");
    }

    /// <summary>
    /// Checks if a given character is a special character that needs to be escaped for literal use.
    /// </summary>
    /// <param name="ch">The character to check.</param>
    /// <returns>True, if the given character is special, false otherwise.</returns>
    public static bool IsSpecial(char ch) => "()[]{}?*+|\\".Contains(ch);

    /// <summary>
    /// Escapes the given string to be used as a literal in a regular expression.
    /// </summary>
    /// <param name="str">The string to escape.</param>
    /// <returns>The escaped string.</returns>
    public static string Escape(string str) =>
        string.Join(string.Empty, str.Select(ch => IsSpecial(ch) ? $"\\{ch}" : ch.ToString()));

    private static char? Escape(char ch) => ch switch
    {
        'n' => '\n',
        'r' => '\r',
        't' => '\t',
        '0' => '\0',
        '\\' => '\\',
        _ => null,
    };
}
