using System;
using Yoakke.Ir.Model;
using Yoakke.Ir.Syntax;

namespace Yoakke.Ir.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var lexer = new IrLexer(Console.In);
            while (true)
            {
                var t = lexer.Next();
                Console.WriteLine($"{t.Text} - {t.Kind}");
            }
        }
    }
}
