// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Lexer.Streams;
using Yoakke.Text;

namespace Yoakke.Lexer.Tests
{
    [TestClass]
    public class CharStreamTests : TestBase<CharStreamTests.TokenType>
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

        public class Lexer : ILexer<Token<TokenType>>
        {
            public Position Position => this.charStream.Position;

            public bool IsEnd => this.charStream.IsEnd;

            private readonly ICharStream charStream;

            public Lexer(string source)
            {
                this.charStream = new TextReaderCharStream(new StringReader(source));
            }

            /// <inheritdoc/>
            public Token<TokenType> Next()
            {
                begin:
                if (this.IsEnd) return this.charStream.ConsumeToken(TokenType.End, 0);
                if (char.IsWhiteSpace(this.charStream.Peek()))
                {
                    this.charStream.Advance();
                    goto begin;
                }
                if (this.charStream.Matches("//"))
                {
                    var i = 0;
                    for (; this.charStream.LookAhead(i, '\n') != '\n'; ++i)
                    {
                        // Pass
                    }
                    this.charStream.Advance(i);
                    goto begin;
                }
                if (this.charStream.Peek() == '+') return this.charStream.ConsumeToken(TokenType.Plus, 1);
                if (this.charStream.Peek() == '-') return this.charStream.ConsumeToken(TokenType.Minus, 1);
                if (char.IsDigit(this.charStream.Peek()))
                {
                    var length = 1;
                    for (; char.IsDigit(this.charStream.LookAhead(length)); ++length)
                    {
                        // Pass
                    }
                    return this.charStream.ConsumeToken(TokenType.Number, length);
                }
                if (char.IsLetter(this.charStream.Peek()))
                {
                    var length = 1;
                    for (; char.IsLetterOrDigit(this.charStream.LookAhead(length)); ++length)
                    {
                        // Pass
                    }
                    var result = this.charStream.ConsumeToken(TokenType.Identifier, length);
                    return result.Text switch
                    {
                        "if" => new Token<TokenType>(result.Range, result.Text, TokenType.KwIf),
                        "else" => new Token<TokenType>(result.Range, result.Text, TokenType.KwElse),
                        _ => result,
                    };
                }
                return this.charStream.ConsumeToken(TokenType.Error, 1);
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
