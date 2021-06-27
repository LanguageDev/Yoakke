using System;

namespace Yoakke.C.Syntax.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceCode = @"
#define FOO() x + y == z
FOO()
";
            var lexer = new CLexer(sourceCode);
            var pp = new CPreProcessor(lexer);

            while (true)
            {
                var token = pp.Next();
                if (token.Kind == CTokenType.End) break;
                Console.WriteLine(token.LogicalText);
            }
        }
    }
}
