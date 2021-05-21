using System;
using System.Collections.Generic;
using Yoakke.Text;

namespace Yoakke.Sample
{
    [Yoakke.Ast.Attributes.Ast]
    [Yoakke.Ast.Attributes.ImplementEquality]
    [Yoakke.Ast.Attributes.Visitor("MyVisitor", typeof(int))]
    abstract partial class Node
    { }

    [Yoakke.Ast.Attributes.Ast]
    partial class Foo : Node
    {
        public readonly int A;
        public readonly IReadOnlyList<Node> B;
        public readonly Node? C;
    }

    class NodeVisitor : Node.MyVisitor
    {
        public void Haha(Node n) => Visit(n);
    }

    class Program
    {
        static void Main(string[] args)
        {
            var k = new Foo(3, new Node[] { }, null);
            var h = k.GetHashCode();

            var v = new NodeVisitor();
            v.Haha(k);

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
