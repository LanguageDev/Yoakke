using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Lexer.Attributes;
using IgnoreAttribute = Yoakke.Lexer.Attributes.IgnoreAttribute;

namespace Yoakke.Lexer.Tests
{
    [TestClass]
    public class LexerGeneratorTests : TestBase<LexerGeneratorTests.TokenType>
    {
        [Lexer("Lexer")]
        public enum TokenType
        {
            [Ignore] [Regex(Regex.Space)] Whitespace,

            [Error] Error,
            [End] End,

            [Token("if")] KwIf,
            [Token("else")] KwElse,
            [Regex(Regex.Ident)] Identifier,
            [Token("+")] Plus,
            [Token("-")] Minus,
            [Regex(Regex.DecInt)] Number,
        }

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
