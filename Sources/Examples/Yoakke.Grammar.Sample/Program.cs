using System;
using System.Collections.Generic;
using System.Linq;
using Yoakke.Collections.Values;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Lr;
using Action = Yoakke.Grammar.Lr.Action;

namespace Yoakke.Grammar.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var cfg = ParseGrammar(@"
S -> E
E -> 1 E
E -> 1
");
            cfg.StartSymbol = "S";
            Console.WriteLine(cfg);

            var table = new LrParsingTable();
            var sTick = cfg.GetProductions("S").First();
            var i0 = cfg.Closure(sTick.InitialLrItem);
            var stk = new Stack<ISet<LrItem>>();
            stk.Push(i0);

            while (stk.TryPop(out var itemSet))
            {
                table.AllocateState(itemSet, out var state);

                // Terminal advance
                var itemsWithTerminals = itemSet
                    .Where(prod => prod.AfterCursor is Symbol.Terminal)
                    .GroupBy(prod => prod.AfterCursor);
                foreach (var group in itemsWithTerminals)
                {
                    var term = (Symbol.Terminal)group.Key!;
                    var nextSet = cfg.Closure(group.Select(prod => prod.Next).ToHashSet());
                    if (table.AllocateState(nextSet, out var nextState)) stk.Push(nextSet);
                    table.AddAction(state, term, Action.Shift.Instance);
                    table.AddGoto(state, term, nextState);
                }

                // Nonterminal advance
                var itemsWithNonterminals = itemSet
                    .Where(prod => prod.AfterCursor is Symbol.Nonterminal)
                    .GroupBy(prod => prod.AfterCursor);
                foreach (var group in itemsWithNonterminals)
                {
                    var nonterm = (Symbol.Nonterminal)group.Key!;
                    var nextSet = cfg.Closure(group.Select(prod => prod.Next).ToHashSet());
                    if (table.AllocateState(nextSet, out var nextState)) stk.Push(nextSet);
                    table.AddGoto(state, nonterm, nextState);
                }

                // Final items
                var finalItems = itemSet.Where(prod => prod.IsFinal);
                foreach (var item in finalItems)
                {
                    var reduction = new Action.Reduce(item.Production);
                    // LR(0)
                    if (item.Production.Name == cfg.StartSymbol)
                    {
                        table.AddAction(state, Symbol.EndOfInput, reduction);
                    }
                    else
                    {
                        foreach (var term in cfg.Terminals) table.AddAction(state, term, reduction);
                    }
                    // SLR
                    // var followSet = cfg.Follow(new Symbol.Nonterminal(item.Production.Name));
                    // foreach (var follow in followSet.Terminals) table.AddAction(state, follow, reduction);
                }
            }

            Console.WriteLine(table);
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
