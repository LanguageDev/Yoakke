using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;
using Yoakke.Parser.Attributes;
using IgnoreAttribute = Yoakke.Lexer.Attributes.IgnoreAttribute;
using Token = Yoakke.Lexer.Token<Yoakke.Parser.Tests.PunctuatedTests.TokenType>;

namespace Yoakke.Parser.Tests
{
    [Parser(typeof(PunctuatedTests.TokenType))]
    partial class ListParser
    {
        [Rule("any0_no_trailing : Lparen (Identifier (',' Identifier)*)? Rparen")]
        private static List<string> ZeroOrMoreNoTrailing(IToken _lp, Punctuated<Token, Token> elements, IToken _rp) =>
            elements.Values.Select(t => t.Text).ToList();

        [Rule("any1_no_trailing : Lparen (Identifier (',' Identifier)*) Rparen")]
        private static List<string> OneOrMoreNoTrailing(IToken _lp, Punctuated<Token, Token> elements, IToken _rp) =>
            elements.Values.Select(t => t.Text).ToList();
    }

    [TestClass]
    public class PunctuatedTests
    {
        [Lexer("ListLexer")]
        public enum TokenType
        {
            [End] End,
            [Error] Error,
            [Ignore] [Regex(Regex.Whitespace)] Whitespace,

            [Token("(")] Lparen,
            [Token(")")] Rparen,
            [Token(",")] Comma,
            [Regex(Regex.Identifier)] Identifier,
        }

        private static List<string> Any0NoTrailing(string source) => new ListParser(new ListLexer(source)).ParseAny0NoTrailing().Ok.Value;

        private static List<string> Any1NoTrailing(string source) => new ListParser(new ListLexer(source)).ParseAny1NoTrailing().Ok.Value;

        [TestMethod]
        public void Empty0NoTrailing()
        {
            Assert.IsTrue(Any0NoTrailing("()").SequenceEqual(Array.Empty<string>()));
        }

        [TestMethod]
        public void One0NoTrailing()
        {
            Assert.IsTrue(Any0NoTrailing("(a)").SequenceEqual(new string[] { "a" }));
        }

        [TestMethod]
        public void Two0NoTrailing()
        {
            Assert.IsTrue(Any0NoTrailing("(a, b)").SequenceEqual(new string[] { "a", "b" }));
        }

        [TestMethod]
        public void Many0NoTrailing()
        {
            Assert.IsTrue(Any0NoTrailing("(a, b, c, d, e)").SequenceEqual(new string[] { "a", "b", "c", "d", "e" }));
        }

        [TestMethod]
        public void One1NoTrailing()
        {
            Assert.IsTrue(Any1NoTrailing("(a)").SequenceEqual(new string[] { "a" }));
        }

        [TestMethod]
        public void Two1NoTrailing()
        {
            Assert.IsTrue(Any1NoTrailing("(a, b)").SequenceEqual(new string[] { "a", "b" }));
        }

        [TestMethod]
        public void Many1NoTrailing()
        {
            Assert.IsTrue(Any1NoTrailing("(a, b, c, d, e)").SequenceEqual(new string[] { "a", "b", "c", "d", "e" }));
        }
    }
}
