// Copyright (c) 2021 Yoakke.
// Licensed under the Apache License, Version 2.0.
// Source repository: https://github.com/LanguageDev/Yoakke

using Xunit;
using Yoakke.Lexer;
using Yoakke.Lexer.Attributes;
using Yoakke.Parser.Attributes;
using IgnoreAttribute = Yoakke.Lexer.Attributes.IgnoreAttribute;

namespace Yoakke.Parser.Tests
{
    public partial class IndirectLeftRecursionTests
    {
        internal enum TokenType
        {
            [End] End,
            [Error] Error,
            [Ignore] [Regex(Regexes.Whitespace)] Whitespace,

            [Regex(Regexes.Identifier)] Identifier,
        }

        [Lexer(typeof(TokenType))]
        internal partial class Lexer
        {
        }

        [Parser(typeof(TokenType))]
        internal partial class Parser
        {
            [Rule("grouping : group_element")]
            private static string Ident(string s) => s;

            [Rule("group_element : grouping Identifier")]
            private static string Group(string group, IToken next) => $"({group}, {next.Text})";

            [Rule("group_element : Identifier")]
            private static string Ident(IToken t) => t.Text;
        }

        private static string Parse(string source) =>
            new Parser(new Lexer(source)).ParseGrouping().Ok.Value;

        [Fact]
        public void TestOne() => Assert.Equal("a", Parse("a"));

        [Fact]
        public void TestTwo() => Assert.Equal("(a, b)", Parse("a b"));

        [Fact]
        public void TestThree() => Assert.Equal("((a, b), c)", Parse("a b c"));

        [Fact]
        public void TestFour() => Assert.Equal("(((a, b), c), d)", Parse("a b c d"));
    }
}
