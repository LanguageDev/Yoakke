// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

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
            [Ignore] [Regex(Regex.Whitespace)] Whitespace,

            [Error] Error,
            [End] End,

            [Token("IF")] [Token("if")] KwIf,
            [Token("else")] KwElse,
            [Regex(Regex.Identifier)] Identifier,
            [Token("+")] Plus,
            [Token("-")] Minus,
            [Regex(Regex.IntLiteral)] Number,
        }

        [TestMethod]
        public void Empty()
        {
            var lexer = new Lexer("");
            Assert.AreEqual(Token("", TokenType.End, Range((0, 0), 0)), lexer.Next());
        }

        [TestMethod]
        public void EmptySpaces()
        {
            var lexer = new Lexer("   ");
            Assert.AreEqual(Token("", TokenType.End, Range((0, 3), 0)), lexer.Next());
        }

        [TestMethod]
        public void SimpleSequence()
        {
            var lexer = new Lexer("if    asd+b 123");
            Assert.AreEqual(Token("if", TokenType.KwIf, Range((0, 0), 2)), lexer.Next());
            Assert.AreEqual(Token("asd", TokenType.Identifier, Range((0, 6), 3)), lexer.Next());
            Assert.AreEqual(Token("+", TokenType.Plus, Range((0, 9), 1)), lexer.Next());
            Assert.AreEqual(Token("b", TokenType.Identifier, Range((0, 10), 1)), lexer.Next());
            Assert.AreEqual(Token("123", TokenType.Number, Range((0, 12), 3)), lexer.Next());
            Assert.AreEqual(Token("", TokenType.End, Range((0, 15), 0)), lexer.Next());
        }

        [TestMethod]
        public void SimpleSequenceWithAlias()
        {
            var lexer = new Lexer("if    asd+b 123 IF");
            Assert.AreEqual(Token("if", TokenType.KwIf, Range((0, 0), 2)), lexer.Next());
            Assert.AreEqual(Token("asd", TokenType.Identifier, Range((0, 6), 3)), lexer.Next());
            Assert.AreEqual(Token("+", TokenType.Plus, Range((0, 9), 1)), lexer.Next());
            Assert.AreEqual(Token("b", TokenType.Identifier, Range((0, 10), 1)), lexer.Next());
            Assert.AreEqual(Token("123", TokenType.Number, Range((0, 12), 3)), lexer.Next());
            Assert.AreEqual(Token("IF", TokenType.KwIf, Range((0, 16), 2)), lexer.Next());
            Assert.AreEqual(Token("", TokenType.End, Range((0, 18), 0)), lexer.Next());
        }

        [TestMethod]
        public void SimpleMultilineSequence()
        {
            var lexer = new Lexer(@"if    
asd+b 123 
-2 b5");
            Assert.AreEqual(Token("if", TokenType.KwIf, Range((0, 0), 2)), lexer.Next());
            Assert.AreEqual(Token("asd", TokenType.Identifier, Range((1, 0), 3)), lexer.Next());
            Assert.AreEqual(Token("+", TokenType.Plus, Range((1, 3), 1)), lexer.Next());
            Assert.AreEqual(Token("b", TokenType.Identifier, Range((1, 4), 1)), lexer.Next());
            Assert.AreEqual(Token("123", TokenType.Number, Range((1, 6), 3)), lexer.Next());
            Assert.AreEqual(Token("-", TokenType.Minus, Range((2, 0), 1)), lexer.Next());
            Assert.AreEqual(Token("2", TokenType.Number, Range((2, 1), 1)), lexer.Next());
            Assert.AreEqual(Token("b5", TokenType.Identifier, Range((2, 3), 2)), lexer.Next());
            Assert.AreEqual(Token("", TokenType.End, Range((2, 5), 0)), lexer.Next());
        }

        [TestMethod]
        public void SimpleMultilineSequenceWithNewline()
        {
            var lexer = new Lexer(@"if    
asd+b 123 
-2 b5
");
            Assert.AreEqual(Token("if", TokenType.KwIf, Range((0, 0), 2)), lexer.Next());
            Assert.AreEqual(Token("asd", TokenType.Identifier, Range((1, 0), 3)), lexer.Next());
            Assert.AreEqual(Token("+", TokenType.Plus, Range((1, 3), 1)), lexer.Next());
            Assert.AreEqual(Token("b", TokenType.Identifier, Range((1, 4), 1)), lexer.Next());
            Assert.AreEqual(Token("123", TokenType.Number, Range((1, 6), 3)), lexer.Next());
            Assert.AreEqual(Token("-", TokenType.Minus, Range((2, 0), 1)), lexer.Next());
            Assert.AreEqual(Token("2", TokenType.Number, Range((2, 1), 1)), lexer.Next());
            Assert.AreEqual(Token("b5", TokenType.Identifier, Range((2, 3), 2)), lexer.Next());
            Assert.AreEqual(Token("", TokenType.End, Range((3, 0), 0)), lexer.Next());
        }

        [TestMethod]
        public void UnknownCharacter()
        {
            var lexer = new Lexer(@"if $");
            Assert.AreEqual(Token("if", TokenType.KwIf, Range((0, 0), 2)), lexer.Next());
            Assert.AreEqual(Token("$", TokenType.Error, Range((0, 3), 1)), lexer.Next());
            Assert.AreEqual(Token("", TokenType.End, Range((0, 4), 0)), lexer.Next());
        }
    }
}
