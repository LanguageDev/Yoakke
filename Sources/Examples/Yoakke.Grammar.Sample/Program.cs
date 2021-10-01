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
            var cfg = ParseGrammar(@"
E -> T E'
E' -> + T E' | ε
T -> F T'
T' -> * F T' | ε
F -> ( E ) | id
");
            cfg.StartSymbol = "E";
            Console.WriteLine(cfg);

            Console.WriteLine();

            Console.WriteLine(cfg.First(new Symbol.Nonterminal("E")));
            Console.WriteLine(cfg.First(new Symbol.Nonterminal("E'")));
            Console.WriteLine(cfg.First(new Symbol.Nonterminal("T")));
            Console.WriteLine(cfg.First(new Symbol.Nonterminal("T'")));
            Console.WriteLine(cfg.First(new Symbol.Nonterminal("F")));
        }

        static ContextFreeGrammar ParseGrammar(string text)
        {
            var result = new ContextFreeGrammar();
            var tokens = text
                .Split(' ', '\r', '\n')
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();
            var arrowPositions = tokens
                .Select((token, index) => (Token: token, Index: index))
                .Where(i => i.Token == "->")
                .Select(i => i.Index)
                .ToList();
            var ruleNames = arrowPositions
                .Select(pos => tokens[pos - 1])
                .ToHashSet();
            for (var i = 0; i < arrowPositions.Count; ++i)
            {
                var arrowPosition = arrowPositions[i];
                var productionName = tokens[arrowPosition - 1];
                var productionsUntil = tokens.Count;
                if (i < arrowPositions.Count - 1) productionsUntil = arrowPositions[i + 1] - 1;
                var productions = tokens.GetRange(arrowPosition + 1, productionsUntil - (arrowPosition + 1));
                while (productions.Count > 0)
                {
                    var end = productions.IndexOf("|");
                    if (end == -1) end = productions.Count;
                    else productions.RemoveAt(end);
                    List<Symbol> prodSymbols = new();
                    if (productions[0] != "ε")
                    {
                        prodSymbols = productions
                            .Take(end)
                            .Select(t => ruleNames.Contains(t)
                                ? (Symbol)new Symbol.Nonterminal(t)
                                : new Symbol.Terminal(t))
                            .ToList();
                    }
                    result.AddProduction(new(productionName, prodSymbols.ToValue()));
                    productions.RemoveRange(0, end);
                }
            }
            return result;
        }
    }
}
