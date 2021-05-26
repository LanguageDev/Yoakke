using System;
using System.Collections.Generic;
using Yoakke.Ast;
using Yoakke.Text;

namespace Yoakke.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var code = @"
(1 + 2) * 3
";
            var src = new SourceFile("test.sample", code);
            var lexer = new Lexer(src);
            var parser = new Parser(lexer);

            var result = parser.ParseExpr();
            Console.WriteLine(PrettyPrinter.Print(result.Ok.Value, PrettyPrintFormat.Xml));
            Console.WriteLine(new TreeEvaluator().Evaluate(result.Ok.Value));
        }
    }
}
