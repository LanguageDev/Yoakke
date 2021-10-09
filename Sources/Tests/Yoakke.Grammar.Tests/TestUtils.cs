// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yoakke.Collections.Values;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Lr;
using Yoakke.Grammar.Lr.Lr0;

namespace Yoakke.Grammar.Tests
{
    internal static class TestUtils
    {
        public static void AssertLr0ItemSet(ILrParsingTable<Lr0Item> table, out int state, params string[] itemTexts) =>
            AssertItemSet(table, out state, ParseLr0Item, itemTexts);

        public static void AssertItemSet<TItem>(
            ILrParsingTable<TItem> table,
            out int state,
            Func<IReadOnlyCfg, string, TItem> itemParser,
            params string[] itemTexts)
            where TItem : ILrItem =>
            Assert.False(table.StateAllocator.Allocate(itemTexts.Select(t => itemParser(table.Grammar, t)).ToHashSet(), out state));

        public static Lr0Item ParseLr0Item(IReadOnlyCfg cfg, string text)
        {
            var fakeProd = ParseProduction(cfg, text);
            var cursor = fakeProd.Right
                .Select((s, i) => (Symbol: s, Index: i))
                .Where(p => p.Symbol.Equals(new Terminal("_")))
                .Select(p => p.Index)
                .First();
            var right = fakeProd.Right.ToList();
            right.RemoveAt(cursor);
            return new Lr0Item(new(fakeProd.Left, right.ToValue()), cursor);
        }

        public static Production ParseProduction(IReadOnlyCfg cfg, string text)
        {
            var parts = text.Split("->");
            var left = new Nonterminal(parts[0].Trim());
            var rightParts = parts[1].Trim().Split(" ").Select(p => p.Trim());
            var right = new List<Symbol>();
            foreach (var part in rightParts)
            {
                right.Add(cfg.Nonterminals.Contains(new(part)) ? new Nonterminal(part) : new Terminal(part));
            }
            return new(left, right.ToValue());
        }

        public static ContextFreeGrammar ParseGrammar(string text)
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
