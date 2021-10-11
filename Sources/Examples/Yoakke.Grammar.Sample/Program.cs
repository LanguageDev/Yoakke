using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Yoakke.Collections;
using Yoakke.Collections.Graphs;
using Yoakke.Collections.Values;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Lr;
using Yoakke.Grammar.Lr.Lalr;
using Yoakke.Grammar.Lr.Lr0;
using Yoakke.Grammar.ParseTree;
using Action = Yoakke.Grammar.Lr.Action;

namespace Yoakke.Grammar.Sample
{
    public class GssNode
    {
        public int State { get; init; }

        public IParseTreeNode? Node { get; init; }

        public List<GssNode> Prev { get; init; } = new();
    }

    class Program
    {
        static void Main(string[] args)
        {
            var cfg = ParseGrammar(@"
S -> NP VP
S -> S PP
NP -> NOUN
NP -> DETERMINER NOUN
NP -> NP PP
PP -> PREPOSITION NP
VP -> VERB NP
NOUN -> I | man | telescope | bed | apartment
DETERMINER -> a | the
PREPOSITION -> on | in | with
VERB -> saw
");
            cfg.AugmentStartSymbol();

            foreach (var sentence in cfg.GenerateSentences())
            {
                Console.WriteLine(string.Join(" ", sentence));
            }

            // var table = LrParsingTable.Lr0(cfg);
            // Console.WriteLine(table.ToHtmlTable());
            // Console.WriteLine();
            // Console.WriteLine();
            // Console.WriteLine(table.ToDotDfa());

            /*
            while (true)
            {
                var input = Console.ReadLine();
                if (input is null) break;
                // TODO: Invoke parse
            }
            */
        }

        static IParseTreeNode Parse(ILrParsingTable table, string input)
        {
            var parser = new LrParser(table);
            foreach (var action in parser.Parse(input, c => new(c.ToString()!)))
            {
                // First we do some state printing
                Console.WriteLine($"State stack: {string.Join(" ", parser.StateStack.Reverse())}");
                Console.WriteLine($"Result stack: {string.Join(" ", parser.ResultStack.Reverse().Select(r => r.Symbol))}");
                Console.WriteLine(action);
            }

            return parser.ResultStack.First();
        }

        static ContextFreeGrammar ParseGrammar(string text)
        {
            var result = new ContextFreeGrammar();
            var tokens = text
                .Split(' ', '\r', '\n')
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();
            var arrowPositions = tokens.IndicesOf("->").ToList();
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
                                ? (Symbol)new Nonterminal(t)
                                : new Terminal(t))
                            .ToList();
                    }
                    result.Productions.Add(new(new(productionName), prodSymbols.ToValue()));
                    productions.RemoveRange(0, end);
                }
            }
            return result;
        }
    }
}
