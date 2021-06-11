using System;
using System.Collections.Generic;

namespace Yoakke.Parser.Generator.Syntax
{
    /// <summary>
    /// The lexer for BNF grammar, producing <see cref="BnfToken"/>s.
    /// </summary>
    internal class BnfLexer
    {
        public static IList<BnfToken> Lex(string source)
        {
            var result = new List<BnfToken>();
            var lexer = new BnfLexer(source);
            while (true)
            {
                var t = lexer.Next();
                result.Add(t);
                if (t.Type == BnfTokenType.End) break;
            }
            return result;
        }

        private readonly string source;
        private int index;

        public BnfLexer(string source)
        {
            this.source = source;
        }

        public BnfToken Next()
        {
            begin:
            if (this.index >= this.source.Length) return new BnfToken(this.index, string.Empty, BnfTokenType.End);

            var ch = this.source[this.index];
            if (ch == ' ' || ch == '\t')
            {
                ++this.index;
                goto begin;
            }

            switch (ch)
            {
            case ':': return this.Make(1, BnfTokenType.Colon);
            case '|': return this.Make(1, BnfTokenType.Pipe);
            case '*': return this.Make(1, BnfTokenType.Star);
            case '+': return this.Make(1, BnfTokenType.Plus);
            case '?': return this.Make(1, BnfTokenType.QuestionMark);
            case '(': return this.Make(1, BnfTokenType.OpenParen);
            case ')': return this.Make(1, BnfTokenType.CloseParen);
            }

            // String literal
            if (ch == '\'')
            {
                int length = 1;
                for (; this.Peek(length, '\'') != '\''; ++length) ;
                if (this.index + length >= this.source.Length) throw new FormatException($"Unclosed string literal starting at index {this.index}");
                // We consume the last ' too
                ++length;
                return this.Make(length, BnfTokenType.StringLiteral);
            }

            // Identifier
            if (IsIdent(ch))
            {
                int length = 1;
                for (; IsIdent(this.Peek(length)); ++length) ;
                return this.Make(length, BnfTokenType.Identifier);
            }

            throw new FormatException($"Unknown character '{this.source[this.index]}' at index {this.index}");
        }

        private BnfToken Make(int length, BnfTokenType type)
        {
            var value = this.source.Substring(this.index, length);
            var token = new BnfToken(this.index, value, type);
            this.index += length;
            return token;
        }

        private char Peek(int offset = 0, char @default = '\0') =>
            this.index + offset >= this.source.Length ? @default : this.source[this.index + offset];

        private static bool IsIdent(char ch) => char.IsLetterOrDigit(ch) || ch == '_';
    }
}
