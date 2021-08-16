// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        [Lexer("Lexer")]
        internal enum TokenType
        {
            [End] End,
            [Error] Error,
            [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

            [Regex(Regexes.Identifier)] Identifier,
        }

        [Parser(typeof(TokenType))]
        internal partial class Parser
        {
#if false
            [Rule("grouping : group_element Identifier")]
            private static string Group(string group, IToken next) => $"({group}, {next.Text})";

            [Rule("group_element : grouping")]
            private static string Ident(string s) => s;

            [Rule("group_element : Identifier")]
            private static string Ident(IToken t) => t.Text;
#endif

            [Rule("A : B C")]
            private static string DoA(string b, string c) => string.Empty;

            [Rule("B : D E")]
            [Rule("B : A X")]
            private static string DoB(string b, string c) => string.Empty;

            [Rule("D : A F")]
            private static string DoD(string b, string c) => string.Empty;

            [Rule("B : Identifier")]
            [Rule("C : Identifier")]
            [Rule("E : Identifier")]
            [Rule("F : Identifier")]
            [Rule("X : Identifier")]
            private static string DoC(IToken _) => string.Empty;
        }

#if false
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
#endif
    }
}
