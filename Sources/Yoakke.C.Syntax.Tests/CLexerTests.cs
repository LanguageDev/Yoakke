using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Text;
using Kind = Yoakke.C.Syntax.CTokenType;

namespace Yoakke.C.Syntax.Tests
{
    [TestClass]
    public class CLexerTests
    {
        private static IEnumerable<object[]> AllTokensInput { get; } = new object[][]
        {
            Case(Kind.KeywordAuto, "auto"),
            Case(Kind.KeywordBreak, "break"),
            Case(Kind.KeywordCase, "case"),
            Case(Kind.KeywordChar, "char"),
            Case(Kind.KeywordConst, "const"),
            Case(Kind.KeywordContinue, "continue"),
            Case(Kind.KeywordDefault, "default"),
            Case(Kind.KeywordDo, "do"),
            Case(Kind.KeywordDouble, "double"),
            Case(Kind.KeywordElse, "else"),
            Case(Kind.KeywordEnum, "enum"),
            Case(Kind.KeywordExtern, "extern"),
            Case(Kind.KeywordFloat, "float"),
            Case(Kind.KeywordFor, "for"),
            Case(Kind.KeywordGoto, "goto"),
            Case(Kind.KeywordIf, "if"),
            Case(Kind.KeywordInt, "int"),
            Case(Kind.KeywordLong, "long"),
            Case(Kind.KeywordRegister, "register"),
            Case(Kind.KeywordReturn, "return"),
            Case(Kind.KeywordShort, "short"),
            Case(Kind.KeywordSigned, "signed"),
            Case(Kind.KeywordSizeof, "sizeof"),
            Case(Kind.KeywordStatic, "static"),
            Case(Kind.KeywordStruct, "struct"),
            Case(Kind.KeywordSwitch, "switch"),
            Case(Kind.KeywordTypedef, "typedef"),
            Case(Kind.KeywordUnion, "union"),
            Case(Kind.KeywordUnsigned, "unsigned"),
            Case(Kind.KeywordVoid, "void"),
            Case(Kind.KeywordVolatile, "volatile"),
            Case(Kind.KeywordWhile, "while"),

            Case(Kind.Identifier, "hello"),
            Case(Kind.Identifier, "ifa"),
            Case(Kind.Identifier, "Hello_123_abc"),

            Case(Kind.IntLiteral, "0x1fb"),
            Case(Kind.IntLiteral, "0X1Fb"),
            Case(Kind.IntLiteral, "0x1fbU"),
            Case(Kind.IntLiteral, "0x1fbL"),
            Case(Kind.IntLiteral, "0XABCU"),

            Case(Kind.IntLiteral, "123"),
            Case(Kind.IntLiteral, "0123"),
            Case(Kind.IntLiteral, "0123l"),
            Case(Kind.IntLiteral, "0123u"),
            Case(Kind.IntLiteral, "0123L"),
            Case(Kind.IntLiteral, "0123U"),

            Case(Kind.CharLiteral, "'a'"),
            Case(Kind.CharLiteral, @"'\''"),
            Case(Kind.CharLiteral, @"'\\'"),
            Case(Kind.CharLiteral, @"'\n'"),
            Case(Kind.CharLiteral, "L'a'"),
            Case(Kind.CharLiteral, @"x'\''"),
            Case(Kind.CharLiteral, @"_'\n'"),
            Case(Kind.CharLiteral, "'abc'"),
            Case(Kind.CharLiteral, "L'abc'"),

            Case(Kind.FloatLiteral, "123e2"),
            Case(Kind.FloatLiteral, "123e+2"),
            Case(Kind.FloatLiteral, "123E-245"),

            Case(Kind.FloatLiteral, "356.123E-245"),
            Case(Kind.FloatLiteral, ".123E-245"),
            Case(Kind.FloatLiteral, "123.E-245"),
            Case(Kind.FloatLiteral, ".123"),
            Case(Kind.FloatLiteral, "123."),
            Case(Kind.FloatLiteral, "123e2f"),
            Case(Kind.FloatLiteral, "123e+2l"),
            Case(Kind.FloatLiteral, "123E-245fL"),
            Case(Kind.FloatLiteral, "356.123E-245lF"),
            Case(Kind.FloatLiteral, ".123E-245LF"),
            Case(Kind.FloatLiteral, "123.E-245lLF"),
            Case(Kind.FloatLiteral, ".123llf"),
            Case(Kind.FloatLiteral, "123.lf"),

            Case(Kind.StringLiteral, @""""""),
            Case(Kind.StringLiteral, @"""abc"""),
            Case(Kind.StringLiteral, @"""\"""""),
            Case(Kind.StringLiteral, @"""abc\""abc"""),
            Case(Kind.StringLiteral, @"""abc\r\nc"""),
            Case(Kind.StringLiteral, @"""'a'"""),
            Case(Kind.StringLiteral, @"A"""""),
            Case(Kind.StringLiteral, @"u""abc"""),
            Case(Kind.StringLiteral, @"_""\"""""),
            Case(Kind.StringLiteral, @"L""abc\""abc"""),
            Case(Kind.StringLiteral, @"G""abc\r\nc"""),
            Case(Kind.StringLiteral, @"R""'a'"""),
            Case(Kind.Ellipsis, "..."),
            Case(Kind.ShiftRightAssign, ">>="),
            Case(Kind.ShiftLeftAssign, "<<="),
            Case(Kind.AddAssign, "+="),
            Case(Kind.SubtractAssign, "-="),
            Case(Kind.MultiplyAssign, "*="),
            Case(Kind.DivideAssign, "/="),
            Case(Kind.ModuloAssign, "%="),
            Case(Kind.BitAndAssign, "&="),
            Case(Kind.BitXorAssign, "^="),
            Case(Kind.BitOrAssign, "|="),
            Case(Kind.ShiftRight, ">>"),
            Case(Kind.ShiftLeft, "<<"),
            Case(Kind.Increment, "++"),
            Case(Kind.Decrement, "--"),
            Case(Kind.Arrow, "->"),
            Case(Kind.LogicalAnd, "&&"),
            Case(Kind.LogicalOr, "||"),
            Case(Kind.LessEqual, "<="),
            Case(Kind.GreaterEqual, ">="),
            Case(Kind.Equal, "=="),
            Case(Kind.NotEqual, "!="),
            Case(Kind.Semicolon, ";"),
            Case(Kind.OpenBrace, "{"),
            Case(Kind.OpenBrace, "<%"),
            Case(Kind.CloseBrace, "}"),
            Case(Kind.CloseBrace, "%>"),
            Case(Kind.Comma, ","),
            Case(Kind.Colon, ":"),
            Case(Kind.Assign, "="),
            Case(Kind.OpenParen, "("),
            Case(Kind.CloseParen, ")"),
            Case(Kind.OpenBracket, "["),
            Case(Kind.OpenBracket, "<:"),
            Case(Kind.CloseBracket, "]"),
            Case(Kind.CloseBracket, ":>"),
            Case(Kind.Dot, "."),
            Case(Kind.BitAnd, "&"),
            Case(Kind.LogicalNot, "!"),
            Case(Kind.BitNot, "~"),
            Case(Kind.Subtract, "-"),
            Case(Kind.Add, "+"),
            Case(Kind.Multiply, "*"),
            Case(Kind.Divide, "/"),
            Case(Kind.Modulo, "%"),
            Case(Kind.Less, "<"),
            Case(Kind.Greater, ">"),
            Case(Kind.BitXor, "^"),
            Case(Kind.BitOr, "|"),
            Case(Kind.QuestionMark, "?"),
        };

        private static object[] Case(Kind kind, string text) => new object[] { Tok(kind, text), text };

        private static Range Rn(int line, int column, int length) => new(new(line, column), length);

        private static CToken Tok(Kind ty, string t) => Tok(ty, Rn(0, 0, t.Length), t);

        private static CToken Tok(Kind ty, Range r, string t) => Tok(ty, r, t, r, t);

        private static CToken Tok(Kind ty, Range r, string t, Range logicalR, string logicalT) => new(r, t, logicalR, logicalT, ty);

        [DynamicData(nameof(AllTokensInput))]
        [DataTestMethod]
        public void LexSingleToken(CToken expected, string text)
        {
            var lexer = new CLexer(text);
            var token = lexer.Next();
            var end = Tok(Kind.End, Rn(0, token.Text.Length, 0), string.Empty);
            Assert.AreEqual(expected, token);
            Assert.AreEqual(end, lexer.Next());
        }
    }
}
