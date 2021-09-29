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
            var grammar = new Grammar();
            var ast = Bnf.Foldl(Bnf.Term('Q'), Bnf.Seq(Bnf.Or(Bnf.Term('a'), Bnf.Term('b')), Bnf.Term('w'), Bnf.Or(Bnf.Seq(Bnf.Term('x'), Bnf.Term('0')), Bnf.Term('y')), Bnf.Term('t')), "Foo()");
            grammar.Add("R", new(ast));

            Console.WriteLine(grammar);
            Console.WriteLine("======================");
            grammar.SplitOrAlternatives();
            Console.WriteLine(grammar);
        }
    }
}
