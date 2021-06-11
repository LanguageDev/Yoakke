// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Yoakke.Ast;
using Yoakke.Text;

namespace Yoakke.Sample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var code = @"
func fib(a) {
    if a < 2 {
        return 1;
    }
    else {
        return fib(a - 1) + fib(a - 2);
    }
}

print(fib(7));
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
            resolve.SymbolTable.GlobalScope.DefineSymbol(
                new ConstSymbol(resolve.SymbolTable.GlobalScope, "print", (Func<object[], object?>)(arg => { Console.WriteLine(arg[0].ToString()); return null; })));
            resolve.Resolve(ast);
            var runtime = new TreeEvaluator(resolve);
            runtime.Execute(ast);
        }
    }
}
