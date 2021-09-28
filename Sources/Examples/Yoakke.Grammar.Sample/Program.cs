using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Grammar.BnfAst;

namespace Yoakke.Grammar.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var ast = Bnf.Transform(Bnf.Seq(Bnf.Or(Bnf.Term('a'), Bnf.Term('b')), Bnf.Term('w'), Bnf.Or(Bnf.Seq(Bnf.Term('x'), Bnf.Term('0')), Bnf.Term('y')), Bnf.Term('t')), "Foo()");
            Console.WriteLine(ast);
            Console.WriteLine();

            var alternatives = new List<IBnfNode>();
            var stk = new Stack<IBnfNode>();
            stk.Push(ast);
            while (stk.TryPop(out var node))
            {
                var alt = node.Traverse().OfType<BnfOrNode>().FirstOrDefault();
                if (alt is null)
                {
                    alternatives.Add(node);
                    continue;
                }

                var firstAlt = node.ReplaceByReference(alt, alt.First);
                var secondAlt = node.ReplaceByReference(alt, alt.Second);
                stk.Push(firstAlt);
                stk.Push(secondAlt);
            }

            foreach (var alt in alternatives) Console.WriteLine(alt);
        }
    }
}
