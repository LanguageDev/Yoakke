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
func max(a, b) {
    if a > b {
        return a;
    }
    else {
        return b;
    }
}
";
            var src = new SourceFile("test.sample", code);
            var lexer = new Lexer(src);
            var parser = new Parser(lexer);

            var result = parser.ParseProgram();
            if (result.IsError)
            {
                Console.WriteLine("Parse error!");
                return;
            }

            var ast = result.Ok.Value;
            Console.WriteLine(PrettyPrinter.Print(ast, PrettyPrintFormat.Xml));
            var resolve = new SymbolResolution();
            resolve.Resolve(ast);
            var runtime = new TreeEvaluator();
            runtime.Bind("print", (Func<object[], object?>)(arg => { Console.WriteLine(arg[0].ToString()); return null; }));
            runtime.Execute(ast);
            //Console.WriteLine(new TreeEvaluator().Evaluate(result.Ok.Value));
        }
    }
}
