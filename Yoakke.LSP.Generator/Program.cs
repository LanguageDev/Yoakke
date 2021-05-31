using System;
using System.Collections.Generic;
using Yoakke.Lexer;

namespace Yoakke.LSP.Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            var lexer = new TsLexer(Console.In);
            var parser = new TsParser(lexer);
            while (true)
            {
                var parseResult = parser.ParseInterface();
                if (parseResult.IsError)
                {
                    Console.WriteLine("Failed to parse!");
                    break;
                }
                var ast = parseResult.Ok;
            }
        }
    }
}
