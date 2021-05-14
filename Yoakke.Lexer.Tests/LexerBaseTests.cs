using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yoakke.Text;

namespace Yoakke.Lexer.Tests
{
    [TestClass]
    public class LexerBaseTests
    {
        public enum TokenType
        {
            Number,
            Identifier,
            KwIf,
            KwElse,
            Plus,
            Minus,
            Error,
            End,
        }

        public class Lexer : LexerBase<TokenType>
        {
            public Lexer(string source) 
                : base(source)
            {
            }

            public override Token<TokenType> Next()
            {
                begin:
                if (Peek() == '\0') return TakeToken(TokenType.End, 0);
                if (char.IsWhiteSpace(Peek()))
                {
                    Skip();
                    goto begin;
                }
                if (Peek() == '+') return TakeToken(TokenType.Plus, 1);
                if (Peek() == '-') return TakeToken(TokenType.Minus, 1);
                if (char.IsDigit(Peek()))
                {
                    int length = 1;
                    for (; char.IsDigit(Peek(length)); ++length) ;
                    return TakeToken(TokenType.Number, length);
                }
                if (char.IsLetter(Peek()))
                {
                    int length = 1;
                    for (; char.IsLetterOrDigit(Peek(length)); ++length) ;
                    var result = TakeToken(TokenType.Identifier, length);
                    return result.Text switch
                    {
                        "if" => new Token<TokenType>(result.Range, result.Text, TokenType.KwIf),
                        "else" => new Token<TokenType>(result.Range, result.Text, TokenType.KwElse),
                        _ => result,
                    };
                }
                return TakeToken(TokenType.Error, 1);
            }
        }

        private static Token<TokenType> Tok(string value, TokenType tt, Range r) =>
            new Token<TokenType>(r, value, tt);

        private static Range Rn((int Line, int Column) p1, (int Line, int Column) p2) =>
            new Range(new Position(p1.Line, p1.Column), new Position(p2.Line, p2.Column));

        private static Range Rn((int Line, int Column) p1, int len) =>
            new Range(new Position(p1.Line, p1.Column), len);

        [TestMethod]
        public void Empty()
        {
            var lexer = new Lexer("");
            Assert.AreEqual(Tok("", TokenType.End, Rn((0, 0), 0)), lexer.Next());
        }

        [TestMethod]
        public void EmptySpaces()
        {
            var lexer = new Lexer("   ");
            Assert.AreEqual(Tok("", TokenType.End, Rn((0, 3), 0)), lexer.Next());
        }

        [TestMethod]
        public void SimpleSequence()
        {
            var lexer = new Lexer("if    asd+b 123");
            Assert.AreEqual(Tok("if", TokenType.KwIf, Rn((0, 0), 2)), lexer.Next());
            Assert.AreEqual(Tok("asd", TokenType.Identifier, Rn((0, 6), 3)), lexer.Next());
            Assert.AreEqual(Tok("+", TokenType.Plus, Rn((0, 9), 1)), lexer.Next());
            Assert.AreEqual(Tok("b", TokenType.Identifier, Rn((0, 10), 1)), lexer.Next());
            Assert.AreEqual(Tok("123", TokenType.Number, Rn((0, 12), 3)), lexer.Next());
            Assert.AreEqual(Tok("", TokenType.End, Rn((0, 15), 0)), lexer.Next());
        }

        [TestMethod]
        public void SimpleMultilineSequence()
        {
            var lexer = new Lexer(@"if    
asd+b 123 
-2 b5");
            Assert.AreEqual(Tok("if", TokenType.KwIf, Rn((0, 0), 2)), lexer.Next());
            Assert.AreEqual(Tok("asd", TokenType.Identifier, Rn((1, 0), 3)), lexer.Next());
            Assert.AreEqual(Tok("+", TokenType.Plus, Rn((1, 3), 1)), lexer.Next());
            Assert.AreEqual(Tok("b", TokenType.Identifier, Rn((1, 4), 1)), lexer.Next());
            Assert.AreEqual(Tok("123", TokenType.Number, Rn((1, 6), 3)), lexer.Next());
            Assert.AreEqual(Tok("-", TokenType.Minus, Rn((2, 0), 1)), lexer.Next());
            Assert.AreEqual(Tok("2", TokenType.Number, Rn((2, 1), 1)), lexer.Next());
            Assert.AreEqual(Tok("b5", TokenType.Identifier, Rn((2, 3), 2)), lexer.Next());
            Assert.AreEqual(Tok("", TokenType.End, Rn((2, 5), 0)), lexer.Next());
        }

        [TestMethod]
        public void SimpleMultilineSequenceWithNewline()
        {
            var lexer = new Lexer(@"if    
asd+b 123 
-2 b5
");
            Assert.AreEqual(Tok("if", TokenType.KwIf, Rn((0, 0), 2)), lexer.Next());
            Assert.AreEqual(Tok("asd", TokenType.Identifier, Rn((1, 0), 3)), lexer.Next());
            Assert.AreEqual(Tok("+", TokenType.Plus, Rn((1, 3), 1)), lexer.Next());
            Assert.AreEqual(Tok("b", TokenType.Identifier, Rn((1, 4), 1)), lexer.Next());
            Assert.AreEqual(Tok("123", TokenType.Number, Rn((1, 6), 3)), lexer.Next());
            Assert.AreEqual(Tok("-", TokenType.Minus, Rn((2, 0), 1)), lexer.Next());
            Assert.AreEqual(Tok("2", TokenType.Number, Rn((2, 1), 1)), lexer.Next());
            Assert.AreEqual(Tok("b5", TokenType.Identifier, Rn((2, 3), 2)), lexer.Next());
            Assert.AreEqual(Tok("", TokenType.End, Rn((3, 0), 0)), lexer.Next());
        }
    }
}
