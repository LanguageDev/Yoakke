// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yoakke.Lexer.Attributes;

namespace Yoakke.Lexer.Tests
{
    [TestClass]
    public partial class RegexesTests
    {
        public enum Identifier
        {
            [End] End,
            [Error] Error,
            [Regex(Regexes.Identifier)] Identifier,
        }

        [Lexer(typeof(Identifier))]
        internal partial class IdentifierLexer
        {
        }

        public enum StringLiteral
        {
            [End] End,
            [Error] Error,
            [Regex(Regexes.StringLiteral)] StringLiteral,
        }

        [Lexer(typeof(StringLiteral))]
        internal partial class StringLiteralLexer
        {
        }

        public enum IntLiteral
        {
            [End] End,
            [Error] Error,
            [Regex(Regexes.IntLiteral)] IntLiteral,
        }

        [Lexer(typeof(IntLiteral))]
        internal partial class IntLiteralLexer
        {
        }

        public enum HexLiteral
        {
            [End] End,
            [Error] Error,
            [Regex(Regexes.HexLiteral)] HexLiteral,
        }

        [Lexer(typeof(HexLiteral))]
        internal partial class HexLiteralLexer
        {
        }

        public enum RealNumberLiteral
        {
            [End] End,
            [Error] Error,
            [Regex(Regexes.RealNumberLiteral)] RealNumberLiteral,
        }

        [Lexer(typeof(RealNumberLiteral))]
        internal partial class RealNumberLiteralLexer
        {
        }

        public enum IeeeFloatLiteral
        {
            [End] End,
            [Error] Error,
            [Regex(Regexes.IeeeFloatLiteral)] IeeeFloatLiteral,
        }

        [Lexer(typeof(IeeeFloatLiteral))]
        internal partial class IeeeFloatLiteralLexer
        {
        }

        public enum LineComment
        {
            [End] End,
            [Error] Error,
            [Regex(Regexes.LineComment)] LineComment,
        }

        [Lexer(typeof(LineComment))]
        internal partial class LineCommentLexer
        {
        }

        public enum MultilineComment
        {
            [End] End,
            [Error] Error,
            [Regex(Regexes.MultilineComment)] MultilineComment,
        }

        [Lexer(typeof(MultilineComment))]
        internal partial class MultilineCommentLexer
        {
        }

        public enum Whitespace
        {
            [End] End,
            [Error] Error,
            [Regex(Regexes.Whitespace)] Whitespace,
        }

        [Lexer(typeof(Whitespace))]
        internal partial class WhitespaceLexer
        {
        }

        [DataTestMethod]

        [DataRow("foo", typeof(Identifier), true, typeof(IdentifierLexer))]
        [DataRow("Foo7", typeof(Identifier), true, typeof(IdentifierLexer))]
        [DataRow("2foo", typeof(Identifier), false, typeof(IdentifierLexer))]
        [DataRow("\"hello\"", typeof(Identifier), false, typeof(IdentifierLexer))]

        [DataRow("\"hello\"", typeof(StringLiteral), true, typeof(StringLiteralLexer))]
        [DataRow("\"\"", typeof(StringLiteral), true, typeof(StringLiteralLexer))]
        [DataRow("\"1\"", typeof(StringLiteral), true, typeof(StringLiteralLexer))]
        [DataRow("\"escaped\t\0test\a\"", typeof(StringLiteral), true, typeof(StringLiteralLexer))]
        [DataRow("\"\\\"internal\\\"\"", typeof(StringLiteral), true, typeof(StringLiteralLexer))]
        [DataRow("unopened\"", typeof(StringLiteral), false, typeof(StringLiteralLexer))]
        [DataRow("\"unclosed", typeof(StringLiteral), false, typeof(StringLiteralLexer))]
        [DataRow("\"hello\r\nworld!\"", typeof(StringLiteral), false, typeof(StringLiteralLexer))]

        [DataRow("0123456789", typeof(IntLiteral), true, typeof(IntLiteralLexer))]
        [DataRow("0a12", typeof(IntLiteral), false, typeof(IntLiteralLexer))]
        [DataRow("+1", typeof(IntLiteral), false, typeof(IntLiteralLexer))]
        [DataRow("-1", typeof(IntLiteral), false, typeof(IntLiteralLexer))]

        [DataRow("0x0123456789ABCDEF", typeof(HexLiteral), true, typeof(HexLiteralLexer))]
        [DataRow("0x0123456789ABCDEFG", typeof(HexLiteral), false, typeof(HexLiteralLexer))]
        [DataRow("x0123456789ABCDEF", typeof(HexLiteral), false, typeof(HexLiteralLexer))]
        [DataRow("1x0123456789ABCDEF", typeof(HexLiteral), false, typeof(HexLiteralLexer))]

