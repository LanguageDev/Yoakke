// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System.Collections.Generic;
using Xunit;
using Yoakke.SynKit.Text;
using Kind = Yoakke.SynKit.C.Syntax.CTokenType;

namespace Yoakke.SynKit.C.Syntax.Tests;

public class CLexerTests
{
    private static ISourceFile fakeLocation = new Text.SourceFile("<no-location>", "1");

    private static Range Rn(int line, int column, int length) => new(new(line, column), length);

    private static Range Rn(Position from, Position to) => new(from, to);

    private static Position Pos(int line, int column) => new(line, column);

    private static CToken Tok(Kind ty, string t) => Tok(ty, Rn(0, 0, t.Length), t);

    private static CToken Tok(Kind ty, Range r, string t) => Tok(ty, r, t, r, t);

    private static CToken Tok(Kind ty, Range r, string t, Range logicalR, string logicalT) => new(new Location(fakeLocation, r), t, logicalR, logicalT, ty);

    [Theory]

    [InlineData(Kind.KeywordAuto, "auto")]
    [InlineData(Kind.KeywordBreak, "break")]
    [InlineData(Kind.KeywordCase, "case")]
    [InlineData(Kind.KeywordChar, "char")]
    [InlineData(Kind.KeywordConst, "const")]
    [InlineData(Kind.KeywordContinue, "continue")]
    [InlineData(Kind.KeywordDefault, "default")]
    [InlineData(Kind.KeywordDo, "do")]
    [InlineData(Kind.KeywordDouble, "double")]
    [InlineData(Kind.KeywordElse, "else")]
    [InlineData(Kind.KeywordEnum, "enum")]
    [InlineData(Kind.KeywordExtern, "extern")]
    [InlineData(Kind.KeywordFloat, "float")]
    [InlineData(Kind.KeywordFor, "for")]
    [InlineData(Kind.KeywordGoto, "goto")]
    [InlineData(Kind.KeywordIf, "if")]
    [InlineData(Kind.KeywordInline, "inline")]
    [InlineData(Kind.KeywordInt, "int")]
    [InlineData(Kind.KeywordLong, "long")]
    [InlineData(Kind.KeywordRegister, "register")]
    [InlineData(Kind.KeywordRestrict, "restrict")]
    [InlineData(Kind.KeywordReturn, "return")]
    [InlineData(Kind.KeywordShort, "short")]
    [InlineData(Kind.KeywordSigned, "signed")]
    [InlineData(Kind.KeywordSizeof, "sizeof")]
    [InlineData(Kind.KeywordStatic, "static")]
    [InlineData(Kind.KeywordStruct, "struct")]
    [InlineData(Kind.KeywordSwitch, "switch")]
    [InlineData(Kind.KeywordTypedef, "typedef")]
    [InlineData(Kind.KeywordUnion, "union")]
    [InlineData(Kind.KeywordUnsigned, "unsigned")]
    [InlineData(Kind.KeywordVoid, "void")]
    [InlineData(Kind.KeywordVolatile, "volatile")]
    [InlineData(Kind.KeywordWhile, "while")]
    [InlineData(Kind.KeywordAlignAs, "_Alignas")]
    [InlineData(Kind.KeywordAlignOf, "_Alignof")]
    [InlineData(Kind.KeywordAtomic, "_Atomic")]
    [InlineData(Kind.KeywordBool, "_Bool")]
    [InlineData(Kind.KeywordComplex, "_Complex")]
    [InlineData(Kind.KeywordGeneric, "_Generic")]
    [InlineData(Kind.KeywordImaginary, "_Imaginary")]
    [InlineData(Kind.KeywordNoReturn, "_Noreturn")]
    [InlineData(Kind.KeywordStaticAssert, "_Static_assert")]
    [InlineData(Kind.KeywordThreadLocal, "_Thread_local")]

    [InlineData(Kind.Identifier, "hello")]
    [InlineData(Kind.Identifier, "ifa")]
    [InlineData(Kind.Identifier, "Hello_123_abc")]
    [InlineData(Kind.Identifier, "Äfoo")]
    [InlineData(Kind.Identifier, "\u0410")]
    [InlineData(Kind.Identifier, "\u0100")]

    [InlineData(Kind.IntLiteral, "0x1fb")]
    [InlineData(Kind.IntLiteral, "0X1Fb")]
    [InlineData(Kind.IntLiteral, "0x1fbU")]
    [InlineData(Kind.IntLiteral, "0x1fbL")]
    [InlineData(Kind.IntLiteral, "0XABCU")]

    [InlineData(Kind.IntLiteral, "123")]
    [InlineData(Kind.IntLiteral, "0123")]
    [InlineData(Kind.IntLiteral, "0123l")]
    [InlineData(Kind.IntLiteral, "0123u")]
    [InlineData(Kind.IntLiteral, "0123L")]
    [InlineData(Kind.IntLiteral, "0123U")]

