using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Collections.Values;
using Yoakke.Grammar.Cfg;

namespace Yoakke.Grammar.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = new ContextFreeGrammar();
            cfg.StartSymbol = "E";
            cfg.AddProduction(new("E", new[] { (Symbol)new Symbol.Nonterminal("T"), new Symbol.Nonterminal("E'") }.ToValue()));
            cfg.AddProduction(new("E'", new[] { (Symbol)new Symbol.Nonterminal("T"), new Symbol.Nonterminal("E'") }.ToValue()));
            Console.WriteLine(cfg);

            Console.WriteLine(cfg.First(new Symbol.Nonterminal("S")));
            Console.WriteLine(cfg.First(new Symbol.Nonterminal("E")));
        }
    }
}
