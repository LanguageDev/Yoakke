// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Yoakke.Grammar.Cfg;
using Yoakke.Grammar.Lr;
using Yoakke.Grammar.Lr.Lr0;

namespace Yoakke.Grammar.Tests
{
    public class Lr0TableTests
    {
        [Fact]
        public void FromLr0Grammar()
        {
            var grammar = TestUtils.ParseGrammar(TestGrammars.Lr0Grammar);
            grammar.AugmentStartSymbol();
            var table = LrParsingTable.Lr0(grammar);

            // Assert state count
            Assert.Equal(8, table.StateAllocator.States.Count);

            // Assert item sets
            TestUtils.AssertLr0ItemSet(
                table,
                out var i0,
                "S' -> _ S",
                "S -> _ a S b",
                "S -> _ a S c",
                "S -> _ d b");
            TestUtils.AssertLr0ItemSet(
                table,
                out var i1,
                "S -> a _ S b",
                "S -> a _ S c",
                "S -> _ a S b",
                "S -> _ a S c",
                "S -> _ d b");
            TestUtils.AssertLr0ItemSet(
                table,
                out var i2,
                "S -> d _ b");
            TestUtils.AssertLr0ItemSet(
                table,
                out var i3,
                "S' -> S _");
            TestUtils.AssertLr0ItemSet(
                table,
                out var i4,
                "S -> d b _");
            TestUtils.AssertLr0ItemSet(
                table,
                out var i5,
                "S -> a S _ b",
                "S -> a S _ c");
            TestUtils.AssertLr0ItemSet(
                table,
                out var i6,
                "S -> a S b _");
            TestUtils.AssertLr0ItemSet(
                table,
                out var i7,
                "S -> a S c _");

            // Assert action table
            Assert.True(table.Action[i0, new("a")].ToHashSet().SetEquals(new[] { new Shift(i1) }));
            Assert.True(table.Action[i0, new("d")].ToHashSet().SetEquals(new[] { new Shift(i2) }));
            Assert.True(table.Action[i1, new("a")].ToHashSet().SetEquals(new[] { new Shift(i1) }));
            Assert.True(table.Action[i1, new("d")].ToHashSet().SetEquals(new[] { new Shift(i2) }));
            Assert.True(table.Action[i3, Terminal.EndOfInput].ToHashSet().SetEquals(new[] { Accept.Instance }));
            Assert.True(table.Action[i5, new("b")].ToHashSet().SetEquals(new[] { new Shift(i6) }));
            Assert.True(table.Action[i5, new("c")].ToHashSet().SetEquals(new[] { new Shift(i7) }));
            foreach (var term in grammar.Terminals)
            {
                Assert.True(table.Action[i4, term].ToHashSet().SetEquals(
                    new[] { new Reduce(TestUtils.ParseProduction(grammar, "S -> d b")) }));
                Assert.True(table.Action[i6, term].ToHashSet().SetEquals(
                    new[] { new Reduce(TestUtils.ParseProduction(grammar, "S -> a S b")) }));
                Assert.True(table.Action[i7, term].ToHashSet().SetEquals(
                    new[] { new Reduce(TestUtils.ParseProduction(grammar, "S -> a S c")) }));
            }

            // Assert goto table
            Assert.Equal(i3, table.Goto[i0, new("S")]);
            Assert.Equal(i5, table.Goto[i1, new("S")]);
        }
    }
}
