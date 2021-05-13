using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Yoakke.Lexer;
using Yoakke.Text;
using Range = Yoakke.Text.Range;

namespace Yoakke.Parser.Tests
{
    enum TokenType
    {
        Add,
        Sub,
        Mul,
        Div,
        Exp,
        Number,
        Lparen,
        Rparen,
        End,
    }

    [Parser(typeof(TokenType))]
    partial class MyParser
    {
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
    public class BasicTests
    {
        private static Token<TokenType> Tok(string v, TokenType tt) => new Token<TokenType>(new Range(), v, tt);

        [TestMethod]
        public void TestMethod1()
        {
            var tokens = new List<IToken> 
            { 
                Tok("1", TokenType.Number),
                Tok("+", TokenType.Add),
                Tok("(", TokenType.Lparen),
                Tok("2", TokenType.Number),
                Tok("+", TokenType.Add),
                Tok("2", TokenType.Number),
                Tok(")", TokenType.Rparen),
                Tok("+", TokenType.Add),
                Tok("1", TokenType.Number),
                Tok("", TokenType.End),
            };

            var p = new MyParser(tokens);
            var res = p.ParseExpression();
        }
    }
}
