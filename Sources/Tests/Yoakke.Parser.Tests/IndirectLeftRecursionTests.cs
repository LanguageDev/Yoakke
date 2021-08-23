// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;
using Yoakke.Parser.Attributes;
using IgnoreAttribute = Yoakke.Lexer.Attributes.IgnoreAttribute;

namespace Yoakke.Parser.Tests
{
    [TestClass]
    public partial class IndirectLeftRecursionTests
    {
        internal enum TokenType
        {
            [End] End,
            [Error] Error,
            [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

            [Regex(Regexes.Identifier)] Identifier,
        }

        [Lexer(typeof(TokenType))]
        internal partial class Lexer
        {
        }

        [Parser(typeof(TokenType))]
        internal partial class Parser
        {
            [Rule("grouping : group_element")]
            private static string Ident(string s) => s;

            [Rule("group_element : grouping Identifier")]
            private static string Group(string group, IToken next) => $"({group}, {next.Text})";

            [Rule("group_element : Identifier")]
            private static string Ident(IToken t) => t.Text;
        }

        private static string Parse(string source) =>
            new Parser(new Lexer(source)).ParseGrouping().Ok.Value;

        [TestMethod]
        public void TestOne() => Assert.AreEqual("a", Parse("a"));

        [TestMethod]
        public void TestTwo() => Assert.AreEqual("(a, b)", Parse("a b"));

        [TestMethod]
        public void TestThree() => Assert.AreEqual("((a, b), c)", Parse("a b c"));

        [TestMethod]
        public void TestFour() => Assert.AreEqual("(((a, b), c), d)", Parse("a b c d"));
    }
}
