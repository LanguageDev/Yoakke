using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;
using Yoakke.Parser.Attributes;
using IgnoreAttribute = Yoakke.Lexer.Attributes.IgnoreAttribute;

namespace Yoakke.Parser.Tests
{
    /**
     NOTE: The following typo caused a very big headache
     [Rule("gouping: Identifier")]

     Proposal: When we detect direct left-recursion that we can't resolve, error out and suggest that it's a possible typo
     */

    [Parser(typeof(LeftRecursionTests.TokenType))]
    partial class LeftRecursionParser
    {
        [Rule("grouping : grouping Identifier")]
        private static string Group(string group, IToken next) => $"({group}, {next.Text})";

        [Rule("grouping: Identifier")]
        private static string Ident(IToken t) => t.Text;
    }

    [TestClass]
    public class LeftRecursionTests
    {
        [Lexer("LeftRecursionLexer")]
        public enum TokenType
        {
            [End] End,
            [Error] Error,
            [Ignore] [Regex(Regex.Whitespace)] Whitespace,

            [Regex(Regex.Identifier)] Identifier,
        }

        private static string Parse(string source) => 
            new LeftRecursionParser(new LeftRecursionLexer(source)).ParseGrouping().Ok.Value;

        [TestMethod]
        public void TestOne()
        {
            Assert.AreEqual("a", Parse("a"));
        }

        [TestMethod]
        public void TestTwo()
        {
            Assert.AreEqual("(a, b)", Parse("a b"));
        }

        [TestMethod]
        public void TestThree()
        {
            Assert.AreEqual("((a, b), c)", Parse("a b c"));
        }

        [TestMethod]
        public void TestFour()
        {
            Assert.AreEqual("(((a, b), c), d)", Parse("a b c d"));
        }
    }
}