    [InlineData(Kind.CharLiteral, "'a'")]
    [InlineData(Kind.CharLiteral, @"'\''")]
    [InlineData(Kind.CharLiteral, @"'\\'")]
    [InlineData(Kind.CharLiteral, @"'\n'")]
    [InlineData(Kind.CharLiteral, "L'a'")]
    [InlineData(Kind.CharLiteral, @"x'\''")]
    [InlineData(Kind.CharLiteral, @"_'\n'")]
    [InlineData(Kind.CharLiteral, "'abc'")]
    [InlineData(Kind.CharLiteral, "L'abc'")]

    [InlineData(Kind.FloatLiteral, "123e2")]
    [InlineData(Kind.FloatLiteral, "123e+2")]
    [InlineData(Kind.FloatLiteral, "123E-245")]

    [InlineData(Kind.FloatLiteral, "356.123E-245")]
    [InlineData(Kind.FloatLiteral, ".123E-245")]
    [InlineData(Kind.FloatLiteral, "123.E-245")]
    [InlineData(Kind.FloatLiteral, ".123")]
    [InlineData(Kind.FloatLiteral, "123.")]
    [InlineData(Kind.FloatLiteral, "123e2f")]
    [InlineData(Kind.FloatLiteral, "123e+2l")]
    [InlineData(Kind.FloatLiteral, "123E-245fL")]
    [InlineData(Kind.FloatLiteral, "356.123E-245lF")]
    [InlineData(Kind.FloatLiteral, ".123E-245LF")]
    [InlineData(Kind.FloatLiteral, "123.E-245lLF")]
    [InlineData(Kind.FloatLiteral, ".123llf")]
    [InlineData(Kind.FloatLiteral, "123.lf")]

