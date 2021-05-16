using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yoakke.Collections.RegEx
{
    /// <summary>
    /// A parser that can parse regular expressions into <see cref="RegExAst"/>s.
    /// </summary>
    public class RegExParser
    {
        private string? source;
        private int index;

        private bool IsEnd => source is null || index >= source.Length;

        /// <summary>
        /// Parses a string into a regex AST.
        /// </summary>
        /// <param name="source">The string to parse.</param>
        /// <returns>The resulting regex AST.</returns>
        public RegExAst Parse(string source)
        {
            this.source = source;
            index = 0;
            return ParseAlt();
        }

        private RegExAst ParseAlt()
        {
            var seq = ParseSeq();
            if (Match('|')) return new RegExAst.Alt(seq, ParseAlt());
            return seq;
        }

        private RegExAst ParseSeq()
        {
            var postfx = ParsePostfix();
            if (!IsEnd && Peek() != '|' && Peek() != ')') return new RegExAst.Seq(postfx, ParseSeq());
            return postfx;
        }

        private RegExAst ParsePostfix()
        {
            var atom = ParseAtom();
            if (Match('?')) return new RegExAst.Opt(atom);
            if (Match('*')) return new RegExAst.Rep0(atom);
            if (Match('+')) return new RegExAst.Rep1(atom);
            if (Match('{'))
            {
                var atLeast = ParseNumber();
                if (Match('}')) return new RegExAst.Quantified(atom, atLeast, atLeast);
                if (Match(','))
                {
                    if (Match('}')) return new RegExAst.Quantified(atom, atLeast, null);
                    var atMost = ParseNumber();
                    Expect('}');
                    return new RegExAst.Quantified(atom, atLeast, atMost);
                }
            }
            return atom;
        }

        private RegExAst ParseAtom()
        {
            if (Match('('))
            {
                var sub = ParseAlt();
                Expect(')');
                return sub;
            }
            if (Peek() == '[') return ParseLiteralRange();
            return ParseLiteral();
        }

        private RegExAst ParseLiteralRange()
        {
            Expect('[');
            bool negate = Match('^');
            var ranges = new List<(char From, char To)>();
            ranges.Add(ParseSingleLiteralRange());
            while (!Match(']')) ranges.Add(ParseSingleLiteralRange());
            return new RegExAst.LiteralRange(negate, ranges);
        }

        private (char From, char To) ParseSingleLiteralRange()
        {
            var from = ParseLiteralRangeAtom();
            if (Match('-'))
            {
                var to = ParseLiteralRangeAtom();
                return (from, to);
            }
            return (from, from);
        }

        private char ParseLiteralRangeAtom()
        {
            if (Match('\\'))
            {
                var ch = Consume();
                var escaped = Escape(ch);
                if (escaped == null) throw new FormatException($"invalid escape \\{ch} in grouping (position {index - 2})");
                return escaped.Value;
            }
            return Consume();
        }

        private RegExAst ParseLiteral()
        {
            if (Match('\\'))
            {
                var ch = Consume();
                if (IsSpecial(ch)) return new RegExAst.Literal(ch);
                var escaped = Escape(ch);
                if (escaped == null) throw new FormatException($"invalid escape \\{ch} (position {index - 2})");
                return new RegExAst.Literal(escaped.Value);
            }
            else
            {
                var ch = Consume();
                if (IsSpecial(ch)) throw new FormatException($"special character {ch} must be escaped (position {index - 1})");
                return new RegExAst.Literal(ch);
            }
        }

        private int ParseNumber()
        {
            var result = "";
            for (; char.IsDigit(Peek()); result += Consume()) ;
            if (result.Length == 0) throw new FormatException($"number expected (position {index})");
            return int.Parse(result);
        }

        private char Peek(int offset = 0, char @default = '\0') =>
            index + offset >= source!.Length ? @default : source[index + offset];

        private char Consume()
        {
            if (IsEnd) throw new FormatException("unexpected end of string");

            var ch = Peek();
            ++index;
            return ch;
        }

        private bool Match(char ch)
        {
            if (Peek() == ch)
            {
                Consume();
                return true;
            }
            return false;
        }

        private void Expect(char ch)
        {
            if (!Match(ch)) throw new FormatException($"{ch} expected (position {index})");
        }

        /// <summary>
        /// Checks if a given character is a special character that needs to be escaped for literal use.
        /// </summary>
        /// <param name="ch">The character to check.</param>
        /// <returns>True, if the given character is special, false otherwise.</returns>
        public static bool IsSpecial(char ch) => "()[]{}?*+|".Contains(ch);

        /// <summary>
        /// Escapes the given string to be used as a literal in a regular expression.
        /// </summary>
        /// <param name="str">The string to escape.</param>
        /// <returns>The escaped string.</returns>
        public static string Escape(string str) =>
            string.Join("", str.Select(ch => IsSpecial(ch) ? $"\\{ch}" : ch.ToString()));

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
}
