using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Yoakke.Lexer;
using Yoakke.Text;

namespace Yoakke.Parser.Tests
{
    [Parser]
    partial class MyParser
    {
        [Rule("statement : expression '+' expression")]
        public static int Foo() => 1;

        [Rule("expression : expression '+' expression")]
        public static int Addition() => 2;
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
                new Tok("1"),
            };

            var p = new MyParser(tokens);
            p.ParseStatement();
            p.ParseExpression();
        }
    }
}
