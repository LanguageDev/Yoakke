using System;

namespace Yoakke.C.Syntax.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceCode = @"
#ifndef FOO
#define FOO

int main() {
    return 0;
}

#endif
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
