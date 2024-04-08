// Copyright (c) 2021-2022 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using System;
using Xunit;
using Yoakke.SynKit.Lexer.Attributes;

namespace Yoakke.SynKit.Lexer.Tests;

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

    public enum Issue140Token
    {
        [End] End,
        [Error] Error,
        [Regex(@"'((\\[^\n\r])|[^\r\n\\'])*'")] Whitespace,
    }

    [Lexer(typeof(Issue140Token))]
    internal partial class Issue140Lexer
    {
    }

    public enum EscapingInsideSingleLiteralRangeToken
    {
        [End] End,
        [Error] Error,
        [Regex(@"[\]]")] Whitespace,
    }

    [Lexer(typeof(EscapingInsideSingleLiteralRangeToken))]
    internal partial class Issue145Lexer
    {
    }

    [Theory]

    [InlineData("foo", typeof(Identifier), true, typeof(IdentifierLexer))]
    [InlineData("Foo7", typeof(Identifier), true, typeof(IdentifierLexer))]
    [InlineData("2foo", typeof(Identifier), false, typeof(IdentifierLexer))]
    [InlineData("Äfoo", typeof(Identifier), false, typeof(IdentifierLexer))]
    [InlineData("\u0410", typeof(Identifier), false, typeof(IdentifierLexer))] // Cyrillic A
    [InlineData("\u0100", typeof(Identifier), false, typeof(IdentifierLexer))] // Latin Ā
    [InlineData("\"hello\"", typeof(Identifier), false, typeof(IdentifierLexer))]

    [InlineData("\"hello\"", typeof(StringLiteral), true, typeof(StringLiteralLexer))]
    [InlineData("\"\"", typeof(StringLiteral), true, typeof(StringLiteralLexer))]
    [InlineData("\"1\"", typeof(StringLiteral), true, typeof(StringLiteralLexer))]
    [InlineData("\"escaped\t\0test\a\"", typeof(StringLiteral), true, typeof(StringLiteralLexer))]
    [InlineData("\"\\\"internal\\\"\"", typeof(StringLiteral), true, typeof(StringLiteralLexer))]
    [InlineData("unopened\"", typeof(StringLiteral), false, typeof(StringLiteralLexer))]
    [InlineData("\"unclosed", typeof(StringLiteral), false, typeof(StringLiteralLexer))]
    [InlineData("\"hello\r\nworld!\"", typeof(StringLiteral), false, typeof(StringLiteralLexer))]

    [InlineData("0123456789", typeof(IntLiteral), true, typeof(IntLiteralLexer))]
    [InlineData("0a12", typeof(IntLiteral), false, typeof(IntLiteralLexer))]
    [InlineData("+1", typeof(IntLiteral), false, typeof(IntLiteralLexer))]
    [InlineData("-1", typeof(IntLiteral), false, typeof(IntLiteralLexer))]

    [InlineData("0x0123456789ABCDEF", typeof(HexLiteral), true, typeof(HexLiteralLexer))]
    [InlineData("0x0123456789ABCDEFG", typeof(HexLiteral), false, typeof(HexLiteralLexer))]
    [InlineData("x0123456789ABCDEF", typeof(HexLiteral), false, typeof(HexLiteralLexer))]
    [InlineData("1x0123456789ABCDEF", typeof(HexLiteral), false, typeof(HexLiteralLexer))]

    [InlineData("0.2", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
    [InlineData("01", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
    [InlineData(".2", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
    [InlineData("0.2e3", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
    [InlineData("0.2e+3", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
    [InlineData("0.2e-3", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
    [InlineData("01_23.45_6", typeof(RealNumberLiteral), true, typeof(RealNumberLiteralLexer))]
    [InlineData("foo", typeof(RealNumberLiteral), false, typeof(RealNumberLiteralLexer))]
    [InlineData("_.2", typeof(RealNumberLiteral), false, typeof(RealNumberLiteralLexer))]
    [InlineData("1._2", typeof(RealNumberLiteral), false, typeof(RealNumberLiteralLexer))]
    [InlineData("0.e3", typeof(RealNumberLiteral), false, typeof(RealNumberLiteralLexer))]
    [InlineData("+1.2", typeof(RealNumberLiteral), false, typeof(RealNumberLiteralLexer))]
    [InlineData("-1.2", typeof(RealNumberLiteral), false, typeof(RealNumberLiteralLexer))]

    [InlineData("0.2", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
    [InlineData("01", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
    [InlineData(".2", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
    [InlineData("0.2e3", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
    [InlineData("0.2e+3", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
    [InlineData("0.2e-3", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
    [InlineData("01_23.45_6", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
    [InlineData("+1.2", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
    [InlineData("-1.2", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
    [InlineData("infinity", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
    [InlineData("+infinity", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
    [InlineData("-infinity", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
    [InlineData("NaN", typeof(IeeeFloatLiteral), true, typeof(IeeeFloatLiteralLexer))]
    [InlineData("+NaN", typeof(IeeeFloatLiteral), false, typeof(IeeeFloatLiteralLexer))]
    [InlineData("-NaN", typeof(IeeeFloatLiteral), false, typeof(IeeeFloatLiteralLexer))]
    [InlineData("foo", typeof(IeeeFloatLiteral), false, typeof(IeeeFloatLiteralLexer))]
    [InlineData("_.2", typeof(IeeeFloatLiteral), false, typeof(IeeeFloatLiteralLexer))]
    [InlineData("1._2", typeof(IeeeFloatLiteral), false, typeof(IeeeFloatLiteralLexer))]
    [InlineData("0.e3", typeof(IeeeFloatLiteral), false, typeof(IeeeFloatLiteralLexer))]

    [InlineData("//hello", typeof(LineComment), true, typeof(LineCommentLexer))]
    [InlineData("// with spaces ", typeof(LineComment), true, typeof(LineCommentLexer))]
    [InlineData("///triple", typeof(LineComment), true, typeof(LineCommentLexer))]
    [InlineData("/single", typeof(LineComment), false, typeof(LineCommentLexer))]
    [InlineData("//line\nbroken", typeof(LineComment), false, typeof(LineCommentLexer))]

    [InlineData("/*single line*/", typeof(MultilineComment), true, typeof(MultilineCommentLexer))]
    [InlineData("/* \n multi \r\n line \r */", typeof(MultilineComment), true, typeof(MultilineCommentLexer))]
    [InlineData("/*****many asterisks*****/", typeof(MultilineComment), true, typeof(MultilineCommentLexer))]
    [InlineData("/*****unbalanced*/", typeof(MultilineComment), true, typeof(MultilineCommentLexer))]
    [InlineData("/* /** nested **/ comments */", typeof(MultilineComment), false, typeof(MultilineCommentLexer))]
    [InlineData("//single", typeof(MultilineComment), false, typeof(MultilineCommentLexer))]
    [InlineData("/* unclosed /", typeof(MultilineComment), false, typeof(MultilineCommentLexer))]

    [InlineData(" ", typeof(Whitespace), true, typeof(WhitespaceLexer))]
    [InlineData("\t", typeof(Whitespace), true, typeof(WhitespaceLexer))]
    [InlineData("\r", typeof(Whitespace), true, typeof(WhitespaceLexer))]
    [InlineData("\n", typeof(Whitespace), true, typeof(WhitespaceLexer))]
    [InlineData("a", typeof(Whitespace), false, typeof(WhitespaceLexer))]

    [InlineData(@"'hello'", typeof(Issue140Token), true, typeof(Issue140Lexer))]
    [InlineData(@"'hello \' bye'", typeof(Issue140Token), true, typeof(Issue140Lexer))]

    [InlineData(@"]", typeof(EscapingInsideSingleLiteralRangeToken), true, typeof(Issue145Lexer))]
    public void SingleTokenAcceptance(string input, Type enumType, bool shouldAccept, Type lexerType)
    {
        dynamic lexer = Activator.CreateInstance(lexerType, input)!;
        dynamic token = lexer.Next();

        var enumValues = Enum.GetValues(enumType);
        dynamic enumEndValue = enumValues.GetValue(0)!;
        dynamic enumValueUnderTest = enumValues.GetValue(2)!;
        if (shouldAccept)
        {
            Assert.Equal(enumValueUnderTest, token.Kind);
            Assert.Equal(enumEndValue, lexer.Next().Kind);
        }
        else
        {
            Assert.False(enumValueUnderTest == token.Kind && enumEndValue == lexer.Next().Kind);
        }
    }
}
