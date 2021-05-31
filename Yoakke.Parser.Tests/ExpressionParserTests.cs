using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;
using Yoakke.Parser.Attributes;
using Yoakke.Text;
using IgnoreAttribute = Yoakke.Lexer.Attributes.IgnoreAttribute;
using Range = Yoakke.Text.Range;

namespace Yoakke.Parser.Tests
{
    [Parser(typeof(ExpressionParserTests.TokenType))]
    partial class ExprParser
    {
        [Rule("top_expression : expression End")]
        private static int TopLevel(int n, IToken _) => n;

        [Right("^")]
        [Left("*", "/")]
        [Left("+", "-")]
        [Rule("expression")]
        private static int Op(int left, IToken op, int right) => op.Text switch
        { 
            "+" => left + right,
            "-" => left - right,
            "*" => left * right,
            "/" => left / right,
            "^" => (int)Math.Pow(left, right),
            _ => throw new InvalidOperationException(),
        };

        [Rule("expression : '(' expression ')'")]
        private static int Grouping(IToken _1, int n, IToken _2) => n;

        [Rule("expression : Number")]
        private static int Number(IToken tok) => int.Parse(tok.Text);
    }

    [TestClass]
    public class ExpressionParserTests
    {
        [Lexer("ExprLexer")]
        public enum TokenType
        {
            [End] End,
            [Error] Error,
            [Ignore] [Regex(Regex.Whitespace)] Whitespace,

            [Token("+")] Add,
            [Token("-")] Sub,
            [Token("*")] Mul,
            [Token("/")] Div,
            [Token("^")] Exp,
            [Regex(Regex.IntLiteral)] Number,
            [Token("(")] Lparen,
            [Token(")")] Rparen,
        }

        private static int Eval(string s) => new ExprParser(new ExprLexer(s)).ParseTopExpression().Ok.Value;

        [TestMethod]
        public void Tests()
        {
            Assert.AreEqual(3, Eval("1+2"));
            Assert.AreEqual(7, Eval("1+2*3"));
            Assert.AreEqual(9, Eval("(1+2)*3"));
            Assert.AreEqual(0, Eval("3-2-1"));
            Assert.AreEqual(2, Eval("3-(2-1)"));
            Assert.AreEqual(6561, Eval("3^2^3"));
            Assert.AreEqual(729, Eval("(3^2)^3"));
        }
    }
}
