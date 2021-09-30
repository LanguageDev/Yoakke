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
            cfg.StartSymbol = "S";
            cfg.AddProduction(new("S", new[] { (Symbol)new Symbol.Nonterminal("E") }.ToValue()));
            cfg.AddProduction(new("E", new[] { (Symbol)new Symbol.Nonterminal("E"), new Symbol.Terminal("x"), new Symbol.Nonterminal("E") }.ToValue()));
            cfg.AddProduction(new("E", new[] { (Symbol)new Symbol.Terminal("z") }.ToValue()));
            cfg.AddProduction(new("E", new Symbol[] { }.ToValue()));
            Console.WriteLine(cfg);
        }
    }
}
