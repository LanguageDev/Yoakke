using System;
using Yoakke.Text;

namespace Yoakke.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var code = @"
1 + 2 * -foo(""hello"", not x)()
";
            var src = new SourceFile("test.sample", code);
            var lexer = new Lexer(src);
            var parser = new Parser(lexer);

            var result = parser.ParseExpr();
            Console.WriteLine(result.Ok.Value.Dump());
        }
    }
}
