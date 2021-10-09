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
using Yoakke.Collections;
using Yoakke.Collections.Values;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Lr;
using Yoakke.Grammar.Lr.Clr;
using Yoakke.Grammar.Lr.Lalr;
using Yoakke.Grammar.Lr.Lr0;

namespace Yoakke.Grammar.Tests
{
    public abstract class LrTestBase<TItem>
        where TItem : ILrItem
    {
        protected ILrParsingTable<TItem> Table { get; set; }

        protected IReadOnlyCfg Grammar => this.Table.Grammar;

        private static Func<IReadOnlyCfg, string, ILrItem> ItemParser =>
              typeof(TItem) == typeof(Lr0Item) ? ParseLr0Item
            : typeof(TItem) == typeof(LalrItem) ? ParseLalrItem
            : typeof(TItem) == typeof(ClrItem) ? ParseClrItem
            : throw new NotSupportedException();

        /* Factory */

        protected Production Production(string text) => ParseUtils.ParseProduction(this.Grammar, text);

        protected Lr.Action Shift(int state) => new Shift(state);

        protected Lr.Action Reduce(string text) => new Reduce(ParseUtils.ParseProduction(this.Grammar, text));

        /* Assertions */

        protected void AssertState(out int state, params string[] itemTexts)
        {
            var itemSet = itemTexts
                .Select(t => ItemParser(this.Table.Grammar, t))
                .OfType<TItem>()
                .ToHashSet();
            Assert.False(this.Table.StateAllocator.Allocate(itemSet, out state));
        }

        protected void AssertAction(int state, string term, params Lr.Action[] actions) =>
            this.AssertAction(state, term == "$" ? Terminal.EndOfInput : new(term), actions);

        protected void AssertAction(int state, Terminal term, params Lr.Action[] actions)
        {
            var actualActions = this.Table.Action[state, term].ToHashSet();
            Assert.True(actualActions.SetEquals(actions));
        }

        /* Parsers */

        private static Lr0Item ParseLr0Item(IReadOnlyCfg cfg, string text)
        {
            var fakeProd = ParseUtils.ParseProduction(cfg, text);
            var cursor = fakeProd.Right.IndicesOf(new Terminal("_")).First();
            var right = fakeProd.Right.ToList();
            right.RemoveAt(cursor);
            return new(new(fakeProd.Left, right.ToValue()), cursor);
        }

        private static LalrItem ParseLalrItem(IReadOnlyCfg cfg, string text)
        {
            var parts = text.Split(", ");
            var lr0 = ParseLr0Item(cfg, parts[0]);
            var lookaheads = parts[1].Split("/").Select(t => t.Trim() == "$" ? Terminal.EndOfInput : new Terminal(t.Trim()));
            return new(lr0.Production, lr0.Cursor, lookaheads.ToHashSet());
        }

        private static ClrItem ParseClrItem(IReadOnlyCfg cfg, string text)
        {
            var lalr = ParseLalrItem(cfg, text);
            Debug.Assert(lalr.Lookaheads.Count() == 1, "LR(1) items should have exactly one lookahead");
            return new(lalr.Production, lalr.Cursor, lalr.Lookaheads.First());
        }
    }
}
