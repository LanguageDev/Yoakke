using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Yoakke.Lexer;
using Yoakke.Text;

namespace Yoakke.Parser.Tests
{
    [Parser]
    partial class MyParser
    {
        [Right("^")]
        [Left("*", "/")]
        [Left("+", "-")]
        /*
         Equivalent form should be:
        [Rule("expression : expression ('+' | '-' | '*' | '/') expression")]
         */
        [Rule("expression : expression '^' expression")]
        [Rule("expression : expression '+' expression")]
        [Rule("expression : expression '-' expression")]
        [Rule("expression : expression '*' expression")]
        [Rule("expression : expression '/' expression")]
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
        private static int Grouping(IToken _, int n, IToken _) => n;

        [Rule("expression : number")]
        private static int JustNumber(int n) => n;

        [Rule("number : one | two")]
        private static int Number(int n) => n;

        [Rule("one : '1'")]
        private static int One(IToken _) => 1;

        [Rule("two : '2'")]
        private static int Two(IToken _) => 2;
    }

    class Tok : IToken
    {
        public Range Range => new Range();
        public string Text { get; }

        public Tok(string text)
        {
            Text = text;
        }

        public bool Equals(IToken other) => other is Tok t && Text == t.Text;
    }

    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            var tokens = new List<IToken> 
            { 
                new Tok("1"),
                new Tok("+"),
                new Tok("2"),
                new Tok("+"),
                new Tok("2"),
                new Tok("+"),
                new Tok("1"),
                new Tok("end"),
            };

            var p = new MyParser(tokens);
            var res = p.ParseAddition();
        }
    }
}
