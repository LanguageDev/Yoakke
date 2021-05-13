using System;
using Yoakke.Lexer.Attributes;

namespace Yoakke.Lexer.Sample
{
    [Lexer("Lexer")]
    public enum TokenType
    {
        [Error] Error,
        [End] End,
        [Ignore] [Regex("[ \r\n\t]")] Whitespace,

        [Token("if")] KwIf,
        [Token("else")] KwElse,
        [Token("func")] KwFunc,
        [Ident] Ident,
        
        [Token("{")] OpenBrace,
        [Token("}")] CloseBrace,
        [Token("(")] OpenParen,
        [Token(")")] CloseParen,

        [Token("+")] Plus,
        [Token("-")] Minus,

        [Regex("[0-9]+")] IntLiteral,
    }

    class Program
    {
        static void Main(string[] args)
        {
            var lexer = new Lexer(Console.In);
            while (true)
            {
                var t = lexer.Next();
                Console.WriteLine($"{t.Text} [{t.Kind}] ({t.Range.Start})");
            }
        }
    }
}
