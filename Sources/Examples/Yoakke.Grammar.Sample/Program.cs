using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = ParseGrammar(@"
S -> NP VP
S -> S PP
NP -> noun
NP -> det noun
NP -> NP PP
PP -> prep NP
VP -> verb NP
");
            cfg.AugmentStartSymbol();

            var table = LrParsingTable.Lalr(cfg);

            while (true)
            {
                var input = Console.ReadLine();
                if (input is null) break;
                GlrParse(new ParallelStacks(table), input);
            }
        }

        static void GlrParse(INondetStack stack, string text)
        {
            var nouns = new[] { "I", "man", "telescope", "bed", "apartment", "park" };
            var determiners = new[] { "a", "the" };
            var prepositions = new[] { "on", "in", "with" };
            var verbs = new[] { "saw" };

            Terminal ToTerm(string word)
            {
                if (nouns!.Contains(word)) return new("noun");
                if (determiners!.Contains(word)) return new("det");
                if (prepositions!.Contains(word)) return new("prep");
                if (verbs!.Contains(word)) return new("verb");
                throw new ArgumentOutOfRangeException();
            }

            var words = text
                .Split(' ')
                .Select(t => t.Trim())
                .Where(t => t.Length > 0)
                .ToList();
            var terminals = words
                .Select(ToTerm)
                .Append(Terminal.EndOfInput)
                .ToList();
            words.Add("$");

            Console.WriteLine("=========================");
            Console.WriteLine(stack.ToDot());
            Console.WriteLine("=========================");
            for (var i = 0; i < words.Count; ++i)
            {
                Console.WriteLine($"processing {words[i]}");
                stack.Feed(terminals[i]);
                Console.WriteLine("=========================");
                Console.WriteLine(stack.ToDot());
                Console.WriteLine("=========================");
                while (stack.Step())
                {
                    Console.WriteLine("=========================");
                    Console.WriteLine(stack.ToDot());
                    Console.WriteLine("=========================");
                }
            }
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