        [DataRow("0.2", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
        [DataRow("01", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
        [DataRow(".2", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
        [DataRow("0.2e3", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
        [DataRow("0.2e+3", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
        [DataRow("0.2e-3", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
        [DataRow("01_23.45_6", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
        [DataRow("foo", typeof(RealNumberLiteral), false, typeof(RealNumberLiteralLexer))]
        [DataRow("_.2", typeof(RealNumberLiteral), false, typeof(RealNumberLiteralLexer))]
        [DataRow("1._2", typeof(RealNumberLiteral), false, typeof(RealNumberLiteralLexer))]
        [DataRow("0.e3", typeof(RealNumberLiteral), false, typeof(RealNumberLiteralLexer))]
        [DataRow("+1.2", typeof(RealNumberLiteral), false, typeof(RealNumberLiteralLexer))]
        [DataRow("-1.2", typeof(RealNumberLiteral), false, typeof(RealNumberLiteralLexer))]

        [DataRow("0.2", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
        [DataRow("01", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
        [DataRow(".2", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
        [DataRow("0.2e3", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
        [DataRow("0.2e+3", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
        [DataRow("0.2e-3", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
        [DataRow("01_23.45_6", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
        [DataRow("+1.2", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
        [DataRow("-1.2", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
        [DataRow("infinity", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
        [DataRow("+infinity", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
        [DataRow("-infinity", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
        [DataRow("NaN", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
        [DataRow("+NaN", typeof(IeeeFloatLiteral), false, typeof(IeeeFloatLiteralLexer))]
        [DataRow("-NaN", typeof(IeeeFloatLiteral), false, typeof(IeeeFloatLiteralLexer))]
        [DataRow("foo", typeof(IeeeFloatLiteral), false, typeof(IeeeFloatLiteralLexer))]
        [DataRow("_.2", typeof(IeeeFloatLiteral), false, typeof(IeeeFloatLiteralLexer))]
        [DataRow("1._2", typeof(IeeeFloatLiteral), false, typeof(IeeeFloatLiteralLexer))]
        [DataRow("0.e3", typeof(IeeeFloatLiteral), false, typeof(IeeeFloatLiteralLexer))]

        [DataRow("//hello", typeof(LineComment), true, typeof(LineCommentLexer))]
        [DataRow("// with spaces ", typeof(LineComment), true, typeof(LineCommentLexer))]
        [DataRow("///triple", typeof(LineComment), true, typeof(LineCommentLexer))]
        [DataRow("/single", typeof(LineComment), false, typeof(LineCommentLexer))]
        [DataRow("//line\nbroken", typeof(LineComment), false, typeof(LineCommentLexer))]

        [DataRow("/*single line*/", typeof(MultilineComment), true, typeof(MultilineCommentLexer))]
        [DataRow("/* \n multi \r\n line \r */", typeof(MultilineComment), true, typeof(MultilineCommentLexer))]
        [DataRow("/*****many asterisks*****/", typeof(MultilineComment), true, typeof(MultilineCommentLexer))]
        [DataRow("/*****unbalanced*/", typeof(MultilineComment), true, typeof(MultilineCommentLexer))]
        [DataRow("/*****unbalanced*/", typeof(MultilineComment), true, typeof(MultilineCommentLexer))]
        [DataRow("/* /** nested **/ comments */", typeof(MultilineComment), false, typeof(MultilineCommentLexer))]
        [DataRow("//single", typeof(MultilineComment), false, typeof(MultilineCommentLexer))]
        [DataRow("/* unclosed /", typeof(MultilineComment), false, typeof(MultilineCommentLexer))]

        [DataRow(" ", typeof(Whitespace), true, typeof(WhitespaceLexer))]
        [DataRow("\t", typeof(Whitespace), true, typeof(WhitespaceLexer))]
        [DataRow("\r", typeof(Whitespace), true, typeof(WhitespaceLexer))]
        [DataRow("\n", typeof(Whitespace), true, typeof(WhitespaceLexer))]
        [DataRow("a", typeof(Whitespace), false, typeof(WhitespaceLexer))]
        public void SingleTokenAcceptance(string input, Type enumType, bool shouldAccept, Type lexerType)
        {
            dynamic lexer = Activator.CreateInstance(lexerType, input)!;
            dynamic token = lexer.Next();

            var enumValues = Enum.GetValues(enumType);
            dynamic enumEndValue = enumValues.GetValue(0)!;
            dynamic enumValueUnderTest = enumValues.GetValue(2)!;
            if (shouldAccept)
            {
                Assert.AreEqual(enumValueUnderTest, token.Kind);
                Assert.AreEqual(enumEndValue, lexer.Next().Kind);
            }
            else
            {
                Assert.IsFalse(enumValueUnderTest == token.Kind && enumEndValue == lexer.Next().Kind);
            }
        }
    }
}
