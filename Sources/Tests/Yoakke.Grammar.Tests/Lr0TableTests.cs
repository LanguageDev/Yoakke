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
    public class Lr0TableTests : LrTestBase<Lr0Item>
    {
        [Fact]
        public void FromLr0Grammar()
        {
            var grammar = ParseUtils.ParseGrammar(TestGrammars.Lr0Grammar);
            grammar.AugmentStartSymbol();
            this.Table = LrParsingTable.Lr0(grammar);

            // Assert state count
            Assert.Equal(8, this.Table.StateAllocator.States.Count);

            // Assert item sets
            this.AssertState(
                out var i0,
                "S' -> _ S",
                "S -> _ a S b",
                "S -> _ a S c",
                "S -> _ d b");
            this.AssertState(
                out var i1,
                "S -> a _ S b",
                "S -> a _ S c",
                "S -> _ a S b",
                "S -> _ a S c",
                "S -> _ d b");
            this.AssertState(
                out var i2,
                "S -> d _ b");
            this.AssertState(
                out var i3,
                "S' -> S _");
            this.AssertState(
                out var i4,
                "S -> d b _");
            this.AssertState(
                out var i5,
                "S -> a S _ b",
                "S -> a S _ c");
            this.AssertState(
                out var i6,
                "S -> a S b _");
            this.AssertState(
                out var i7,
                "S -> a S c _");

            // Assert action table
            this.AssertAction(i0, "a", this.Shift(i1));
            this.AssertAction(i0, "d", this.Shift(i2));
            this.AssertAction(i1, "a", this.Shift(i1));
            this.AssertAction(i1, "d", this.Shift(i2));
            this.AssertAction(i3, "$", Accept.Instance);
            this.AssertAction(i5, "b", this.Shift(i6));
            this.AssertAction(i5, "c", this.Shift(i7));
            foreach (var term in grammar.Terminals)
            {
                this.AssertAction(i4, term, this.Reduce("S -> d b"));
                this.AssertAction(i6, term, this.Reduce("S -> a S b"));
                this.AssertAction(i7, term, this.Reduce("S -> a S c"));
            }

            // Assert goto table
            Assert.Equal(i3, this.Table.Goto[i0, new("S")]);
            Assert.Equal(i5, this.Table.Goto[i1, new("S")]);
        }

        [Fact]
        public void FromSlrGrammar()
        {
            var grammar = ParseUtils.ParseGrammar(TestGrammars.SlrGrammar);
            grammar.AugmentStartSymbol();
            this.Table = LrParsingTable.Lr0(grammar);

            // Assert state count
            Assert.Equal(5, this.Table.StateAllocator.States.Count);

            // Assert item sets
            this.AssertState(
                out var i0,
                "S' -> _ S",
                "S -> _ E",
                "E -> _ 1 E",
                "E -> _ 1");
            this.AssertState(
                out var i1,
                "E -> 1 _ E",
                "E -> 1 _",
                "E -> _ 1 E",
                "E -> _ 1");
            this.AssertState(
                out var i2,
                "S' -> S _");
            this.AssertState(
                out var i3,
                "S -> E _");
            this.AssertState(
                out var i4,
                "E -> 1 E _");

            // Assert action table
            this.AssertAction(i0, "1", this.Shift(i1));
            this.AssertAction(i1, "$", this.Reduce("E -> 1"));
            this.AssertAction(i1, "1", this.Shift(i1), this.Reduce("E -> 1"));
            this.AssertAction(i2, "$", Accept.Instance);
            this.AssertAction(i3, "$", this.Reduce("S -> E"));
            this.AssertAction(i3, "1", this.Reduce("S -> E"));
            this.AssertAction(i4, "$", this.Reduce("E -> 1 E"));
            this.AssertAction(i4, "1", this.Reduce("E -> 1 E"));

            // Assert goto table
            Assert.Equal(i2, this.Table.Goto[i0, new("S")]);
            Assert.Equal(i3, this.Table.Goto[i0, new("E")]);
            Assert.Equal(i4, this.Table.Goto[i1, new("E")]);
        }
    }
}
