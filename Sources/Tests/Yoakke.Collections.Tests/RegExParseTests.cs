// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Collections.RegEx;

namespace Yoakke.Collections.Tests
{
    [TestClass]
    public class RegExParseTests
    {
        private static RegExAst Alt(params RegExAst[] nodes)
        {
            var result = nodes[nodes.Length - 1];
            for (var i = nodes.Length - 2; i >= 0; --i) result = new RegExAst.Alt(nodes[i], result);
            return result;
        }

        private static RegExAst Seq(params RegExAst[] nodes)
        {
            var result = nodes[nodes.Length - 1];
            for (var i = nodes.Length - 2; i >= 0; --i) result = new RegExAst.Seq(nodes[i], result);
            return result;
        }

        private static RegExAst Quantified(RegExAst node, int atLeast, int? atMost = null) =>
            new RegExAst.Quantified(node, atLeast, atMost);

        private static RegExAst ChRange(bool negate, params object[] vs)
        {
            var ranges = vs.Select(v => v is char ch ? (ch, ch) : ((char, char))v).ToList();
            return new RegExAst.LiteralRange(negate, ranges);
        }

        private static RegExAst Opt(RegExAst node) => new RegExAst.Opt(node);

        private static RegExAst Rep0(RegExAst node) => new RegExAst.Rep0(node);

        private static RegExAst Rep1(RegExAst node) => new RegExAst.Rep1(node);

        private static RegExAst Ch(char ch) => new RegExAst.Literal(ch);

        [TestMethod]
        public void SimpleSequence()
        {
            var ast = new RegExParser().Parse("abc");
            Assert.AreEqual(ast, Seq(Ch('a'), Ch('b'), Ch('c')));
        }

        [TestMethod]
        public void SimpleAlternation()
        {
            var ast = new RegExParser().Parse("a|b|c");
            Assert.AreEqual(ast, Alt(Ch('a'), Ch('b'), Ch('c')));
        }

        [TestMethod]
        public void SeqenceHigherPriorityThanAlternation()
        {
            var ast = new RegExParser().Parse("ab|cd");
            Assert.AreEqual(ast, Alt(Seq(Ch('a'), Ch('b')), Seq(Ch('c'), Ch('d'))));
        }

        [TestMethod]
        public void GroupingOverridesPriotity()
        {
            var ast = new RegExParser().Parse("a(b|c)d");
            Assert.AreEqual(ast, Seq(Ch('a'), Alt(Ch('b'), Ch('c')), Ch('d')));
        }

        [TestMethod]
        public void Optional()
        {
            var ast = new RegExParser().Parse("a?");
            Assert.AreEqual(ast, Opt(Ch('a')));
        }

        [TestMethod]
        public void RepeatAnyTimes()
        {
            var ast = new RegExParser().Parse("a*");
            Assert.AreEqual(ast, Rep0(Ch('a')));
        }

        [TestMethod]
        public void RepeatAtLeastOnce()
        {
            var ast = new RegExParser().Parse("a+");
            Assert.AreEqual(ast, Rep1(Ch('a')));
        }

        [TestMethod]
        public void RepeatExactly3Times()
        {
            var ast = new RegExParser().Parse("a{3}");
            Assert.AreEqual(ast, Quantified(Ch('a'), 3, 3));
        }

        [TestMethod]
        public void RepeatBetween3And6Times()
        {
            var ast = new RegExParser().Parse("a{3,6}");
            Assert.AreEqual(ast, Quantified(Ch('a'), 3, 6));
        }

        [TestMethod]
        public void RepeatAtLeast3Times()
        {
            var ast = new RegExParser().Parse("a{3,}");
            Assert.AreEqual(ast, Quantified(Ch('a'), 3));
        }

        [TestMethod]
        public void LiteralRange()
        {
            var ast = new RegExParser().Parse("[A-Za-z0-9_]");
            Assert.AreEqual(ast, ChRange(false, ('A', 'Z'), ('a', 'z'), ('0', '9'), '_'));
        }

        [TestMethod]
        public void NegatedLiteralRange()
        {
            var ast = new RegExParser().Parse("[^A-Za-z0-9_]");
            Assert.AreEqual(ast, ChRange(true, ('A', 'Z'), ('a', 'z'), ('0', '9'), '_'));
        }
    }
}
