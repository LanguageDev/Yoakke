using System;
using System.Collections.Generic;
using Yoakke.Text;

namespace Yoakke.Sample
{
    [Yoakke.Ast.Attributes.Ast]
    [Yoakke.Ast.Attributes.ImplementEquality]
    abstract partial class Node
    { }

    [Yoakke.Ast.Attributes.Ast]
    partial class Foo : Node
    {
        public readonly int A;
        public readonly IReadOnlyList<int> B;
    }

    class Program
    {
        static void Main(string[] args)
        {
            var k = new Foo(3, new int[] { 1, 2, 3 });
            var h = k.GetHashCode();

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
