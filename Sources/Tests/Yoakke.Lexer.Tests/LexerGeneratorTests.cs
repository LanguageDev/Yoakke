// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Xunit;
using Yoakke.Lexer.Attributes;
using IgnoreAttribute = Yoakke.Lexer.Attributes.IgnoreAttribute;

namespace Yoakke.Lexer.Tests
{
    public partial class LexerGeneratorTests : TestBase<LexerGeneratorTests.TokenType>
    {
        public enum TokenType
        {
            [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

            [Error] Error,
            [End] End,

            [Token("IF")] [Token("if")] KwIf,
            [Token("else")] KwElse,
            [Regex(Regexes.Identifier)] Identifier,
            [Token("+")] Plus,
            [Token("-")] Minus,
            [Regex(Regexes.IntLiteral)] Number,
        }

        [Lexer(typeof(TokenType))]
        internal partial class Lexer
        {
        }

        [Fact]
        public void Empty()
        {
            var lexer = new Lexer(string.Empty);
            Assert.Equal(Token(string.Empty, TokenType.End, Range((0, 0), 0)), lexer.Next());
        }

        [Fact]
        public void EmptySpaces()
        {
            var lexer = new Lexer("   ");
            Assert.Equal(Token(string.Empty, TokenType.End, Range((0, 3), 0)), lexer.Next());
        }

        [Fact]
        public void SimpleSequence()
        {
            var lexer = new Lexer("if    asd+b 123");
            Assert.Equal(Token("if", TokenType.KwIf, Range((0, 0), 2)), lexer.Next());
            Assert.Equal(Token("asd", TokenType.Identifier, Range((0, 6), 3)), lexer.Next());
            Assert.Equal(Token("+", TokenType.Plus, Range((0, 9), 1)), lexer.Next());
            Assert.Equal(Token("b", TokenType.Identifier, Range((0, 10), 1)), lexer.Next());
            Assert.Equal(Token("123", TokenType.Number, Range((0, 12), 3)), lexer.Next());
            Assert.Equal(Token(string.Empty, TokenType.End, Range((0, 15), 0)), lexer.Next());
        }

        [Fact]
        public void SimpleSequenceWithAlias()
        {
            var lexer = new Lexer("if    asd+b 123 IF");
            Assert.Equal(Token("if", TokenType.KwIf, Range((0, 0), 2)), lexer.Next());
            Assert.Equal(Token("asd", TokenType.Identifier, Range((0, 6), 3)), lexer.Next());
            Assert.Equal(Token("+", TokenType.Plus, Range((0, 9), 1)), lexer.Next());
            Assert.Equal(Token("b", TokenType.Identifier, Range((0, 10), 1)), lexer.Next());
            Assert.Equal(Token("123", TokenType.Number, Range((0, 12), 3)), lexer.Next());
            Assert.Equal(Token("IF", TokenType.KwIf, Range((0, 16), 2)), lexer.Next());
            Assert.Equal(Token(string.Empty, TokenType.End, Range((0, 18), 0)), lexer.Next());
        }

        [Fact]
        public void SimpleMultilineSequence()
        {
            var lexer = new Lexer(@"if    
asd+b 123 
-2 b5");
            Assert.Equal(Token("if", TokenType.KwIf, Range((0, 0), 2)), lexer.Next());
            Assert.Equal(Token("asd", TokenType.Identifier, Range((1, 0), 3)), lexer.Next());
            Assert.Equal(Token("+", TokenType.Plus, Range((1, 3), 1)), lexer.Next());
            Assert.Equal(Token("b", TokenType.Identifier, Range((1, 4), 1)), lexer.Next());
            Assert.Equal(Token("123", TokenType.Number, Range((1, 6), 3)), lexer.Next());
            Assert.Equal(Token("-", TokenType.Minus, Range((2, 0), 1)), lexer.Next());
            Assert.Equal(Token("2", TokenType.Number, Range((2, 1), 1)), lexer.Next());
            Assert.Equal(Token("b5", TokenType.Identifier, Range((2, 3), 2)), lexer.Next());
            Assert.Equal(Token(string.Empty, TokenType.End, Range((2, 5), 0)), lexer.Next());
        }

        [Fact]
        public void SimpleMultilineSequenceWithNewline()
        {
            var lexer = new Lexer(@"if    
asd+b 123 
-2 b5
");
            Assert.Equal(Token("if", TokenType.KwIf, Range((0, 0), 2)), lexer.Next());
            Assert.Equal(Token("asd", TokenType.Identifier, Range((1, 0), 3)), lexer.Next());
            Assert.Equal(Token("+", TokenType.Plus, Range((1, 3), 1)), lexer.Next());
            Assert.Equal(Token("b", TokenType.Identifier, Range((1, 4), 1)), lexer.Next());
            Assert.Equal(Token("123", TokenType.Number, Range((1, 6), 3)), lexer.Next());
            Assert.Equal(Token("-", TokenType.Minus, Range((2, 0), 1)), lexer.Next());
            Assert.Equal(Token("2", TokenType.Number, Range((2, 1), 1)), lexer.Next());
            Assert.Equal(Token("b5", TokenType.Identifier, Range((2, 3), 2)), lexer.Next());
            Assert.Equal(Token(string.Empty, TokenType.End, Range((3, 0), 0)), lexer.Next());
        }

        [Fact]
        public void UnknownCharacter()
        {
            var lexer = new Lexer(@"if $");
            Assert.Equal(Token("if", TokenType.KwIf, Range((0, 0), 2)), lexer.Next());
            Assert.Equal(Token("$", TokenType.Error, Range((0, 3), 1)), lexer.Next());
            Assert.Equal(Token(string.Empty, TokenType.End, Range((0, 4), 0)), lexer.Next());
        }
    }
}