    [InlineData(Kind.StringLiteral, @"""""")]
    [InlineData(Kind.StringLiteral, @"""abc""")]
    [InlineData(Kind.StringLiteral, @"""\""""")]
    [InlineData(Kind.StringLiteral, @"""abc\""abc""")]
    [InlineData(Kind.StringLiteral, @"""abc\r\nc""")]
    [InlineData(Kind.StringLiteral, @"""'a'""")]
    [InlineData(Kind.StringLiteral, @"A""""")]
    [InlineData(Kind.StringLiteral, @"u""abc""")]
    [InlineData(Kind.StringLiteral, @"_""\""""")]
    [InlineData(Kind.StringLiteral, @"L""abc\""abc""")]
    [InlineData(Kind.StringLiteral, @"G""abc\r\nc""")]
    [InlineData(Kind.StringLiteral, @"R""'a'""")]
    [InlineData(Kind.Ellipsis, "...")]
    [InlineData(Kind.ShiftRightAssign, ">>=")]
    [InlineData(Kind.ShiftLeftAssign, "<<=")]
    [InlineData(Kind.AddAssign, "+=")]
    [InlineData(Kind.SubtractAssign, "-=")]
    [InlineData(Kind.MultiplyAssign, "*=")]
    [InlineData(Kind.DivideAssign, "/=")]
    [InlineData(Kind.ModuloAssign, "%=")]
    [InlineData(Kind.BitAndAssign, "&=")]
    [InlineData(Kind.BitXorAssign, "^=")]
    [InlineData(Kind.BitOrAssign, "|=")]
    [InlineData(Kind.ShiftRight, ">>")]
    [InlineData(Kind.ShiftLeft, "<<")]
    [InlineData(Kind.Increment, "++")]
    [InlineData(Kind.Decrement, "--")]
    [InlineData(Kind.Arrow, "->")]
    [InlineData(Kind.LogicalAnd, "&&")]
    [InlineData(Kind.LogicalOr, "||")]
    [InlineData(Kind.LessEqual, "<=")]
    [InlineData(Kind.GreaterEqual, ">=")]
    [InlineData(Kind.Equal, "==")]
    [InlineData(Kind.NotEqual, "!=")]
    [InlineData(Kind.Semicolon, ";")]
    [InlineData(Kind.OpenBrace, "{")]
    [InlineData(Kind.OpenBrace, "<%")]
    [InlineData(Kind.CloseBrace, "}")]
    [InlineData(Kind.CloseBrace, "%>")]
    [InlineData(Kind.Comma, ",")]
    [InlineData(Kind.Colon, ":")]
    [InlineData(Kind.Assign, "=")]
    [InlineData(Kind.OpenParen, "(")]
    [InlineData(Kind.CloseParen, ")")]
    [InlineData(Kind.OpenBracket, "[")]
    [InlineData(Kind.OpenBracket, "<:")]
    [InlineData(Kind.CloseBracket, "]")]
    [InlineData(Kind.CloseBracket, ":>")]
    [InlineData(Kind.Dot, ".")]
    [InlineData(Kind.BitAnd, "&")]
    [InlineData(Kind.LogicalNot, "!")]
    [InlineData(Kind.BitNot, "~")]
    [InlineData(Kind.Subtract, "-")]
    [InlineData(Kind.Add, "+")]
    [InlineData(Kind.Multiply, "*")]
    [InlineData(Kind.Divide, "/")]
    [InlineData(Kind.Modulo, "%")]
    [InlineData(Kind.Less, "<")]
    [InlineData(Kind.Greater, ">")]
    [InlineData(Kind.BitXor, "^")]
    [InlineData(Kind.BitOr, "|")]
    [InlineData(Kind.QuestionMark, "?")]
    public void LexSingleToken(Kind kind, string text)
    {
        var expected = Tok(kind, text);
        var lexer = new CLexer(text);
        var token = lexer.Next();
        var end = Tok(Kind.End, Rn(0, token.Text.Length, 0), string.Empty);
        Assert.Equal(expected, token);
        Assert.Equal(end, lexer.Next());
    }

    [Theory]

    [InlineData(Kind.Identifier, "Äfoo")]
    [InlineData(Kind.Identifier, "\u0410")]
    [InlineData(Kind.Identifier, "\u0100")]
    public void DisabledUnicodeCharacters(Kind kind, string text)
    {
        var expected = Tok(kind, text);
        var lexer = new CLexer(text) { AllowUnicodeCharacters = false };
        var token = lexer.Next();
        Assert.NotEqual(expected, token);
    }

    [Fact]
    public void SimpleSequence()
    {
        var sourceCode = "int x = 2;";
        var lexer = new CLexer(sourceCode);
        Assert.Equal(Tok(Kind.KeywordInt, Rn(0, 0, 3), "int"), lexer.Next());
        Assert.Equal(Tok(Kind.Identifier, Rn(0, 4, 1), "x"), lexer.Next());
        Assert.Equal(Tok(Kind.Assign, Rn(0, 6, 1), "="), lexer.Next());
        Assert.Equal(Tok(Kind.IntLiteral, Rn(0, 8, 1), "2"), lexer.Next());
        Assert.Equal(Tok(Kind.Semicolon, Rn(0, 9, 1), ";"), lexer.Next());
        Assert.Equal(Tok(Kind.End, Rn(0, 10, 0), string.Empty), lexer.Next());
    }

    [Fact]
    public void LineContinuatedSequence()
    {
        var sourceCode = @"char* x = ""ab\
cd"";";
        var lexer = new CLexer(sourceCode);
        Assert.Equal(Tok(Kind.KeywordChar, Rn(0, 0, 4), "char", Rn(0, 0, 4), "char"), lexer.Next());
        Assert.Equal(Tok(Kind.Multiply, Rn(0, 4, 1), "*", Rn(0, 4, 1), "*"), lexer.Next());
        Assert.Equal(Tok(Kind.Identifier, Rn(0, 6, 1), "x", Rn(0, 6, 1), "x"), lexer.Next());
        Assert.Equal(Tok(Kind.Assign, Rn(0, 8, 1), "=", Rn(0, 8, 1), "="), lexer.Next());
        Assert.Equal(Tok(Kind.StringLiteral, Rn(Pos(0, 10), Pos(1, 3)), @"""ab\
cd""", Rn(0, 10, 6), @"""abcd"""), lexer.Next());
        Assert.Equal(Tok(Kind.Semicolon, Rn(1, 3, 1), ";", Rn(0, 16, 1), ";"), lexer.Next());
        Assert.Equal(Tok(Kind.End, Rn(1, 4, 0), string.Empty, Rn(0, 17, 0), string.Empty), lexer.Next());
    }

    [Fact]
    public void TrigraphLineContinuatedSequence()
    {
        var sourceCode = @"char* x = ""ab??/
cd"";";
        var lexer = new CLexer(sourceCode);
        Assert.Equal(Tok(Kind.KeywordChar, Rn(0, 0, 4), "char", Rn(0, 0, 4), "char"), lexer.Next());
        Assert.Equal(Tok(Kind.Multiply, Rn(0, 4, 1), "*", Rn(0, 4, 1), "*"), lexer.Next());
        Assert.Equal(Tok(Kind.Identifier, Rn(0, 6, 1), "x", Rn(0, 6, 1), "x"), lexer.Next());
        Assert.Equal(Tok(Kind.Assign, Rn(0, 8, 1), "=", Rn(0, 8, 1), "="), lexer.Next());
        Assert.Equal(Tok(Kind.StringLiteral, Rn(Pos(0, 10), Pos(1, 3)), @"""ab??/
cd""", Rn(0, 10, 6), @"""abcd"""), lexer.Next());
        Assert.Equal(Tok(Kind.Semicolon, Rn(1, 3, 1), ";", Rn(0, 16, 1), ";"), lexer.Next());
        Assert.Equal(Tok(Kind.End, Rn(1, 4, 0), string.Empty, Rn(0, 17, 0), string.Empty), lexer.Next());
    }
}
