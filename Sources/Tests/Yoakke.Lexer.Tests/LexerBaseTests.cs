﻿// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yoakke.Lexer.Tests
{
    [TestClass]
    public class LexerBaseTests : TestBase<LexerBaseTests.TokenType>
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
                if (this.IsEnd) return this.TakeToken(TokenType.End, 0);
                if (char.IsWhiteSpace(this.Peek()))
                {
                    this.Skip();
                    goto begin;
                }
                if (this.Matches("//"))
                {
                    var i = 0;
                    for (; this.Peek(i, '\n') != '\n'; ++i)
                    {
                    }
                    this.Skip(i);
                    goto begin;
                }
                if (this.Peek() == '+') return this.TakeToken(TokenType.Plus, 1);
                if (this.Peek() == '-') return this.TakeToken(TokenType.Minus, 1);
                if (char.IsDigit(this.Peek()))
                {
                    var length = 1;
                    for (; char.IsDigit(this.Peek(length)); ++length)
                    {
                    }
                    return this.TakeToken(TokenType.Number, length);
                }
                if (char.IsLetter(this.Peek()))
                {
                    var length = 1;
                    for (; char.IsLetterOrDigit(this.Peek(length)); ++length)
                    {
                    }
                    var result = this.TakeToken(TokenType.Identifier, length);
                    return result.Text switch
                    {
                        "if" => new Token<TokenType>(result.Range, result.Text, TokenType.KwIf),
                        "else" => new Token<TokenType>(result.Range, result.Text, TokenType.KwElse),
                        _ => result,
                    };
                }
                return this.TakeToken(TokenType.Error, 1);
            }
        }

        [TestMethod]
        public void Empty()
        {
            var lexer = new Lexer(string.Empty);
            Assert.AreEqual(Token(string.Empty, TokenType.End, Range((0, 0), 0)), lexer.Next());
        }

        [TestMethod]
        public void EmptySpaces()
        {
            var lexer = new Lexer("   ");
            Assert.AreEqual(Token(string.Empty, TokenType.End, Range((0, 3), 0)), lexer.Next());
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
            Assert.AreEqual(Token(string.Empty, TokenType.End, Range((0, 15), 0)), lexer.Next());
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
            Assert.AreEqual(Token(string.Empty, TokenType.End, Range((2, 5), 0)), lexer.Next());
        }

        [TestMethod]
        public void SimpleMultilineSequenceCommented()
        {
            var lexer = new Lexer(@"if    
// asd+b 123 
-2 b5");
            Assert.AreEqual(Token("if", TokenType.KwIf, Range((0, 0), 2)), lexer.Next());
            Assert.AreEqual(Token("-", TokenType.Minus, Range((2, 0), 1)), lexer.Next());
            Assert.AreEqual(Token("2", TokenType.Number, Range((2, 1), 1)), lexer.Next());
            Assert.AreEqual(Token("b5", TokenType.Identifier, Range((2, 3), 2)), lexer.Next());
            Assert.AreEqual(Token(string.Empty, TokenType.End, Range((2, 5), 0)), lexer.Next());
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
            Assert.AreEqual(Token(string.Empty, TokenType.End, Range((3, 0), 0)), lexer.Next());
        }
    }
}