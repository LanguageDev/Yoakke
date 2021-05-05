using System;
using System.Collections.Generic;
using System.Text;

namespace Yoakke.Parser.Generator
{
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

        private string source;
        private int index;

        public BnfLexer(string source)
        {
            this.source = source;
        }

        public BnfToken Next()
        {
            begin:
            if (index >= source.Length) return new BnfToken(index, string.Empty, BnfTokenType.End);

            var ch = source[index];
            if (ch == ' ' || ch == '\t')
            {
                ++index;
                goto begin;
            }

            switch (ch)
            {
            case ':': return Make(1, BnfTokenType.Colon);
            case '|': return Make(1, BnfTokenType.Pipe);
            case '*': return Make(1, BnfTokenType.Star);
            case '+': return Make(1, BnfTokenType.Plus);
            case '?': return Make(1, BnfTokenType.QuestionMark);
            case '(': return Make(1, BnfTokenType.OpenParen);
            case ')': return Make(1, BnfTokenType.CloseParen);
            }

            // String literal
            if (ch == '\'')
            {
                int length = 1;
                for (; Peek(length, '\'') != '\''; ++length) ;
                if (index + length >= source.Length) throw new FormatException($"Unclosed string literal starting at index {index}");
                // We consume the last ' too
                ++length;
                return Make(length, BnfTokenType.StringLiteral);
            }

            // Kind literal
            if (ch == '`')
            {
                int length = 1;
                for (; Peek(length, '`') != '`'; ++length) ;
                if (index + length >= source.Length) throw new FormatException($"Unclosed kind literal starting at index {index}");
                // We consume the last ` too
                ++length;
                return Make(length, BnfTokenType.KindLiteral);
            }

            // Identifier
            if (char.IsLetter(ch))
            {
                int length = 1;
                for (; char.IsLetterOrDigit(Peek(length)); ++length) ;
                return Make(length, BnfTokenType.Identifier);
            }

            throw new FormatException($"Unknown character '{source[index]}' at index {index}");
        }

        private BnfToken Make(int length, BnfTokenType type)
        {
            var value = source.Substring(index, length);
            var token = new BnfToken(index, value, type);
            index += length;
            return token;
        }

        private char Peek(int offset = 0, char @default = '\0') => 
            index + offset >= source.Length ? @default : source[index + offset];
    }
}
